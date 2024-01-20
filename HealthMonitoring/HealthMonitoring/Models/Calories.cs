using System;
using System.Collections.Generic;
using System.Text;

namespace HealthMonitoring.Models
{
    public class Calories
    {
        public string Key { get; set; }
        public string CalorieTaken { get; set; }
        public string GoalCalorie { get; set; }
        public string Status { get; set; }
        public string Created { get; set; }
        public bool IsCompleted
        {
            get
            {
                int goal = Convert.ToInt16(GoalCalorie);
                int progress = Convert.ToInt16(CalorieTaken);
                if (progress >= goal)
                {
                    return false;
                }
                return true;
            }
        }
        public string StatusColor
        {
            get
            {
                int goal = Convert.ToInt16(GoalCalorie);
                int progress = Convert.ToInt16(CalorieTaken);
                if (progress >= goal)
                {
                    return "#1eb980";
                }
                return "#FF605C";
            }
        }
    }
}
