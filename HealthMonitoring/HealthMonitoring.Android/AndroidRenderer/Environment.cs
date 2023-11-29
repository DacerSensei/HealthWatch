using Android.OS;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(HealthMonitoring.Droid.AndroidRender.Environment))]
namespace HealthMonitoring.Droid.AndroidRender
{
    public class Environment : IEnvironment
    {
        [System.Obsolete]
        public void SetStatusBarColor(System.Drawing.Color color, bool darkStatusBarTint)
        {
            if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Lollipop)
                return;
            // Current Activity or null if not initialized or not started.
            var activity = Platform.CurrentActivity;
            var window = activity.Window;
            window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
            window.SetStatusBarColor(color.ToPlatformColor());

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                var flag = (Android.Views.StatusBarVisibility)Android.Views.SystemUiFlags.LightStatusBar;
                window.DecorView.SystemUiVisibility = darkStatusBarTint ? flag : 0;
            }
        }
    }
}