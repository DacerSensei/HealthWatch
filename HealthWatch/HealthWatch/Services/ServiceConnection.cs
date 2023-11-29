using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HealthWatch.Services.Foreground;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HealthWatch.Services
{
    public class ServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public ForegroundHealthServices ForegroundHealthServices { get; private set; }
        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            MyBinder binder = (MyBinder)service;
            ForegroundHealthServices = binder.GetInstance();
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            ForegroundHealthServices = null;
        }
    }
}