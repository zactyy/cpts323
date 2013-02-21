using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TargetClass;

namespace File_Readers.Data
{
    public class XML_File_Reader : File_Reader_Base
    {


                /// <summary>
        /// Writes an INI file type that contains the
        /// Targets in the list passed in.  
        /// </summary>
        /// <param name="targets_to_write">list of targets to write</param>
        /// <param name="file_to_write">file targets will be written to</param>
        public override void Write_File(List<ActualTarget> targets_to_write, string file_to_write)
        {

        }

        /// <summary>
        /// Parses an INI file to extract Target information
        /// </summary>
        /// <param name="lines">a string array of lines to be parsed</param>
        /// <returns>A list of Targets</returns>
        public override List<ActualTarget> Acquire_Targets(string file_to_read)
        {

            List<ActualTarget> targetList = new List<ActualTarget>();
            //ActualTarget friendTarget("friendTarget", 10, 11, 12, true);
            //ActualTarget foeTarget("foeTarget", 20, 21, 22, false);
            //ActualTarget nonameTarget(30, 31, 32, true);
            //targetList.Add(friendTarget);
            targetList.Add(new ActualTarget(100, 200, 300, true));
            targetList.Add(new ActualTarget("friendTarget", 10, 11, 12, true));
            targetList.Add(new ActualTarget("foeTarget", 20, 21, 22, false));

            return targetList;
        }

        /// <summary>
        /// Reads an XML file.
        /// </summary>
        /// <param name="path"></param>
        static void ReadXMLFile(string path)
        {
            // Create a file stream so that we can read the data.
            using (XmlTextReader reader = new XmlTextReader(path))
            {
                // The load the document DOM
                XmlDocument document = new XmlDocument();
                document.Load(reader);

                // Grab the first node
                XmlNode mainNode = document.FirstChild;

                // Then get the list of nodes containing the data we want. 
                XmlNodeList nodes = mainNode.ChildNodes;

                foreach (XmlNode node in nodes)
                {
                    double yPos = Convert.ToDouble(node.Attributes["yPos"].Value);
                    double xPos = Convert.ToDouble(node.Attributes["xPos"].Value);
                    double zPos = Convert.ToDouble(node.Attributes["zPos"].Value);
                    bool isFriend = Convert.ToBoolean(node.Attributes["isFriend"].Value);

                    XmlAttribute attribute = node.Attributes[0];
                    Console.WriteLine("x: {0} y: {1} z: {2}  Is Friend: {3}  node: {4}", xPos, yPos, zPos, isFriend, node.Attributes[0]);
                }
            }
        }

            //end XML_File_Reader class definition
    }
}
