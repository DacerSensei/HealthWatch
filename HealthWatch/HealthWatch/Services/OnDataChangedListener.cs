using Android.Gms.Wearable;
using static Android.Gms.Wearable.DataClient;

namespace HealthWatch.Services
{
    public class OnDataChangedListener : Java.Lang.Object, IOnDataChangedListener
    {
        public void OnDataChanged(DataEventBuffer dataEvents)
        {
            foreach (var dataEvent in dataEvents)
            {
                if (dataEvent.Type == DataEvent.TypeChanged)
                {
                    var dataItem = dataEvent.DataItem;
                    var dataMap = DataMapItem.FromDataItem(dataItem).DataMap;
                    var message = dataMap.GetString("key");
                }
            }
        }
    }
}