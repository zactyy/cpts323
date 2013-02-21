using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using TargetClass;

namespace File_Readers.Data
{
    public class INI_File_Reader : File_Reader_Base
    {
        /// <summary>
        /// Writes an INI file type that contains the
        /// Targets in the list passed in.  
        /// </summary>
        /// <param name="targets_to_write">list of targets to write</param>
        /// <param name="file_to_write">file targets will be written to</param>
        public abstract void Write_File(List<ActualTarget> targets_to_write, string file_to_write)
        {
            using (TextWriter writer = File.CreateText(file_to_write))
            {
                foreach (ActualTarget Target in targets_to_write)
                {
                    writer.WriteLine("[Target]");
                    if (Target.Name != null)
                    {
                        writer.Write("Name = ");
                        writer.WriteLine(Target.Name);
                    }
                    writer.Write("x = ");
                    writer.WriteLine(Target.X_coordinate);
                    writer.Write("y = ");
                    writer.WriteLine(Target.Y_coordinate);
                    writer.Write("z = ");
                    writer.WriteLine(Target.Z_coordinate);
                    writer.Write("friend = ");
                    if (Target.Friend == true)
                    {
                        writer.WriteLine("yes");
                    }
                    else
                    {
                        writer.WriteLine("no");
                    }
                }
            }
        }

        /// <summary>
        /// Parses an INI file to extract Target information
        /// </summary>
        /// <param name="lines">a string array of lines to be parsed</param>
        /// <returns>A list of Targets</returns>
        abstract List<ActualTarget> Acquire_Targets(string file_to_read)
        {
            List<ActualTarget> target_list;
            ActualTarget tempTarget;
            string[] filelines = Read_File(file_to_read);
            int step = 0;

            foreach (string line in filelines)
            {
                // This is a mini-state machine that
                // steps through the format of the INI file.  
                // This only works if the key-value pairs are 
                // given in the exact order as specified in the 
                // example file good.ini.  Regular expressions
                // might be helpful here to remedy that.  or 
                // more clever programming!  
                // Maybe one step for all key-value pairs... 
                // but then no way to know if all coordinates are
                // given
                switch (step)
                {
                    case 0: // Look for Group
                        if (Is_Ignorable(line))
                        {
                            step = 0;
                        }
                        else if (Is_ValidGroup(line))
                        {
                            step = 1; // next should be key-value pairs
                        }
                        else
                        {
                            // TODO: throw exception
                            Console.WriteLine("Invalid INI file, bad format.");
                            step = 6;
                        }
                        break;
                    case 1:  // Get Name (or X if no name)
                        string value;
                        if (Is_KeyMatch(line, "name", ref value))
                        {
                            tempTarget.Name = Convert.ToInt32(value);
                            step = 2; // Now get x
                        }
                        else if (Is_KeyMatch(line, "x", ref value))
                        {
                            // checking for x since name may or may not be there.
                            tempTarget.X_coordinate = Convert.ToInt32(value);
                            step = 3; // Now get y
                        }
                        else
                        {
                            Console.WriteLine("Invalid INI file, bad format.");
                            // TODO: Throw exception here instead of step = 6? 
                            // or make it handle any order of key-value pairs
                            step = 6;
                        
                        }
                        break;
                    case 2:  // Get X
                        value = "";
                        if (Is_KeyMatch(line, "x", ref value))
                        {
                            // checking for x since name may or may not be there.
                            tempTarget.X_coordinate = Convert.ToInt32(value);
                            step = 3; // Now get y
                        }
                        else
                        {
                            Console.WriteLine("Invalid INI file, bad format.");
                            //TODO: Throw exception here instead of step = 6;
                            step = 6;
                        }
                        break;
                    case 3: // Get Y
                        value = "";
                        if (Is_KeyMatch(line, "y", ref value))
                        {
                            tempTarget.Y_coordinate = Convert.ToInt32(value);
                            step = 4; // Now get z
                        }
                        else
                        {
                            Console.WriteLine("Invalid INI file, bad format.");
                            //TODO: Throw exception here instead of step = 6;
                            step = 6;
                        }
                        break;
                    case 4:  // Get Z
                        value = "";
                        if (Is_KeyMatch(line, "z", ref value))
                        {
                            tempTarget.Z_coordinate = Convert.ToInt32(value);
                            step = 4; // Now get friend
                        }
                        else
                        {
                            Console.WriteLine("Invalid INI file, bad format.");
                            //TODO: Throw exception here instead of step = 6;
                            step = 6;
                        }
                        break;
                    case 5:  // Get Friend
                        if (Is_KeyMatch(line, "friend", ref value))
                        {
                            if (value == "yes")
                            {
                                tempTarget.Friend = true;
                            }
                            else
                            {
                                tempTarget.Friend = false;
                            }
                            // this is the last key-value pair, so 
                            // add target to list
                            target_list.Add(tempTarget);
                            // clear out target info
                            tempTarget.Name = null;
                            tempTarget.X_coordinate = 0;
                            tempTarget.Y_coordinate = 0;
                            tempTarget.Z_coordinate = 0;
                            tempTarget.Friend = true;
                            step = 0; // Look for more groups
                        }
                        else
                        {
                            Console.WriteLine("Invalid INI file, bad format.");
                            //TODO: Throw exception here instead of step = 6;
                            step = 6;
                        }
                        break;
                    case 6:
                        // just a dummy spot for invalid errors to go for now while the loop finishes
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Ensures the expectedkey matches the key in the line.  
        /// If match found, the reference variable value will contain 
        /// the value from the file.  
        /// </summary>
        /// <param name="line">line that should contain key-value pair</param>
        /// <param name="expectedkey">name of key expected</param>
        /// <param name="value">a reference variable to store the value</param>
        /// <returns>true if keyword matches, and value will contain the corresponding value</returns>
        private bool Is_KeyMatch(string line, string expectedkey, ref string value)
        {
            string[] words = line.Split(' ');
            if (Is_ValidKeyValue(words))
            {
                if (words[0].ToLower() == expectedkey)
                {
                    value = words[2];
                    return true;
                }
                return false;
            }
            return false; 
        }

        /// <summary>
        /// Utility function to determine whether a line
        /// should just be ignored.  
        /// </summary>
        /// <param name="line">a line from a file</param>
        /// <returns></returns>
        private bool Is_Ignorable(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return true;
            }
            else if (line[0] == ';')
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        /// <summary>
        /// Checks for open and close brackets 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true if valid group format</returns>
        private bool Is_ValidGroup(string line)
        {
            if (line[0] == '[')
            {
                // Check for closing bracket
                if (!(line.IndexOf(']') < 0))
                {
                    // Valid Group found 
                    // The next 5 lines (or 4?) are key-value pairs associated with this Target
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks for to ensure only 3 words and contains
        /// equals sign in middle, verifying valid INI format.  
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true if valid key-value format found</returns>
        private bool Is_ValidKeyValue(string[] words)
        {
            // split on spaces and ensure only 3 words 
            // that middle word is = for valid key-value pairs
            if (words.Length != 3)
            {
                return false;
            }
            else if (words[1] != "=")
            {
                return false;
            }
            return true;
        }


        //end INI_File_Reader Class
    }
}
