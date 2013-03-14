/*
 * FileProcessorFactory.cs
 * implements a factory class for fileprocessors.
 * Written for CptS323 at Washington State University, Spring 2013
 * Team McCallister Home Security: Chris Walters, Jennifer Mendez, Zachary Tynnisma
 * Written By: Chris Walters
 * Last Modified By: Chris Walters
 * Last Modified On: March 13, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Asml_McCallisterHomeSecurity.FileProcessors
{
    class FileProcessorFactory
    {
        /// <summary>
        /// Factory method, Creates a file processor for the given file type, or throws an
        /// ArgumentException if the file type is unsupported.
        /// </summary>
        /// <param name="filePath">A string containing the file to be processed.</param>
        /// <returns>A file processor of the proper type for the given file, ready to process said 
        /// file.</returns>
        public FileProcessor Create(string filePath)
        {
            FileProcessor createdProcessor = null;
            /* grab the extension of the file, and then based on that extension either return the proper
             * proper processor or throw an exception, if invalid file type.
             */
            string ext = Path.GetExtension(filePath);
            if (ext == ".ini")
            {
                createdProcessor = new IniProcessor(filePath);
            }
            else if (ext == ".xml")
            {
                createdProcessor = new XMLProcessor(filePath);
            }
            else
            {
                throw new ArgumentException("Invalid file.");
            }
            return createdProcessor;
        }
    }
}