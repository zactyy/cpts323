using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetManagement;

namespace searchmodes
{
    public class searchall : searchmode
    {
        public override List<Target> search(List<Target> targetlist)
        {
            List<Target> returnList = new List<Target>();
            foreach (Target t in targetlist)
            {
                if (t.Destroyed == false)
                {
                    returnList.Add(t);
                }
            }         
            return returnList;
        }
    }
}
