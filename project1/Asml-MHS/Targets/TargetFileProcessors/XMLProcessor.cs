/*
 * XMLProcessor.cs
 * implements XML target reader class.
 * Written for CptS323 at Washington State University, Spring 2013
 * Written By: Chris Walters
 * Last Modified By: Chris Walters
 * Date Modified: March 18, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Globalization;
using TargetManagement;

namespace TargetManagement.TargetFileProcessors
{
    class XMLProcessor : FileProcessor
    {
        public XMLProcessor(string fp = null)
        {
            this.FilePath = fp;
        }
        
        public override List<Target> ProcessFile()
        {
            XmlReader fr = XmlReader.Create(this.FilePath);
            List<Target> _output = new List<Target>();
            Target _current_target;
            while (fr.Read())  // read the xml file until the end.
            {
                string node_name = fr.Name.ToLower();
                switch (fr.NodeType)
                {
                    case XmlNodeType.Element:
                        if (node_name != "targets" && node_name != "target") // if element is not named Targets or Target, file format is invalid.
                        {
                            throw new XmlException("Invalid Format.");
                        }
                        // if element is named Target, read it's attributes and store them as a Target.
                        else if (node_name == "target") 
                        {
                            _current_target = new Target();
                            while (fr.MoveToNextAttribute())
                            {
                                // each attribute should be a target object attribute name and corresponding value.
                                string attribute_name = fr.Name.ToLower();
                                string attribute_value = fr.Value.ToLower();
                                if (attribute_name != "isfriend" && attribute_name != "name") 
                                {                                  
                                    SetTargetPositionValue(_current_target, attribute_name, attribute_value);
                                }
                                else
                                {
                                    if (attribute_name == "isfriend")
                                    {
                                        SetTargetFriend(_current_target, attribute_value);
                                    }
                                    else if (attribute_name == "name")
                                    {
                                        _current_target.Name = attribute_value;
                                    }
                                }
                            }
                            _output.Add(_current_target);
                        }
                        break;
                    case XmlNodeType.Comment:  // ignore comments
                        break;
                    case XmlNodeType.XmlDeclaration: // ignore declarations
                        break;
                    case XmlNodeType.Whitespace: // ignore whitespace
                        break;
                    case XmlNodeType.EndElement:  // ignore end elements
                        break;
                    default: // if defaulted, throw format is invalid.
                        throw new XmlException("Invalid Format.");
                }
            }
            return _output;
        }

        /// <summary>
        /// Set the friend property of the target
        /// </summary>
        /// <param name="_current_target">A target object</param>
        /// <param name="attribute_value">a string containing "true" or "false"</param>
        private void SetTargetFriend(Target _current_target, string attribute_value)
        {
            if (attribute_value == "true")
            {
                _current_target.Friend = true;
            }
            else if (attribute_value == "false")
            {
                _current_target.Friend = false;
            }
            else
            {
                throw new XmlException("Invalid Format");
            }
        }

        /// <summary>
        /// Set x,y,z position value, or throw an error if attribute_name isn't xpos, ypos, or zpos.
        /// </summary>
        /// <param name="_current_target">a Target obj</param>
        /// <param name="attribute_name">a string containing the attribute name</param>
        /// <param name="value">a string containing the attribue value</param>
        private void SetTargetPositionValue(Target _current_target, string attribute_name, string value)
        {
            switch (attribute_name)
            {
                case "xpos":
                    _current_target.X_coordinate = Convert.ToDecimal(value);
                    break;
                case "ypos":
                    _current_target.Y_coordinate = Convert.ToDecimal(value);
                    break;
                case "zpos":
                    _current_target.Z_coordinate = Convert.ToDecimal(value);
                    break;
                default:
                    throw new XmlException("Invalid Format");
            }
        }
    }
}