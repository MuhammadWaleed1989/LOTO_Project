using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace TestWebApi.Models
{
    public class tblUser
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }

        public bool? IsDeleted { get; set; }

        public bool? IsAdmin { get; set; }

    }
}
