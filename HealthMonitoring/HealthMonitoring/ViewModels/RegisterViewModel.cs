using Firebase.Auth;
using Firebase.Database.Query;
using HealthMonitoring.Config;
using HealthMonitoring.Models;
using HealthMonitoring.Services;
using HealthMonitoring.Views;
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
    public class RegisterViewModel : ObservableObject
    {
        Page CurrentPage;
        public RegisterViewModel()
        {
            CurrentPage = Application.Current.MainPage;
            LoginCommand = new AsyncCommand(LoginExecute);
            RegisterCommand = new AsyncCommand(RegisterExecute);
        }

        public ICommand LoginCommand { get; }
        private async Task LoginExecute()
        {
            await CurrentPage.Navigation.PopModalAsync();
        }

        public ICommand RegisterCommand { get; }
        private async Task RegisterExecute()
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
            if (string.IsNullOrWhiteSpace(Password))
            {
                await ToastManager.ShowToast("Password cannot be empty", Color.FromHex("#FF605C"));
                return;
            }
            if (Password.Length < 6)
            {
                await ToastManager.ShowToast("Password must be at least 6 characters", Color.FromHex("#FF605C"));
                return;
            }
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                await ToastManager.ShowToast("First name cannot be empty", Color.FromHex("#FF605C"));
                return;
            }
            if (FirstName.Length < 3)
            {
                await ToastManager.ShowToast("First name must be at least 3 characters", Color.FromHex("#FF605C"));
                return;
            }
            if (string.IsNullOrWhiteSpace(LastName))
            {
                await ToastManager.ShowToast("Last name cannot be empty", Color.FromHex("#FF605C"));
                return;
            }
            if (LastName.Length < 3)
            {
                await ToastManager.ShowToast("Last name must be at least 3 characters", Color.FromHex("#FF605C"));
                return;
            }
            if (string.IsNullOrWhiteSpace(Gender))
            {
                await ToastManager.ShowToast("Gender cannot be empty", Color.FromHex("#FF605C"));
                return;
            }

            try
            {
                UserCredential userCredential = await Database.FirebaseAuthClient.CreateUserWithEmailAndPasswordAsync(Email, Password, $"{FirstName} {LastName}");
                
                Debug.WriteLine(userCredential.AuthCredential);
                DataSensor dataSensor = new DataSensor()
                {
                    SmartWatchName = "Unknown",
                    SmartWatchStatus = "Disconnected",
                    StepSensor = "0",
                    HeartRateSensor = "0",
                    Battery = "0",
                };
                Models.User user = new Models.User()
                {
                    Uid = userCredential.User.Uid,
                    FirstName = FirstName,
                    LastName = LastName,
                    Birthday = Birthday.Date.ToString("MMMM dd, yyyy"),
                    Gender = Gender,
                    Contact = Contact,
                    Height = Height,
                    Weight = Weight,
                    DataSensors = dataSensor
                };
                await Database.FirebaseClient.Child("users").PostAsync(user);
                await CurrentPage.Navigation.PopModalAsync();
                await ToastManager.ShowToast("Registered Successfully", Color.FromHex("#1eb980"));
                
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

        private bool isValidEmail;

        public bool IsValidEmail
        {
            get => isValidEmail;
            set => SetProperty(ref isValidEmail, value);
        }

        private string contact = string.Empty;
        public string Contact
        {
            get => contact;
            set => SetProperty(ref contact, value);
        }

        private string gender;
        public string Gender
        {
            get => gender;
            set => SetProperty(ref gender, value);
        }

        private string weight = string.Empty;
        public string Weight
        {
            get => weight;
            set => SetProperty(ref weight, value);
        }

        private string height = string.Empty;
        public string Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }

        private string email = string.Empty;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        private DateTime birthday = DateTime.Now.Date;
        public DateTime Birthday
        {
            get => birthday;
            set => SetProperty(ref birthday, value);
        }

        private string firstName = string.Empty;
        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }

        private string lastName = string.Empty;
        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }

        private string password = string.Empty;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private DateTime maximumDate = DateTime.Now.Date;
        public DateTime MaximumDate
        {
            get => maximumDate;
        }
    }
}
