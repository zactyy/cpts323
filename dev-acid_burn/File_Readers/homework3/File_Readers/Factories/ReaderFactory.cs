
using File_Readers.Data;

namespace File_Readers.Factories
{
    /// <summary>
    /// Class for creating file readers/parsers.  
    /// </summary>
    public class Reader_Factory
    {
        /// <summary>
        /// Create a reader based on a type directive.  
        /// </summary>
        /// <param name="reader_type"></param>
        /// <returns></returns>
        public File_Reader_Base Create_Reader(File_Types reader_type)
        {
            File_Reader_Base reader = null;

            switch (reader_type)
            {
                case File_Types.ini:
                    reader = new INI_File_Reader();
                    break;
                case File_Types.xml:
                    reader = new XML_File_Reader();
                    break;
                default:
                    break;
             }
            return reader;
        }
    }
}