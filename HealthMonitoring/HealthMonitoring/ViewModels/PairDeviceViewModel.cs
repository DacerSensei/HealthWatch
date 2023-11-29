using Android.Bluetooth;
using Android.Content;
using HealthMonitoring.Services;
using HealthMonitoring.Views;
using System.Threading.Tasks;

using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class PairDeviceViewModel : ObservableObject
    {
        public PairDeviceViewModel()
        {
            bluetoothService.BluetoothInitialize();
            ScanCommand = new AsyncCommand(ScanExecute);
            SelectDeviceCommand = new Command(SelectDeviceExecute);
            StartRecieveCommand = new AsyncCommand(StartRecieveExecute);
            StopRecieveCommand = new Command(StopRecieveExecute);
        }

        IBluetoothService bluetoothService = DependencyService.Get<IBluetoothService>();

        public ICommand ScanCommand { get; }
        public ICommand SelectDeviceCommand { get; }
        public ICommand StartRecieveCommand { get; }
        public ICommand StopRecieveCommand { get; }

        private void StopRecieveExecute()
        {
            //bluetoothService.WriteCharacteristic("STOP_HEART_RATE");
        }

        private async Task StartRecieveExecute()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new HeartMonitor());
            //bluetoothService.WriteCharacteristic("START_HEART_RATE");
        }

        private async Task ScanExecute()
        {
            if (!IsConnected)
            {
                if (!await PermissionsGrantedAsync())
                {
                    await Application.Current.MainPage.DisplayAlert("Permission required", "Application needs location permission", "OK");
                    IsLoading = false;
                    return;
                }
                BLEDevicesList.Clear();
                IsLoading = true;
                var devices = await bluetoothService.ScanForDevicesAsync();
                for (int i = 0; i < devices.Count; i++)
                {
                    BLEDevicesList.Add(devices[i]);
                }
                IsLoading = false;
            }
            else
            {
                bluetoothService.DisconnectDevice();
            }
        }

        private async void SelectDeviceExecute(object obj)
        {
            BluetoothDevice device = obj as BluetoothDevice;
            IsLoading = true;
            await bluetoothService.ConnectToDevice(device);
            BLEDevicesList.Clear();
            IsLoading = false;
            bluetoothService.ConnectionStateChanged += BluetoothService_ConnectionStateChanged;
        }

        private void BluetoothService_ConnectionStateChanged(object sender, ProfileState value)
        {
            if (value == ProfileState.Connected)
            {
                IsConnected = true;
                ButtonStateText = "Disconnect";
            }
            else if (value == ProfileState.Disconnected)
            {
                IsConnected = false;
                ButtonStateText = "Scan Devices";
            }
        }

        public ObservableRangeCollection<BluetoothDevice> BLEDevicesList { get; } = new ObservableRangeCollection<BluetoothDevice>();

        private bool isLoading;

        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        private bool isConnected = false;

        public bool IsConnected
        {
            get => isConnected;
            set
            {
                SetProperty(ref isConnected, value);
                OnPropertyChanged(nameof(ButtonStateColor));
            }
        }

        private string buttonStateText = "Scan Devices";
        public string ButtonStateText
        {
            get => buttonStateText;
            set => SetProperty(ref buttonStateText, value);
        }

        public string ButtonStateColor
        {
            get
            {
                if (IsConnected)
                {
                    return "#d83c3e";
                }
                else
                {
                    return "#000";
                }
            }
        }

        private async Task<bool> PermissionsGrantedAsync()      // Function to make sure that all the appropriate approvals are in place
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }

    }
}
