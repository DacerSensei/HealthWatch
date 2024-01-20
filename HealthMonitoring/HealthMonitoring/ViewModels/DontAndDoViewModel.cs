using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class DontAndDoViewModel : ObservableObject
    {
        public DontAndDoViewModel()
        {
            GoBackCommand = new AsyncCommand(GoBackExecute);

        }

        public ICommand GoBackCommand { get; }
        private async Task GoBackExecute()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
