/*
 * DefaultVideo.cs
 * Default video plugin for the application, just displays normal video, at 30fps
 * CptS323, Spring 2013
 * Team McCallister Home Security
 * Written By: Chris Walters
 * Last Modified By: Chris Walters
 * Last Modified On: April 21, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Drawing;
using ASMLEngineSdk;
using System.Windows;
using WebCamera;

namespace VideoSys
{
    /// <summary>
    /// Default video plugin class for the application, displays video at 30fps, but that
    /// can be modified via the Framerate property(currently not accessible to user).
    /// </summary>
    public class DefaultVideo:IDisposable, IVideoPlugin
    {
        private Camera _webcamera;
        private DispatcherTimer _video_timer;
        private Image _image;
        public event EventHandler NewImage;
        private TimeSpan _framerate;
        private Object _lock;
        private int _height;
        private int _width;

        /// <summary>
        /// Class constructor.
        /// </summary>
        public DefaultVideo()
        {
            _webcamera = Camera.GetInstance();
            _video_timer = new DispatcherTimer();
            /*
             * default video settings
             */
            _framerate = new TimeSpan(0, 0, 0, 0, 34); // sets timer to 34ms, aka 30fps: 1000ms/30frames = 34ms/frame
            _width = 640;
            _height = 480;
            _video_timer.Tick += new EventHandler(CollectImage);
            _video_timer.Interval = _framerate;
            _image = null;
            _lock = new Object();
        }
         
        ~DefaultVideo()
        {
            Dispose(false);
        }

        #region Dispose
        public bool IsDisposed
        {
            get;
            set;
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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region properties
        /// <summary>
        /// Framerate property adjusts camera framerate.
        /// </summary> 
        public int Framerate
        {
            get
            {
                lock (_lock)
                {
                    return _framerate.Milliseconds;
                }
            }
            set
            {
                lock (_lock)
                {
                    if (value >= 1 && value <= 100) // between 1 and 100 frames per second
                    {
                        _framerate = new TimeSpan(0, 0, 0, 0, (1000 / value));
                        _video_timer.Stop();
                        _video_timer.Interval = _framerate;
                        _video_timer.Start();
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("framerate must be between 1 and 60 frames per second");
                    }
                }
            }
        }


        public int Height
        {
            get
            {
                lock (_lock)
                {
                    return _height;
                }
            }
            set
            {
                lock (_lock)
                {
                    _height = value;
                }
            }
        }

        public int Width
        {
            get
            {
                lock (_lock)
                {
                    return _width;
                }
            }

            set
            {
                lock (_lock)
                {
                    _width = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// Get the current image.
        /// </summary>
        /// <returns>A bitmap image.</returns>
        public Bitmap GetImage()
        {
            Bitmap temp = new Bitmap(_width, _height);
                using (Graphics g = Graphics.FromImage((Image)temp))
                {
                    //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(_image, 0, 0, _width, _height);
                    g.Dispose();
                }
                return temp;
        }


        /// <summary>
        /// Method to start the plugin capturing video.
        /// </summary>
        public void Start()
        {
            _video_timer.Start();
        }

        // Method to stop the plugin from capturing video.
        public void Stop()
        {
            _video_timer.Stop();
        }

        /// <summary>
        /// Event handler, grabs next frame from the webcamera.
        /// </summary>
        /// <param name="sender">object that triggered the event</param>
        /// <param name="e">event arguments</param>
        private void CollectImage(Object sender, EventArgs e)
        {
            if (_image != null)
            {
                _image.Dispose();
            }
            // get image from webcam
            _image = _webcamera.GetImage();
            // notify observers of new image.
            if (NewImage != null)
            {
                NewImage(this, new EventArgs());
            }
        }

    }
}
