using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BioHax.Models
{
    public class Service
    {
        public int ServiceId { get; set; }

        // user ID from AspNetUser table
        public string OwnerID { get; set; }

        public string Provider { get; set; }
        public string Type { get; set; }

        public ServiceStatus Status { get; set; }
    }

    public enum ServiceStatus
    {
        Submitted,
        Approved,
        Rejected
    }
}
