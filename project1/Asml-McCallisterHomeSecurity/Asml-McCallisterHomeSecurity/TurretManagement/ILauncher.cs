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


}
