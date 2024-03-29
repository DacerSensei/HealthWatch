﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.CommunityToolkit.ObjectModel;

namespace HealthMonitoring.Models
{
    public class DataSensor : ObservableObject
    {
        private string smartWatchName;
        public string SmartWatchName
        {
            get => smartWatchName;
            set => SetProperty(ref smartWatchName, value);
        }

        private string battery;
        public string Battery
        {
            get => battery;
            set => SetProperty(ref battery, value);
        }

        private string smartWatchStatus;
        public string SmartWatchStatus
        {
            get => smartWatchStatus;
            set => SetProperty(ref smartWatchStatus, value);
        }

        private string heartRateSensor;
        public string HeartRateSensor
        {
            get => heartRateSensor;
            set => SetProperty(ref heartRateSensor, value);
        }

        private string stepSensor;
        public string StepSensor
        {
            get => stepSensor;
            set
            {
                SetProperty(ref stepSensor, value);
                StepCounterChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler StepCounterChanged;

    }
}
