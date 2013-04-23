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

        private IMissileLauncher _turret;
        private const int MAX_MISSILES = 4;

        /// <summary>
        /// Lock for thread safety.
        /// </summary>
        private Object _lock;

        /// <summary>
        ///  the current target of destroy mode.
        /// </summary>
        private Target _current_target;

        /// <summary>
        /// Event for updating current target.
        /// </summary>
        public delegate void TargetUpdate();
        public TargetUpdate CurrentTargetChanged;

        private searchmode _search_mode;

        private List<string> _search_modes;

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
            _timer = new ThreadedTimer.Timer();
            _timer.TimeCaptured += new EventHandler<TimerEventArgs>(_timer_TimeCaptured);
            TurretReset();
            
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
        /// 
        /// </summary>
        public void SearchAndDestroy() 
        {
            //Start timer

            List<Target> tempHitList = _search_mode.search(_target_manager.Targets);
            DestroyTargets(tempHitList);
        }
   
        /// <summary>
        /// This is the thread where the destroying of targets takes place.  
        /// </summary>
        /// <param name="fireTargets"></param>
        public void DestroyTargets(List<Target> fireTargets)
        {
            if (fireTargets.Count > 0)
            {
                if (!_bw.IsBusy)
                {
                    _bw.DoWork += DestroyTargetsThread;
                    _timer.Start();
                    _bw.RunWorkerAsync(fireTargets);
                }
            }
            else
            {
                throw new OperationsError("No targets matching that type detected!");
            }
        }

        public void DestroyTargetsThread(Object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            List<Target> fireTargets = (List<Target>)e.Argument;
            foreach(Target target in fireTargets)
            {
                if (bw.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    /* if time elapsed is 2 minutes, destroy failed, end attempt*/
                    if (_time_elapsed.Minutes == 2)
                    {
                        break;
                    }
                    CurrentTarget = target;
                    _turret.MoveTo(target.Phi, target.Theta);
                    _turret.Fire();
                    _target_manager.validate(target);
                }
            }
            /* remove this event handler from the dowork event so the background worker can be reused by others
             at completion */
            _timer.Stop();
            bw.DoWork -= DestroyTargetsThread;
            /* notifyGUI that search and destroy has completed.*/
            if (sdCompleted != null)
            {
                sdCompleted();
            }
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
            }
        }
        #endregion

        #region TurretControls

        public void ReloadTurret()
        {
            NumberMissiles = MAX_MISSILES;
        }

        public int NumberMissiles
        {
            get;
            set;
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

        public void TurretReset()
        {
            if (!_bw.IsBusy)
            {
                _bw.DoWork += Turret_Reset_Work;
                _bw.RunWorkerAsync();
            }
        }

        private void Turret_Reset_Work(object sender, DoWorkEventArgs e)
        {
            _turret.Reset();
            _bw.DoWork -= Turret_Reset_Work;
        }
        #endregion

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
        #region Target Detection
        /// <summary>
        /// A method to add targets identified by the video feed.  This will add
        /// them to the list one at a time.        
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="friend"></param>
        public void LocatedTarget(int x, int y, int z, bool friend)  
        {
            _target_manager.AddTarget(x, y, z, friend);
        }
        #endregion

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
        /// CurrentTargetInfo passes target data to the GUI so it can be displayed on the video overlay.
        /// </summary>
        /// <returns>A tuple containing the name of the target, its angle theta, its angle phi, and a string containing the friend/foe status of the target.</returns>
        public  Tuple<string, double, double, string> CurrentTargetInfo()
        {
            Tuple<string, double, double, string> temp;
            if (_current_target != null)
            {
                string friend = "Foe";
                if (_current_target.Friend == true)
                {
                    friend = "Friend";
                }
                temp = new Tuple<string, double, double, string>(_current_target.Name, Math.Round(_current_target.Theta, 2), Math.Round(_current_target.Phi, 2), friend);
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
