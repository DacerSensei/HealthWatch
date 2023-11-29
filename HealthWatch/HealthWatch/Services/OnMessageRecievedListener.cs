using Android.Gms.Wearable;
using System.Text;
using static Android.Gms.Wearable.MessageClient;

namespace HealthWatch.Services
{
    public class OnMessageRecievedListener : Java.Lang.Object, IOnMessageReceivedListener
    {
        public void OnMessageReceived(IMessageEvent messageEvent)
        {
            byte[] data = messageEvent.GetData();
            if (data != null)
            {
                string message = Encoding.UTF8.GetString(data);
                System.Diagnostics.Debug.WriteLine("Received message: " + message);
            }
        }
    }
}