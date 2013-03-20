/* OperationsManager.cs
 * This file defines an operations manager class that should...
 * CptS323, Spring 2013
 * Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
 * Written by: Jennifer Mendez
 * Last modified by: Jennifer Mendez
 * Date modified: March 20, 2013
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asml_McCallisterHomeSecurity.Targets;
using Asml_McCallisterHomeSecurity.FileProcessors;
using Asml_McCallisterHomeSecurity.TurretManagement;

namespace Asml_McCallisterHomeSecurity.OperationsManager
{
    /// <summary>
    /// The Operations Manager class serves as the interface between the 
    /// GUI and all other modules.  This way, the GUI doesn't have to know
    /// how to interact with everything, it just asks the Operations 
    /// Manager to get some action done.  These functions will try to 
    /// perform an action and if the attempt fails, an exception should
    /// be thrown.  
    /// </summary>
    public class OperationsManager
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

        /// <summary>
        /// Factory for file readers.
        /// </summary>
        private FileProcessorFactory _reader_factory;
       //TODO-ADD private Turret _turret;
        private TurretManager _turret;

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
            NumberMissiles = 4;
            // Set up access to all needed objects
            _target_manager = TargetManager.GetInstance();
            _reader_factory = FileProcessorFactory.GetInstance();
            _turret = TurretManager.GetInstance();
            
        }
        
        // Interface with the Turret - for Manual Operation
        public void TurretMoveLeft()
        {
            //TODO-ADD
            _turret.DecreaseAzimuth(10);
           
        }
        public void TurretMoveRight()
        {
            _turret.IncreaseAzimuth(10);
            
        }
        public void TurretMoveUp()
        {
            _turret.IncreaseAttitude(10);
            
        }
        public void TurretMoveDown()
        {
            //TODO-ADD
            _turret.DecreaseAttitude(10);
           
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
                //TODO-ADD no missiles remaining error?
            }
            
        }

        public void TurretReset()
        {
            // Not sure if we were going to have this one or not, but there is a placeholder for it.  
            // TODO-ADD
            _turret.ResetToOrigin();
            
        }

        // Interface with the File Reader(s)
        public void LoadFile(string targetfile)
        {
            FileProcessor _reader = _reader_factory.Create(targetfile);
            _target_manager.ClearTargetList();
            _target_manager.AddTargets(_reader.ProcessFile());
        }
        
        // Interface with Target Manager
        public ObservableCollection<Target> TargetInfo
        {
            get
            {
                return _target_manager.Targets;
            }
            private set {}
        }

        public void ReloadTurret()
        {
            NumberMissiles = 4;
        }

        public int NumberMissiles
        {
            get;
            set;
        }

        /// <summary>
        /// A method to add targets identified by the video feed.  This will add
        /// them to the list one at a time.  This will be called by the 
        /// module that analyzes the video images.  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="friend"></param>
        public void LocatedTarget(int x, int y, int z, bool friend)  
        {
            _target_manager.AddTarget(x, y, z, friend);
        }
    }
}
