using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool? IsAdmin { get; set; }


        public AuthenticateResponse(tblUser user, string token)
        {
            Id = user.UserID;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.UserName;
            Token = token;
            Email = user.Email;
            IsAdmin= user.IsAdmin;
        }
    }
}
