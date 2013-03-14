using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurretControl;
namespace TurretManager
{
    public class TurretManager// class manages turret status
    {
        static private bool instanceFlag = false; // ensures singleton status

        Turret ActiveTurret{//turret  managed
            get;
            set;
        }
        private int ThetaX// azimuth value
        {
            get;
            set;
        }
        private int ThetaY// attitude value
        {
            get;
            set;
        }
        public TurretManager(){//class constructor
            if (instanceFlag)
            {
                throw new SingletonException("Thats no Moon!!!! i.e thats a singleton dumbass!"); // throw exception
            }
            ActiveTurret = new Turret();//instantiate a turret to control
            this.ResetToOrigin();//reset turret so its ready
        }

        public void IncreaseAttitude(int degrees)
        {
            ThetaY += degrees;
            ActiveTurret.command_Up(Convert.ToInt32(degrees * 50));
        }
        public void DecreaseAttitude(int degrees)
        {
            ThetaY -= degrees;
            ActiveTurret.command_Down(Convert.ToInt32(degrees * 50));
        }
        public void IncreaseAzimuth(int degrees)
        {
            ThetaX += degrees;
            ActiveTurret.command_Right(Convert.ToInt32(degrees * 20.4));
        }
        public void DecreaseAzimuth(int degrees)
        {
            ThetaX -= degrees;
            ActiveTurret.command_Left(Convert.ToInt32(degrees * 20.4));
        }
        public void ResetToOrigin()//moves turret back to 0,0
        {
            ThetaX = 0;
            ThetaY = 0;
            ActiveTurret.command_reset();
        }
        public void AssumeFiringPosition(int NewThetaX,int NewThetaY)// will move turret to required thetas
        {
            // need catch for out of range angles
            NewThetaX = ThetaX - NewThetaX;
            NewThetaY = ThetaY - NewThetaY;
            ModifyAttitude(NewThetaY);
            ModifyAzimuth(NewThetaX);
        }
        public void ModifyAzimuth(int MovementValue)// determines the appropiate azimuth modification
        {
            if(MovementValue<=0)// negative change
            {
                DecreaseAzimuth(Math.Abs(MovementValue));
                return;
            }
            IncreaseAzimuth(MovementValue);
            return;
        }
        public void ModifyAttitude(int MovementValue)// determines the appropiate attitude modification
        {
            if (MovementValue <= 0)// negative change
            {
                DecreaseAttitude(Math.Abs(MovementValue));
                return;
            }
            IncreaseAttitude(MovementValue);
            return;
        }
        public int[] CurrentPosition()// returns current position 
        {
            int[] Angles= {0,0};
            Angles[0] = this.ThetaX;
            Angles[1] = this.ThetaY;
            return Angles;
        }
        public void Fire()//issues fire command
        {
            ActiveTurret.command_Fire();
        }
    }
}
