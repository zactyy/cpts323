using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetManagement;

namespace searchmode
{
    class searchfriends : searchmode
    {
        public override List<Target> search(List<Target> targetlist)
        {
            returnlist = new List<Target>();
            foreach (Target tango in targetlist)
            {
                if (tango.Friend == true)
                    returnlist.Add(tango);
            }
            return returnlist;
        }
       
    }
}
