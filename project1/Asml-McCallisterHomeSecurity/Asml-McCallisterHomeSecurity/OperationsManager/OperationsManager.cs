using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        OperationsManager()
        {
            // Create all needed objects

            // Zachary - Create a Turret object here 
            // Chris - Create a Reader object here
        }
        
        // Interface with the Turret - for Manual Operation
        public void TurretMoveLeft();
        public void TurretMoveRight();
        public void TurretMoveUp();
        public void TurretMoveDown();
        public void TurretFire();

        // Interface with the File Reader(s)
        public void LoadFile(string targetfile);
        
        // Interface with Target Manager
        public void GetTargetInfo();
        public void LocatedTarget(int x, int y, int z, bool friend);  // Won't be called by GUI, but by the video analyzer piece
    }
}
