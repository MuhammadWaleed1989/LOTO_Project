using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class tblUserOfGame
    {
        public int GameUserID { get; set; }

        public int? UserID { get; set; }

        public int? GameID { get; set; }

        public bool? IsDeleted { get; set; }
        public string ConnectionId { get; set; }
    }

}
