/*
 * FileProcessors.cs
 * implementation file for the abstract FileProcessor class.
 *  Written for CptS323 at Washington State University, Spring 2013
 *  Team McCallister Home Security: Chris Walters, Jennifer Mendez, Zachary Tynnisma
 *  Written By: Chris Walters
 *  Last Modified By: Chris Walters
 *  Date Modified: March 18, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Asml_McCallisterHomeSecurity.Targets;

namespace Asml_McCallisterHomeSecurity.FileProcessors
{
    /// <summary>
    /// FileProcessor abstract class
    /// used as a base for the IniProcessor and XMLProcessor classes.
    /// Requires processFile() to be implemented and contains a 
    /// factory method to create the proper processor for a given file.
    /// </summary>
    public abstract class FileProcessor
    {
        private string _file_path = null;

       // a concrete class must implement processFile to inherit the class.
       public abstract List<Target> ProcessFile();

        /// <summary>
        /// Automatic filepath property.
        /// </summary>
       public string FilePath
       {
           get;
           set;
       } 
    }
}