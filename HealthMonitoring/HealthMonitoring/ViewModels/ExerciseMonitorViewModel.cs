using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class ExerciseMonitorViewModel : ObservableObject
    {
        public ExerciseMonitorViewModel()
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
            Water = initialData.Water;
            WorkOutSession = initialData.WorkOutSession;

            var SubsribeToSensors = Database.FirebaseClient.Child("sensors").AsObservable<object>().Subscribe(sensor =>
            {
                if (sensor.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                {
                    if (sensor.Key.ToString() == "Water")
                    {
                        Water = sensor.Object.ToString();
                    }
                    if (sensor.Key.ToString() == "WorkOutSession")
                    {
                        WorkOutSession = sensor.Object.ToString();
                    }
                }
            }, error =>
            {
                Debug.WriteLine($"Error in subscription: {error.Message}");
            });

            var result = await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Calories").OnceAsync<Calories>();
            TotalCalories = result.Sum(c => Convert.ToInt16(c.Object.CalorieTaken));

        }

        private string water;
        public string Water
        {
            get => water;
            set => SetProperty(ref water, value);
        }

        private string workOutSession;
        public string WorkOutSession
        {
            get => workOutSession;
            set => SetProperty(ref workOutSession, value);
        }

        private int totalCalories;
        public int TotalCalories
        {
            get => totalCalories;
            set => SetProperty(ref totalCalories, value);
        }
    }
}
