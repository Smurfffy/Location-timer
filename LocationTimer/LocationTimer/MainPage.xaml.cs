using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;// msdn universal app documentation referenced for map and geolocation.
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
using Windows.Media.Capture;// msdn universal app documentaion referenced for camera
using Windows.Storage;

namespace LocationTimer
{
    public sealed partial class MainPage : Page
    {
        DispatcherTimer Timer;
        Stopwatch stopwatch;
        private long milli, sec, min, hr, day, time;
        String longitude, latitude;

        private async void btnPhoto_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI camera = new CameraCaptureUI(); // creates the camera interface
            camera.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg; // saves image as a jpeg
            camera.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);// default crop size

            StorageFile photoTaken = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo); // saves the photo

            //handles program if a photo is not taken.
            if (photoTaken == null)
            {
                return;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //Resets the time to 00:00:00:00:000
            if (btnReset.Content.ToString() == "Reset")
            {
                stopwatch.Reset();
                day = hr = min = sec = milli = 0;
                tblTimeDisplay.Text = "00:00:00:00:000";
                btnReset.IsEnabled = false; // greys out reset button
                btnFinish.IsEnabled = false; // greys out finish buttons
                btnStartStop.IsEnabled = true; // enables start button
                btnPhoto.IsEnabled = false; // disables photo button
                txtInformation.Text = ""; // clears text block
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            btnStartStop.IsEnabled = false;
            btnPhoto.IsEnabled = true;
            btnFinish.IsEnabled = false;
            //location gotten again for results
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    // Get the new location of the user.
                    Geolocator geolocator = new Geolocator();
                    Geoposition geoposition = await geolocator.GetGeopositionAsync();
                    Geopoint geopoint = geoposition.Coordinate.Point;

                    // Set the map location to the users new location.
                    Map.Center = geopoint;
                    Map.ZoomLevel = 18; // set the zoom level, 18 seems to be best suited to my ideas
                    //saves time taken in milliseconds
                    time = stopwatch.ElapsedMilliseconds;
                    //displays information to txtInformation text block
                    txtInformation.Text = ("You went from Latitude: " + latitude + ", Longitude: " + longitude + " to " + "Latitute: " + geoposition.Coordinate.Latitude.ToString() + ", Longitude: " + geoposition.Coordinate.Longitude.ToString() + " in "  + time.ToString() + "ms");
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

        //Finding out the ellapsed time in milliseconds
        private void MyStopwatchTimer_Tick(object sender, object e)
        {
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
                //Timer starts
                Timer.Start();
                stopwatch.Start();
                btnStartStop.Content = "Pause"; // Changes text of button to Pause
                btnReset.IsEnabled = false; // disables reset button
                btnFinish.IsEnabled = false; // disables finish button
                btnStartStop.Background = new SolidColorBrush(Colors.Red); // changed colour of button to red

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
            else
            {
                //Timer Stops
                Timer.Stop();
                stopwatch.Stop();
                btnReset.IsEnabled = true; // enables reset button
                btnFinish.IsEnabled = true; // enables finish button
                btnStartStop.Content = "Start";
                btnStartStop.Background = new SolidColorBrush(Colors.Green);
            }
        }
    }
}
