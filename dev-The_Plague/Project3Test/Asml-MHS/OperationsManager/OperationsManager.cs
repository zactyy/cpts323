/* OperationsManager.cs
 * This file defines an operations manager class that acts as a mediator 
 * between all other system components.  
 * CptS323, Spring 2013
 * Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
 * Written by: Jennifer Mendez
 * Last modified by: Chris Walters
 * Date modified: April 23, 2013
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetManagement;
using TargetManagement.TargetFileProcessors;
using ASMLEngineSdk;
using TurretManagement;
using System.Windows.Controls;
using System.Drawing;
using System.Threading;
using searchmodes;
using System.ComponentModel; // for background worker
using ThreadedTimer;
using Detectors;


namespace OperationsManager
{
    /// <summary>
    /// The Operations Manager class serves as the interface between the 
    /// GUI and all other modules.  This way, the GUI doesn't have to know
    /// how to interact with everything, it just asks the Operations 
    /// Manager to get some action done.  These functions will try to 
    /// perform an action and if the attempt fails, an exception should
    /// be thrown.  
    /// </summary>
    public class OperationsManager:IDisposable
    {
        #region Member_Variables
        /// <summary>
        /// Singleton reference to track creation of 
        /// one instance of Operations Manager.
        /// </summary>
        private static OperationsManager _rules_them_all = null;
                
        /// <summary>
        /// OperationsManager has one target manager, this is it.  
        /// </summary>
        private TargetManager _target_manager;

        private TargetDetector _td;

        private IMissileLauncher _turret;
        private const int MAX_MISSILES = 4;

        private const int LAUNCHER_OFFSET_FROM_CAMERA = 4;
        /// <summary>
        /// Lock for thread safety.
        /// </summary>
        private Object _lock;

        /// <summary>
        ///  the current target of destroy mode.
        /// </summary>
        private Target _current_target;

        private bool _visual_target;
        /// <summary>
        /// Event for updating current target.
        /// </summary>
        public delegate void TargetUpdate();
        public TargetUpdate CurrentTargetChanged;

        /// <summary>
        /// Event for notifying GUI about out of ammo status.
        /// </summary>
        public delegate void AmmoUpdate();
        public AmmoUpdate AmmoReduced;

        private searchmode _search_mode;

        private List<string> _search_modes;

        private int _number_missiles;

        /// <summary>
        /// background worker for turret and S&D mode operations
        /// </summary>
        /// <returns></returns>
        private BackgroundWorker _bw;

        public ThreadedTimer.Timer _timer;

        private TimeSpan _time_elapsed;
        #endregion

        #region OpsManager
        public static OperationsManager GetInstance()
        {
            if(_rules_them_all == null) 
            {
                _rules_them_all = new OperationsManager();
            }
            return _rules_them_all;
        }

        /// <summary>
        /// Finalize method
        /// </summary>
        ~OperationsManager()
        {
            this.Dispose(false);
        }
        /// <summary>
        /// Operations Manager constructor.  
        /// </summary>
        private OperationsManager()
        {
            NumberMissiles = MAX_MISSILES;
            // Set up access to all needed objects
            _target_manager = TargetManager.GetInstance();
            _turret = new MissileLauncherAdapter();
            _target_manager.TargetAdded += on_targets_changed;
            _lock = new Object();
            /* we need a way to make these search modes more plug-n-play*/
            _search_modes = new List<string>();
            _search_modes.Add("Foes");
            _search_modes.Add("Friends");
            _search_modes.Add("All");
            _search_mode = new searchfoe();
            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _td = new TargetDetector();
            _td.ImageProcessed += new EventHandler(on_targets_detected);
            _timer = new ThreadedTimer.Timer();
            _timer.TimeCaptured += new EventHandler<TimerEventArgs>(_timer_TimeCaptured);
            TurretReset();
            _visual_target = true;
            
        }

        void _timer_TimeCaptured(object sender, TimerEventArgs e)
        {
            _time_elapsed = e.LastTime;
        }
        #endregion

        #region Dispose
        /// <summary>
        /// public dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// actual disposal method.
        /// </summary>
        /// <param name="dispose_others"></param>
        private void Dispose(bool dispose_others)
        {
            if (dispose_others == true)
            {
                _timer.Destroy();
                _target_manager.Dispose();
                ((IDisposable)_turret).Dispose();
                
                
            }
            _rules_them_all = null;
        }
        #endregion 

        /*
         * search and destroy mode stuff
         */
        #region Search and Destroy 

        public delegate void sdcomplete();
        public sdcomplete sdCompleted;

        public void SetCurrentMode(string selectedMode)
        {
            CurrentMode = selectedMode.ToLower();
            switch (CurrentMode)
            {
                case "foes":
                    _search_mode = new searchfoe();
                    break;
                case "friends":
                    _search_mode = new searchfriends();
                    break;
                case "all":
                    _search_mode = new searchall();
                    break;
                default:
                    throw new OperationsError("Invalid Mode");
            }
        }

        /// <summary>
        ///  Search and Destroy
        /// </summary>
        public void SearchAndDestroy() 
        {
            if (!_bw.IsBusy)
            {
                _bw.DoWork += DestroyTargetsThread;
                _timer.Start();
                _bw.RunWorkerAsync(new Tuple<searchmode, List<Target>>(_search_mode, _target_manager.Targets));
            }
            else
            {
                throw new Exception("Turret is busy.");
            }
            
        }
   
        public void DestroyTargetsThread(Object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            Tuple<searchmode, List<Target>> args = (Tuple<searchmode, List<Target>>)e.Argument;
            searchmode search = args.Item1;
            List<Target> searchableTargets = args.Item2;
            while (_time_elapsed.Minutes < 2)
            {
                if (bw.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else if (e.Cancel == true)
                {
                    break;   
                }
                List<Target> fireTargets = search.search(searchableTargets);
                fireTargets.Sort(FastestPath);
                foreach (Target target in fireTargets)
                {
                    if (bw.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        /* if time elapsed is 2 minutes, destroy failed, end attempt*/
                        if (_time_elapsed.Minutes >= 2)
                        {
                            break;
                        }
                        if (NumberMissiles == 0)
                        {
                            break;
                        }
                        else
                        {
                            CurrentTarget = target;
                            TurretMoveToTarget(target.Theta, target.Phi);
                            TurretFire();
                            _target_manager.validate(target);
                            CurrentTarget = null;
                        }
                    }
                }
                // Since we're supposed to fire all remaining missile if no foe targets still exist...
                while (NumberMissiles > 0)
                {
                    TurretFire();
                }
            }
            /* remove this event handler from the dowork event so the background worker can be reused by others
             at completion */
            _timer.Stop();
            _turret.Reset();
            bw.DoWork -= DestroyTargetsThread;
            /* notifyGUI that search and destroy has completed.*/
            if (sdCompleted != null)
            {
                sdCompleted();
            }
        }

        private int FastestPath(Target t1, Target t2)
        {
            if(t1.Theta == t2.Theta)
            {
                return 0;
            }
            /*if t1 and t2 thetas are both negatives..reverse the sort order, this achieves the effect 
             * of the path being from largest to smallest negative(aka -1 is shot at before -2)
             * while in the other direction from smallest to largest.
             *  This results in firing at each target in order of the closest one first as the turret swings 
             *  left, then it moves to all targets right of the 0,0 point firing at them in order*/
            else if (t1.Theta < 0 && t2.Theta < 0)
            {
                if(t1.Theta > t2.Theta)
                {
                    return -1;
                }
            }
            else if (t1.Theta < t2.Theta)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Stops the destroy mode from running.
        /// </summary>
        public void Stop()
        {

            if (_bw.IsBusy)
            {
                _bw.CancelAsync();
                _timer.Stop();
                TurretReset();
            }
        }
        #endregion

        /*
         * Turret controls
         */
        #region TurretControls

        public void ReloadTurret()
        {
            NumberMissiles = MAX_MISSILES;
        }

        public int NumberMissiles
        {
            get
            {
                return _number_missiles;
            }
            set{
                _number_missiles = value;
                if(AmmoReduced != null)
                {
                    AmmoReduced();
                }
            }
        }
        // Interface with the Turret 
        public void TurretMoveToTarget(double theta_x, double theta_y)
        {
            _turret.MoveTo(theta_x, theta_y);
        }

        public void TurretMoveLeft()
        {
            _turret.MoveBy(0,-10);           
        }

        public void TurretMoveRight()
        {
            _turret.MoveBy(0, 10);
        }

        public void TurretMoveUp()
        {
            _turret.MoveBy(10, 0);            
        }

        public void TurretMoveDown()
        {
            _turret.MoveBy(-10, 0);           
        }

        public void TurretFire()
        {
            if (NumberMissiles > 0)
            {
                NumberMissiles--;
                _turret.Fire();
            }
            else
            {
                // should probably replace this with a custom LauncherAmmunitionDepletedException later.
                throw new Exception("No missiles remaining! Game over man! Game over!(unless you hit the reload button)");
            }
            
        }

        public bool TurretReset()
        {
            if (!_bw.IsBusy)
            {
                _bw.DoWork += Turret_Reset_Work;
                _bw.RunWorkerAsync();
                return true;
            }
            return false;
        }

        private void Turret_Reset_Work(object sender, DoWorkEventArgs e)
        {
            _turret.Reset();
            _bw.DoWork -= Turret_Reset_Work;
        }
        #endregion

        /* 
         * Target manager stuff
         */
        #region TargetMangager
        // Interface with the File Reader(s)
        public void LoadFile(string targetfile)
        {
            _target_manager.LoadFromFile(targetfile);
        }
        

        // Interface with Target Manager
        public List<ListViewItem> TargetInfo
        {
            get
            {
                List<ListViewItem> tmp = new List<ListViewItem>();
                foreach (Target target in _target_manager.Targets)
                {
                    var item = new ListViewItem();
                    // using a temp object prevents escape of the Targets in the target manager list.
                    Target temp = new Target(target);
                    item.Content = temp;
                    tmp.Add(item);
                }
                return tmp;
            }
            private set { }
        }
        #endregion

        /*
         * Target detector
         * */
        #region TargetDetector
        public void DetectTargets(System.Drawing.Image img){
            _td.DetectTargets(img);
        }

        public List<Tuple<Double, Double, Double, Double, Boolean>> DetectedTargets(){
            return _td.DetectedTargets();
        }

        // add all detected targets to the target list..but first clear the list.
        private void on_targets_detected(object sender, EventArgs e)
        {
            if (_visual_target == true)
            {
                List<Tuple<Double, Double, Double, Double, Boolean>> targetInfo = DetectedTargets();
                _target_manager.ClearTargetList();
                foreach (Tuple<Double, Double, Double, Double, Boolean> target in targetInfo)
                {                    
                    double x_coord = target.Item1;
                    double y_coord = target.Item2;
                    double z_coord = target.Item3 + LAUNCHER_OFFSET_FROM_CAMERA;
                    bool friend = target.Item5;
                    _target_manager.AddTarget(x_coord, y_coord, z_coord, friend);
                }
            }
        }
        #endregion

        /*
         * target list events
         */
        #region Target List Event(s)
        public delegate void TargetsChanged();

        /// <summary>
        /// used to pass a change in target list up the chain(if anyone is observing for a change).
        /// </summary>
        public TargetsChanged ChangedTargets;

        /// <summary>
        /// even handler for when targets list changes, passes event to observer, if any.
        /// </summary>
        private void on_targets_changed()
        {
            if (ChangedTargets != null)
            {
                ChangedTargets();
            }
        }
        #endregion

        /*
         * target detection stuff, not yet implemented
         */

        /*
         * manager properties
         */
        #region Properties
        public string CurrentMode
        {
            get;
            set;
        }
        public Target CurrentTarget
        {

            get
            {
                lock (_lock)
                {
                    return _current_target;
                }
            }
            set
            {
                lock (_lock)
                {
                    _current_target = value;
                    if (CurrentTargetChanged != null)
                    {
                        CurrentTargetChanged();
                    }
                }
            }
        }


        /// <summary>
        /// CurrentTargetInfo passes data to the GUI so it can be displayed on the video overlay.
        /// </summary>
        /// <returns>A tuple containing the name of the target, the turrrets angle theta and angle phi, and a string containing the friend/foe status of the target.</returns>
        public  Tuple<string, double, double, string> CurrentGUIInfo()
        {
            Tuple<string, double, double, string> temp;
            if (_current_target != null)
            {
                string friend = "Foe";
                if (_current_target.Friend == true)
                {
                    friend = "Friend";
                }
                temp = new Tuple<string, double, double, string>(_current_target.Name, Math.Round(_turret.Theta, 2), Math.Round(_turret.Phi, 2), friend);
            }
            else
            {
                temp = new Tuple<string, double, double, string>("None", 0, 0, "Friend");
            }
            return temp;
        }

        public List<string> Modes
        {
            get
            {
                return _search_modes;
            }
            private set
            {
            }
        }
        #endregion
    }
}