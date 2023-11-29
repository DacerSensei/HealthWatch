using Android.Bluetooth.LE;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HealthMonitoring.Droid.Services
{
    public class ScanCallback : Android.Bluetooth.LE.ScanCallback
    {
        private List<Android.Bluetooth.BluetoothDevice> scannedDevices;
        private HashSet<string> discoveredDevices;
        public ScanCallback(List<Android.Bluetooth.BluetoothDevice> scannedDevices, HashSet<string> discoveredDevices)
        {
            this.scannedDevices = scannedDevices;
            this.discoveredDevices = discoveredDevices;
        }

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);
            string deviceAddress = result.Device.Address;
            if (result.Device.Type == Android.Bluetooth.BluetoothDeviceType.Le && !discoveredDevices.Contains(deviceAddress))
            {
                discoveredDevices.Add(deviceAddress);
                scannedDevices.Add(result.Device);
            }
            
        }

        public override void OnBatchScanResults(IList<ScanResult> results)
        {
            base.OnBatchScanResults(results);
        }

        public override void OnScanFailed(ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);
        }
    }
}