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
        }

        public ObservableCollection<Goals> GoalsList { get; } = new ObservableCollection<Goals>();

        public ICommand AddCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand StartGoalCommand { get; set; }

        private async Task AddExecute()
        {
            if(GoalsList.Any(g => g.IsCompleted))
            {
                await ToastManager.ShowToast("You cannot add until you finish your current goal", Color.FromHex("#FF605C"));
                return;
            }
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddGoal());
            await LoadedExecute();
        }

        private async Task StartGoalExecute()
        {
            await DependencyService.Get<IBluetoothService>().WriteCharacteristicAsync("START_STEP_COUNTER");
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
                foreach(var item in result.Reverse())
                {
                    item.Object.Key = item.Key;
                    GoalsList.Add(item.Object);
                }
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
