﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LocationTimer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        DispatcherTimer Timer;
        Stopwatch stopwatch;
        private long milli, sec, min, hr, day, time;
        List<long> timeList;

        private void btnLapReset_Click(object sender, RoutedEventArgs e)
        {
            //long timeRightNow;
            //TextBlock tblLapTime;

            if (btnLapReset.Content.ToString() == "Reset")
            {
                // rezero all timers
                stopwatch.Reset();
                day = hr = min = sec = milli = 0;
                tblTimeDisplay.Text = "00:00:00:00:000";
                btnLapReset.IsEnabled = false;

            }
            /*else     // Text = "Lap"
            {
                // save the current time, add to list
                if (timeList == null)
                {
                    timeList = new List<long>();
                    time = 0;
                }
                // get the ellapsed milliseconde
                // subtract the last one and then store the difference
                timeRightNow = stopwatch.ElapsedMilliseconds;
                timeList.Add(timeRightNow - time);
                time = timeRightNow;

                tblLapTime = new TextBlock();
                tblLapTime.Text = timeList.Last().ToString();
                tblLapTime.HorizontalAlignment = HorizontalAlignment.Center;

                spLapTimes.Children.Add(tblLapTime);
            }*/
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (stopwatch == null)
            {
                stopwatch = new Stopwatch();
            }
            // check for the timer and then set up.
            if (Timer == null)
            {
                milli = sec = min = hr = day = 0;
                Timer = new DispatcherTimer();
                Timer.Tick += MyStopwatchTimer_Tick;
                Timer.Interval = new TimeSpan(0, 0, 0, 0, 1); // 1 millisecond
            }
            base.OnNavigatedTo(e);
        }

        private void MyStopwatchTimer_Tick(object sender, object e)
        {
            // update the textblock with the time elapsed
            // figure out the elapsed time using the timer properties
            // some maths division and modulus
            milli = stopwatch.ElapsedMilliseconds;

            sec = milli / 1000;
            milli = milli % 1000;

            min = sec / 60;
            sec = sec % 60;

            hr = min % 60;
            min = min % 60;

            day = hr / 24;
            hr = hr % 24;

            tblTimeDisplay.Text = hr.ToString("00") + ":" +
                                   min.ToString("00") + ":" +
                                   sec.ToString("00") + ":" +
                                   milli.ToString("000");

        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            // start the stop watch
            // kick off a timer.
            if (btnStartStop.Content.ToString() == "Start")
            {
                // start the timer, change the text
                Timer.Start();
                stopwatch.Start();
                //btnLapReset.Content = "Lap";
                btnStartStop.Content = "Pause";
                btnStartStop.Background = new SolidColorBrush(Colors.Red);
            }
            else   //equal to stop
            {
                Timer.Stop();
                stopwatch.Stop();
                //btnLapReset.Content = "Reset";
                btnLapReset.IsEnabled = true;
                btnStartStop.Content = "Start";
                btnStartStop.Background = new SolidColorBrush(Colors.Green);
            }
        }
    }
}