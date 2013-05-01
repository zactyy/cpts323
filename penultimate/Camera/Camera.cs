// Camera.cs
// The camera is a singleton class that manages access to a locally connected webcam
// CptS323, Spring 2013
// Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
// Written by: Chris Walters
// Last Modified By: Chris Walters
// Last Modified On: April 21, 2013
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using System.Drawing;
using System.Windows.Media;
using ASMLEngineSdk;
namespace WebCamera
{
    /// <summary>
    /// The Camera class is a singleton that maintains access to a local connected webcam.
    /// </summary>
    public class Camera:IVideo, IDisposable
    {
        /// <summary>
        ///  The singleton instance
        /// </summary>
        private static Camera _instance;
        
        /// <summary>
        /// The camera object, it is a emguCV Capture object
        /// </summary>
        private Capture _webcamera;

        /// <summary>
        /// This camera object will potentially be accessed by multiple threads,
        /// and as such, the access needs to be synchronized, this is the lock 
        /// </summary>
        private Object _lock;

        private Camera()
        {
            _webcamera = new Capture();
            _lock = new Object();
        }

        ~Camera()
        {
            Dispose(false);
        }


        public static Camera GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Camera();
            }
            return _instance;
        }

        #region Dispose
        private void Dispose(bool cleanupOthers)
        {
            if (!IsDisposed)
            {
                if (cleanupOthers)
                {
                    _webcamera.Dispose();
                }
            }
            IsDisposed = true;
        }

        private bool IsDisposed
        {
            get;
            set;
        }

        public void Dispose()
        {
           /*  Singleton pattern in use, IGNORE THIS METHOD!
            * Don't want any one thread to be able to Dispose() since the instanc e
            * may still be in use elsewhere.
            * Dispose(true);
            * GC.SuppressFinalize(this);*/
        }
        #endregion 

        /// <summary>
        ///  Return the latest Bitmap retrieved from the camera.
        /// </summary>
        /// <returns>A Bitmap image.</returns>
        public Image GetImage()
        {
            lock (_lock)
            {
                Image _image = _webcamera.QueryFrame().ToBitmap();
                return _image;
            }
        }
    }
}
