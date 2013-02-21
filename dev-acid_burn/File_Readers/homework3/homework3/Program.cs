using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File_Readers.Data;

namespace homework3
{
    class Program
    {
        /// <summary>
        /// This program interfaces with the File_Readers class library.  
        /// The file name passed to the 
        /// 
        /// </summary>
        /// <param name="args">name of the file to be parsed</param>
        static void Main(string[] args)
        {
            File_Reader file = new File_Reader(arg[0]);
            
        }
    }
}
