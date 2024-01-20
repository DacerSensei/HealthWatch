using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using HealthMonitoring.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class FoodManagementViewModel : ObservableObject
    {
        public FoodManagementViewModel()
        {
            LoadedCommand = new AsyncCommand(LoadedExecute);
            AddCommand = new AsyncCommand(AddExecute);
            BackCommand = new AsyncCommand(BackExecute);
            AddGoalTakenCommand = new Command(AddGoalTakenExecute);
            DeleteCommand = new Command(DeleteExecute);
        }

        public ObservableCollection<Calories> CaloriesList { get; } = new ObservableCollection<Calories>();

        public ICommand AddCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand AddGoalTakenCommand { get; set; }

        private async void DeleteExecute(object parameter)
        {
            Calories calories = parameter as Calories;
            if (calories != null)
            {
                if (await Application.Current.MainPage.DisplayAlert("Notice", "Are you sure you want to delete?", "Yes", "No"))
                {
                    var deleteGoalTask = Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Calories/{calories.Key}").DeleteAsync();
                    try
                    {
                        await Task.WhenAll(deleteGoalTask);
                        await LoadedExecute();
                        await ToastManager.ShowToast("Current goal has been deleted", Color.FromHex("#1eb980"));
                    }
                    catch (Exception)
                    {
                        await ToastManager.ShowToast("Something went wrong", Color.FromHex("#FF605C"));
                    }
                }
            }
        }

        private async Task AddExecute()
        {
            if (CaloriesList.Any(g => g.IsCompleted))
            {
                await ToastManager.ShowToast("You cannot add until you finish your current goal", Color.FromHex("#FF605C"));
                return;
            }
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddGoalCalories());
        }

        private async void AddGoalTakenExecute(object parameter)
        {
            Calories calorie = parameter as Calories;
            if (calorie != null)
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new AddCalories()
                {
                    BindingContext = new AddCaloriesViewModel(calorie)
                });
            }
        }

        private async Task BackExecute()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private async Task LoadedExecute()
        {
            try
            {
                CaloriesList.Clear();
                var result = await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Calories").OnceAsync<Calories>();
                foreach (var item in result.Reverse())
                {
                    item.Object.Key = item.Key;
                    CaloriesList.Add(item.Object);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
