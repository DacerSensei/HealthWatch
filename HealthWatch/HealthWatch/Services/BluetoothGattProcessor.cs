using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HealthWatch.Services.Foreground;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthWatch.Services
{
    public class BluetoothGattProcessor
    {
        private BluetoothAdapter bluetoothAdapter;
        private BluetoothGattServer gattServer;
        private MyAdvertiseCallback advertisingCallback;
        private bool isAdvertising = false;
        public BluetoothGattProcessor()
        {
            bluetoothAdapter = BluetoothGattServerManager.GetBluetoothManager().Adapter;
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
                // Bluetooth is not supported on this device
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
                    // Bluetooth is enabled, you can check for a connection
                    CheckBluetoothConnection();

                }
            }
            if (bluetoothAdapter.BluetoothLeAdvertiser != null && !isAdvertising)
            {
                // Start advertising only if it's not already active
                StartAdvertisingGattServer();
                isAdvertising = true;
            }
        }

        private void CheckBluetoothConnection()
        {
            advertisingCallback = new MyAdvertiseCallback();
            gattServer = BluetoothGattServerManager.GetGattServer();
            BluetoothGattServerManager.InitializeGattServer();
        }

        private void StartAdvertisingGattServer()
        {
            AdvertiseSettings settings = new AdvertiseSettings.Builder().SetAdvertiseMode(AdvertiseMode.LowLatency).SetConnectable(true).Build();
            AdvertiseData data = new AdvertiseData.Builder().SetIncludeDeviceName(true).SetIncludeTxPowerLevel(true).Build();
            AdvertiseData scanResponseData = new AdvertiseData.Builder().AddServiceUuid(new ParcelUuid(BluetoothGattServerManager.GattServiceUUID)).SetIncludeTxPowerLevel(true).Build();
            var advertiser = bluetoothAdapter.BluetoothLeAdvertiser;
            if (advertiser != null)
            {
                advertiser.StartAdvertising(settings, data, scanResponseData, advertisingCallback);
            }
        }

        private void SendDataToBluetooth()
        {
            System.Diagnostics.Debug.WriteLine("Send Data");
            string dataToSend = "Hello, client!"; // Prepare the data to send
            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSend);

            gattServer.NotifyCharacteristicChanged(BluetoothGattServerManager.GetGattServerCallback().GetConnectedDevice(), BluetoothGattServerManager.GetGattCharacteristic(), false, dataBytes);
        }
    }
}