using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetManagement;

namespace searchmode
{
    public abstract class searchmode
    {
        public List<Target> returnlist = null;

        public abstract List<Target> search(List<Target> targetlist);

    }
}
