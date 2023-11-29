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
using System.Text;

namespace HealthWatch.Services
{
    public class MyBinder : Binder
    {
        private ForegroundHealthServices foregroundHealthServices;
        
        public MyBinder(ForegroundHealthServices foregroundHealthServices)
        {
            this.foregroundHealthServices = foregroundHealthServices;
        }

        public ForegroundHealthServices GetInstance()
        {
            return foregroundHealthServices;
        }
    }
}