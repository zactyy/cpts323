using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IniLibrary
{
    public class Output
    {
        /// <summary>
        /// display a to the console
        /// </summary>
        /// <param name="message"> the string that will be printed </param>
        public void Print(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Reads a INI format file, checks for correct syntax in groups 
        /// and keywords, appends a prefix to all keys and groups, then writes 
        /// it all back to the modified file.  
        /// </summary>
        /// <param name="filepath"></param>
        public void ParseIniFile(string filepath)
        {
            //TODO: Clean this function up - break it up?

            // Create new file name
            string extension = Path.GetExtension(filepath);
            string outputPath = filepath.Replace(extension, "-modified" + extension);

            // Read file, check for correct syntax, write to modified file      
            string[] lines = File.ReadAllLines(filepath);
            using (TextWriter writer = File.CreateText(outputPath))
            {
                Output printer = new Output();
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        // blank line, so don't prefix
                        printer.Print(line);
                        writer.WriteLine(line);
                    }
                    else if (line[0] == ';')
                    {
                        // This is a comment, just write it as is
                        printer.Print(line);
                        writer.WriteLine(line);
                    }
                    else
                    {
                        // Not whitespace or comment, so check to ensure proper INI format
                        // Check for [ at start of Group 
                        // It could be a [ or a key or invalid
                        if (line[0] == '[')
                        {
                            // Check for closing bracket
                            if (line.IndexOf(']') < 0)
                            {
                                // Invalid tag, reject file
                                Console.Write("Invalid format '");
                                Console.Write(line);
                                Console.WriteLine("'");
                                // TODO: finda better way - return to main?
                                Console.ReadLine();
                                Environment.Exit(0);
                            }
                            else
                            {
                                // Its good, so prefix it
                                string prefixedline = line.Insert(1, "prefixedGroup-");
                                printer.Print(prefixedline);
                                writer.WriteLine(prefixedline);
                            }
                        }
                        else
                        {
                            // Its not whitespace, comments or a group
                            // so it must be a key value pair.
                            // check for an =, if there isn't one, its invalid.
                            if (line.IndexOf('=') < 0)
                            {
                                Console.Write("Invalid format '");
                                Console.Write(line);
                                Console.WriteLine("'");
                                // TODO: finda better way - return to main?
                                Console.ReadLine();
                                Environment.Exit(0);
                            }
                            else 
                            {
                                // Its good, so prefix the key
                                //REMOVEstring prefixedline = "prefixed-" + line;
                                string prefixedline = line.Insert(0, "prefixedKey-");
                                printer.Print(prefixedline);
                                writer.WriteLine(prefixedline);
                            }
                        }
                    }
                }
            }
        }
    }
}
