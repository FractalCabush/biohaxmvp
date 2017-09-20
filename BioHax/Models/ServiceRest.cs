using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioHax.Models
{
    public class ServiceRest
    {
        public int ServiceId { get; set; }
        public string Provider { get; set; }

        public ServiceRest(NDEFUri model)
        {
            this.Provider = model.Provider;
            this.ServiceId = model.ServiceId;
        }
    }
}
