// Target.cs
// Class definition for a Target.  
// CptS323, Spring 2013
// Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
// Written by: Jennifer Mendez
// Last modified by: Chris Walters
// Date modified: March 16, 2013

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
        }

        public Target(string i_name, int i_x_coordinate, int i_y_coordinate, int i_z_coordinate, bool i_friend)
        {
            Name = i_name;
            X_coordinate = i_x_coordinate;
            Y_coordinate = i_y_coordinate;
            Z_coordinate = i_z_coordinate;
            Friend = i_friend;
        }

        public Target(int i_x_coordinate, int i_y_coordinate, int i_z_coordinate, bool i_friend)
        {
            Name = null;
            X_coordinate = i_x_coordinate;
            Y_coordinate = i_y_coordinate;
            Z_coordinate = i_z_coordinate;
            Friend = i_friend;
        }

        public Target(Target copy)
        {
            this.Name = copy.Name;
            this.X_coordinate = copy.X_coordinate;
            this.Y_coordinate = copy.Y_coordinate;
            this.Z_coordinate = copy.Z_coordinate;
            this.Friend = copy.Friend;
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

        public bool Friend
        {
            get;
            set;
        }
    }
}
