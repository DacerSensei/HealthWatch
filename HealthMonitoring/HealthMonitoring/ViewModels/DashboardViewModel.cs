using Firebase.Auth;
using HealthMonitoring.Config;
using HealthMonitoring.Services;
using HealthMonitoring.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Services.UserManager.User.CurrentGoalChanged += User_CurrentGoalChanged;
        }

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
            await Application.Current.MainPage.Navigation.PushAsync(new GoalsMonitor());
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
                if (Services.UserManager.User.CurrentGoal != null)
                {
                    return $"{((Convert.ToDouble(Services.UserManager.User.DataSensors.StepSensor) + Convert.ToDouble(Services.UserManager.User.CurrentGoal.StepsTaken)) / Convert.ToDouble(Services.UserManager.User.CurrentGoal.TotalSteps) * 100).ToString("0")}";
                }
                return "0";
            }
        }
    }
}
