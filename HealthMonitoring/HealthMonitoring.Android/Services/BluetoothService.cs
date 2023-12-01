using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using HealthMonitoring.Droid.Services;
using HealthMonitoring.Services;
using Java.Util;
using Plugin.BLE.Android;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Android.Bluetooth.BluetoothAdapter;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothService))]
namespace HealthMonitoring.Droid.Services
{
    public class BluetoothService : IBluetoothService
    {
        private readonly UUID ServiceUuid = UUID.FromString("491ef3e4-be34-4c5d-8d94-45a2eddbb9df");
        private readonly UUID CharacteristicUuid = UUID.FromString("8d68ee2e-5e8a-403e-b60f-c371209162b6");
        private List<BluetoothDevice> scannedDevices = new List<BluetoothDevice>();
        private HashSet<string> discoveredDevices = new HashSet<string>();

        private BluetoothGatt bluetoothGatt;
        private BluetoothAdapter bluetoothAdapter;
        private BluetoothManager bluetoothManager;
        private BluetoothGattCallback bluetoothGattCallback;

        private BluetoothLeScanner bluetoothScanner;
        private ScanCallback LeScanCallback;
        private bool Scanning;

        public event EventHandler<ProfileState> ConnectionStateChanged;
        public event EventHandler<string> CharacteristicValueChanged;

        public void BluetoothInitialize()
        {
            bluetoothManager = (BluetoothManager)Application.Context.GetSystemService(Context.BluetoothService);
            bluetoothAdapter = bluetoothManager.Adapter;
            if (Application.Context.PackageManager.HasSystemFeature(PackageManager.FeatureBluetoothLe))
            {
                System.Diagnostics.Debug.WriteLine("BLE is supported on this device");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("BLE is not supported on this device");
            }
            if (bluetoothAdapter == null)
            {
                System.Diagnostics.Debug.WriteLine("Bluetooth is not available on this device");
            }
            else
            {
                //Check if Bluetooth is enabled
                if (!bluetoothAdapter.IsEnabled)
                {
                    System.Diagnostics.Debug.WriteLine("Please enable your bluetooth");
                }
                else
                {
                    bluetoothScanner = bluetoothAdapter.BluetoothLeScanner;
                    LeScanCallback = new ScanCallback(scannedDevices, discoveredDevices);
                    bluetoothGattCallback = new BluetoothGattCallback(this);
                }
            }
        }

        public async Task<bool> ConnectToDevice(BluetoothDevice device)
        {
            if (bluetoothAdapter.GetRemoteDevice(device.Address) == null)
            {
                System.Diagnostics.Debug.WriteLine("Device not found or cannot be connected, handle accordingly.");
                return false;
            }
            try
            {
                bluetoothGatt = await Task.Run(() => { return device.ConnectGatt(Application.Context, false, bluetoothGattCallback); });
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error connecting to device: {ex.Message}");
                return false;
            }

        }

        public void DisconnectDevice()
        {
            if (bluetoothGatt != null)
            {
                bluetoothGatt.Disconnect();
            }
        }

        public async Task<List<BluetoothDevice>> ScanForDevicesAsync()
        {
            if (bluetoothAdapter == null || !bluetoothAdapter.IsEnabled)
            {
                return null;
            }
            discoveredDevices.Clear();
            scannedDevices.Clear();
            bluetoothScanner?.StartScan(LeScanCallback);

            await Task.Delay(5000);

            bluetoothScanner.StopScan(LeScanCallback);

            return scannedDevices;
        }

        public async Task WriteCharacteristicAsync(string Message)
        {
            var characteristic = bluetoothGatt.GetService(ServiceUuid).GetCharacteristic(CharacteristicUuid);
            if (characteristic != null)
            {
                byte[] value = Encoding.UTF8.GetBytes(Message);
                await Task.Run(() => bluetoothGatt.WriteCharacteristic(characteristic, value, (int)GattWriteType.Default));
                System.Diagnostics.Debug.WriteLine("Write Successfully");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Characteristic is null failed to write");
            }
        }

        public bool IsBluetoothEnabled()
        {
            return bluetoothAdapter.IsEnabled;
        }

        public void OnCharacteristicValueChanged(string value)
        {
            CharacteristicValueChanged?.Invoke(this, value);
        }

        public void OnConnectionStateChanged(ProfileState value)
        {
            ConnectionStateChanged?.Invoke(this, value);
        }
    }
}