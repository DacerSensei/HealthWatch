using Android.Widget;
using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using HealthMonitoring.Views;
using System;
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
    public class GoalsMonitorViewModel : ObservableObject
    {
        public GoalsMonitorViewModel()
        {
            LoadedCommand = new AsyncCommand(LoadedExecute);
            AddCommand = new AsyncCommand(AddExecute);
            BackCommand = new AsyncCommand(BackExecute);
            StartGoalCommand = new AsyncCommand(StartGoalExecute);
            DeleteCommand = new Command(DeleteExecute);
        }

        public ObservableCollection<Goals> GoalsList { get; } = new ObservableCollection<Goals>();

        public ICommand AddCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand StartGoalCommand { get; set; }

        private async void DeleteExecute(object parameter)
        {
            Goals goal = parameter as Goals;
            if (goal != null)
            {
                if (await Application.Current.MainPage.DisplayAlert("Notice", "Are you sure you want to delete?", "Yes", "No"))
                {
                    var deleteGoalTask = Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Goals/{goal.Key}").DeleteAsync();
                    var deleteCurrentGoalTask = Database.FirebaseClient.Child($"users/{UserManager.User.Key}/CurrentGoal").DeleteAsync();
                    try
                    {
                        await Task.WhenAll(deleteGoalTask, deleteCurrentGoalTask);
                        await LoadedExecute();
                        await ToastManager.ShowToast("Current goal has been deleted", Color.FromHex("#1eb980"));
                    }
                    catch (Exception) {
                        await ToastManager.ShowToast("Something went wrong", Color.FromHex("#FF605C"));
                    }
                }
            }
        }

        private async Task AddExecute()
        {
            if (GoalsList.Any(g => g.IsCompleted))
            {
                await ToastManager.ShowToast("You cannot add until you finish your current goal", Color.FromHex("#FF605C"));
                return;
            }
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddGoal());
        }

        private async Task StartGoalExecute()
        {
            try
            {
                if (!IsStarted)
                {
                    await DependencyService.Get<IBluetoothService>().WriteCharacteristicAsync("START_STEP_COUNTER");
                    StepMode = "Stop";
                    StepModeColor = "#d83c3e";
                }
                else
                {
                    await DependencyService.Get<IBluetoothService>().WriteCharacteristicAsync("STOP_STEP_COUNTER");
                    StepMode = "Start";
                    StepModeColor = "#1eb980";
                }
            }
            catch (Exception)
            {
                await ToastManager.ShowToast("Can't start goal, unable to detect smartwatch", Color.FromHex("#FF605C"));
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
                GoalsList.Clear();
                var result = await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Goals").OnceAsync<Goals>();
                foreach (var item in result.Reverse())
                {
                    item.Object.Key = item.Key;
                    GoalsList.Add(item.Object);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private string stepMode = "Start";

        public string StepMode
        {
            get => stepMode;
            set => SetProperty(ref stepMode, value);
        }

        private string stepModeColor = "#1eb980";

        public string StepModeColor
        {
            get => stepModeColor;
            set => SetProperty(ref stepModeColor, value);
        }

        private bool isStarted = false;

        public bool IsStarted
        {
            get => isStarted;
            set => SetProperty(ref isStarted, value);
        }

    }
}
