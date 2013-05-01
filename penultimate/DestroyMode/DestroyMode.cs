using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetManagement;

namespace DestroyMode
{
    public class DestroyMode
    {
        public DestroyMode();
        public delegate void TargetInfo(Target);
        public TargetInfo AttackingTarget(Target);

        


    }
}
