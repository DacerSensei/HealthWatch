using Firebase.Auth;
using Firebase.Auth.Providers;
using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Views;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;
using Application = Xamarin.Forms.Application;

[assembly: ExportFont("Poppins-Regular.ttf", Alias = "PoppinsRegular"), ExportFont("Poppins-Bold.ttf", Alias = "PoppinsBold")]
namespace HealthMonitoring
{
    public partial class App : Application
    {
        public App()
        {
            App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            InitializeComponent();
            DependencyService.Get<IEnvironment>().SetStatusBarColor(Color.FromHex("#028090"), false);
            MainPage = new NavigationPage(new Login());
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
