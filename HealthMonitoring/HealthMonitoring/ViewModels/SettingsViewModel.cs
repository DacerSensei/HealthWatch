using HealthMonitoring.Config;
using HealthMonitoring.Views;
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
    public class SettingsViewModel : ObservableObject
    {
        public SettingsViewModel()
        {
            LogoutCommand = new AsyncCommand(LogoutExecute);
            EditInformationCommand = new AsyncCommand(EditInformationExecute);
            ChangePasswordCommand = new AsyncCommand(ChangePasswordExecute);
        }

        public ICommand LogoutCommand { get; }
        public ICommand EditInformationCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        private async Task LogoutExecute()
        {
            Database.FirebaseAuthClient.SignOut();
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async Task EditInformationExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new EditInformation());
        }

        private async Task ChangePasswordExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ChangePassword());

        }
    }
}
