// TurretManager.cs
// The TurretManager is a singleton manages an ILauncher
// it provides tracking of positions and acknowledges
// degree agruement adjustments to 
// CptS323, Spring 2013
// Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
// Written by: Zachary Tyynismaa
// Last modified by: Zachary Tyynismaa
// Date modified: March 21, 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurretManagement
{
    public class TurretManager// class manages turret status
    {
        /// <summary>
        /// Singleton reference to track creation of 
        /// one instance of Turret Manager.
        /// </summary>
        private static TurretManager _instance;

        public static TurretManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TurretManager();
            }
            return _instance;
        }

        /// <summary>
        /// Turret manager controls a Turret, this is it
        /// </summary>
        private ILauncher ActiveTurret;

        /// <summary>
        /// azimuth value or angle on the x plane
        /// with respect to the origin,  holds current
        /// value so we know current position
        /// </summary>
        private int ThetaX
        {
            get;
            set;
        }

        /// <summary>
        /// attitude value or angle on the y plane
        /// with respect to the origin,  holds current
        /// value so we know current position
        /// </summary>
        private int ThetaY 
        {
            get;
            set;
        }
        /// <summary>
        /// class constructor
        /// </summary>
        private TurretManager(){
            //instantiate a turret to control
            ActiveTurret = Turret.GetInstance();
            //reset turret so its ready
            this.ResetToOrigin();
        }

        /// <summary>
        /// take an integer argument for the number of
        /// degrees and raises the turret by the corresponding amount
        /// </summary>
        /// <param name="degrees">number of degrees to move</param>
        public void IncreaseAttitude(int degrees)
        {
            ThetaY += degrees;
            ActiveTurret.command_Up(Convert.ToInt32(degrees * 50));
        }
        /// <summary>
        /// take an integer argument for the number of
        /// degrees and lowers the turret by the corresponding amount
        /// </summary>
        /// <param name="degrees">number of degrees to move</param>
        public void DecreaseAttitude(int degrees)
        {
            ThetaY -= degrees;
            ActiveTurret.command_Down(Convert.ToInt32(degrees * 50));
        }

        /// <summary>
        /// take an integer argument for the number of
        /// degrees and rotates the turret by the corresponding amount
        /// to the right
        /// </summary>
        /// <param name="degrees">number of degrees to move</param>
        public void IncreaseAzimuth(int degrees)
        {
            ThetaX += degrees;
            ActiveTurret.command_Right(Convert.ToInt32(degrees * 20.4));
        }

        /// <summary>
        /// take an integer argument for the number of
        /// degrees and rotates the turret by the corresponding amount
        /// to the left
        /// </summary>
        /// <param name="degrees">number of degrees to move</param>
        public void DecreaseAzimuth(int degrees)
        {
            ThetaX -= degrees;
            ActiveTurret.command_Left(Convert.ToInt32(degrees * 20.4));
        }

        /// <summary>
        /// Moves the turret back to level and center position (origin)
        /// </summary>
        public void ResetToOrigin()//moves turret back to 0,0
        {
            ThetaX = 0;
            ThetaY = 0;
            ActiveTurret.command_reset();
        }

        /// <summary>
        /// Given a required firing will move turret to closest
        /// possible position
        /// </summary>
        /// <param name="NewThetaX">required azimuth</param>
        /// <param name="NewThetaY">reguired attitude</param>
        public void AssumeFiringPosition(int NewThetaX,int NewThetaY)
        {
            // TODO need catch for out of range angles

            // find how much turret must move from current position
            NewThetaX = ThetaX - NewThetaX;
            NewThetaY = ThetaY - NewThetaY;
            ModifyAttitude(NewThetaY);
            ModifyAzimuth(NewThetaX);
        }

        /// <summary>
        /// determines the appropiate azimuth modification
        /// </summary>
        /// <param name="MovementValue"> azimuth change needed</param>
        public void ModifyAzimuth(int MovementValue)
        {
            // negative change
            if(MovementValue<=0)
            {
                //if so decrease azimuth
                DecreaseAzimuth(Math.Abs(MovementValue));
                return;
            }
            //if not increase azimuth
            IncreaseAzimuth(MovementValue);
        }

        /// <summary>
        /// determines the appropiate attitude modification
        /// </summary>
        /// <param name="MovementValue"> attitude change needed</param>
        public void ModifyAttitude(int MovementValue)
        {
            // negative change
            if (MovementValue <= 0)
            {
                //if so decrease attitude
                DecreaseAttitude(Math.Abs(MovementValue));
                return;
            }
            //if not increase attitude
            IncreaseAttitude(MovementValue);
        }

        /// <summary>
        /// returns current position of turret
        /// </summary>
        /// <returns> array of current turret angles</returns>
        public int[] CurrentPosition()// returns current position 
        {
            int[] Angles= {0,0};
            Angles[0] = this.ThetaX;
            Angles[1] = this.ThetaY;
            return Angles;
        }
        /// <summary>
        /// issues fire command
        /// </summary>
        public void Fire()
        {
            ActiveTurret.command_Fire();
        }

         ~TurretManager()
        {
            this.Dispose(false);
        }

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
                ((IDisposable)this.ActiveTurret).Dispose();
            }
            _instance = null;
        }
    }
}
