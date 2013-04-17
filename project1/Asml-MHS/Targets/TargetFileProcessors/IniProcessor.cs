/*
 * IniProcessor.cs
 * implements ini target reader class.
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
using System.Text.RegularExpressions;
using System.Xml;
using TargetManagement; // for Target object.

namespace TargetManagement.TargetFileProcessors
{
    public class IniProcessor:FileProcessor
    {
        private static string _invalid_ini_format_message = "invalid format";
      
        /// <summary>
        ///  Class constructor.
        /// </summary>
        /// <param name="fp">a string containing the filepath of the file to process.</param>
        public IniProcessor(string fp = null)
        {
            this.FilePath = fp;
        }

        /// <summary>
        /// Process ini file which processor was instantianted with.
        /// </summary>
        /// <returns> A List of Target objects</returns>
        /// <exception cref="InvalidIniFormat"></exception>
        public override List<Target> ProcessFile()
        {
            string[] fileLines = File.ReadAllLines(this.FilePath);
            List<Target> _output = new List<Target>();
            Target _current_target = null;
            foreach (string line in fileLines)
            {
                string trimedLine = line.Trim();
                if (trimedLine.StartsWith(";")) // ignore comments, continue on to next line.
                {
                    continue;
                }
                else if (string.IsNullOrEmpty(trimedLine)) // also ignore blank lines.
                {
                    continue;
                }
                else
                {
                    if (lineMatches(trimedLine)) // if line is a group declaration add new target to the list.
                    {
                        _current_target = new Target();
                        _output.Add(_current_target);
                    }
                    else // line is a key=value pair.
                    {
                        // 
                        if (_current_target == null)
                        {
                            throw new InvalidIniFormat(_invalid_ini_format_message);
                        }
                        string[] keyvalue = trimedLine.Split('=');
                        string key = keyvalue[0].Trim().ToLower(); // grab key, trim whitespace, and make all lower.
                        string value = keyvalue[1].Trim().ToLower(); // same as above but for value.
                        if (key == "friend") // if the left side of the keyvalue pair is "isFriend" set the friend value of the target.
                        {
                            TargetSetFriend(_current_target, value);
                        }
                        else if (key == "name") // if the left side of the keyvalue pair is "name" set the name of the target.
                        {
                          _current_target.Name = value;
                        }
                        else
                        {
                            
                            TargetSetPositionValue(_current_target, key, value);                            
                        }
                    }
                }
            }
        return _output;
        }

        /// <summary>
        /// Set x,y,z position value of target
        /// </summary>
        /// <param name="_current_target">A target object</param>
        /// <param name="key">string containing the key</param>
        /// <param name="value">string containing the value</param>
        private void TargetSetPositionValue(Target _current_target, string key, string value)
        {
            /* try/catch exceptions from Convert operation, solely for the purpose of changing them to InvalidIniFormat exceptions */
            try
            {
                /* turn the left side of the key value pair into ASCII and check to make sure it is either x, y, or z, then add value to target if possible. */
                switch (key)
                {
                    case "x":
                        _current_target.X_coordinate = Convert.ToDouble(value);
                        break;
                    case "y":
                        _current_target.Y_coordinate = Convert.ToDouble(value);
                        break;
                    case "z":
                        _current_target.Z_coordinate = Convert.ToDouble(value);
                        break;
                    default: // if it reaches this point, it is an invalid file
                        throw new InvalidIniFormat(_invalid_ini_format_message);
                }
            }
            catch (FormatException ex) // change exception from convert to invalidiniformat, but keep previous exception around for debugging.
            {
                throw new InvalidIniFormat(_invalid_ini_format_message, ex);
            }
            catch (OverflowException ex) // change exception from convert to invalidinitformat, but keep previous exception around for debugging.
            {
                throw new InvalidIniFormat(_invalid_ini_format_message, ex);
            }
        }

        /// <summary>
        /// Set the friend property
        /// </summary>
        /// <param name="_current_target">A target object</param>
        /// <param name="value">string containing "true" or "false"</param>
        private void TargetSetFriend(Target _current_target, string value)
        {
            if (value == "yes")
            {
                _current_target.Friend = true;
            }
            else if (value == "no")
            {
                _current_target.Friend = false;
            }
            else
            {
                throw new InvalidIniFormat(_invalid_ini_format_message); // if the value is invalid throw exception.
            }
        }

        /// <summary>
        /// determines if the given line is of the [groupname] or key=value format and throws an exception if the line 
        /// matches neither.
        /// </summary>
        /// <param name="trimedLine"></param>
        /// <returns></returns>
        private bool lineMatches(string trimedLine)
        {
            /* if the line is of the form "[groupname]" make a new Target return True as it matches a new target
             * declaration, if it matches the form "key=value" return false as it is a key=value pair. Otherwise   
             * throw an exception as the file is invalid
             */
            if (Regex.IsMatch(trimedLine, "^\\[[^\\[-^\\]]+\\]$"))
            {
                if (trimedLine.Contains('=')) // the groupname cannot be a key=value pair, aka cannot contain '='
                {
                    throw new InvalidIniFormat(_invalid_ini_format_message);
                }
                else
                {
                    return true; // the line is a valid group declaration.
                }
            }
            else if (Regex.IsMatch(trimedLine, "^[\\s*\\w\\s*]+=[\\s*\\w\\s*]+$"))
            {
                return false; // the line is a key=value pair.
            }
            else if (Regex.IsMatch(trimedLine, "^[\\s*\\w\\s*]+=[\\s*\\d\\.\\d*\\s*]+$"))
            {
                return false; // the line is a key=value pair with a decimal number on the right side.
            }
            else
            {
                throw new InvalidIniFormat(_invalid_ini_format_message); // line is invalid.
            }
        }
    }
}