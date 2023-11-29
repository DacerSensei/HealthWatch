using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HealthMonitoring.Droid.Services;
using HealthMonitoring.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceOrientationService))]
namespace HealthMonitoring.Droid.Services
{
    class DeviceOrientationService : IDeviceOrientationService
    {
        public bool IsLocationServiceEnabled()
        {
            LocationManager locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
        }
    }
}