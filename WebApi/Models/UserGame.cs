using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class tblUserGame
    {
        public int UserGameID { get; set; }
        public int? UserID { get; set; }
        public int? GameID { get; set; }
        public int? Value { get; set; }
        public bool? IsConfirmed { get; set; }

        public bool? IsDeleted { get; set; }

    }
}
