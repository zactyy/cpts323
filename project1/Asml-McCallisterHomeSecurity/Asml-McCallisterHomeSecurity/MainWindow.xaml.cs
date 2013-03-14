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
using Asml_McCallisterHomeSecurity.OperationsManager;

namespace Asml_McCallisterHomeSecurity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MoveUpButton(object sender, RoutedEventArgs e)
        {
            _rules_them_all.TurretMoveUp();
        }

        private void MoveDownButton(object sender, RoutedEventArgs e)
        {
            _rules_them_all.TurretMoveDown();
        }

        private void MoveLeftButton(object sender, RoutedEventArgs e)
        {
            OperationsManager.TurretMoveLeft();
        }

        private void MoveRightButton(object sender, RoutedEventArgs e)
        {
            OperationsManager.TurretMoveRight();
        }

        private void TurretFireClick(object sender, RoutedEventArgs e)
        {
            OperationsManager.
        }
    }
}
