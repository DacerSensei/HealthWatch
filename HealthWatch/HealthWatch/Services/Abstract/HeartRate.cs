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
    public class HeartRate : SensorEventListenerBase
    {
        protected override SensorType SensorType => SensorType.HeartRate;
        private float sensorData = 0;
        private float SensorData
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

        public HeartRate(Context context) : base(context)
        {
        }

        public override void OnSensorChanged(SensorEvent e)
        {
            if (BluetoothGattServerManager.GetGattServerCallback().GetConnectedDevice() != null)
            {
                if (e.Sensor.Type == SensorType && e.Values[0] != SensorData)
                {
                    SensorData = e.Values[0];
                    System.Diagnostics.Debug.WriteLine($"{SensorType} Sensor Data: {SensorData}");
                    BluetoothGattServerManager.GetGattServer().NotifyCharacteristicChanged(BluetoothGattServerManager.GetGattServerCallback().GetConnectedDevice(), BluetoothGattServerManager.GetGattServer().GetService(BluetoothGattServerManager.GattServiceUUID).GetCharacteristic(BluetoothGattServerManager.GattCharacteristicUUID), false, Encoding.UTF8.GetBytes("HeartRate:" + SensorData.ToString()));
                }
            }
        }

        public override object GetData()
        {
            return SensorData;
        }
    }
}