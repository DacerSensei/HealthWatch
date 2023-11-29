using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;
using Android.Bluetooth.LE;
using Android.Bluetooth;
using Android.Content;
using HealthWatch.Services;
using HealthWatch.Services.Foreground;
using AndroidX.Core.Content;
using System.Collections.Generic;
using Java.Lang;

namespace HealthWatch
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        private BluetoothGattProcessor BlueoothGattProcessor;
        private bool IsBind;
        private Handler handler = new Handler(Looper.MainLooper); // For cancelling delay disconnected to connected
        private Runnable delayRunnable; // this is for runnable if i can cancel or not the changing

        TextView BluetoothStatus;
        TextView DeviceConnected;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);

            BluetoothStatus = FindViewById<TextView>(Resource.Id.Status);
            DeviceConnected = FindViewById<TextView>(Resource.Id.DeviceName);
            Button Button = FindViewById<Button>(Resource.Id.myButton);
            Button Button2 = FindViewById<Button>(Resource.Id.secondButton);
            Button.Click += GetStarted;
            Button2.Click += SecondButton;

            StartForegroundHealth();
            BlueoothGattProcessor = new BluetoothGattProcessor();
            BluetoothGattServerManager.GetGattServerCallback().ConnectionStateChanged += MainActivity_ConnectionStateChanged;
            SetAmbientEnabled();
            Window.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
        }

        private void MainActivity_ConnectionStateChanged(BluetoothDevice device, ProfileState newState)
        {
            RunOnUiThread(() =>
            {
                if (newState == ProfileState.Connected)
                {
                    handler.RemoveCallbacks(delayRunnable);
                    BluetoothStatus.Text = "Connected";
                    BluetoothStatus.SetTextColor(Android.Graphics.Color.Rgb(56, 176, 0));
                    DeviceConnected.Text = device.Name;
                }
                else if (newState == ProfileState.Disconnected)
                {
                    DeviceConnected.Text = string.Empty;
                    BluetoothStatus.Text = "Disconnected";
                    BluetoothStatus.SetTextColor(Android.Graphics.Color.Rgb(193, 18, 31));
                    delayRunnable = new Runnable(() =>
                    {
                        // After waiting, change the text and color
                        RunOnUiThread(() =>
                        {
                            // After waiting, change the text and color
                            BluetoothStatus.Text = "Connecting";
                            BluetoothStatus.SetTextColor(Android.Graphics.Color.Rgb(251, 133, 0));
                        });
                    });
                    handler.PostDelayed(delayRunnable, 2000);
                }
            });
        }

        private void GetStarted(object sender, EventArgs e)
        {
            SensorLibrary sensorLibrary = new SensorLibrary(this);
            sensorLibrary.GetBattery();
            //ServiceConnection.ForegroundHealthServices.ControlSensor(Services.Enums.SensorCommand.START_HEART_RATE);
        }

        private void SecondButton(object sender, EventArgs e)
        {
            //ServiceConnection.ForegroundHealthServices.ControlSensor(Services.Enums.SensorCommand.STOP_HEART_RATE);
            System.Diagnostics.Debug.WriteLine("Foreground Status: " + IsServiceRunning("com.healthwatch.ForegroundHealthServices"));
        }

        private void StartForegroundHealth()
        {

            Intent intent = new Intent(Application.Context, typeof(ForegroundHealthServices));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                ContextCompat.StartForegroundService(Application.Context, intent);
            }
            else
            {
                Application.Context.StartService(intent);
            }
            DoBindService(intent);
        }

        private void DoBindService(Intent intent)
        {
            if (!IsBind)
            {
                Application.Context.BindService(intent, BluetoothGattServerManager.GetServiceConnection(), Bind.AutoCreate);
                IsBind = true;
            }
        }

        private void DoUnbindService()
        {
            if (IsBind)
            {
                // Release information about the service's state.
                UnbindService(BluetoothGattServerManager.GetServiceConnection());
                IsBind = false;
            }
        }

        private bool IsServiceRunning(string serviceName)
        {
            bool serviceRunning = false;
            ActivityManager am = (ActivityManager)GetSystemService(Context.ActivityService);
            IList<ActivityManager.RunningServiceInfo> runningServices = am.GetRunningServices(50);

            foreach (var runningServiceInfo in runningServices)
            {
                if (runningServiceInfo.Service.ClassName.Equals(serviceName))
                {
                    serviceRunning = true;

                    if (runningServiceInfo.Foreground)
                    {
                        System.Diagnostics.Debug.WriteLine("Foreground Health Service is running");
                    }
                }
            }

            return serviceRunning;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}


