using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HealthMonitoring.Droid.Services;
using HealthMonitoring.Services;
using Java.Lang;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(HealthMonitoring.Droid.Services.BluetoothGattCallback))]
namespace HealthMonitoring.Droid.Services
{
    public class BluetoothGattCallback : Android.Bluetooth.BluetoothGattCallback
    {
        private readonly UUID ServiceUuid = UUID.FromString("491ef3e4-be34-4c5d-8d94-45a2eddbb9df");
        private readonly UUID CharacteristicUuid = UUID.FromString("8d68ee2e-5e8a-403e-b60f-c371209162b6");

        private TaskCompletionSource<bool> characteristicWriteCompletionSource;

        public BluetoothGattCallback(BluetoothService bluetoothService)
        {
            this.bluetoothService = bluetoothService;
        }

        BluetoothService bluetoothService;
        BluetoothGattService service;
        BluetoothGattCharacteristic characteristic;

        public event EventHandler<string> CharacteristicValueChanged;

        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);
            bluetoothService.OnConnectionStateChanged(newState);
            if (newState == ProfileState.Connected)
            {
                gatt.DiscoverServices();
            }
            else if (newState == ProfileState.Disconnected)
            {

            }
        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicWrite(gatt, characteristic, status);
            if (status == GattStatus.Success)
            {
                System.Diagnostics.Debug.WriteLine("Write Successfully");
            }
            characteristicWriteCompletionSource?.TrySetResult(true);
        }

        public override async void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);

            if (status == GattStatus.Success)
            {
                // Services discovered, now find the desired service
                service = gatt.GetService(ServiceUuid);
                if (service != null)
                {
                    characteristic = service.GetCharacteristic(CharacteristicUuid);
                    if (characteristic != null)
                    {
                        gatt.SetCharacteristicNotification(characteristic, true);
                        BluetoothGattDescriptor descriptor = characteristic.GetDescriptor(UUID.FromString("00002902-0000-1000-8000-00805F9B34FB"));
                        try
                        {
                            await WriteCharacteristicAsync(gatt, characteristic, BluetoothGattDescriptor.EnableNotificationValue.ToArray(), GattWriteType.Default);
                            await WriteCharacteristicAsync(gatt, characteristic, Encoding.UTF8.GetBytes("START_HEART_RATE"), GattWriteType.Default);
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine($"Error during BLE operations: {ex.Message}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("The desired characteristic with the specified UUID was not found");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("The desired service with the specified UUID was not found");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Service discovery failed, handle accordingly.");
            }
        }

        public async Task WriteCharacteristicAsync(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, byte[] value, GattWriteType writeType)
        {
            characteristicWriteCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                gatt.WriteCharacteristic(characteristic, value, (int)writeType);
                await characteristicWriteCompletionSource.Task;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                characteristicWriteCompletionSource = null;
            }
        }

        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, byte[] value)
        {
            base.OnCharacteristicChanged(gatt, characteristic, value);
            bluetoothService.OnCharacteristicValueChanged(Encoding.UTF8.GetString(value));
        }
    }
}