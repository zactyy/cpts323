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
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.WPF;
using System.Drawing;
using System.Windows.Threading;

namespace CameraTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Capture _webcamera;
        private DispatcherTimer _video_timer;
        public MainWindow()
        {
            InitializeComponent();
            _webcamera = new Capture();
            VideoBox.Source = GetImage();
            _video_timer = new DispatcherTimer();
            _video_timer.Tick += new EventHandler(VideoTimer_Tick);
            _video_timer.Interval = new TimeSpan(0, 0, 0, 0, 34); // sets timer to 32ms, aka 30fps 1000ms/30frames = 34ms
            _video_timer.Start();

        }

        private BitmapSource GetImage()
        {
            BitmapSource source = BitmapSourceConvert.ToBitmapSource(_webcamera.QueryFrame());
            return source;
        }

        private void VideoTimer_Tick(object sender, EventArgs e)
        {
            VideoBox.Source = GetImage();
        }
    }
}
