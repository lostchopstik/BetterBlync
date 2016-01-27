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
        private bool threadRunning = false;

        private enum AlertType
        {
            Call,
            IM
        }

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                blync = new BlyncControl();
                lync = new LyncStatus();
                setStatusLight();
                lastKnownAvailability = lync.LyncAvailability;
                initTimer();
            }
            catch ( Exception ex )
            {
                MessageBox.Show( $"Something went wrong!\nError: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error );
                Close();
            }
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
            // Check to see if any new lights are connected
            blync.FindBlyncLights();

            // Get current status from Lync client
            lync.GetStatus();

            // Store the last color
            lastKnownAvailability = lync.LyncAvailability;

            // Change to new color
            setStatusLight();
        }

        private void setStatusLight()
        {
            if ( lync.InACall || lync.NewMessage )
            {
                // if alert is call type is call
                if ( !threadRunning )
                {
                    threadRunning = true;
                    if ( lync.InACall )
                        thread = new System.Threading.Thread( () => flashRedLight( AlertType.Call ) );
                    else if ( lync.NewMessage )
                        thread = new System.Threading.Thread( () => flashRedLight( AlertType.IM ) );
                    thread.Start();
                    return;
                }
                else return;
            }
            else
            {
                if ( thread != null ) thread.Abort();
            }

            switch ( lync.LyncAvailability )
            {
                case ContactAvailability.Free:
                    if ( blync.CurrentColor != BlyncControl.BlyncColor.Green )
                        blync.SetColor( BlyncControl.BlyncColor.Green );
                    break;

                case ContactAvailability.Busy:
                    if ( blync.CurrentColor != BlyncControl.BlyncColor.Red )
                        blync.SetColor( BlyncControl.BlyncColor.Red );
                    break;

                case ContactAvailability.DoNotDisturb:
                    if ( blync.CurrentColor != BlyncControl.BlyncColor.Purple )
                        blync.SetColor( BlyncControl.BlyncColor.Purple );
                    break;

                case ContactAvailability.Offline:
                    blync.TurnOffLight();
                    break;

                case ContactAvailability.FreeIdle:
                case ContactAvailability.TemporarilyAway:
                case ContactAvailability.Away:
                    if ( blync.CurrentColor != BlyncControl.BlyncColor.Yellow )
                        blync.SetColor( BlyncControl.BlyncColor.Yellow );
                    break;

                default:
                    if ( blync.CurrentColor != BlyncControl.BlyncColor.Green )
                        blync.SetColor( BlyncControl.BlyncColor.Green );
                    break;
            }
        }

        private void flashRedLight(AlertType type)
        {
            if ( type == AlertType.Call )
            {
                do
                {
                    blync.SetColor( BlyncControl.BlyncColor.Red );
                    System.Threading.Thread.Sleep( 400 );
                    blync.TurnOffLight();
                    System.Threading.Thread.Sleep( 400 );
                }
                while ( lync.InACall );
            }
            else if ( type == AlertType.IM )
            {
                do
                {
                    blync.SetColor( BlyncControl.BlyncColor.Red );
                    System.Threading.Thread.Sleep( 400 );
                    blync.TurnOffLight();
                    System.Threading.Thread.Sleep( 400 );
                }
                while ( lync.NewMessage );
            }
            threadRunning = false;
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
            threadRunning = true;
            thread = new System.Threading.Thread( PartyOn );
            thread.Start();
        }

        private void PartyOn()
        {
            int interval = 150;
            int count = 500;
            do
            {
                foreach ( BlyncControl.BlyncColor color in Enum.GetValues( typeof( BlyncControl.BlyncColor ) ) )
                {
                    System.Threading.Thread.Sleep( interval );
                    blync.SetColor( color );
                }
                count--;
            } while ( count > 0 );
            threadRunning = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ( blync != null )
                blync.CloseDevices();
        }
    }
}