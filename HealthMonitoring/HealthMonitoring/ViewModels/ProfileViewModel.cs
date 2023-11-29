using HealthMonitoring.Config;
using HealthMonitoring.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class ProfileViewModel : ObservableObject
    {
        public ProfileViewModel()
        {
            LogoutCommand = new AsyncCommand(LogoutExecute);
        }

        public ICommand LogoutCommand { get; }
        private async Task LogoutExecute()
        {
            Database.FirebaseAuthClient.SignOut();
            await Application.Current.MainPage.Navigation.PushAsync(new Login());
        }
    }
}
