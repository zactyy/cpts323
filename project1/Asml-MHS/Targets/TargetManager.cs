// TargetManager.cs
// The Target Manager is a singleton class that manages the 
// list of Targets.  We don't want multiple target lists, or any confusion about 
// who is in charge of known targets.  
// CptS323, Spring 2013
// Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
// Written by: Jennifer Mendez
// Last modified by: Jennifer Mendez
// Date modified: April 22, 2013

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetManagement
{
    /// <summary>
    /// The TargetManager is a Singleton that maintains 
    /// knowledge of all the targets and controls additions to
    /// and information about the Target list.  
    /// </summary>
    public class TargetManager:IDisposable 
    {
        /// <summary>
        /// Singleton instance of TargetManager
        /// </summary>
        private static TargetManager _instance;
        /// <summary>
        /// singleton fileprocessorfactory. Only one is needed.
        /// </summary>
        private static TargetFileProcessors.FileProcessorFactory _reader_factory; 
        /// <summary>
        /// Targets list.
        /// </summary>
        private List<Target> _targets;
        
        public delegate void targetsChanged();

        public targetsChanged TargetAdded;


        /// <summary>
        ///  returns insance of TargetManager
        /// </summary>
        /// <returns></returns>
        public static TargetManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TargetManager();
            }
            return _instance;
        }

        /// <summary>
        ///  Finalize method
        /// </summary>
        ~TargetManager()
        {
            this.Dispose(false);
        }

       

        /// <summary>
        /// validate target destruction.
        /// </summary>
        /// <param name="currTarget">a target whose status is to be determined.</param>
        /// <returns></returns>
        public bool validate(Target currTarget)
        {
            /* for now we just return true if target is shot at, due to lack of anyway to actually validate*/
            return true;
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
        /// actualy disposal method
        /// </summary>
        /// <param name="dispose_others"></param>
        protected virtual void Dispose(bool dispose_others)
        {
            if (dispose_others == true)
            {
                this._targets.Clear();
                this._targets = null;
                _instance = null;
            }
        }

        /// <summary>
        /// Target Manager Constructor
        /// </summary>
        private TargetManager()
        {
            _targets = new List<Target>();
            _reader_factory = TargetFileProcessors.FileProcessorFactory.GetInstance();
        }

        /// <summary>
        /// Method to add target a target one at a time to the target list.  
        /// </summary>
        /// <param name="new_name"></param>
        /// <param name="new_x"></param>
        /// <param name="new_y"></param>
        /// <param name="new_z"></param>
        /// <param name="friend"></param>
        public void AddTarget(double new_x, double new_y, double new_z, bool friend, string new_name = "")
        {
            Target tempTarget = new Target(new_name, new_x, new_y, new_z, friend);
            _targets.Add(tempTarget);
            if (TargetAdded != null)
            {
                TargetAdded();
            }
        }

        /// <summary>
        /// Method to add a whole list of targets, that might be read in from 
        /// file into the target list.  
        /// </summary>
        /// <param name="listOfTargets"></param>
        public void AddTargets(List<Target> listOfTargets)
        {
            listOfTargets.ForEach(_targets.Add);
            if (TargetAdded != null)
            {
                TargetAdded();
            }
        }

        /// <summary>
        /// Simply returns the current list of targets.  
        /// </summary>
        /// <returns></returns>
        public List<Target> Targets
        {
            get
            {
                List<Target> temp = new List<Target>(_targets);
                return temp;
            }
        }

        public void LoadFromFile(string fp)
        {
            TargetFileProcessors.FileProcessor _reader = _reader_factory.Create(fp);
            this.ClearTargetList();
            this.AddTargets(_reader.ProcessFile());
        }

        /// <summary>
        /// Clears the list of targets.  
        /// </summary>
        public void ClearTargetList()
        {
            _targets.Clear();
        }
    }
}