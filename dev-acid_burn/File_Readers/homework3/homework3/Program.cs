using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File_Readers.Data;
using File_Readers.Factories;
using TargetClass;
using System.IO;

namespace homework3
{
    class Program
    {


        /// <summary>
        /// This program interfaces with the File_Readers class library.  
        /// 
        /// </summary>
        /// <param name="args">name of the file to be parsed</param>
        static void Main(string[] args)
        {
            List<ActualTarget> The_Target_List = new List<ActualTarget>();
            File_Types fileType;
            Reader_Factory Factory = new Reader_Factory();
            
            string[]words = args[0].Split('.');
            // In case there are dots in the file name, takes the stuff after last dot
            string extension = words[words.Length - 1];
            if (Enum.TryParse(extension, true, out fileType) )
            {
                if (Enum.IsDefined(typeof(File_Types), fileType))
                {
                    File_Reader_Base reader = Factory.Create_Reader(fileType);
                    reader.Acquire_Targets(args[0]);
                }
                else
                {
                    Console.WriteLine("Invalid file.");
                }
            }
            else
            {
                Console.WriteLine("Invalid file.");
            }
            Console.ReadLine();// TODO: REMOVE
            
        }
    }
}
