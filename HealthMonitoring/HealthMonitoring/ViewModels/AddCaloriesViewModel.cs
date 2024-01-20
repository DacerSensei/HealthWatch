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
    public class AddCaloriesViewModel : ObservableObject
    {
        private Calories currentCalories;
        public AddCaloriesViewModel(Calories currentCalories)
        {
            this.currentCalories = currentCalories;
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
                int lastTaken = Convert.ToInt16(currentCalories.CalorieTaken);
                int currentTaken = Convert.ToInt16(Calories);
                await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Calories/{currentCalories.Key}").PatchAsync(new { CalorieTaken = lastTaken + currentTaken });
                await Application.Current.MainPage.Navigation.PopModalAsync();
                await ToastManager.ShowToast("You have taken a calories", Color.FromHex("#1eb980"));
            }
            catch (Exception)
            {
                await ToastManager.ShowToast("Something went wrong", Color.FromHex("#FF605C"));
            }
        }
    }
}
