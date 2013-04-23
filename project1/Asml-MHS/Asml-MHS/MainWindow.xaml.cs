using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OperationsManager;
using VideoSys;
using Microsoft.Win32;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using ThreadedTimer;

namespace Asml_McCallisterHomeSecurity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OperationsManager.OperationsManager  _rules_them_all;
        private List<IVideoPlugin> _video_plugins;
        private IVideoPlugin _eye_of_sauron;
        private TimeSpan _elapsed_time;

        public MainWindow()
        {

            InitializeComponent();
            _rules_them_all = OperationsManager.OperationsManager.GetInstance();
            this.DataContext = this._rules_them_all;
            string version_string = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
            string program_title = "ASML-McCallister Home Security ";
            this.Title = program_title + version_string;
            lblNumMissiles.Content = _rules_them_all.NumberMissiles.ToString();
            _rules_them_all.ChangedTargets += on_targets_changed;
            _rules_them_all.sdCompleted += Search_Destroy_Complete;
            _rules_them_all._timer.TimeCaptured += new EventHandler<TimerEventArgs>(_timer_TimeCaptured);
            _video_plugins = new List<IVideoPlugin>();
            // add video plugins to list here
            _video_plugins.Add(new DefaultVideo());
            // set current plugin here.
            _eye_of_sauron = _video_plugins.First();
            // setup resoultion information and event handler, start plugin.
            _eye_of_sauron.Width = (int)imgVideo.Width;
            _eye_of_sauron.Height = (int)imgVideo.Height;
            _eye_of_sauron.NewImage += new EventHandler(on_image_changed);
            _eye_of_sauron.Start();
            // Mode List initialization
            cmbModes.ItemsSource = _rules_them_all.Modes;
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _rules_them_all.Dispose();
        }

        void _timer_TimeCaptured(object sender, TimerEventArgs e)
        {
            _elapsed_time = e.LastTime;
        }


        /*
         * Turret controls in this region 
         */
        #region Turret_Controls 

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rules_them_all.TurretMoveUp();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rules_them_all.TurretMoveDown();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rules_them_all.TurretMoveLeft();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rules_them_all.TurretMoveRight();
                winHomeScreen.Focus();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void btnFire_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rules_them_all.TurretFire();
                lblNumMissiles.Content = _rules_them_all.NumberMissiles.ToString();
                winHomeScreen.Focus();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rules_them_all.TurretReset();
                winHomeScreen.Focus();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }
        #endregion


        /*
         * Target info controls in this region
         */
        #region target_info controls
        /// <summary>
        /// retrieves a filename via OpenFileDialog and then attempts to process it for targets.
        /// if an error occurs, reports it to the user va a messagebox.
        /// </summary>
        /// <param name="sender">Object representing the control</param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "";
            dialog.Filter = "Target Files(.ini, .xml)|*.ini;*.xml| All Files(*.*)|*.*";

            Nullable<bool> result = dialog.ShowDialog();
            string file_name = null;

            if (result == true)
            {
                file_name = dialog.FileName;
                string display_name = dialog.SafeFileName; // just the file name, not the directory info.
                try
                {
                    _rules_them_all.LoadFile(file_name);
                    lblTargetFileName.Content = display_name;
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                    lblTargetFileName.Content = "No Targets Detected.";
                }
            }         
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rules_them_all.ReloadTurret();
                lblNumMissiles.Content = _rules_them_all.NumberMissiles;
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }  
        }

        private void on_targets_changed()
        {
            lstTargets.Items.Clear();
            foreach (ListViewItem item in _rules_them_all.TargetInfo)
            {
                lstTargets.Items.Add(item);
            }
        }
        #endregion

        /*
         * video controls
         */
        #region video
        /// <summary>
        /// whenever a new image is observed in the current plugin,
        /// this event handler grabs that image and writes the overlay, 
        /// then displays it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void on_image_changed(Object sender, EventArgs e)
        {
            Bitmap image = ((IVideoPlugin)sender).GetImage();
            /* draw overlay on the image*/
            using (Graphics g = Graphics.FromImage(image))
            {
                /* setup necessary information and objects*/
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                System.Drawing.Brush brush = System.Drawing.Brushes.Crimson;
                Font overlayFont = new Font("Calibri", 14);
                /* tuple holds the actual target info to be drawn*/
                Tuple<string, double, double, string> targetInfo = _rules_them_all.CurrentGUIInfo();
                /* points are where the strings should be drawn on the image*/
                PointF upperLeft = new PointF(5, 5);
                PointF bottomLeft = new PointF(5, (float)(imgVideo.ActualHeight-20));
                PointF upperRight = new PointF((float)(imgVideo.ActualWidth - 120),  5);
                PointF bottomRight = new PointF((float)(imgVideo.ActualWidth - 120), (float)(imgVideo.ActualHeight-20));
                char degreeSymbol = (char)176;
                /*actually draw the overlay here*/
                g.DrawString("Current Target: " + targetInfo.Item1, overlayFont, brush, upperLeft);
                g.DrawString("Loc: " + targetInfo.Item2.ToString() + degreeSymbol + " x " + targetInfo.Item3.ToString() + degreeSymbol, overlayFont, brush, bottomLeft);
                g.DrawString("Time: " + new DateTime(_elapsed_time.Ticks).ToString("mm:ss.FFF"), overlayFont, brush, upperRight);
                g.DrawString("Status: " + targetInfo.Item4.ToString(), overlayFont, brush, bottomRight);
                g.Dispose();
            }
            /* set image source to new image*/
            imgVideo.Source = null;
            imgVideo.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            image.Dispose();
        }
       
        /// <summary>
        /// video stop button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVideoStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _eye_of_sauron.NewImage -= new EventHandler(on_image_changed);
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// video start button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnVideoStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _eye_of_sauron.NewImage += new EventHandler(on_image_changed);
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }
        #endregion


        /*
         * mode controls
         */
        #region mode
        /// <summary>
        /// mode start button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rules_them_all.SearchAndDestroy();
                Disable_Buttons();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// mode stop button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rules_them_all.Stop();
                Enable_Buttons();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void Disable_Buttons()
        {
            btnStartMode.IsEnabled = false;
            btnReset.IsEnabled = false;
            btnRight.IsEnabled = false;
            btnLeft.IsEnabled = false;
            btnUp.IsEnabled = false;
            btnDown.IsEnabled = false;
            btnLoad.IsEnabled = false;
            btnFire.IsEnabled = false;
        }

        private void Enable_Buttons()
        {
            if (!btnStartMode.Dispatcher.CheckAccess())
            {
                btnStartMode.Dispatcher.Invoke(Enable_Buttons);
            }
            else
            {
                btnStartMode.IsEnabled = true;
                btnStopMode.IsEnabled = true;
                btnReload.IsEnabled = true;
                btnReset.IsEnabled = true;
                btnRight.IsEnabled = true;
                btnLeft.IsEnabled = true;
                btnUp.IsEnabled = true;
                btnDown.IsEnabled = true;
                btnLoad.IsEnabled = true;
                btnFire.IsEnabled = true;
            }
        }
        #endregion

        #region miscellaneous
        /// <summary>
        /// method to display error to user in a msgbox
        /// </summary>
        /// <param name="Message"></param>
        private void DisplayError(string Message)
        {
            System.Windows.MessageBox.Show(Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// event handler for when user selects a different mode in the GUI combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = 0; // there should only be *one* selected mode, and thus there will be only this
            string selectedMode = (string)e.AddedItems[index];
            _rules_them_all.SetCurrentMode(selectedMode);
        }

        /// <summary>
        /// event handler for when search and destroy mode ends.
        /// </summary>
        private void Search_Destroy_Complete()
        {
            Enable_Buttons();
        }

        #endregion
    }
}