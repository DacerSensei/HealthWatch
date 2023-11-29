using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthWatch.Services.Abstract.Base
{
    public abstract class SensorEventListenerBase : Java.Lang.Object, ISensorEventListener
    {
        protected SensorManager SensorManager;
        protected Sensor Sensor;
        protected bool IsSensorRegistered = false;

        protected abstract SensorType SensorType { get; }

        public SensorEventListenerBase(Context context)
        {
            SensorManager = (SensorManager)context.GetSystemService(Context.SensorService);
            Sensor = SensorManager.GetDefaultSensor(SensorType);
        }

        public abstract object GetData();
        public virtual void Reset()
        {
            // Implement the default value of data
        }

        public abstract void OnSensorChanged(SensorEvent e);

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            // Handle accuracy changes if needed.
        }

        public void StartSensorMonitoring()
        {
            if (Sensor != null && !IsSensorRegistered)
            {
                SensorManager.RegisterListener(this, Sensor, SensorDelay.Normal);
                IsSensorRegistered = true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"{SensorType} Sensor is not available in this device");
            }
        }

        public void StopSensorMonitoring()
        {
            if (Sensor != null && IsSensorRegistered)
            {
                SensorManager.UnregisterListener(this);
                IsSensorRegistered = false;
                Reset();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"{SensorType} Sensor is not available in this device or is not registered.");
            }
        }
    }
}