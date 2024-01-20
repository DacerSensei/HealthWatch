using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using HealthMonitoring.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthMonitoring.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        public LoginViewModel()
        {
            LoginCommand = new AsyncCommand(LoginExecute);
            RegisterCommand = new AsyncCommand(RegisterExecute);
            ForgotPasswordCommand = new AsyncCommand(ForgotPasswordExecute);
        }

        public ICommand LoadedCommand { get; set; }

        public ICommand ForgotPasswordCommand { get; }
        private async Task ForgotPasswordExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ForgotPassword());
        }

        public ICommand LoginCommand { get; }
        private async Task LoginExecute()
        {
            //if (string.IsNullOrWhiteSpace(Email))
            //{
            //    await ToastManager.ShowToast("Email cannot be empty", Color.FromHex("#FF605C"));
            //    return;
            //}
            //if (!IsValidEmail)
            //{
            //    await ToastManager.ShowToast("Email is not valid", Color.FromHex("#FF605C"));
            //    return;
            //}
            //if (string.IsNullOrWhiteSpace(Password))
            //{
            //    await ToastManager.ShowToast("Password cannot be empty", Color.FromHex("#FF605C"));
            //    return;
            //}
            //if (Password.Length < 6)
            //{
            //    await ToastManager.ShowToast("Password must be at least 6 characters", Color.FromHex("#FF605C"));
            //    return;
            //}
            try
            {
                //UserCredential userCredential = await Database.FirebaseAuthClient.SignInWithEmailAndPasswordAsync(Email, Password);
                UserCredential userCredential = await Database.FirebaseAuthClient.SignInWithEmailAndPasswordAsync("dacerz14@gmail.com", "123456");

                IReadOnlyCollection<FirebaseObject<Models.User>> users = await Database.FirebaseClient.Child("users").OnceAsync<Models.User>();
                var currentUser = users.Where(user => user.Object.Uid == userCredential.User.Uid).FirstOrDefault();
                Services.UserManager.User = currentUser.Object;
                Services.UserManager.User.Key = currentUser.Key;
                Services.UserManager.User.Email = userCredential.User.Info.Email;

                await Application.Current.MainPage.Navigation.PushAsync(new MainMenu());

                Email = string.Empty;
                Password = string.Empty;
            }
            catch (FirebaseAuthException ex)
            {
                switch (ex.Reason)
                {
                    case AuthErrorReason.UserDisabled:
                        await ToastManager.ShowToast("Account Disabled", Color.FromHex("#FF605C"));

                        break;
                    case AuthErrorReason.InvalidEmailAddress:
                        await ToastManager.ShowToast("Invalid Email Address", Color.FromHex("#FF605C"));

                        break;
                    case AuthErrorReason.UnknownEmailAddress:
                        await ToastManager.ShowToast("Email Address didn't exist", Color.FromHex("#FF605C"));
                        break;
                    case AuthErrorReason.Unknown:
                        await ToastManager.ShowToast("Your email or password is incorrect", Color.FromHex("#FF605C"));
                        break;
                    default:
                        Debug.WriteLine(ex.Message);
                        break;
                }
            }

        }

        public ICommand RegisterCommand { get; }
        private async Task RegisterExecute()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Register());
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

        private string password;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

    }
}
