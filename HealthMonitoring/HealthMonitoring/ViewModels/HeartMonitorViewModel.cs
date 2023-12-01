using Android.Bluetooth;
using HealthMonitoring.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class HeartMonitorViewModel : ObservableObject
    {
        public HeartMonitorViewModel()
        {
            GoBackCommand = new AsyncCommand(GoBackExecute);
            //bluetoothService.WriteCharacteristic("START_HEART_RATE");
        }

        IBluetoothService bluetoothService = DependencyService.Get<IBluetoothService>();


        public ICommand GoBackCommand { get; }
        private async Task GoBackExecute()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
