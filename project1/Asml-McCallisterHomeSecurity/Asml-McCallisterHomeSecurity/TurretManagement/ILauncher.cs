using System;
using System.Threading;
using UsbLibrary;

namespace Asml_McCallisterHomeSecurity.TurretManagement
{
    public interface ILauncher
    {
        void command_Right(int degrees);
        void command_Left(int degrees);
        void command_Up(int degrees);
        void command_Down(int degrees);
        void command_Fire();
        void command_reset();
    }

    /// <summary>
    /// Interface for controlling a missile launcher.
    /// </summary>
    public interface IMissileLauncher
    {
        /// <summary>
        /// Resets the missile launcher 
        /// </summary>
        void Reset();
        /// <summary>
        /// Moves the missile launcher by a relative amount.
        /// </summary>
        /// <param name="phi"></param>
        /// <param name="phi"></param>
        void MoveBy(double phi, double psi);
        /// <summary>
        /// Moves the missile launcher to an absolute position.
        /// </summary>
        /// <param name="phi"></param>
        /// <param name="psi"></param>
        void MoveTo(double phi, double psi);
        /// <summary>
        /// Fires a missile.
        /// </summary>
        void Fire();
        /// <summary>
        /// Gets the phi position of the missile launcher.
        /// </summary>
        double Phi { get; }
        /// <summary>
        /// Gets the psi position of the missile launcher.
        /// </summary>
        double Psi { get; }
    }

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

        public void MoveTo(double phi, double psi)
        {
            m_launcher.AssumeFiringPosition(Convert.ToInt32(phi), Convert.ToInt32(psi));

        }

        public void MoveBy(double phi, double psi)
        {
            m_launcher.ModifyAttitude(Convert.ToInt32(phi));
            m_launcher.ModifyAzimuth(Convert.ToInt32(psi));
        }

        public double Phi
        {
            get
            {
                return Convert.ToDouble(m_launcher.CurrentPosition()[1]);
            }
        }

        public double Psi
        {
            get
            {
                return Convert.ToDouble(m_launcher.CurrentPosition()[0]);
            }
        }
    }


}
