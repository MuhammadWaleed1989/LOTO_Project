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

        public int? Value1 { get; set; }

        public int? Value2 { get; set; }

        public int? Value3 { get; set; }

        public int? Value4 { get; set; }

        public int? Value5 { get; set; }

        public int? Value6 { get; set; }
    }
}
