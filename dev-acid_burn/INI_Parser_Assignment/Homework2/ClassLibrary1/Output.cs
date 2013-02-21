// Jennifer Mendez
// CptS 323, Spring 2013
// Homework 2 - INI File Reader
// Filename: Output.cs
//#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IniLibrary
{
    // Admittedly, my class and function names are not that great.  
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
        /// Reads an INI format file, checks for correct syntax in groups 
        /// and keywords, appends a prefix to all keys and groups, then writes 
        /// it all back to the modified file.  
        /// 
        /// Other things to do with this function:
        /// The function is long and would benefit from being broken up.  
        /// 
        /// I'd like to put the targets into a "Target" class that stores the key
        /// value pairs and then have a seperate class that writes ini files
        /// but the need to keep the comments intact makes that a more difficult
        /// task and since we won't need to track the comments from the ini
        /// file for the project, I'll just keep this function this way for now.  
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        public void ParseIniFile(string filepath)
        {
            // Other ways to handle this:
            // I'd like to put the targets into a class that has attributes for 
            // key value pairs, but 

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
#if DEBUG
                        printer.Print(line);
#endif
                        writer.WriteLine(line);
                    }
                    else if (line[0] == ';')
                    {
                        // This is a comment, just write it as is
#if DEBUG
                        printer.Print(line);
#endif
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
                                // Exiting could be done better, 
                                // perhaps with an exception or returning to main
                                Console.ReadLine();
                                Environment.Exit(0);
                            }
                            else
                            {
                                // Its good, so prefix it
                                string prefixedline = line.Insert(1, "prefixedGroup-");
#if DEBUG
                                printer.Print(prefixedline);
#endif
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
                                // Exiting could be done better, 
                                // perhaps with an exception or returning to main
                                Console.ReadLine();
                                Environment.Exit(0);
                            }
                            else 
                            {
                                // Its good, so prefix the key
                                string prefixedline = line.Insert(0, "prefixedKey-");
#if DEBUG
                                printer.Print(prefixedline);
#endif  
                                writer.WriteLine(prefixedline);
                            }
                        }
                    }
                }
            }
        }
    }
}
