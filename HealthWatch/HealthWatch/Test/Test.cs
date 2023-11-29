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

namespace HealthWatch.Test
{
    public class Test : BluetoothGattCallback
    {
        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, byte[] value, [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, value, status);
        }
    }
}