using Firebase.Database.Query;
using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using Newtonsoft.Json;
using Org.Apache.Http.Impl;
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
    public class AddGoalViewModel : ObservableObject
    {
        public AddGoalViewModel()
        {
            SaveCommand = new AsyncCommand(SaveExecute);
            BackCommand = new AsyncCommand(BackExecute);
        }

        public ICommand SaveCommand { get; set; }
        public ICommand BackCommand { get; set; }

        private async Task BackExecute()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private string meters;
        public string Meters
        {
            get => meters;
            set => SetProperty(ref meters, value);
        }


        private async Task SaveExecute()
        {
            try
            {
                var request = new Goals()
                {
                    StepsTaken = "0",
                    TotalSteps = Meters,
                    Created = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                    Completed = "Unknown",
                    Status = "Incomplete"
                };
                try
                {
                    await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Goals").PostAsync(request);
                    await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/CurrentGoal").PutAsync(request);
                    Services.UserManager.User.CurrentGoal = request;
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    await ToastManager.ShowToast("You set a new goal", Color.FromHex("#1eb980"));
                }
                catch (Exception)
                {
                    await ToastManager.ShowToast("Something went wrong", Color.FromHex("#FF605C"));
                }
                

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }
}
