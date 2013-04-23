using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            _video_plugins = new List<IVideoPlugin>();
            // add plugins to list here
            _video_plugins.Add(new DefaultVideo());
            // set current plugin here.
            _eye_of_sauron = _video_plugins.First();
            _eye_of_sauron.Width = (int)imgVideo.Width;
            _eye_of_sauron.Height = (int)imgVideo.Height;
            _eye_of_sauron.NewImage += new EventHandler(on_image_changed);
            _eye_of_sauron.Start();

            // Mode List initialization
           // lstModes.DataContext = _rules_them_all.SearchModeList
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

        #region video
        private void on_image_changed(Object sender, EventArgs e)
        {
            Bitmap image = ((IVideoPlugin)sender).GetImage();
            /* draw overlay on the image*/
            using (Graphics g = Graphics.FromImage(image))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                System.Drawing.Brush brush = System.Drawing.Brushes.Crimson;
                Font overlayFont = new Font("Calibri", 14);
                Tuple<string, double, double, string> targetInfo = _rules_them_all.CurrentTargetInfo();
                PointF upperLeft = new PointF(5, 5);
                PointF bottomLeft = new PointF(5, (float)(imgVideo.ActualHeight-20));
                PointF upperRight = new PointF((float)(imgVideo.ActualWidth - 120),  5);
                PointF bottomRight = new PointF((float)(imgVideo.ActualWidth - 120), (float)(imgVideo.ActualHeight-20));
                char degreeSymbol = (char)176;
                g.DrawString("Current Target: " + targetInfo.Item1, overlayFont, brush, upperLeft);
                g.DrawString("Loc: " + targetInfo.Item2.ToString() + degreeSymbol + " x " + targetInfo.Item3.ToString() + degreeSymbol, overlayFont, brush, bottomLeft);
                g.DrawString("Time:00:00:00", overlayFont, brush, upperRight);
                g.DrawString("Status: " + targetInfo.Item4.ToString(), overlayFont, brush, bottomRight);
            }
            /* set image source to new image*/
            imgVideo.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }
       

        private void DisplayError(string Message)
        {
            System.Windows.MessageBox.Show(Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);            
        }

        private void lstModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void lstTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


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
    }
}