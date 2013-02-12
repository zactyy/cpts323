using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniLibrary;
using System.IO;

namespace IniParser
{
    public class IniParser
    {
        /// <summary>
        /// make it read two different files, prefix them, 
        /// to add comd line args, right click on the solution properties
        /// 
        /// to 
        /// 
        /// </summary>
        /// <param name="args"></param>
        
        public static void Main(string[] args)
        {
            Output temp = new Output();
            
            //REMOVE
            //string[] lines = File.ReadAllLines(args[0]);
            //string[] lines = File.ReadAllLines("filename");

            //check for valid file type
            string extension = Path.GetExtension(args[0]);
            if (!extension.Equals("ini", StringComparison.OrdinalIgnoreCase))
            {
                temp.ParseIniFile(args[0]);
            }
            else
            {
                Console.Write("Invalid File Type [");
                Console.Write(extension);
                Console.WriteLine("]");
            }

            /*
             * REMOVE
            foreach (string line in lines) 
            {
                //temp.Print(line);
                temp.ReadFile(args[0]);
            }
             * */

            Console.ReadLine();
        }
    }
}