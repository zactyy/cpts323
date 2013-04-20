/*
 * IVideoPlugin interface for video system.
 * Written By Chris Walters
 * Last Modified: Chris Walters
 * Last Modified On: April 18, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace VideoSys
{
    public interface IVideoPlugin
    {
        /// <summary>
        /// Gets the most recent image from the plugin
        /// </summary>
        /// <returns>A bitmap image</returns>
        Bitmap GetImage(); // allow other objects to get most recently sampled image.


        /// <summary>
        /// Starts the plugin.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the plugin.
        /// </summary>
        void Stop();

        /// <summary>
        /// Event for notifying observers of new image.
        /// </summary>
        event EventHandler NewImage; // event for when a new image is sampled.

        /// <summary>
        /// Property for setting desired framerate. If changed by multiple threads, could lead to 
        /// one or more threads not getting the data they require.
        /// </summary>
        int Framerate
        {
            get;
            set;
        }

        /// <summary>
        /// desired height of returned image.
        /// </summary>
        int Height
        {
            get;
            set;
        }

        /// <summary>
        /// desired width of returned image.
        /// </summary>
        int Width
        {
            get;
            set;
        }

    }
}
