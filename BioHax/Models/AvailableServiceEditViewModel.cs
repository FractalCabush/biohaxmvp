using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BioHax.Models
{
    public class AvailableServiceEditViewModel
    {
        public int ServiceId { get; set; }
        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
