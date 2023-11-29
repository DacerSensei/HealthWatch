using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace HealthMonitoring.Models
{
    public class User : ObservableObject
    {
        public string Uid { get; set; }

        private string contact;
        public string Contact
        {
            get => contact;
            set
            {
                SetProperty(ref contact, value);
            }
        }

        private string gender;
        public string Gender
        {
            get => gender;
            set
            {
                SetProperty(ref gender, value);
                OnPropertyChanged(nameof(GenderColor));
            }
        }

        private string weight;
        public string Weight
        {
            get => weight;
            set
            {
                SetProperty(ref weight, value);
            }
        }

        private string height;
        public string Height
        {
            get => height;
            set
            {
                SetProperty(ref height, value);
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                SetProperty(ref email, value);
            }
        }

        private string birthday;
        public string Birthday
        {
            get => birthday;
            set
            {
                SetProperty(ref birthday, value);
            }
        }

        private string firstName;
        public string FirstName
        {
            get => firstName;
            set
            {
                SetProperty(ref firstName, value);
                OnPropertyChanged(nameof(CompleteName));
                OnPropertyChanged(nameof(InitialName));
            }
        }

        private string lastName;
        public string LastName
        {
            get => lastName;
            set
            {
                SetProperty(ref lastName, value);
                OnPropertyChanged(nameof(CompleteName));
                OnPropertyChanged(nameof(InitialName));

            }
        }

        public string CompleteName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string InitialName
        {
            get
            {
                return FirstName[0].ToString() + LastName[0].ToString();
            }
        }

        public string GenderColor
        {
            get
            {
                if(Gender != null)
                {
                    return Gender.ToUpper() == "MALE" ? "#02b0f0" : "#f75b95";
                }
                return "#000";
            }
        }
    }
}
