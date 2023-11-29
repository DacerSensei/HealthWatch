using Firebase.Auth;
using HealthMonitoring.Config;
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
        }



        public ICommand HeartRateCommand { get; }
        private async Task HeartRateExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new HeartMonitor());
        }

        public ICommand FitnessCommand { get; }
        private async Task FitnessExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new HeartMonitor());
        }

        public ICommand DoCommand { get; }
        private async Task DoExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new HeartMonitor());
        }

    }
}
