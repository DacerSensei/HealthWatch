using HealthMonitoring.Models;
using Xamarin.Forms.Xaml;

namespace HealthMonitoring.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public static class UserManager
    {
        public static User User { get; set; }
    }
}