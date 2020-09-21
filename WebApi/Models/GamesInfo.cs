using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class tblGames
    {
        public int GameID { get; set; }

        public string GameName { get; set; }

        public int? WinValue1 { get; set; }

        public int? WinValue2 { get; set; }

        public int? WinValue3 { get; set; }

        public int? WinValue4 { get; set; }

        public int? WinValue5 { get; set; }

        public int? WinValue6 { get; set; }
        public int[] WinValueList { get; set; }

        public bool? IsDeleted { get; set; }
        public bool? IsGameStart { get; set; }

        public bool? IsGamePause { get; set; }

        public bool? IsGameFinish { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? StartTime { get; set; }
        public string StartDateAndTime { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? EndTime { get; set; }
        public string EndDateAndTime { get; set; }
        public string WinnerImage { get; set; }
    }
    public class GameData
    { 
        public tblGames gameInfo { get; set; }
        public List<tblGamesDetail> gameDetail { get; set; }
    }
}
