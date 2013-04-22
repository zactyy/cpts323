using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetManagement;

namespace searchmode
{
    class searchall : searchmode
    {
        public override List<Target> search(List<Target> targetlist)
        {
            returnlist = new List<Target>(targetlist);

            return returnlist;
        }
    }
}
