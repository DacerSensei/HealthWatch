using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthWatch.Services.Enums
{
    public enum SensorCommand
    {
        START_HEART_RATE,
        STOP_HEART_RATE,
        START_STEP_COUNTER,
        STOP_STEP_COUNTER,
        START_OFFBODY,
        STOP_OFFBODY,
    }
}