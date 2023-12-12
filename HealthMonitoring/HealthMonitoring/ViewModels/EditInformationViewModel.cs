using Firebase.Auth;
using Firebase.Database.Query;
using HealthMonitoring.Config;
using HealthMonitoring.Models;
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
    public class EditInformationViewModel : ObservableObject
    {
        public EditInformationViewModel()
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
            if (!await Application.Current.MainPage.DisplayAlert("Notice", "Are you sure you want to change your information?", "Yes", "No", FlowDirection.LeftToRight))
            {
                return;
            }
            try
            {
                await Database.FirebaseClient.Child($"users/{Services.UserManager.User.Key}").PatchAsync(new
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Birthday = Birthday.Date.ToString("MMMM dd, yyyy"),
                    Gender = Gender,
                    Contact = Contact,
                    Height = Height,
                    Weight = Weight
                });
                Services.UserManager.User.FirstName = FirstName;
                Services.UserManager.User.LastName = LastName;
                Services.UserManager.User.Birthday = Birthday.Date.ToString("MMMM dd, yyyy");
                Services.UserManager.User.Gender = Gender;
                Services.UserManager.User.Contact = Contact;
                Services.UserManager.User.Height = Height;
                Services.UserManager.User.Weight = Weight;
                await Application.Current.MainPage.Navigation.PopModalAsync();
                await ToastManager.ShowToast("Your information has been changed", Color.FromHex("#1eb980"));

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

        private string contact = Services.UserManager.User.Contact;
        public string Contact
        {
            get => contact;
            set => SetProperty(ref contact, value);
        }

        private string gender = Services.UserManager.User.Gender;
        public string Gender
        {
            get => gender;
            set => SetProperty(ref gender, value);
        }

        private string weight = Services.UserManager.User.Weight;
        public string Weight
        {
            get => weight;
            set => SetProperty(ref weight, value);
        }

        private string height = Services.UserManager.User.Height;
        public string Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }

        private DateTime birthday = Convert.ToDateTime(Services.UserManager.User.Birthday);
        public DateTime Birthday
        {
            get => birthday;
            set => SetProperty(ref birthday, value);
        }

        private string firstName = Services.UserManager.User.FirstName;
        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }

        private string lastName = Services.UserManager.User.LastName;
        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }

        private DateTime maximumDate = DateTime.Now.Date;
        public DateTime MaximumDate
        {
            get => maximumDate;
        }
    }
}
