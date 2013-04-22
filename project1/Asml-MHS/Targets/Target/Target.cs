// Target.cs
// Class definition for a Target.  
// CptS323, Spring 2013
// Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
// Written by: Jennifer Mendez
// Last modified by: Chris Walters
// Date modified: April 17, 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetManagement
{
    public class Target
    {
        public Target()
        {
            Name = null;
            X_coordinate = 0;
            Y_coordinate = 0;
            Z_coordinate = 0;
            Friend = true;
            Theta = 0;
            Phi = 0;
        }

        public Target(string i_name, double i_x_coordinate, double i_y_coordinate, double i_z_coordinate, bool i_friend)
        {
            Name = i_name;
            X_coordinate = i_x_coordinate;
            Y_coordinate = i_y_coordinate;
            Z_coordinate = i_z_coordinate;
            Friend = i_friend;
            this.CalculateAngles();
        }

        public Target(double i_x_coordinate, double i_y_coordinate, double i_z_coordinate, bool i_friend)
        {
            Name = null;
            X_coordinate = i_x_coordinate;
            Y_coordinate = i_y_coordinate;
            Z_coordinate = i_z_coordinate;
            Friend = i_friend;
            this.CalculateAngles();
        }


        public Target(Target copy)
        {
            this.Name = copy.Name;
            this.X_coordinate = copy.X_coordinate;
            this.Y_coordinate = copy.Y_coordinate;
            this.Z_coordinate = copy.Z_coordinate;
            this.Friend = copy.Friend;
        }

        // determines angles from origin to targets coordinate assigns to Theta and Phi
        private void CalculateAngles()
        {
            double temp = (X_coordinate/Z_coordinate);
            // multiply by 180/pi to convert from radians to degrees
            Theta = Math.Atan(temp) * (180/Math.PI);
            double hypotnus = Math.Sqrt((X_coordinate * X_coordinate + Z_coordinate * Z_coordinate));
            // multiply by 180/pi to convert radians to degrees.
            Phi = Math.Atan(Y_coordinate / hypotnus) * (180/Math.PI);
        }

        public string Name
        {
            get;
            set;
        }

        public double X_coordinate
        {
            get;
            set;
        }

        public double Y_coordinate
        {
            get;
            set;
        }
        
        public double Z_coordinate
        {
            get;
            set;
        }

        public double Theta
        {
            get;
            set;
        }

        public double Phi
        {
            get;
            set;
        }

        public bool Friend
        {
            get;
            set;
        }

        /// <summary>
        /// compares two targets to see if they are NOT the same.
        /// </summary>
        /// <param name="self">the current target.</param>
        /// <param name="comparitor">a target to compare to.</param>
        /// <returns></returns>
        public static bool operator !=(Target self, Target comparitor)
        {
            return !(self == comparitor);
        }

        /// <summary>
        /// compares two targets to see if they are the same.
        /// </summary>
        /// <param name="self">the current target.</param>
        /// <param name="comparitor">a target to compare to.</param>
        /// <returns></returns>
        public static bool operator ==(Target self, Target comparitor)
        {
            /* if the two target objects share the same name, location, and friend status, they
             * are the same target. */
            if (self.Name == comparitor.Name &&
                self.X_coordinate == comparitor.X_coordinate &&
                self.Y_coordinate == comparitor.Y_coordinate &&
                self.Z_coordinate == comparitor.Z_coordinate &&
                self.Friend == comparitor.Friend
                )
            {
                return true;
            }
            return false;
        }
    }
}
