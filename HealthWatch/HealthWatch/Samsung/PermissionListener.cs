using Android.App;
using Android.Content;
using Android.Drm;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Samsung.Android.Sdk.Healthdata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Com.Samsung.Android.Sdk.Healthdata.HealthPermissionManager;

namespace HealthWatch.Samsung
{
    public class PermissionListener : Java.Lang.Object, IHealthResultHolderResultListener
    {
        public void OnResult(Java.Lang.Object permissionResult)
        {
            PermissionResult result = (PermissionResult)permissionResult;
            IDictionary<PermissionKey, Java.Lang.Boolean> resultMap = result.ResultMap;
            if (resultMap.Values.Contains(Java.Lang.Boolean.False))
            {
                System.Diagnostics.Debug.WriteLine("All required permissions are not granted.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("All required permissions are granted.");
            }
        }
    }
}