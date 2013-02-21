using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetClass
{
    //public class ActualTarget
    //{
    //    (Target, Name, x, y, z, friend)
    //    Tuple<string, string, int, int, int, bool> Target;
    //}

    // Or maybe Key-Value pairs? 

    /// <summary>
    /// A class that stores Target information.  I understand this 
    /// implementation the best and it seems to allow the flexibility
    /// for Name to not be set.  
    /// </summary>
    public class ActualTarget
    {
        public ActualTarget(string i_name, int i_x_coordinate, int i_y_coordinate, int i_z_coordinate, bool i_friend)
        {
            Name = i_name;
            X_coordinate = i_x_coordinate;
            Y_coordinate = i_y_coordinate;
            Z_coordinate = i_z_coordinate;
            Friend = i_friend;
        }

        public ActualTarget(int i_x_coordinate, int i_y_coordinate, int i_z_coordinate, bool i_friend)
        {
            Name = null;
            X_coordinate = i_x_coordinate;
            Y_coordinate = i_y_coordinate;
            Z_coordinate = i_z_coordinate;
            Friend = i_friend;
        }

        public string Name
        {
            get;
            set;
        }

        public int X_coordinate
        {
            get;
            set;
        }

        public int Y_coordinate
        {
            get;
            set;
        }
        
        public int Z_coordinate
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
