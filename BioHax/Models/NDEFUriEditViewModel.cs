using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BioHax.Models
{
    public class NDEFUriEditViewModel
    {
        public int ServiceId { get; set; }
        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; }
        [Required(ErrorMessage = "URI is required")]
        public string URI { get; set; }
    }
}
