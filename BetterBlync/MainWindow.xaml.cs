using Microsoft.Lync.Model;
using System;
using System.Windows;
using System.Windows.Threading;

namespace BetterBlync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlyncControl blync;
        private LyncStatus lync;
        private DispatcherTimer timer;
        private ContactAvailability lastKnownAvailability;
        private System.Threading.Thread thread;

        public MainWindow()
        {
            InitializeComponent();
            blync = new BlyncControl();
            lync = new LyncStatus();
            setStatusLight();
            lastKnownAvailability = lync.LyncAvailability;
            initTimer();
        }

        private void initTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan( 0, 0, 0, 0, 200 );
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lync.GetStatus();
            // Check if availability has changed to prevent wear to the light
            if ( lastKnownAvailability != lync.LyncAvailability )
            {
                lastKnownAvailability = lync.LyncAvailability;
                setStatusLight();
            }
        }

        private void setStatusLight()
        {
            if ( lync.InACall )
            {
                thread = new System.Threading.Thread( flashRedLight );
                thread.Start();
            }
            else
            {
                if ( thread != null ) thread.Abort();
            }

            switch ( lync.LyncAvailability )
            {
                case ContactAvailability.Free:
                    blync.SetColor( BlyncControl.BlyncColor.Green );
                    break;

                case ContactAvailability.Busy:
                    blync.SetColor( BlyncControl.BlyncColor.Red );
                    break;

                case ContactAvailability.DoNotDisturb:
                    blync.SetColor( BlyncControl.BlyncColor.Purple );
                    break;

                case ContactAvailability.Offline:
                    blync.TurnOffLight();
                    break;

                case ContactAvailability.FreeIdle:
                case ContactAvailability.TemporarilyAway:
                case ContactAvailability.Away:
                    blync.SetColor( BlyncControl.BlyncColor.Yellow );
                    break;

                default:
                    blync.SetColor( BlyncControl.BlyncColor.Green );
                    break;
            }
        }

        private void flashRedLight()
        {
            do
            {
                blync.SetColor( BlyncControl.BlyncColor.Red );
                System.Threading.Thread.Sleep( 400 );
                blync.TurnOffLight();
                System.Threading.Thread.Sleep( 400 );
            }
            while ( true );
        }

        private void btnBlue_Click(object sender, RoutedEventArgs e)
        {
            blync.SetColor( BlyncControl.BlyncColor.Blue );
            timer.Stop();
        }

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            blync.SetColor( BlyncControl.BlyncColor.Green );
            timer.Stop();
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            blync.SetColor( BlyncControl.BlyncColor.Red );
            timer.Stop();
        }

        private void btnYellow_Click(object sender, RoutedEventArgs e)
        {
            blync.SetColor( BlyncControl.BlyncColor.Yellow );
            timer.Stop();
        }

        private void btnPurple_Click(object sender, RoutedEventArgs e)
        {
            blync.SetColor( BlyncControl.BlyncColor.Purple );
            timer.Stop();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            blync.ResetLight();
            timer.Start();
            if ( thread != null )
            {
                thread.Abort();
            }
        }

        private void btnShutoff_Click(object sender, RoutedEventArgs e)
        {
            blync.TurnOffLight();
            timer.Stop();
            if ( thread != null )
            {
                thread.Abort();
            }
        }

        private void btnParty_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            thread = new System.Threading.Thread( PartyOn );
            thread.Start();
        }

        private void PartyOn()
        {
            int interval = 150;
            int count = 50;
            do
            {
                foreach ( BlyncControl.BlyncColor color in Enum.GetValues( typeof( BlyncControl.BlyncColor ) ) )
                {
                    System.Threading.Thread.Sleep( interval );
                    blync.SetColor( color );
                }
                count--;
            } while ( count > 0 );
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            blync.CloseDevices();
        }
    }
}