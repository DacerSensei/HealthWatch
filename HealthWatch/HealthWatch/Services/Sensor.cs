using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using HealthWatch.Services.Abstract;
using Java.Util;
using System.Runtime.Remoting.Contexts;
using BatteryStatus = Android.OS.BatteryStatus;

namespace HealthWatch.Services
{
    public class SensorLibrary
    {
        private BatteryManager batteryManager;
        private Android.Content.Context context;

        public SensorLibrary(Android.Content.Context context)
        {
            this.context = context;
        }

        public void GetBattery()
        {
            batteryManager = (BatteryManager)context.GetSystemService(Android.Content.Context.BatteryService);
            // Get battery level
            int batteryLevel = batteryManager.GetIntProperty((int)BatteryProperty.Capacity);

            // Get battery status (e.g., charging, discharging)
            int batteryStatus = batteryManager.GetIntProperty((int)BatteryProperty.Status);

            System.Diagnostics.Debug.WriteLine("Battery Level: " + batteryLevel);
            System.Diagnostics.Debug.WriteLine("Battery Status: " + GetBatteryStatusString(batteryStatus));
        }

        private string GetBatteryStatusString(int status)
        {
            switch (status)
            {
                case (int)BatteryStatus.Charging:
                    return "Charging";
                case (int)BatteryStatus.Discharging:
                    return "Discharging";
                case (int)BatteryStatus.NotCharging:
                    return "Not Charging";
                case (int)BatteryStatus.Full:
                    return "Full";
                default:
                    return "Unknown";
            }
        }
    }
}