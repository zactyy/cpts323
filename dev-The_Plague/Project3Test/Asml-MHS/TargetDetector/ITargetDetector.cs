/* ITargetDetector.cs
 * Defines interface required for all TargetDetectors for ASML project
 * Written for CptS323 at Washington State University, Spring 2013
 * Written By: Chris Walters
 * Last Modified By: Chris Walters
 * Last Modified On: April 29, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Detectors
{
    public interface ITargetDetector
    {
        void DetectTargets(Image imageToDetect);
        List<Tuple<Double, Double, Double, Double, Boolean>> DetectedTargets();
    }
}
