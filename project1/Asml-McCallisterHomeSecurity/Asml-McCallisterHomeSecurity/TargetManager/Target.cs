using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asml_McCallisterHomeSecurity.TargetManager
{
    public class Target
    {
        public ActualTarget()
        {
            Name = null;
            X_coordinate = 0;
            Y_coordinate = 0;
            Z_coordinate = 0;
            Friend = true;
        }

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
