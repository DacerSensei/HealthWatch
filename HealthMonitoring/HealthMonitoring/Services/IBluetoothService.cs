using Android.Bluetooth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HealthMonitoring.Services
{
    public interface IBluetoothService
    {
        void BluetoothInitialize();
        Task<List<BluetoothDevice>> ScanForDevicesAsync();
        Task<bool> ConnectToDevice(BluetoothDevice device);
        void DisconnectDevice();
        event EventHandler<ProfileState> ConnectionStateChanged;
        event EventHandler<string> CharacteristicValueChanged;
        Task WriteCharacteristicAsync(string Message);
        bool IsBluetoothEnabled();
    }
}
