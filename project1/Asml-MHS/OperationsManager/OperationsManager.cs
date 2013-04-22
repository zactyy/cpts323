/* OperationsManager.cs
 * This file defines an operations manager class that acts as a mediator 
 * between all other system components.  
 * CptS323, Spring 2013
 * Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
 * Written by: Jennifer Mendez
 * Last modified by: Jennifer Mendez
 * Date modified: April 22, 2013
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

        /// <summary>
        /// The thread the destroy mode will be run on.  
        /// </summary>
        private Thread _destroy_thread;
        /// <summary>
        /// Flag to track whether destroy should be currently running.  
        /// </summary>
        private bool _active_destroy_mode;
        /// <summary>
        /// Fired when the processing is started.
        /// </summary>
        public event EventHandler ThreadStarted;
        /// <summary>
        /// Fired when the processing is stopped.
        /// </summary>
        public event EventHandler ThreadStopped;
        /// <summary>
        /// Helps synchronize destroy thread event.
        /// </summary>
        private ManualResetEvent _wait_event;
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
            // this manual reset event helps synchronize between threads.  
            _wait_event = new ManualResetEvent(false);
            _lock = new Object();
            //_seach_mode_list.Add(0, "Idle");
            //_seach_mode_list.Add(1, "Foes");
            //_seach_mode_list.Add(2, "Friends");
            //_seach_mode_list.Add(3, "All");
            
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
                _target_manager.Dispose();
                ((IDisposable)_turret).Dispose();
                
                
            }
            _rules_them_all = null;
        }
        #endregion 

        #region Search and Destroy 
        public void SearchAndDestroy() 
        {
            //Start timer

            //Call zach's search and destroy 
            //List<Target> tempHitList = SearchMode(_target_manager.Targets, CurrentMode);
            //SetUpDestroyThread(tempHitList);

        }

        
        /// <summary>
        /// Sets up and starts the destroy mode thread.  
        /// </summary>
        private void SetUpDestroyThread(List<Target> hitList)
        {
            // The () parameter is supposed to be 
            _destroy_thread = new Thread(() => DestroyTargetsThread(hitList));
            _destroy_thread.Start();
            
        }

        /// <summary>
        /// This is the thread where the destroying of targets takes place.  
        /// </summary>
        /// <param name="fireTargets"></param>
        public void DestroyTargetsThread(List<Target> fireTargets)
        {
            WaitHandle[] events = new WaitHandle[] { _wait_event };
            int runEvent = WaitHandle.WaitAny(events);
            
            
            foreach (Target target in fireTargets)
            {            
                // We will wait a few milliseconds for an event.  
                // Using events here as a trigger to either continue with this mode or abort.  
                runEvent = WaitHandle.WaitAny(events, 50);
                if (runEvent == 0)
                {
                    _wait_event.Reset();
                    break;
                }
                else if (runEvent == 258) // timeout
                {
                    CurrentTarget = target;
                    _turret.MoveTo(target.Theta, target.Phi);
                    _turret.Fire();                      
                }
            }
            CurrentTarget = null;
        }

        /// <summary>
        /// Stops the destroy mode from running.
        /// </summary>
        public void Stop()
        {
            _wait_event.Set();

            if (ThreadStopped != null)
            {
                ThreadStopped(this, null);
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
            // Not sure if we were going to have this one or not, but there is a placeholder for it.  
            // TODO-ADD
            _turret.Reset();
            
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
        /// List of available search modes.
        /// </summary>
        public Dictionary<int, string> SeachModeList
        {
            get;
            set;
        }
        #endregion
    }
}
