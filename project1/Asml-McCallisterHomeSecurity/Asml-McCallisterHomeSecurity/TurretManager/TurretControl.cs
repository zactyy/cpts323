using System;
using System.Threading;
using UsbLibrary;

namespace Asml_McCallisterHomeSecurity.TurretManagement
{
    /// <summary>
    /// this class is from the provided API
    /// </summary>
    public class Turret
    {
        // API confirms there is a device present
        private bool DevicePresent;

        // API ensures single instance
        private static Turret _instance;

        public static Turret GetInstance(){
            if (_instance == null)
            {
                _instance = new Turret();
            }
            return _instance;
        }

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

        /// <summary>
        /// class constructor
        /// </summary>
        private Turret()
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
            
        }

        /// <summary>
        /// sends stop command to turret
        /// </summary>
        public void command_Stop()
        {
            this.SendUSBData(this.STOP);
        }

        /// <summary>
        /// sends right command to turret
        /// </summary>
        /// <param name="degrees"></param>
        public void command_Right(int degrees)
        {
            this.moveMissileLauncher(this.RIGHT, degrees);
        }

        /// <summary>
        /// sends left command to turret
        /// </summary>
        /// <param name="degrees"></param>
        public void command_Left(int degrees)
        {
            this.moveMissileLauncher(this.LEFT, degrees);
        }

        /// <summary>
        /// sends up command to turret
        /// </summary>
        /// <param name="degrees"></param>
        public void command_Up(int degrees)
        {
            this.moveMissileLauncher(this.UP, degrees);
        }

        /// <summary>
        /// sends down command to turret
        /// </summary>
        /// <param name="degrees"></param>
        public void command_Down(int degrees)
        {
            this.moveMissileLauncher(this.DOWN, degrees);
        }

        /// <summary>
        /// sends fire command to turret
        /// </summary>
        public void command_Fire()
        {
            this.moveMissileLauncher(this.FIRE, 5000);
        }
        /// <summary>
        /// toggles led on turret
        /// </summary>
        /// <param name="turnOn"></param>
        public void command_switchLED(Boolean turnOn)
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
        public void command_reset()
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


