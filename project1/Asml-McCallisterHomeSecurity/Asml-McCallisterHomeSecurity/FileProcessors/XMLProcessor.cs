/*
 * XMLProcessor.cs
 * implements XML target reader class.
 * Written for CptS323 at Washington State University, Spring 2013
 * Written By: Chris Walters
 * Last Modified By: Chris Walters
 * Date Modified: March 13, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Globalization;
using Asml_McCallisterHomeSecurity.Targets;

namespace Asml_McCallisterHomeSecurity.FileProcessors
{
    class XMLProcessor : FileProcessor
    {
        string _filePath = null;

        public XMLProcessor(string fileP)
        {
            this._filePath = fileP;
        }
        
        public override List<Target> ProcessFile()
        {
            XmlReader fr = XmlReader.Create(_filePath);
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
                                if (attribute_name != "isfriend" && attribute_name != "name") 
                                {
                                        switch (attribute_name)
                                        {
                                            case "xpos":
                                                _current_target.X_coordinate = Convert.ToDecimal(fr.Value);
                                                break;
                                            case "ypos":
                                                _current_target.Y_coordinate = Convert.ToDecimal(fr.Value);
                                                break;
                                            case "zpos":
                                                _current_target.Z_coordinate = Convert.ToDecimal(fr.Value);
                                                break;
                                            default:
                                                throw new XmlException("Invalid Format");
                                        }
                                }
                                else
                                {
                                    if (fr.Name == "isFriend")
                                    {
                                        if (fr.Value.ToLower() == "true")
                                        {
                                            _current_target.Friend = true;
                                        }
                                        else if (fr.Value.ToLower() == "false")
                                        {
                                            _current_target.Friend = false;
                                        }
                                        else
                                        {
                                            throw new XmlException("Invalid Format");
                                        }
                                    }
                                    else if (fr.Name.ToLower() == "name")
                                    {
                                        _current_target.Name = fr.Value;
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
    }
}