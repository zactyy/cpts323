/* TargetDetector.cs
 * Implements a circular target detection system.
 * detects targets in provided image.
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
using System.ComponentModel;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace Detectors
{
    /// <summary>
    /// TargetDetector detects circular targets in an Image
    /// </summary>
    public class TargetDetector:ITargetDetector
    {
        private List<Tuple<Double, Double, Double, Double, Boolean>> _targets;
        private BackgroundWorker bw;
        private Object _lock;
        private const int THRESHOLD_MAX = 150;
        private const int THRESHOLD_MIN = 75;
        private const double ACCUMULATOR_RESOLUTION = 1;
        private const double MIN_DISTANCE = 20.0;
        private const int MIN_RADIUS = 10;
        private const int MAX_RADIUS = 0;
        public event EventHandler ImageProcessed;
        
        public TargetDetector()
        {
            _targets = new List<Tuple<Double, Double, Double, Double, Boolean>>();
            bw = new BackgroundWorker();
            _lock = new Object();
            bw.DoWork += new DoWorkEventHandler(DetectTargets_work);
        }

        /// <summary>
        /// start detection of targets in an Image
        /// </summary>
        /// <param name="image">A System.Drawing.Image image</param>
        public void DetectTargets(Image image){
            // this may end up ignoring one or more images if they come in before the worker is done
            // but that's better than a crash.
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync(image);
            }
        }
        /// <summary>
        /// Worker thread, does the actual detection of targets in a backgroundworker thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">contains parameters passed to the thread, should only be a Bitmap.</param>
        private void DetectTargets_work(Object sender, DoWorkEventArgs e)
        {
            Bitmap img = (Bitmap)e.Argument;
            /* using modified emguCV example code to try and find targets*/
            Image<Hsv, Byte> circleImage = new Image<Hsv, byte>(new Bitmap(img)).PyrDown().PyrUp();  
            Image<Gray, Byte> gray = circleImage.Convert<Gray, byte>().PyrDown().PyrUp();
            Gray cannyThreshold = new Gray(THRESHOLD_MAX);
            Gray circleAccumulatorThreshold = new Gray(THRESHOLD_MIN);
            CircleF[] circles = gray.HoughCircles(
                cannyThreshold,
                circleAccumulatorThreshold,
                ACCUMULATOR_RESOLUTION, //Resolution of the accumulator used to detect centers of the circles
                MIN_DISTANCE, //min distance between circles
                MIN_RADIUS, //min radius of circles
                MAX_RADIUS //max radius of circles
                )[0]; //Get the circles from the first channel                        
            lock (_lock)
            {
                _targets.Clear(); 
                Image<Gray, Byte>[] channels = circleImage.Split();
                try
                {
                    //channels[0] is the mask for hue less than 20 or larger than 160
                    CvInvoke.cvInRangeS(channels[0], new MCvScalar(20), new MCvScalar(160), channels[0]);
                    channels[0]._Not();

                    //channels[1] is the mask for satuation of at least 10, this is mainly used to filter out white pixels
                    channels[1]._ThresholdBinary(new Gray(10), new Gray(255.0));

                    CvInvoke.cvAnd(channels[0], channels[1], channels[0], IntPtr.Zero);
                }
                finally
                {
                    channels[1].Dispose();
                    channels[2].Dispose();
                }
                foreach (CircleF t in circles)
                {
                    bool friend = false;
                    Gray pixelColor = (channels[0])[Convert.ToInt32(t.Radius / 2), Convert.ToInt32(t.Radius / 2)];
                    Gray test = new Gray(126);
                    if (test.Intensity < pixelColor.Intensity)
                    {
                        friend = true;
                    }
                    Tuple<Double, Double, Double, Double, Boolean> t2 = new Tuple<Double, Double, Double, Double, Boolean>(t.Center.X, 0, t.Center.Y, t.Radius, friend);
                    _targets.Add(t2);
                }
            }
            if(ImageProcessed != null){
                ImageProcessed(this, null); 
            }
        }

        public List<Tuple<Double, Double, Double, Double, Boolean>> DetectedTargets()
        {
            List<Tuple<Double, Double, Double, Double, Boolean>> temp;
            lock (_lock)
            {
                temp= new List<Tuple<Double, Double, Double, Double, Boolean>>(_targets);                
            }
            return temp;
        }
    }
}
