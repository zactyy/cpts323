using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsbLibrary;
using TurretControl;

namespace TurretManager
{
    class Program
    {
        static void Main(string[] args)
        {
            TurretManager m = new TurretManager();
            m.ResetToOrigin();
    
            Console.WriteLine("Please enter operation:");
            string line = Console.ReadLine();
            while (!line.ToLower().Contains("quit"))
            {
                if (line.ToLower().Contains("fire"))
                {
                    m.Fire();
                }
                else if (line.ToLower().Contains("reset"))
                {
                    m.ResetToOrigin();
                }

                if (line.ToLower().Contains("left"))
                {
                    Console.WriteLine("Please enter angle:");
                    int i = Convert.ToInt32(Console.ReadLine());
                    m.DecreaseAzimuth(i);
                }
                if (line.ToLower().Contains("right"))
                {
                    Console.WriteLine("Please enter angle:");
                    int i = Convert.ToInt32(Console.ReadLine());
                    m.IncreaseAzimuth(i);
                }
                if (line.ToLower().Contains("up"))
                {
                    Console.WriteLine("Please enter angle:");
                    int i = Convert.ToInt32(Console.ReadLine());
                    m.IncreaseAttitude(i);
                }
                if (line.ToLower().Contains("down"))
                {
                    Console.WriteLine("Please enter angle:");
                    int i = Convert.ToInt32(Console.ReadLine());
                    m.DecreaseAttitude(i);
                }
                int [] status= m.CurrentPosition();
                Console.WriteLine("\nCurrent Position: ThetaX {0} - ThetaY {1}", status[0], status[1]);

                Console.WriteLine("Please enter operation:");
                line = Console.ReadLine();
            }


        }
    }

    
}
