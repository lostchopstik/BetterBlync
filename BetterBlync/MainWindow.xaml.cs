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

namespace BetterBlync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlyncControl blync;
        public MainWindow()
        {
            InitializeComponent();
            blync = new BlyncControl();
        }

        private void btnBlue_Click(object sender, RoutedEventArgs e)
        {
            blync.ChangeToBlue();
        }

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            blync.ChangeToGreen();
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            blync.ChangeToRed();
        }

        private void btnYellow_Click(object sender, RoutedEventArgs e)
        {
            blync.ChangeToYellow();
        }

        private void btnPurple_Click(object sender, RoutedEventArgs e)
        {
            blync.ChangeToPurple();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            blync.ResetLight();
        }

        private void btnShutoff_Click(object sender, RoutedEventArgs e)
        {
            blync.TurnOffLight();
        }

        private void btnParty_Click(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread th = new System.Threading.Thread( PartyOn );
            th.Start();
        }

        private void PartyOn()
        {
            int interval = 100;
            int count = 10;
            do
            {
                System.Threading.Thread.Sleep( interval );
                blync.ChangeToBlue();
                System.Threading.Thread.Sleep( interval );
                blync.ChangeToRed();
                System.Threading.Thread.Sleep( interval );
                blync.ChangeToPurple();
                System.Threading.Thread.Sleep( interval );
                blync.ChangeToYellow();
                System.Threading.Thread.Sleep( interval );
                blync.ChangeToGreen();
                count--;
            } while ( count > 0 );
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            blync.CloseDevices();
        }
    }
}
