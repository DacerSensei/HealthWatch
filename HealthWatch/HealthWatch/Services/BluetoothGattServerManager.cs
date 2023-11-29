using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthWatch.Services
{
    public static class BluetoothGattServerManager
    {
        private static BluetoothGattServer gattServer;
        private static BluetoothManager bluetoothManager;
        private static MyGattServerCallback gattServerCallback;
        private static ServiceConnection serviceConnection;
        private static BluetoothGattService gattService;
        private static BluetoothGattCharacteristic gattCharacteristic;
        private static BluetoothGattDescriptor gattDescriptor;
        public static readonly UUID GattServiceUUID = UUID.FromString("491ef3e4-be34-4c5d-8d94-45a2eddbb9df");
        public static readonly UUID GattCharacteristicUUID = UUID.FromString("8d68ee2e-5e8a-403e-b60f-c371209162b6");
        public static readonly UUID GattDescriptorUUID = UUID.FromString("00002902-0000-1000-8000-00805f9b34fb");

        public static BluetoothGattServer GetGattServer()
        {
            if (gattServer == null)
            {
                gattServer = GetBluetoothManager().OpenGattServer(Application.Context, GetGattServerCallback());
            }
            return gattServer;
        }

        public static void InitializeGattServer()
        {
            gattService = new BluetoothGattService(GattServiceUUID, GattServiceType.Primary);
            gattCharacteristic = new BluetoothGattCharacteristic(GattCharacteristicUUID,
                    GattProperty.Read | GattProperty.Write | GattProperty.Notify | GattProperty.Indicate,
                    GattPermission.Read | GattPermission.Write
                );
            gattCharacteristic.WriteType = GattWriteType.Default;
            gattDescriptor = new BluetoothGattDescriptor(GattDescriptorUUID,
                GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            gattDescriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray());
            
            gattCharacteristic.AddDescriptor(gattDescriptor);
            gattService.AddCharacteristic(gattCharacteristic);
            gattServer.AddService(gattService);
        }

        public static BluetoothManager GetBluetoothManager()
        {
            if (bluetoothManager == null)
            {
                bluetoothManager = (BluetoothManager)Application.Context.GetSystemService(Context.BluetoothService);
            }
            return bluetoothManager;
        }

        public static MyGattServerCallback GetGattServerCallback()
        {
            if (gattServerCallback == null)
            {
                gattServerCallback = new MyGattServerCallback();
            }
            return gattServerCallback;
        }

        public static BluetoothGattService GetGattService()
        {
            if (gattService != null)
            {
                gattService = new BluetoothGattService(GattServiceUUID, GattServiceType.Primary);
            }
            return gattService;
        }

        public static BluetoothGattCharacteristic GetGattCharacteristic()
        {
            if (gattCharacteristic != null)
            {
                gattCharacteristic = new BluetoothGattCharacteristic(GattCharacteristicUUID,
                    GattProperty.Read | GattProperty.Write | GattProperty.Notify | GattProperty.Indicate,
                    GattPermission.Read | GattPermission.Write
                );
                gattCharacteristic.WriteType = GattWriteType.Default;
            }
            return gattCharacteristic;
        }

        public static BluetoothGattDescriptor GetGattDescriptor()
        {
            if (gattDescriptor != null)
            {
                gattDescriptor = new BluetoothGattDescriptor(GattDescriptorUUID,
                GattDescriptorPermission.Read | GattDescriptorPermission.Write);
                gattDescriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray());
            }
            return gattDescriptor;
        }

        public static ServiceConnection GetServiceConnection()
        {
            if (serviceConnection == null)
            {
                serviceConnection = new ServiceConnection();
            }
            return serviceConnection;
        }
    }
}