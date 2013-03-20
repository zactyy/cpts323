/*
 * FileProcessorFactory.cs
 * implements a factory class for fileprocessors.
 * Written for CptS323 at Washington State University, Spring 2013
 * Team McCallister Home Security: Chris Walters, Jennifer Mendez, Zachary Tynnisma
 * Written By: Chris Walters
 * Last Modified By: Chris Walters
 * Date Modified: March 18, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Asml_McCallisterHomeSecurity.FileProcessors
{
    class FileProcessorFactory:IDisposable 
    {
        private static FileProcessorFactory _instance = null;

        private FileProcessorFactory() { }

        public static FileProcessorFactory GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FileProcessorFactory();
            }
            return _instance;        
        }

        /// <summary>
        /// finalize method
        /// </summary>
        ~FileProcessorFactory()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// public dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// actual diposal method.
        /// </summary>
        /// <param name="dispose_others"></param>
        protected void Dispose(bool dispose_others)
        {
            if (dispose_others == true)
            {
                //nothing to do here...
            }
            _instance = null;
        }

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
                throw new ArgumentException("Invalid file. Targets may be loaded only from INI or XML file types.");
            }
            return createdProcessor;
        }
    }
}