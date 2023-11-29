using Firebase.Auth;
using HealthMonitoring.Config;
using HealthMonitoring.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class ForgotPasswordViewModel : ObservableObject
    {
        public ForgotPasswordViewModel()
        {
            ForgotPasswordCommand = new AsyncCommand(ForgotPasswordExecute);
            BackCommand = new AsyncCommand(BackExecute);
        }

        public ICommand BackCommand { get; }
        private async Task BackExecute()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        public ICommand ForgotPasswordCommand { get; }
        private async Task ForgotPasswordExecute()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await ToastManager.ShowToast("Email cannot be empty", Color.FromHex("#FF605C"));
                return;
            }
            if (!IsValidEmail)
            {
                await ToastManager.ShowToast("Email is not valid", Color.FromHex("#FF605C"));
                return;
            }
            try
            {
                await Database.FirebaseAuthClient.ResetEmailPasswordAsync(Email);
                await Application.Current.MainPage.Navigation.PopModalAsync();
                await ToastManager.ShowToast("We've send an email to your account", Color.FromHex("#1eb980"));
            }
            catch (FirebaseAuthException ex)
            {
                switch (ex.Reason)
                {
                    case AuthErrorReason.UnknownEmailAddress:
                        await ToastManager.ShowToast("Email didn't exist", Color.FromHex("#FF605C"));
                        break;
                    default:
                        Debug.WriteLine(ex.Message);
                        break;
                }
            }
        }

        private bool isValidEmail;

        public bool IsValidEmail
        {
            get => isValidEmail;
            set => SetProperty(ref isValidEmail, value);
        }

        private string email;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }
    }
}
