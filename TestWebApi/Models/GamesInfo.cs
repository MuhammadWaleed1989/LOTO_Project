using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApi.Models
{
    public class tblGames
    {
        public int GameID { get; set; }

        public string GameName { get; set; }

        public int? Cell1Value { get; set; }

        public int? Cell2Value { get; set; }

        public int? Cell3Value { get; set; }

        public int? Cell4Value { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
