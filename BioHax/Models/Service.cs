using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BioHax.Models
{
    public abstract class Service
    {
        public int ServiceId { get; set; }

        // user ID from AspNetUser table
        public string OwnerID { get; set; }
        public ServiceStatus Status { get; set; }
    }

    public enum ServiceStatus
    {
        Submitted,
        Approved,
        Rejected
    }
}
