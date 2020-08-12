using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApi.Models
{
    public class PlaceInfo
    {
        public int Id { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        public string About { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
