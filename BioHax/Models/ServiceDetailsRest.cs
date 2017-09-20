using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioHax.Models
{
    public class ServiceDetailsRest
    {
        public string URI { get; set; }
        public string Provider { get; set; }

        public ServiceDetailsRest(NDEFUri model)
        {
            this.URI = Encoding.UTF8.GetString(model.Record.URI);
            this.Provider = model.Provider;
        }
    }
}
