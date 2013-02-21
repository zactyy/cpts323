using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetClass;

namespace File_Readers.Data
{
    public abstract class File_Reader_Base
    {
        /// <summary>
        /// Opens a file, reads each line into an array of
        /// strings.  This function will store each line from 
        /// the filename given into an array of strings which
        /// is returned.  Should work the same for any reader.
        /// But if not, can be overridden.  
        /// </summary>
        /// <param name="filename">name of file to be read</param>
        /// <returns>array of strings containing lines from the file</returns>
        public virtual string[] Read_File(string filename)
        {
            //TODO check for path or file name? 
            string[] lines = File.ReadAllLines(filename);
            return lines;
        }

        /// <summary>
        /// display a msg to the console
        /// </summary>
        /// <param name="message"> the string that will be printed </param>
        public void Console_Print(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Each reader should also be able to write a file in its 
        /// specific format.  
        /// </summary>
        /// <param name="targets_to_write">list of targets to write</param>
        /// <param name="file_to_write">file targets will be written to</param>
        public abstract void Write_File(List<ActualTarget> targets_to_write, string file_to_write);

        /// <summary>
        /// The actual file parsing should happen here and return a list of Targets.
        /// </summary>
        /// <param name="lines">a string array of lines to be parsed</param>
        /// <returns>A list of Targets</returns>
        public abstract List<ActualTarget> Acquire_Targets(string file_to_read);

        ///// <summary>
        ///// REMOVE?  This is a Factory Method that will use the file extension 
        ///// passed in to determine which type of File_Reader object
        ///// should be created.  
        ///// </summary>
        ///// <param name="file_extension">extension of the file to be parsed</param>
        ///// <returns>An appropriate File_Reader object</returns>
        //public File_Reader_Base Factory_Method(string file_extension)
        //{
        //    // TODO: include dot in ext?  i can't remember
        //    if (file_extension == ".ini")
        //    {
        //        return new INI_File_Reader;
        //    }
        //    else if (file_extension == ".xml") 
        //    {
        //        return new XML_File_Reader;
        //    }
        //}
    }

    /// <summary>
    /// Types of files that can be read
    /// </summary>
    public enum File_Types
    {
        ini,
        xml
    }
}
