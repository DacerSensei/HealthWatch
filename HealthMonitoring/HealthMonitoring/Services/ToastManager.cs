using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace HealthMonitoring.Services
{
    public static class ToastManager
    {
        public static async Task ShowToast(string message, Color color)
        {
            ToastOptions ToastOption = new ToastOptions
            {
                CornerRadius = 0,
                Duration = TimeSpan.FromSeconds(5),
                BackgroundColor = color,
                MessageOptions = new MessageOptions
                {
                    Padding = new Thickness(5),
                    Foreground = Color.White,
                    Message = message
                },
                IsRtl = true,
            };
            await Application.Current.MainPage.DisplayToastAsync(ToastOption);
        }
        
    }
}
