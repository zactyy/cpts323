// ILauncher.cs
// The provides adapters and interfaces for ILauncher, IMissileLauncher adn MissileLauncherAdapter
// CptS323, Spring 2013
// Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
// Written by: Zachary Tyynismaa
// Last modified by: Zachary Tyynismaa
// Date modified: April 10, 2013


using System;
using System.Threading;
using UsbLibrary;
using ASMLEngineSdk;

namespace TurretManagement
{
    

    public class MissileLauncherAdapter : IMissileLauncher
    {
        TurretManager m_launcher;
        public MissileLauncherAdapter()
        {
            m_launcher = TurretManager.GetInstance();
        }

        public void Reset()
        {
            m_launcher.ResetToOrigin();
        }

        public void Fire()
        {
            m_launcher.Fire();
        }

        public void MoveTo(double phi, double theta)
        {
            m_launcher.AssumeFiringPosition(Convert.ToInt32(phi), Convert.ToInt32(theta));

        }

        public void MoveBy(double phi, double theta)
        {
            m_launcher.ModifyAttitude(Convert.ToInt32(phi));
            m_launcher.ModifyAzimuth(Convert.ToInt32(theta));
        }

        public double Phi
        {
            get
            {
                return Convert.ToDouble(m_launcher.CurrentPosition()[1]);
            }
        }

        public double Theta
        {
            get
            {
                return Convert.ToDouble(m_launcher.CurrentPosition()[0]);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose_others)
        {
            if (dispose_others == true)
            {
                this.m_launcher = null;
            }
        }

        ~MissileLauncherAdapter()
        {
            this.Dispose(false);
        }
    }


}
