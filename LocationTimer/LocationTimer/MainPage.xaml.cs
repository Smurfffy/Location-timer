using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace LocationTimer
{
    public sealed partial class MainPage : Page
    {
        //Geolocator geoLo;
        DispatcherTimer Timer;
        Stopwatch stopwatch;
        private long milli, sec, min, hr, day, time;
        //List<long> timeList;

        private void btnLapReset_Click(object sender, RoutedEventArgs e)
        {
            if (btnLapReset.Content.ToString() == "Reset")
            {
                // rezero all timers
                stopwatch.Reset();
                day = hr = min = sec = milli = 0;
                tblTimeDisplay.Text = "00:00:00:00:000";
                btnLapReset.IsEnabled = false;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            //setupLocation();
            updateMap();
        }

        private async void updateMap()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            Geoposition position = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(2), TimeSpan.FromSeconds(30));
        }

       /* public async void setupLocation()
        {
            // ask for permission 
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    {
                        MessageDialog accessMsg = new MessageDialog("retreving location");
                        await accessMsg.ShowAsync();
                        geoLo = new Geolocator();
                        geoLo.DesiredAccuracy = PositionAccuracy.High;
                        //geoLo = new Geolocator { DesiredAccuracyInMeters = _desiredAccuracy };
                        geoLo.ReportInterval = (uint)5000;
                        // set up the events
                        // status changed, position changed
                        //geoLo.StatusChanged += MyGeo_StatusChanged;
                        // myGeo.PositionChanged += MyGeo_PositionChanged;
                        // get our current position.
                        //Geoposition pos = await myGeo.GetGeopositionAsync();

                        //updateMainPage(pos);

                        break;
                    }
                case GeolocationAccessStatus.Denied:
                    {
                        MessageDialog accessMsg = new MessageDialog("Please turn on location data");
                        await accessMsg.ShowAsync();
                        break;
                    }
                default:
                    {
                        MessageDialog accessMsg = new MessageDialog("Unspecified problem accessing location data");
                        await accessMsg.ShowAsync();
                        break;
                    }
            }
        }*/

      /*  private async void MyGeo_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            // use the dispatcher with lambda fuction to update the UI thread.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // code to run in method to update UI
                switch (args.Status)
                {
                    case PositionStatus.Ready:
                        // what here?
                        txtInformation.Text = "Locations services normal";
                        break;
                    case PositionStatus.Disabled:
                        txtInformation.Text = "Turn on location services";
                        break;
                    case PositionStatus.NoData:
                        txtInformation.Text = "No data received from Location services";
                        break;
                    case PositionStatus.Initializing:
                        txtInformation.Text = "Initialising Location services";
                        break;
                    default:
                        txtInformation.Text = "Unknown problem with your location services";
                        break;
                }

            });

        }*/

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
