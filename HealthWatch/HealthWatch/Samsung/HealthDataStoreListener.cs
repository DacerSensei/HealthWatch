using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Drm;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Samsung.Android.Sdk.Healthdata;
using HealthWatch.Services.Abstract;
using Java.Lang;
using Java.Util;
using Org.Apache.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Com.Samsung.Android.Sdk.Healthdata.HealthPermissionManager;
using Exception = System.Exception;

namespace HealthWatch.Samsung
{
    public class HealthDataStoreListener : Java.Lang.Object, HealthDataStore.IConnectionListener
    {
        internal HealthDataStore Store { get; set; }
        private HealthPermissionManager healthPermissionManager;
        private HashSet<PermissionKey> permissions = new HashSet<PermissionKey>
        {
            new PermissionKey(HealthConstants.UserProfileDataType, PermissionType.Read)
        };
        public void OnConnected()
        {
            healthPermissionManager = new HealthPermissionManager(Store);
            try
            {
                healthPermissionManager.RequestPermissions(permissions).SetResultListener(new PermissionListener());
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("requestPermissions() fails: " + e.Message);
            }
        }

        public void OnConnectionFailed(HealthConnectionErrorResult error)
        {
            ShowConnectionFailureReason(error);
        }

        private void ShowConnectionFailureReason(HealthConnectionErrorResult error)
        {
            string result;
            if (error.HasResolution)
            {
                switch (error.ErrorCode)
                {
                    case HealthConnectionErrorResult.PlatformNotInstalled:
                        result = "Please install Samsung Health";
                        break;
                    case HealthConnectionErrorResult.OldVersionPlatform:
                        result = "Please upgrade Samsung Health";
                        break;
                    case HealthConnectionErrorResult.PlatformDisabled:
                        result = "Please enable Samsung Health";
                        break;
                    case HealthConnectionErrorResult.UserAgreementNeeded:
                        result = "Please agree to Samsung Health policy";
                        break;
                    case HealthConnectionErrorResult.OldVersionSdk:
                        result = "The SDK's Data library version is outdated to cooperate with the Samsung Health's health data framework";
                        break;
                    case HealthConnectionErrorResult.ConnectionFailure:
                        result = "The connection is not established in the health data framework";
                        break;
                    case HealthConnectionErrorResult.Unknown:
                        result = "Unknown error";
                        break;
                    default:
                        result = "Please make Samsung Health available";
                        break;
                }
                System.Diagnostics.Debug.WriteLine(result);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Device doesn't support Samsung Health");
            }
        }
        public void OnDisconnected()
        {
            Store.DisconnectService();
        }
    }
}