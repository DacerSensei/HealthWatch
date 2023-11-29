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
    public class StepCounter : SensorEventListenerBase
    {
        protected override SensorType SensorType => SensorType.StepCounter;

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

        public StepCounter(Context context) : base(context)
        {
        }

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
                    SensorData++;
                    System.Diagnostics.Debug.WriteLine(SensorData);
                    BluetoothGattServerManager.GetGattServer().NotifyCharacteristicChanged(BluetoothGattServerManager.GetGattServerCallback().GetConnectedDevice(), BluetoothGattServerManager.GetGattServer().GetService(BluetoothGattServerManager.GattServiceUUID).GetCharacteristic(BluetoothGattServerManager.GattCharacteristicUUID), false, Encoding.UTF8.GetBytes("StepCounter:" + SensorData.ToString()));
                }
            }
        }

        public override void Reset()
        {
            SensorData = 0;
        }
    }
}