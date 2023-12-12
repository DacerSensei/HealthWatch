using Firebase.Database.Query;
using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using HealthMonitoring.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class DashboardViewModel : ObservableObject
    {
        public DashboardViewModel()
        {
            HeartRateCommand = new AsyncCommand(HeartRateExecute);
            FitnessCommand = new AsyncCommand(FitnessExecute);
            DoCommand = new AsyncCommand(DoExecute);
            UserManager.User.CurrentGoalChanged += User_CurrentGoalChanged;
            UserManager.User.DataSensors.StepCounterChanged += User_CurrentGoalChanged;
            GoalComplete += GoalCompleteEvent;
        }

        private async void GoalCompleteEvent(object sender, EventArgs e)
        {
            try
            {
                await DependencyService.Get<IBluetoothService>().WriteCharacteristicAsync("STOP_STEP_COUNTER");
                var result = await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Goals").OnceAsync<Goals>();
                if (result != null)
                {
                    var goal = result.FirstOrDefault(g => g.Object.IsCompleted);
                    if (goal != null)
                    {
                        await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Goals/{goal.Key}").PatchAsync(new { Status = "Completed", StepsTaken = UserManager.User.DataSensors.StepSensor });
                    }
                }
            }
            catch (Exception)
            {
                await ToastManager.ShowToast("Something went wrong", Color.FromHex("FF605C"));
            }
        }

        public EventHandler GoalComplete;

        private void User_CurrentGoalChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(GoalPercentage));
        }

        public ICommand HeartRateCommand { get; }
        private async Task HeartRateExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new HeartMonitor());
        }

        public ICommand FitnessCommand { get; }
        private async Task FitnessExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new GoalsMonitor());
        }

        public ICommand DoCommand { get; }
        private async Task DoExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new HeartMonitor());
        }

        public string GoalPercentage
        {
            get
            {
                if (UserManager.User.CurrentGoal != null)
                {
                    var goalsCalculation = ((Convert.ToDouble(UserManager.User.DataSensors.StepSensor) + Convert.ToDouble(UserManager.User.CurrentGoal.StepsTaken)) / Convert.ToDouble(UserManager.User.CurrentGoal.TotalSteps) * 100);
                    if(goalsCalculation >= 100)
                    {
                        GoalComplete?.Invoke(this, EventArgs.Empty);
                        return "100";
                    }
                    return $"{((Convert.ToDouble(UserManager.User.DataSensors.StepSensor) + Convert.ToDouble(UserManager.User.CurrentGoal.StepsTaken)) / Convert.ToDouble(UserManager.User.CurrentGoal.TotalSteps) * 100).ToString("0")}";
                }
                return "0";
            }
        }
    }
}
