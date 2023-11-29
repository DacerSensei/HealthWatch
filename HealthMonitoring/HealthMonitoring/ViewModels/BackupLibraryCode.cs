using HealthMonitoring.Models;
using HealthMonitoring.Services;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class BackupLibraryCode : ObservableObject
    {
        public static ICharacteristic Characteristic { get; private set; }
        public static IService Service { get; private set; }
        public BackupLibraryCode()
        {
            bluetooth = CrossBluetoothLE.Current;
            bluetoothAdapter = CrossBluetoothLE.Current.Adapter;// Point _bluetoothAdapter to the current adapter on the phone

            bluetoothAdapter.DeviceDiscovered += (sender, foundBleDevice) =>   // When a BLE Device is found, run the small function below to add it to our list
            {
                if (foundBleDevice.Device != null && !string.IsNullOrEmpty(foundBleDevice.Device.Name))
                {
                    BLEDevicesList.Add(foundBleDevice.Device);
                }
            };
            ScanCommand = new AsyncCommand(ScanExecute);
            SelectDeviceCommand = new Command(SelectDeviceExecute);
            StartRecieveCommand = new AsyncCommand(StartRecieveExecute);
            StopRecieveCommand = new AsyncCommand(StopRecieveExecute);
        }

        private readonly IBluetoothLE bluetooth;
        private readonly IAdapter bluetoothAdapter; // Class for the Bluetooth adapter

        public ICommand ScanCommand { get; }
        public ICommand SelectDeviceCommand { get; }
        public ICommand StartRecieveCommand { get; }
        public ICommand StopRecieveCommand { get; }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private bool isLoading;

        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        private string heartRate;
        public string HeartRate
        {
            get => heartRate;
            set => SetProperty(ref heartRate, value);
        }

        public ObservableRangeCollection<IDevice> BLEDevicesList { get; } = new ObservableRangeCollection<IDevice>();

        private async Task StopRecieveExecute()
        {
            if (Characteristic != null)
            {
                IsLoading = true;
                cancellationTokenSource.Cancel();
                if (Characteristic.CanWrite)
                {
                    byte[] stopHeartRate = Encoding.UTF8.GetBytes("STOP_HEART_RATE");
                    if(await Characteristic.WriteAsync(stopHeartRate) == 0)
                    {
                        Debug.WriteLine("Write Successfully STOP_HEART_RATE");
                    }
                }
                IsLoading = false;
                
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Notice", "You are not connected to bluetooth", "Ok");
            }
        }

        private async Task StartRecieveExecute()
        {
            if (Characteristic != null)
            {
                IsLoading = true;
                if (Characteristic.CanWrite)
                {
                    byte[] startHeartRate = Encoding.UTF8.GetBytes("START_HEART_RATE");
                    if (await Characteristic.WriteAsync(startHeartRate) == 0)
                    {
                        Debug.WriteLine("Write Successfully START_HEART_RATE");
                    }
                }
                if (Characteristic.CanUpdate)
                {
                    Characteristic.ValueUpdated += (o, args) =>
                    {
                        HeartRate = args.Characteristic.StringValue;
                        Debug.WriteLine(args.Characteristic.StringValue);
                    };
                    IsLoading = false;
                    await Characteristic.StartUpdatesAsync(cancellationTokenSource.Token);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Notice", "You are not connected to bluetooth", "Ok");
            }

        }

        private async Task ScanExecute()
        {
            if (bluetooth.State == BluetoothState.Off)
            {
                await Application.Current.MainPage.DisplayAlert("Oops!", "You need to turn on your Bluetooth first", "OK");
                return;
            }
            if (!DependencyService.Get<IDeviceOrientationService>().IsLocationServiceEnabled())
            {
                await Application.Current.MainPage.DisplayAlert("Oops!", "You need to turn on your Location first", "OK");
                return;
            }

            if (bluetooth.IsOn)
            {
                IsLoading = true;       // Swith the Isbusy Indicator on

                if (!await PermissionsGrantedAsync())
                {
                    await Application.Current.MainPage.DisplayAlert("Permission required", "Application needs location permission", "OK");
                    IsLoading = false;
                    return;
                }

                BLEDevicesList.Clear();

                if (!bluetoothAdapter.IsScanning)                                                              // Make sure that the Bluetooth adapter is scanning for devices
                {
                    bluetoothAdapter.ScanMode = ScanMode.LowLatency;
                    await bluetoothAdapter.StartScanningForDevicesAsync();
                }
                IsLoading = false;
            }
        }

        private async void SelectDeviceExecute(object obj)
        {
            BLEDevicesList.Clear();
            IDevice device = obj as IDevice;

            if (device.State == DeviceState.Connected)
            {
                await Application.Current.MainPage.DisplayAlert("Notice", "You already connected to this Device", "Ok");
                return;
            }
            else
            {
                try
                {
                    ConnectParameters connectParameters = new ConnectParameters(false, true);
                    await bluetoothAdapter.ConnectToDeviceAsync(device, connectParameters);          // if we are not connected, then try to connect to the BLE Device selected
                    Guid serviceGuid = new Guid("491ef3e4-be34-4c5d-8d94-45a2eddbb9df");
                    Service = await device.GetServiceAsync(serviceGuid);
                    if (Service != null)
                    {
                        Guid characteristicGuid = new Guid("8d68ee2e-5e8a-403e-b60f-c371209162b6");
                        Characteristic = await Service.GetCharacteristicAsync(characteristicGuid);
                    }
                }
                catch
                {
                    await Application.Current.MainPage.DisplayAlert("Error connecting", $"Error connecting to BLE device: {device.Name ?? "N/A"}", "Retry");       // give an error message if it is not possible to connect
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
