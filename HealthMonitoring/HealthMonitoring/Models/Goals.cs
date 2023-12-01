using System;
using System.Collections.Generic;
using System.Text;

namespace HealthMonitoring.Models
{
    public class Goals
    {
        public string Key { get; set; }
        public string StepsTaken { get; set; }
        public string TotalSteps { get; set; }
        public string Status { get; set; }
        public string Created { get; set; }
        public string Completed { get; set; }
    }
}
