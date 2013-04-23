// TurretManager.cs
// The TurretManager is a singleton manages an ILauncher
// it provides tracking of positions and acknowledges
// degree agruement adjustments to 
// CptS323, Spring 2013
// Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
// Written by: Zachary Tyynismaa
// Last modified by: Chris Walters
// Date modified: April 23, 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UsbLibrary;

namespace TurretManagement
{
    public class TurretManager// class manages turret status
    {
        /// <summary>
        /// Singleton reference to track creation of 
        /// one instance of Turret Manager.
        /// </summary>
        private static TurretManager _instance;

        private bool DevicePresent;
        //Bytes used in command
        private byte[] UP;
        private byte[] RIGHT;
        private byte[] LEFT;
        private byte[] DOWN;

        private byte[] FIRE;
        private byte[] STOP;
        private byte[] LED_OFF;
        private byte[] LED_ON;

        private UsbHidPort USB;

        public static TurretManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TurretManager();
            }
            return _instance;
        }


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
        private TurretManager()
        {
            this.UP = new byte[10];
            this.UP[1] = 2;
            this.UP[2] = 2;

            this.DOWN = new byte[10];
            this.DOWN[1] = 2;
            this.DOWN[2] = 1;

            this.LEFT = new byte[10];
            this.LEFT[1] = 2;
            this.LEFT[2] = 4;

            this.RIGHT = new byte[10];
            this.RIGHT[1] = 2;
            this.RIGHT[2] = 8;

            this.FIRE = new byte[10];
            this.FIRE[1] = 2;
            this.FIRE[2] = 0x10;

            this.STOP = new byte[10];
            this.STOP[1] = 2;
            this.STOP[2] = 0x20;

            this.LED_ON = new byte[9];
            this.LED_ON[1] = 3;
            this.LED_ON[2] = 1;

            this.LED_OFF = new byte[9];
            this.LED_OFF[1] = 3;

            this.USB = new UsbHidPort();
            this.USB.ProductId = 0;
            this.USB.SpecifiedDevice = null;
            this.USB.VendorId = 2123;
            this.USB.OnSpecifiedDeviceRemoved += new EventHandler(this.USB_OnSpecifiedDeviceRemoved);
            this.USB.OnDataRecieved += new DataRecievedEventHandler(this.USB_OnDataRecieved);
            this.USB.OnSpecifiedDeviceArrived += new EventHandler(this.USB_OnSpecifiedDeviceArrived);

            this.USB.VID_List[0] = 0xa81;
            this.USB.PID_List[0] = 0x701;
            this.USB.VID_List[1] = 0x2123;
            this.USB.PID_List[1] = 0x1010;
            this.USB.ID_List_Cnt = 2;

            IntPtr handle = new IntPtr();
            this.USB.RegisterHandle(handle);
            /*this reset was causing issues at startup, so changed where the reset occurs --Chris*/
            //reset turret so its ready
            //this.ResetToOrigin();
        }

        /// <summary>
        /// take an integer argument for the number of
        /// degrees and raises the turret by the corresponding amount
        /// </summary>
        /// <param name="degrees">number of degrees to move</param>
        public void IncreaseAttitude(int degrees)
        {
            ThetaY += degrees;
            this.command_Up(Convert.ToInt32(degrees * 50));
        }
        /// <summary>
        /// take an integer argument for the number of
        /// degrees and lowers the turret by the corresponding amount
        /// </summary>
        /// <param name="degrees">number of degrees to move</param>
        public void DecreaseAttitude(int degrees)
        {
            ThetaY -= degrees;
            this.command_Down(Convert.ToInt32(degrees * 50));
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
            this.command_Right(Convert.ToInt32(degrees * 20.4));
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
            this.command_Left(Convert.ToInt32(degrees * 20.4));
        }

        /// <summary>
        /// Moves the turret back to level and center position (origin)
        /// </summary>
        public void ResetToOrigin()//moves turret back to 0,0
        {
            ThetaX = 0;
            ThetaY = 0;
            this.command_reset();
        }

        /// <summary>
        /// Given a required firing will move turret to closest
        /// possible position
        /// </summary>
        /// <param name="NewThetaX">required azimuth</param>
        /// <param name="NewThetaY">reguired attitude</param>
        public void AssumeFiringPosition(int NewThetaX, int NewThetaY)
        {
            // TODO need catch for out of range angles

            // find how much turret must move from current position
            if (ThetaX > 0 && NewThetaX > ThetaX)
            {
                NewThetaX = ThetaX + (NewThetaX - ThetaX);
            }
            else if (ThetaX > 0 && NewThetaX < ThetaX)
            {
                NewThetaX = NewThetaX - ThetaX;
            }
            else if (ThetaX < 0 && NewThetaX < ThetaX)
            {
                NewThetaX = NewThetaX - ThetaX;
            }
            else if (ThetaX < 0 && NewThetaX > ThetaX)
            {
                NewThetaX = NewThetaX - ThetaX;
            }
            else if (ThetaX > 0 && NewThetaX < ThetaX)
            {
                NewThetaX = ThetaX - NewThetaX;
            }

            // find how much turret must move from current position
            if (ThetaY > 0 && NewThetaY > ThetaY)
            {
                NewThetaY = ThetaY + (NewThetaY - ThetaY);
            }
            else if (ThetaY > 0 && NewThetaY < ThetaY)
            {
                NewThetaY = NewThetaY - ThetaY;
            }
            else if (ThetaY < 0 && NewThetaY < ThetaY)
            {
                NewThetaY = NewThetaY - ThetaY;
            }
            else if (ThetaY < 0 && NewThetaY > ThetaY)
            {
                NewThetaY = NewThetaY - ThetaY;
            }
            else if (ThetaY > 0 && NewThetaY < ThetaY)
            {
                NewThetaY = ThetaY - NewThetaY;
            }
            /* check to make sure it's within movement range*/
            if (Math.Abs(NewThetaX) > 120)
            {
                if (NewThetaX < 0)
                {
                    NewThetaX = -120 + Math.Abs(ThetaX);
                }
                else
                {
                    NewThetaX = 120 - Math.Abs(ThetaX);
                }
            }
            else if (NewThetaY > 45)
            {
                NewThetaY = 45 - Math.Abs(ThetaY);
            }
            else if (NewThetaY < -20)
            {
                NewThetaY = -20 + Math.Abs(ThetaY);
            }
            if (NewThetaY != ThetaY)
            {
                ModifyAttitude(NewThetaY);
            }
            if (NewThetaX != ThetaX)
            {
                ModifyAzimuth(NewThetaX);
            }
        }

        /// <summary>
        /// determines the appropiate azimuth modification
        /// </summary>
        /// <param name="MovementValue"> azimuth change needed</param>
        public void ModifyAzimuth(int MovementValue)
        {
            // negative change
            if (MovementValue <= 0)
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
            int[] Angles = { 0, 0 };
            Angles[0] = this.ThetaX;
            Angles[1] = this.ThetaY;
            return Angles;
        }
        /// <summary>
        /// issues fire command
        /// </summary>
        public void Fire()
        {
            this.command_Fire();
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
               // is there anything we need to check?
            }
            _instance = null;
        }



        /// <summary>
        /// class constructor
        /// </summary>

        /// <summary>
        /// sends stop command to turret
        /// </summary>
        private void command_Stop()
        {
            this.SendUSBData(this.STOP);
        }

        /// <summary>
        /// sends right command to turret
        /// </summary>
        /// <param name="degrees"></param>
        private void command_Right(int degrees)
        {
            this.moveMissileLauncher(this.RIGHT, degrees);
        }

        /// <summary>
        /// sends left command to turret
        /// </summary>
        /// <param name="degrees"></param>
        private void command_Left(int degrees)
        {
            this.moveMissileLauncher(this.LEFT, degrees);
        }

        /// <summary>
        /// sends up command to turret
        /// </summary>
        /// <param name="degrees"></param>
        private void command_Up(int degrees)
        {
            this.moveMissileLauncher(this.UP, degrees);
        }

        /// <summary>
        /// sends down command to turret
        /// </summary>
        /// <param name="degrees"></param>
        private void command_Down(int degrees)
        {
            this.moveMissileLauncher(this.DOWN, degrees);
        }

        /// <summary>
        /// sends fire command to turret
        /// </summary>
        private void command_Fire()
        {
            this.moveMissileLauncher(this.FIRE, 5000);
        }
        /// <summary>
        /// toggles led on turret
        /// </summary>
        /// <param name="turnOn"></param>
        private void command_switchLED(Boolean turnOn)
        {
            if (DevicePresent)
            {
                if (turnOn)
                {
                    this.SendUSBData(this.LED_ON);
                }
                else
                {
                    this.SendUSBData(this.LED_OFF);
                }
                this.SendUSBData(this.STOP);
            }
        }

        /// <summary>
        /// resets turret position
        /// </summary>
        private void command_reset()
        {
            if (DevicePresent)
            {
                this.moveMissileLauncher(this.LEFT, 5500);
                this.moveMissileLauncher(this.RIGHT, 2750);
                this.moveMissileLauncher(this.UP, 2000);
                this.moveMissileLauncher(this.DOWN, 500);
            }
        }
        /// <summary>
        /// sends action commands to turret
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="interval"></param>
        private void moveMissileLauncher(byte[] Data, int interval)
        {
            if (DevicePresent)
            {
                this.command_switchLED(true);
                this.SendUSBData(Data);
                Thread.Sleep(interval);
                this.SendUSBData(this.STOP);
                this.command_switchLED(false);
            }
        }

        /// <summary>
        /// sends command data to turret
        /// </summary>
        /// <param name="Data"></param>
        private void SendUSBData(byte[] Data)
        {
            if (this.USB.SpecifiedDevice != null)
            {
                this.USB.SpecifiedDevice.SendData(Data);
            }
        }

        /// <summary>
        /// the below are usb event handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void USB_OnDataRecieved(object sender, DataRecievedEventArgs args)
        {

        }

        private void USB_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {
            this.DevicePresent = true;
            if (this.USB.ProductId == 0x1010)
            {
                this.command_switchLED(true);
            }
        }

        private void USB_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {
            this.DevicePresent = false;
        }

    }
}
