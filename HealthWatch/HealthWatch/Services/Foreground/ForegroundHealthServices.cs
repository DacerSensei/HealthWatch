using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using HealthWatch.Services.Abstract;
using HealthWatch.Services.Enums;
using Java.Lang;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using static Com.Samsung.Android.Sdk.Internal.Healthdata.Query.ComparisonFilter;

namespace HealthWatch.Services.Foreground
{
    [Service(Name = "com.healthwatch.ForegroundHealthServices")]
    public class ForegroundHealthServices : Service
    {
        private const string ChannelID = "CHANNEL_HEALTH";
        public HeartRate HeartRate = new HeartRate(Application.Context);
        public StepCounter StepCounter = new StepCounter(Application.Context);
        private bool isHeartRateMonitoring = false;
        private bool isStepCounting = false;

        private NotificationManager NotificationManager;
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return new MyBinder(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            NotificationManager.Cancel(1);
            if (isHeartRateMonitoring)
            {
                HeartRate.StopSensorMonitoring();
            }
            if (isStepCounting)
            {
                StepCounter.StopSensorMonitoring();
            }
            StopForeground(StopForegroundFlags.Remove);
            StopSelf();
        }

        public void ControlSensor(SensorCommand command)
        {
            switch (command)
            {
                case SensorCommand.START_HEART_RATE:
                    if (!isHeartRateMonitoring && HeartRate != null)
                    {
                        HeartRate.StartSensorMonitoring();
                        isHeartRateMonitoring = true;
                    }
                    break;
                case SensorCommand.START_STEP_COUNTER:
                    if (!isStepCounting && StepCounter != null)
                    {
                        StepCounter.StartSensorMonitoring();
                        isStepCounting = true;
                    }
                    break;
                case SensorCommand.STOP_HEART_RATE:
                    if (isHeartRateMonitoring && HeartRate != null)
                    {
                        HeartRate.StopSensorMonitoring();
                        isHeartRateMonitoring = false;
                    }
                    break;
                case SensorCommand.STOP_STEP_COUNTER:
                    if (isStepCounting && StepCounter != null)
                    {
                        StepCounter.StopSensorMonitoring();
                        isStepCounting = false;
                    }
                    break;
                default: break;
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channel = new NotificationChannel(ChannelID, "FitHealth", NotificationImportance.High);
                    NotificationManager = GetSystemService(NotificationService) as NotificationManager;
                    NotificationManager.CreateNotificationChannel(channel);
                }

                var notification = new NotificationCompat.Builder(this, ChannelID)
                    .SetSmallIcon(Resource.Mipmap.ic_launcher)
                    .SetContentTitle("FitHealth")
                    .SetContentText("Monitoring your vitals")
                    .SetOngoing(true)
                    .SetChannelId(ChannelID)
                    .Build();

                ServiceCompat.StartForeground(this, 1, notification, (int)ForegroundService.TypeNone);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
            }
        }
    }
}