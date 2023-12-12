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
    public class ChangePasswordViewModel : ObservableObject
    {
        public ChangePasswordViewModel()
        {
            GoBackCommand = new AsyncCommand(GoBackExecute);
            SaveCommand = new AsyncCommand(SaveExecute);
        }

        public ICommand GoBackCommand { get; }
        private async Task GoBackExecute()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        public ICommand SaveCommand { get; }
        private async Task SaveExecute()
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                await ToastManager.ShowToast("New password cannot be empty", Color.FromHex("#FF605C"));
                return;
            }
            if (NewPassword.Length < 6)
            {
                await ToastManager.ShowToast("New password must be at least 6 characters", Color.FromHex("#FF605C"));
                return;
            }
            if (NewPassword != ConfirmPassword)
            {
                await ToastManager.ShowToast("Your new password and confirm password didn't match", Color.FromHex("#FF605C"));
                return;
            }
            if (!await Application.Current.MainPage.DisplayAlert("Notice", "Are you sure you want to change your password?", "Yes", "No", FlowDirection.LeftToRight))
            {
                return;
            }
            try
            {
                await Database.FirebaseAuthClient.User.ChangePasswordAsync(NewPassword);
                await Application.Current.MainPage.Navigation.PopModalAsync();
                await ToastManager.ShowToast("Your password has been changed", Color.FromHex("#1eb980"));
            }
            catch (FirebaseAuthException ex)
            {
                switch (ex.Reason)
                {
                    case AuthErrorReason.InvalidEmailAddress:
                        await ToastManager.ShowToast("Invalid Email Address", Color.FromHex("#FF605C"));
                        break;
                    case AuthErrorReason.UnknownEmailAddress:
                        await ToastManager.ShowToast("Email didn't exist", Color.FromHex("#FF605C"));
                        break;
                    case AuthErrorReason.EmailExists:
                        await ToastManager.ShowToast("Email already used", Color.FromHex("#FF605C"));
                        break;
                    default:
                        Debug.WriteLine(ex.Message);
                        break;
                }
            }
        }

        private string newPassword = string.Empty;
        public string NewPassword
        {
            get => newPassword;
            set => SetProperty(ref newPassword, value);
        }

        private string confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }
    }
}
