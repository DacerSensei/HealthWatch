using Android.Widget;
using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace HealthMonitoring.ViewModels
{
    public class GoalsMonitorViewModel : ObservableObject
    {
        public GoalsMonitorViewModel()
        {
            LoadedCommand = new AsyncCommand(LoadedExecute);
        }

        
        public ObservableCollection<Goals> GoalsList { get; } = new ObservableCollection<Goals>();

        public ICommand LoadedCommand { get; set; }

        private async Task LoadedExecute()
        {
            try
            {
                var result = await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Goals").OnceAsync<Goals>();
                foreach(var item in result)
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
