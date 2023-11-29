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
    public class GetStartedViewModel : ObservableObject
    {
        public GetStartedViewModel()
        {
            LoginCommand = new AsyncCommand(LoginExecute);
            RegisterCommand = new AsyncCommand(RegisterExecute);
        }

        public ICommand LoginCommand { get; }
        private async Task LoginExecute()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Login());
        }

        public ICommand RegisterCommand { get; }
        private async Task RegisterExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Register());
        }
    }
}
