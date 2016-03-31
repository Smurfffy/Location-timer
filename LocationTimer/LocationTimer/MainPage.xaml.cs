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
        String longitude, latitude;

        private void btnLapReset_Click(object sender, RoutedEventArgs e)
        {
            if (btnLapReset.Content.ToString() == "Reset")
            {
                stopwatch.Reset();
                day = hr = min = sec = milli = 0;
                tblTimeDisplay.Text = "00:00:00:00:000";
                btnLapReset.IsEnabled = false;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    // Get the current location of the user.
                    Geolocator geolocator = new Geolocator();
                    Geoposition geoposition = await geolocator.GetGeopositionAsync();
                    Geopoint geopoint = geoposition.Coordinate.Point;

                    // Set the map location to the users current location.
                    Map.Center = geopoint;
                    Map.ZoomLevel = 18; // set the zoom level, 18 seems to be best suited to my ideas
                    //saves time taken in milliseconds
                    time = stopwatch.ElapsedMilliseconds;
                    //displays information to txtInformation text block
                    txtInformation.Text = ("You went from Latitude " + latitude + " Longitude " + longitude + " to " + "Latitute " + geoposition.Coordinate.Latitude.ToString() + " Longitude " + geoposition.Coordinate.Longitude.ToString() + " in "  + time.ToString() + "ms");
                    break;

                case GeolocationAccessStatus.Denied:
                    // yet to make it do something if acces is denied
                    break;

                case GeolocationAccessStatus.Unspecified:
                    // yet to make it do something if some unexpected happends
                    break;
            }
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

        private async void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (btnStartStop.Content.ToString() == "Start")
            {
                // start the timer, change the text
                Timer.Start();
                stopwatch.Start();
                btnStartStop.Content = "Pause";
                btnStartStop.Background = new SolidColorBrush(Colors.Red);

                // Set your current location.
                var accessStatus = await Geolocator.RequestAccessAsync();
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:

                        // Get the current location of the user.
                        Geolocator geolocator = new Geolocator();
                        Geoposition geoposition = await geolocator.GetGeopositionAsync();
                        Geopoint geopoint = geoposition.Coordinate.Point;

                        // Set the map location to the users current location.
                        Map.Center = geopoint;
                        Map.ZoomLevel = 18; // set the zoom level, 18 seems to be best suited to my ideas
                        longitude = geoposition.Coordinate.Longitude.ToString();
                        latitude = geoposition.Coordinate.Latitude.ToString();
                        break;

                    case GeolocationAccessStatus.Denied:
                        // yet to make it do something if acces is denied
                        break;

                    case GeolocationAccessStatus.Unspecified:
                        // yet to make it do something if some unexpected happends
                        break;
                }
            }
            else   //equal to stop
            {
                Timer.Stop();
                stopwatch.Stop();
                btnLapReset.IsEnabled = true;
                btnStartStop.Content = "Start";
                btnStartStop.Background = new SolidColorBrush(Colors.Green);
            }
        }
    }
}
