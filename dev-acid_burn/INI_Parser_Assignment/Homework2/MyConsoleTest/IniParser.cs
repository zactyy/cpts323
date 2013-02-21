// Jennifer Mendez
// CptS 323, Spring 2013
// Homework 2 - INI File Reader
// Filename: IniParser.cs
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
        /// This is simply an interface to the IniLibrary.  
        /// This class checks the extension of the file name passed, then 
        /// calls the inifileparser to deal with the file.  
        /// </summary>
        /// <param name="args">the name of an ini file to be parsed</param>
        
        public static void Main(string[] args)
        {
            Output temp = new Output();
            
            //check for valid file type
            string extension = Path.GetExtension(args[0]);
            if (extension.Equals(".ini", StringComparison.CurrentCultureIgnoreCase) )
            {
                temp.ParseIniFile(args[0]);
            }
            else
            {
                Console.Write("Invalid File Type '");
                Console.Write(extension);
                Console.WriteLine("'");
            }
            Console.ReadLine();
        }
    }
}