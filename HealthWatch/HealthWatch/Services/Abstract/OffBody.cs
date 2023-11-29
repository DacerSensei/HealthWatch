using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HealthWatch.Services.Abstract.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthWatch.Services.Abstract
{
    public class OffBody : SensorEventListenerBase
    {
        public OffBody(Context context) : base(context)
        {
        }
        private int sensorData;
        public int SensorData
        {
            get { return sensorData; }
            set
            {
                if (sensorData != value)
                {
                    sensorData = value;
                }

            }
        }

        protected override SensorType SensorType => SensorType.LowLatencyOffbodyDetect;

        public override object GetData()
        {
            return SensorData;
        }

        public override void OnSensorChanged(SensorEvent e)
        {
            if (BluetoothGattServerManager.GetGattServerCallback().GetConnectedDevice() != null)
            {
                if (e.Sensor.Type == SensorType)
                {
                    SensorData = (int)Math.Round(e.Values[0]);
                    if (SensorData != 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"{SensorType} Sensor Data: Worn");
                        BluetoothGattServerManager.GetGattServer().NotifyCharacteristicChanged(BluetoothGattServerManager.GetGattServerCallback().GetConnectedDevice(), BluetoothGattServerManager.GetGattServer().GetService(BluetoothGattServerManager.GattServiceUUID).GetCharacteristic(BluetoothGattServerManager.GattCharacteristicUUID), false, Encoding.UTF8.GetBytes("OffBody:1"));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"{SensorType} Sensor Data: Not Worn");
                        BluetoothGattServerManager.GetGattServer().NotifyCharacteristicChanged(BluetoothGattServerManager.GetGattServerCallback().GetConnectedDevice(), BluetoothGattServerManager.GetGattServer().GetService(BluetoothGattServerManager.GattServiceUUID).GetCharacteristic(BluetoothGattServerManager.GattCharacteristicUUID), false, Encoding.UTF8.GetBytes("OffBody:0"));
                    }
                }
            }
        }
    }
}