using Android.Content;
using HealthMonitoring.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(BorderlessEntryAndroid))]
namespace HealthMonitoring.Droid
{
    public class BorderlessEntryAndroid : EntryRenderer
    {
        public BorderlessEntryAndroid(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                if (Control != null)
                {
                    this.Control.SetBackground(null);
                }
            }
        }
    }
}