using Microsoft.Lync.Model;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace BetterBlync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlyncControl blync = new BlyncControl();
        private LyncStatus lync = new LyncStatus();
        private DispatcherTimer timer;
        private Thread thread;

        private bool threadRunning = false, firstMinimizeShown = false;
        private BlyncControl.BlyncColor IncomingCall, IncomingIM, Available, Busy, Away, DoNotDisturb;

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
                setStatusLight();
                initTimer();
                initSettings();
            }
            catch ( Exception ex )
            {
                MessageBox.Show( $"Something went wrong!\nError: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error );
                Close();
            }
        }

        private void initSettings()
        {
            IncomingCall = (BlyncControl.BlyncColor)Enum.Parse( typeof( BlyncControl.BlyncColor ), Properties.Settings.Default.Color_IncomingCall );
            cbIncomingCall.SelectedItem = "Red";
            IncomingIM = (BlyncControl.BlyncColor)Enum.Parse( typeof( BlyncControl.BlyncColor ), Properties.Settings.Default.Color_IncomingIM );
            Available = (BlyncControl.BlyncColor)Enum.Parse( typeof( BlyncControl.BlyncColor ), Properties.Settings.Default.Color_Available );
            Busy = (BlyncControl.BlyncColor)Enum.Parse( typeof( BlyncControl.BlyncColor ), Properties.Settings.Default.Color_Busy );
            Away = (BlyncControl.BlyncColor)Enum.Parse( typeof( BlyncControl.BlyncColor ), Properties.Settings.Default.Color_Away );
            DoNotDisturb = (BlyncControl.BlyncColor)Enum.Parse( typeof( BlyncControl.BlyncColor ), Properties.Settings.Default.Color_DoNotDisturb );

            blync = new BlyncControl();
            lync = new LyncStatus();
        }

        private void initTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan( 0, 0, 0, 0, 200 );
            timer.Tick += new EventHandler( Timer_Tick );
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Check to see if any new lights are connected
            // blync.FindBlyncLights();

            // Get current status from Lync client
            lync.GetStatus();

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
                        thread = new Thread( () => flashLight( AlertType.Call ) );
                    else if ( lync.NewMessage )
                        thread = new Thread( () => flashLight( AlertType.IM ) );
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
                    if ( blync.CurrentColor != Available )
                        blync.SetColor( Available );
                    break;

                case ContactAvailability.Busy:
                    if ( blync.CurrentColor != Busy )
                        blync.SetColor( Busy );
                    break;

                case ContactAvailability.DoNotDisturb:
                    if ( blync.CurrentColor != DoNotDisturb )
                        blync.SetColor( DoNotDisturb );
                    break;

                case ContactAvailability.Offline:
                    blync.TurnOffLight();
                    break;

                case ContactAvailability.FreeIdle:
                case ContactAvailability.TemporarilyAway:
                case ContactAvailability.Away:
                    if ( blync.CurrentColor != Away )
                        blync.SetColor( Away );
                    break;

                default:
                    if ( blync.CurrentColor != BlyncControl.BlyncColor.Green )
                        blync.SetColor( BlyncControl.BlyncColor.Green );
                    break;
            }
        }

        private void flashLight(AlertType type)
        {
            if ( type == AlertType.Call )
            {
                do
                {
                    blync.SetColor( IncomingCall );
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
                    blync.SetColor( IncomingIM );
                    System.Threading.Thread.Sleep( 400 );
                    blync.TurnOffLight();
                    System.Threading.Thread.Sleep( 400 );
                }
                while ( lync.NewMessage );
            }
            threadRunning = false;
        }

        private void cbIncomingCall_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string newColor = cbIncomingCall.SelectedValue.ToString();
            newColor = newColor.Substring( newColor.LastIndexOf( ' ' ) ).Trim();

            IncomingCall = (BlyncControl.BlyncColor)Enum.Parse( typeof( BlyncControl.BlyncColor ), newColor ); ;

            Properties.Settings.Default["Color_IncomingCall"] = newColor;
            Properties.Settings.Default.Save();
        }

        private void cbIncomingMessage_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string newColor = cbIncomingCall.SelectedValue.ToString();
            newColor = newColor.Substring( newColor.LastIndexOf( ' ' ) ).Trim();

            IncomingCall = (BlyncControl.BlyncColor)Enum.Parse( typeof( BlyncControl.BlyncColor ), newColor ); ;

            Properties.Settings.Default["Color_IncomingIM"] = newColor;
            Properties.Settings.Default.Save();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            blync.FindBlyncLights();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ( blync != null )
                blync.CloseDevices();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            string title = "Better Blync";
            string message = "Better Blync is still running in the background...";
            if ( WindowState == WindowState.Minimized )
            {
                if ( !firstMinimizeShown )
                {
                    firstMinimizeShown = true;
                    blyncNotifyIcon.ShowBalloonTip( title, message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info );
                }

                ShowInTaskbar = false;
            }
            else
            {
                ShowInTaskbar = true;
            }
        }

        private void cntxExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void blyncNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            // Return window to screen when tray icon is double clicked
            WindowState = WindowState.Normal;
        }
    }
}