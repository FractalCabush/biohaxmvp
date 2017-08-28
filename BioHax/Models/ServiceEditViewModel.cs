using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BioHax.Models
{
    public class ServiceEditViewModel
    {
        public int ServiceId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Provider { get; set; }
        public string Type { get; set; }
    }
}
