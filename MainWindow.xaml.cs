using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MeetingCostTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public bool IsTimerRunning
        {
            set
            {
                btnPause.IsEnabled = value;
                //inputParticipantCount.IsEnabled = !value;
                btnStart.IsEnabled = !value;
                btnReset.IsEnabled = !value;
                inputCostPerHourPerPerson.IsEnabled = !value;
            }
        }
        private double timerInterval = 1000;
        private System.Timers.Timer t;
        private double elapsedTimeMs = 0;
        private double currentCost = 0;

        public MainWindow()
        {
            InitializeComponent();
            updateWindowOnTop();
            initTimer();
            updateGui(0);
        }

        private void initTimer()
        {
            t = new System.Timers.Timer(timerInterval);
            t.Elapsed += timerElapsed;

        }



        private string buildElapsedTimeString(double elapsedTimeMs)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(elapsedTimeMs);
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);
        }
        private void timerElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                elapsedTimeMs += timerInterval;
                currentCost += timerInterval / 1000 / 60 / 60 * Properties.Settings.Default.CostPerPersonHour * Properties.Settings.Default.Participants;

                updateGui(elapsedTimeMs);
            }));
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }


        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            t.Stop();
            IsTimerRunning = false;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Reset?", "Reset Timer", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                elapsedTimeMs = 0;
                currentCost = 0;
                updateGui(elapsedTimeMs);
            }

        }

        private void updateGui(double elapsedTimeMs)
        {

            labelElapsedTime.Content = buildElapsedTimeString(elapsedTimeMs);
            labelCost.Content = string.Format("{0:N0}", currentCost).Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, "") + " €";
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            IsTimerRunning = true;
            t.Start();
        }

        private void CBoxWindowOnTop_Changed(object sender, RoutedEventArgs e)
        {
            updateWindowOnTop();
        }
        private void updateWindowOnTop()
        {
            mainWindow.Topmost = Properties.Settings.Default.KeepWindowOnTop;
        }
    }
}
