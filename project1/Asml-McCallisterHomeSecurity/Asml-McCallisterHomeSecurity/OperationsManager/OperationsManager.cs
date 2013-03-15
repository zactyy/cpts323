/* OperationsManager.cs
 * This file defines an operations manager class that should...
 * CptS323, Spring 2013
 * Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
 * Written by: Jennifer Mendez
 * Last modified by: Chris Walters
 * Date modified: March 14, 2013
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asml_McCallisterHomeSecurity.Targets;
using Asml_McCallisterHomeSecurity.FileProcessors;

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
            // Set up access to all needed objects
            _target_manager = TargetManager.GetInstance();
            _reader_factory = FileProcessorFactory.GetInstance();
            // Create a Turret object here 
            // Create a Reader object here
        }
        
        // Interface with the Turret - for Manual Operation
        public void TurretMoveLeft()
        {
            //TODO-ADD
            //_turret.MoveLeft();
            throw new NotImplementedException();
        }
        public void TurretMoveRight()
        {
            //TODO-ADD
            //_turret.MoveRight();
            throw new NotImplementedException();
        }
        public void TurretMoveUp()
        {
            //TODO-ADD
            //_turret.MoveUp();
            throw new NotImplementedException();
        }
        public void TurretMoveDown()
        {
            //TODO-ADD
            //_turret.MoveDown();
            throw new NotImplementedException();
        }
        public void TurretFire()
        {
            //TODO-ADD
            //_turret.Fire();
            throw new NotImplementedException();
        }

        public void TurretReset()
        {
            // Not sure if we were going to have this one or not, but there is a placeholder for it.  
            // TODO-ADD
            //_turret.Reset();
            throw new NotImplementedException();
        }

        // Interface with the File Reader(s)
        public void LoadFile(string targetfile)
        {
            FileProcessor _reader = _reader_factory.Create(targetfile); 
            _target_manager.AddTargets(_reader.ProcessFile());
        }
        
        // Interface with Target Manager
        public List<Target> GetTargetInfo()
        {
            List<Target> tempTargets = new List<Target>();
            _target_manager.AddTarget(100, 200, 300, true, "mytarget");
            _target_manager.AddTarget(111, 222, 333, false, "enemy");
            _target_manager.AddTarget(123, 234, 345, true, "sometarget");
            tempTargets = _target_manager.GetTargets();
            return tempTargets;
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
