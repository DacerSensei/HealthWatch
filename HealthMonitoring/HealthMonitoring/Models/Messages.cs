using System;
using System.Collections.Generic;
using System.Text;

namespace HealthMonitoring.Models
{
    public class Messages
    {
        public string Key { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedTime { get; set; }
        public string Message { get; set; }

        internal IEnumerable<object> Reverse()
        {
            throw new NotImplementedException();
        }
    }
}
