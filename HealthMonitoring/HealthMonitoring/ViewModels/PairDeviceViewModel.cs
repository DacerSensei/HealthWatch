using Android.Bluetooth;
using Android.Content;
using Firebase.Database.Query;
using HealthMonitoring.Config;
using HealthMonitoring.Services;
using HealthMonitoring.Views;
using System;
using System.Threading.Tasks;

using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Diagnostics;
using static Android.Graphics.BlurMaskFilter;
using System.Collections.Generic;
using HealthMonitoring.Models;
using System.Linq;

namespace HealthMonitoring.ViewModels
{
    public class PairDeviceViewModel : ObservableObject
    {
        public PairDeviceViewModel()
        {
            bluetoothService.BluetoothInitialize();
            ScanCommand = new AsyncCommand(ScanExecute);
            SelectDeviceCommand = new Command(SelectDeviceExecute);
        }

        IBluetoothService bluetoothService = DependencyService.Get<IBluetoothService>();

        public ICommand ScanCommand { get; }
        public ICommand SelectDeviceCommand { get; }
        public ICommand StartRecieveCommand { get; }
        public ICommand StopRecieveCommand { get; }

        private async Task ScanExecute()
        {

            if (!bluetoothService.IsBluetoothEnabled())
            {
                await ToastManager.ShowToast("Enable your bluetooth to scan for devices", Color.FromHex("FF605C"));
                return;
            }
            if (!DependencyService.Get<IDeviceOrientationService>().IsLocationServiceEnabled())
            {
                await ToastManager.ShowToast("Enable your location to scan for devices", Color.FromHex("FF605C"));
                return;
            }
            if (!IsConnected)
            {
                if (!await PermissionsGrantedAsync())
                {
                    await ToastManager.ShowToast("Application needs location permission", Color.FromHex("FF605C"));
                    IsLoading = false;
                    return;
                }
                BLEDevicesList.Clear();
                IsLoading = true;
                List<BluetoothDevice> devices = await bluetoothService.ScanForDevicesAsync();
                if (devices != null)
                {
                    for (int i = 0; i < devices.Count; i++)
                    {
                        BLEDevicesList.Add(devices[i]);
                    }
                }
                IsLoading = false;
            }
            else
            {
                bluetoothService.CharacteristicValueChanged -= BluetoothService_CharacteristicValueChanged;
                bluetoothService.DisconnectDevice();
            }
        }

        private async void SelectDeviceExecute(object obj)
        {
            BluetoothDevice device = obj as BluetoothDevice;
            IsLoading = true;
            if (await bluetoothService.ConnectToDevice(device))
            {
                BLEDevicesList.Clear();
                bluetoothService.ConnectionStateChanged += BluetoothService_ConnectionStateChanged;
                bluetoothService.CharacteristicValueChanged += BluetoothService_CharacteristicValueChanged;
                IsLoading = false;
            }
        }

        private async void BluetoothService_CharacteristicValueChanged(object sender, string e)
        {
            string heartRate = "HeartRate:";
            string stepCounter = "StepCounter:";
            if (e.Contains(heartRate))
            {
                var value = e.Substring(heartRate.Length);
                UserManager.User.DataSensors.HeartRateSensor = value;
                try
                {
                    await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/DataSensors").PatchAsync(new { HeartRateSensor = value });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else if (e.Contains(stepCounter))
            {
                var value = e.Substring(stepCounter.Length);
                Debug.WriteLine(value);
                UserManager.User.DataSensors.StepSensor = value;
                try
                {
                    await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/DataSensors").PatchAsync(new { StepSensor = value });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

        }

        private async void BluetoothService_ConnectionStateChanged(object sender, ProfileState value)
        {
            if (value == ProfileState.Connected)
            {
                IsConnected = true;
                ButtonStateText = "Disconnect";
                try
                {
                    await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/DataSensors").PatchAsync(new { SmartWatchStatus = "Connected" });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else if (value == ProfileState.Disconnected)
            {
                IsConnected = false;
                ButtonStateText = "Scan Devices";
                try
                {
                    await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/DataSensors").PatchAsync(new { SmartWatchStatus = "Disconnected" });
                    await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/DataSensors").PatchAsync(new { StepSensor = "0" });
                    var result = await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Goals").OnceAsync<Goals>();
                    if (result != null)
                    {
                        var goal = result.FirstOrDefault(g => g.Object.IsCompleted);
                        if (goal != null)
                        {
                            await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/CurrentGoal").PatchAsync(new { StepsTaken = UserManager.User.DataSensors.StepSensor });
                            await Database.FirebaseClient.Child($"users/{UserManager.User.Key}/Goals/{goal.Key}").PatchAsync(new { StepsTaken = UserManager.User.DataSensors.StepSensor });
                        }
                        UserManager.User.DataSensors.StepSensor = "0";
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
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
