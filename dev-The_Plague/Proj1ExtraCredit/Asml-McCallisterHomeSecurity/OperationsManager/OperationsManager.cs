/* OperationsManager.cs
 * This file defines an operations manager class that should...
 * CptS323, Spring 2013
 * Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
 * Written by: Jennifer Mendez
 * Last modified by: Chris Walters
 * Date modified: March 21, 2013
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetManagement;
using TargetManagement.TargetFileProcessors;
using TurretManagement;
using System.Windows.Controls;

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

        public static OperationsManager GetInstance()
        {
            if(_rules_them_all == null) 
            {
                _rules_them_all = new OperationsManager();
            }
            return _rules_them_all;
        }

        private OperationsManager()
        {
            NumberMissiles = MAX_MISSILES;
            // Set up access to all needed objects
            _target_manager = TargetManager.GetInstance();
            _turret = new MissileLauncherAdapter();
            _target_manager.ChangedTargets += on_targets_changed;
        }


        public int NumberMissiles
        {
            get;
            set;
        }
        
        #region Dispose implementation
        /// <summary>
        /// Finalize method
        /// </summary>
        ~OperationsManager()
        {
            this.Dispose(false);
        }

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
                _turret.Dispose();// currently not disposable */
            }
            _rules_them_all = null;
        }

        #endregion 
        
        #region Turret Control
        // Interface with the Turret - for Manual Operation
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

        public void ReloadTurret()
        {
            NumberMissiles = MAX_MISSILES;
        }
        #endregion 

        #region TargetMangement
        // tell targetmanger to load a file
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
                    item.Content = target;
                    tmp.Add(item);
                }
                return tmp;
            }
            // the collection should not be changed from here. Only the TargetManager should change it.
            private set {}
        }


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
    }
}
