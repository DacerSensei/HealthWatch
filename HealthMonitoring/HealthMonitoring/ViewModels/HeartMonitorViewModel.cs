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
            bluetoothService.WriteCharacteristic("START_HEART_RATE");
            bluetoothService.CharacteristicValueChanged += HeartMonitorViewModel_CharacteristicValueChanged;
        }

        IBluetoothService bluetoothService = DependencyService.Get<IBluetoothService>();

        private void HeartMonitorViewModel_CharacteristicValueChanged(object sender, string e)
        {
            string find = "HeartRate:";
            if (e.Contains(find))
            {
                HeartRate = e.Substring(find.Length);
            }
        }

        private string heartRate;
        public string HeartRate
        {
            get => heartRate;
            set => SetProperty(ref heartRate, value);
        }


        public ICommand GoBackCommand { get; }
        private async Task GoBackExecute()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
