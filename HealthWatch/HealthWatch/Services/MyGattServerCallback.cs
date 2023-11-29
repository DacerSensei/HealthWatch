using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Core.Content;
using HealthWatch.Services.Abstract;
using HealthWatch.Services.Enums;
using HealthWatch.Services.Foreground;
using Java.Util;
using System;
using System.Text;
using System.Threading.Tasks;
using static Android.Bluetooth.BluetoothClass;

namespace HealthWatch.Services
{
    public class MyGattServerCallback : BluetoothGattServerCallback
    {
        // Implement methods to handle GATT server events, e.g., onConnectionStateChange, onCharacteristicReadRequest, etc.
        private static readonly string TAG = typeof(MyGattServerCallback).Name;

        public event Action<BluetoothDevice, ProfileState> ConnectionStateChanged;

        private BluetoothDevice connectedDevice;
        private bool deviceConnected;
        private bool oldDeviceConnected;
        private OffBody OffBody = new OffBody(Application.Context);
        private bool isOffBody = false;

        public override void OnConnectionStateChange(BluetoothDevice device, [GeneratedEnum] ProfileState status, [GeneratedEnum] ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);
            ConnectionStateChanged?.Invoke(device, newState);
            if (newState == ProfileState.Connected)
            {
                connectedDevice = device;
                deviceConnected = true;
                OffBody.StartSensorMonitoring();
                System.Diagnostics.Debug.WriteLine($"Device {device.Address} connected");
                //DeviceConnected(Application.Context, device.Name);
                
            }
            else if (newState == ProfileState.Disconnected)
            {
                connectedDevice = null;
                deviceConnected = false;
                OffBody.StopSensorMonitoring();
                BluetoothGattServerManager.GetServiceConnection().ForegroundHealthServices.ControlSensor(SensorCommand.STOP_HEART_RATE);
                BluetoothGattServerManager.GetServiceConnection().ForegroundHealthServices.ControlSensor(SensorCommand.STOP_STEP_COUNTER);
                System.Diagnostics.Debug.WriteLine($"Device {device.Address} disconnected");
                //DeviceDisconnected(Application.Context);
                
            }
        }

        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            BluetoothGattServerManager.GetGattServer().SendResponse(device, requestId, GattStatus.Success, offset, value);
            System.Diagnostics.Debug.WriteLine("From Write:" + Encoding.UTF8.GetString(value));
            string command = Encoding.UTF8.GetString(value);
            SensorCommand parsedCommand;
            if (Enum.TryParse(command, out parsedCommand))
            {
                BluetoothGattServerManager.GetServiceConnection().ForegroundHealthServices.ControlSensor(parsedCommand);
            }
        }

        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattCharacteristic characteristic)
        {
            BluetoothGattServerManager.GetGattServer().SendResponse(device, requestId, GattStatus.Success, offset, characteristic.GetValue());
            System.Diagnostics.Debug.WriteLine("From Read: " + Encoding.UTF8.GetString(characteristic.GetValue()));
        }

        public override void OnNotificationSent(BluetoothDevice device, [GeneratedEnum] GattStatus status)
        {
            base.OnNotificationSent(device, status);
            System.Diagnostics.Debug.WriteLine("Notification has been sent!");
        }

        public BluetoothDevice GetConnectedDevice()
        {
            return connectedDevice;
        }

    }
}