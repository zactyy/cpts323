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
    public class Camera:IVideo, IDisposable
    {
        private static Camera _instance;
        private Capture _webcamera;
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
