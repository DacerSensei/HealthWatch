using Android.Bluetooth;
using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class HeartMonitorViewModel : ObservableObject
    {
        public HeartMonitorViewModel()
        {
            GoBackCommand = new AsyncCommand(GoBackExecute);
            LoadedCommand = new AsyncCommand(LoadedExecute);
        }

        public ICommand GoBackCommand { get; }
        private async Task GoBackExecute()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        public ICommand LoadedCommand { get; }

        private async Task LoadedExecute()
        {
            OtherSensor initialData = await Database.FirebaseClient.Child("sensors").OnceSingleAsync<OtherSensor>();
            BodyComposition = initialData.BodyComposition;
            BloodPressure = initialData.BloodPressure;
            BloodOxygen = initialData.BloodOxygen;

            var SubsribeToSensors = Database.FirebaseClient.Child("sensors").AsObservable<object>().Subscribe(sensor =>
            {
                if (sensor.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                {
                    if (sensor.Key.ToString() == "BodyComposition")
                    {
                        BodyComposition = sensor.Object.ToString();
                    }
                    if (sensor.Key.ToString() == "BloodPressure")
                    {
                        BloodPressure = sensor.Object.ToString();
                    }
                    if (sensor.Key.ToString() == "BloodOxygen")
                    {
                        BloodOxygen = sensor.Object.ToString();
                    }
                }
            }, error =>
            {
                Debug.WriteLine($"Error in subscription: {error.Message}");
            });
        }

        private string bloodPressure;
        public string BloodPressure
        {
            get => bloodPressure;
            set => SetProperty(ref bloodPressure, value);
        }

        private string bloodOxygen;
        public string BloodOxygen
        {
            get => bloodOxygen;
            set => SetProperty(ref bloodOxygen, value);
        }

        private string bodyComposition;
        public string BodyComposition
        {
            get => bodyComposition;
            set => SetProperty(ref bodyComposition, value);
        }
    }
}
