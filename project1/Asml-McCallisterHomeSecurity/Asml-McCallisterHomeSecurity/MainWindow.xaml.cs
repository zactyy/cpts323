﻿using System;
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
using Microsoft.Win32;

namespace Asml_McCallisterHomeSecurity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OperationsManager.OperationsManager  _rules_them_all;
        public MainWindow()
        {

            InitializeComponent();
            _rules_them_all = OperationsManager.OperationsManager.GetInstance();
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
            _rules_them_all.TurretMoveLeft();
        }

        private void MoveRightButton(object sender, RoutedEventArgs e)
        {
           _rules_them_all.TurretMoveRight();
        }

        private void TurretFireClick(object sender, RoutedEventArgs e)
        {
            _rules_them_all.TurretFire();
        }

        /// <summary>
        /// retrieves a filename via OpenFileDialog and then attempts to process it for targets.
        /// if an error occurs, reports it to the user va a messagebox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "";
            dialog.Filter = "Target Files(.ini, .xml)|*.ini;*.xml";

            Nullable<bool> result = dialog.ShowDialog();
            string _file_name = null;

            if (result == true)
            {
                _file_name = dialog.FileName;
                try
                {
                    _rules_them_all.LoadFile(_file_name);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TurretReset(object sender, RoutedEventArgs e)
        {
            _rules_them_all.TurretReset();
        }


       
    }
}