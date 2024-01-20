using Firebase.Database.Query;
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
    public class AddGoalCaloriesViewModel : ObservableObject
    {
        public AddGoalCaloriesViewModel()
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

        private string calories;
        public string Calories
        {
            get => calories;
            set => SetProperty(ref calories, value);
        }


        private async Task SaveExecute()
        {
            try
            {
                var request = new Calories()
                {
                    CalorieTaken = "0",
                    GoalCalorie = Calories,
                    Created = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                    Status = "Incomplete"
                };
                try
                {
                    await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Calories").PostAsync(request);
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    await ToastManager.ShowToast("You set a new calories goal", Color.FromHex("#1eb980"));
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
