/* Timer.cs
 * A threaded timer.
 * CptS323, Spring 2013
 * Team McCallister Home Security: Chris Walters, Jennifier Mendez, Zachary Tynnisma
 * Written by: Jennifer Mendez  (inspired by ThreadingExample written by Brian 
 * Last modified by: Chris Walters 
 * Date modified: April 23, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadedTimer
{



    /// <summary>
    /// A threaded timer class. 
    /// </summary>
    public class Timer
    {
        #region Members
        /// <summary>
        /// Lock to protect critical regions.
        /// </summary>
        private object _lock_object;
        /// <summary>
        /// The last time collected in the timer, 
        /// this is the value reported back to the observers.
        /// </summary>
        private TimeSpan _last_time;
        /// <summary>
        /// Provides a marker for the start of the timer.  
        /// </summary>
        private DateTime _timer_start;
        /// <summary>
        /// Processing thread.  
        /// </summary>
        private Thread _thread;
        /// <summary>
        /// Flag to track whether timer should remain available.
        /// </summary>
        private bool _is_active;
        /// <summary>
        /// Flag to indicate whether the timer is running.  
        /// </summary>
        private bool _is_running;
        /// <summary>
        /// To synchronize threads thread event.
        /// </summary>
        private ManualResetEvent _wait_event;
        private ManualResetEvent _kill_event;
        #endregion


        #region Events
        /// <summary>
        /// Fired when time is ready.
        /// </summary>
        public event EventHandler<TimerEventArgs> TimeCaptured;
        /// <summary>
        /// Fired when timer is turned on.
        /// </summary>
        public event EventHandler ThreadStarted;
        /// <summary>
        /// Fired when timer is stopped.  
        /// </summary>
        public event EventHandler ThreadStopped;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Timer()
        {
            _last_time = TimeSpan.Zero;
            _timer_start = DateTime.Now;

            _wait_event = new ManualResetEvent(false);
            _kill_event = new ManualResetEvent(false);

            _is_active = true;
            _is_running = false;

            // used like a mutex. 
            _lock_object = new object();

            SetupThread();
        }

        #region Threading
        /// <summary>
        /// Set up and start the timer thread.  
        /// </summary>
        private void SetupThread()
        {
            ThreadStart start = new ThreadStart(TimerThread);
            _thread = new Thread(start);            
            _thread.Start();
            _wait_event.Set();
        }

        private void TimerThread()
        {
            WaitHandle[] events = new WaitHandle[] { _wait_event };

            // Keep this thread alive and responsive to timer needs.  
            while (_is_active)
            {
                int eventHandle = WaitHandle.WaitAny(events, 50);
                if (eventHandle == 0)
                {
                    _wait_event.Reset();
                    int runEvent = 0;
                    // now just wait for start and stop timer events.  
                    while(_is_running) 
                    {                   
                        // wait 50 milliseconds in case there is an event to stop.  
                        runEvent = WaitHandle.WaitAny(events, 50);
                        if (runEvent == 0)
                        {
                            _wait_event.Reset();
                        }
                        else if (runEvent == 258) // 258 is a timeout event
                        {
                            //DateTime currentTime = new DateTimeOffset(_timer_start);
                            //lock around the data
                            lock (_lock_object)
                            {
                                _last_time = (DateTime.Now - _timer_start);
                            }

                            if (this.TimeCaptured != null && _last_time != null)
                            {
                                this.TimeCaptured(this, new TimerEventArgs(_last_time));
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the thread going.  
        /// </summary>
        public void Start()
        {
            _is_running = true;
            // This timer will start at zero, and count up.  
            lock (_lock_object)
            {
                _timer_start = DateTime.Now;
            }

            _wait_event.Set();

            if (ThreadStarted != null)
            {
                ThreadStarted(this, null);
            }
        }
        /// <summary>
        /// Stops the thread.  
        /// </summary>
        public void Stop()
        {
            _is_running = false;
            _wait_event.Set();

            if(ThreadStopped != null)
            {
                ThreadStopped(this, null);
            }
        }

        /// <summary>
        /// Kill the timer.
        /// </summary>
        public void Destroy()
        {
            _is_active = false;
            _kill_event.Set();
        }

        /// <summary>
        /// Returns the last time recorded.  
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetLastTime()
        {
            return _last_time;
        }
        #endregion  
    }

     /// <summary>
    /// An event arguments class to avoid creating a delegate.  
    /// </summary>
    public class TimerEventArgs : EventArgs
    {
        public TimerEventArgs(TimeSpan latest)
        {
            LastTime = latest;
        }

        public TimeSpan LastTime
        {
            get;
            // private set is used so that no one can mess with the data later.
            private set;
        }
    }
}
