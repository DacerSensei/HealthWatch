using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace HealthMonitoring.ViewModels
{
    public class ChatViewModel : ObservableObject
    {
        public ChatViewModel()
        {
            LoadedCommand = new AsyncCommand(LoadedExecute);
            RefreshCommand = new AsyncCommand(RefreshExecute);
        }

        public ObservableCollection<Messages> MessagesList { get; set; } = new ObservableCollection<Messages>();

        public ICommand LoadedCommand { get; }

        private async Task LoadedExecute()
        {
            MessagesList.Clear();
            try
            {
                IReadOnlyCollection<Firebase.Database.FirebaseObject<Messages>> initialData = await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Messages").OnceAsync<Messages>();

                foreach (var item in initialData)
                {
                    item.Object.Key = item.Key;
                    MessagesList.Add(item.Object);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            var SubsribeToSensors = Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Messages").AsObservable<Messages>().Subscribe(sensor =>
            {
                if (sensor.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                {
                    sensor.Object.Key = sensor.Key;
                    MessagesList.Add(sensor.Object);
                }
            }, error =>
            {
                Debug.WriteLine($"Error in subscription: {error.Message}");
            });
        }

        public ICommand RefreshCommand { get; }

        private async Task RefreshExecute()
        {
            MessagesList.Clear();
            try
            {
                IReadOnlyCollection<Firebase.Database.FirebaseObject<Messages>> initialData = await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Messages").OnceAsync<Messages>();

                foreach (var item in initialData)
                {
                    item.Object.Key = item.Key;
                    MessagesList.Add(item.Object);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            IsRefreshing = false;
        }

        bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
    }
}
