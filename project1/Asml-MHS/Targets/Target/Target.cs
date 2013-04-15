// Target.cs
// Class definition for a Target.  
// CptS323, Spring 2013
// Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
// Written by: Jennifer Mendez
// Last modified by: Zachary Tyynismaa
// Date modified: April 15, 2013

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

        public Target(string i_name, int i_x_coordinate, int i_y_coordinate, int i_z_coordinate, bool i_friend)
        {
            Name = i_name;
            X_coordinate = i_x_coordinate;
            Y_coordinate = i_y_coordinate;
            Z_coordinate = i_z_coordinate;
            Friend = i_friend;
            this.CalculateAngles();
        }

        public Target(int i_x_coordinate, int i_y_coordinate, int i_z_coordinate, bool i_friend)
        {
            Name = null;
            X_coordinate = i_x_coordinate;
            Y_coordinate = i_y_coordinate;
            Z_coordinate = i_z_coordinate;
            Friend = i_friend;
            this.CalculateAngles();
        }

        // determines angles from origin to targets coordinate assigns to Theta and Phi
        private void CalculateAngles()
        {
            double temp = (double)(X_coordinate/Z_coordinate);
            Theta = Math.Atan(temp);
            double hypotnus = Math.Sqrt((double)(X_coordinate * X_coordinate + Z_coordinate * Z_coordinate));
            Phi = Math.Atan((double)Y_coordinate / hypotnus);
        }

        public string Name
        {
            get;
            set;
        }

        public decimal X_coordinate
        {
            get;
            set;
        }

        public decimal Y_coordinate
        {
            get;
            set;
        }
        
        public decimal Z_coordinate
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
    }
}
