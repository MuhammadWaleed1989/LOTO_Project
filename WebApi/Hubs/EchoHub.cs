using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebApi.helper;
using WebApi.Models;
using System.Data;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApi.Services;

namespace WebApi.Hubs
{
    public class EchoHub: Hub
    {
        //private readonly IUserGameService _userGameService;
        //public EchoHub(IUserGameService userGameService)
        //{
        //    _userGameService = userGameService;
        //}
        private static List<tblUser> _groups = new List<tblUser>();
        private static List<NumbersGroup> _numbersGroup = new List<NumbersGroup>();
        
        private readonly AppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;

        public EchoHub(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionStrings)
        {
            _appSettings = appSettings.Value;
            _connectionStrings = connectionStrings.Value;
        }
        //public override Task OnDisconnectedAsync(Exception exception)
        //{
        //    if (_numbersGroup.Any(g => g.UserName == Context.User))
        //    {
        //        var gs = _numbersGroup.Where(g => g.Owner == Context.ConnectionId).ToList();
        //        for (int i = 0; i < gs.Count; i++)
        //        {
        //            BroadcastGroup(gs[i], true);
        //            _numbersGroup.Remove(gs[i]);
        //        }
        //    }
        //    return base.OnDisconnectedAsync(exception);
        //}

        //public void CreateOrJoin(string key, string email)
        //{
        //    var group = _numbersGroup.FirstOrDefault(g => g.Key == key);
        //    if (group == null)
        //    {
        //        group = new NumbersGroup { Key = key, Owner = Context.ConnectionId };
        //        _numbersGroup.Add(group);
        //    }

        //    if (group.HasFinished || group.HasStarted)
        //    {
        //        throw new Exception("You cannot join a group which has started or finished");
        //    }

        //    group.Glasses.Add(new Glass { ConnectionId = Context.ConnectionId, Email = email });

        //    BroadcastNumberGroup(group);
        //}
        public void GameStart(int gameID)
        {
            //    var group = _numbersGroup.FirstOrDefault(g => g.Owner == Context.ConnectionId && !g.HasFinished && !g.HasStarted);
            //    if (group != null)
            //    {
            //        group.HasStarted = true;
            BroadcastGameNumberGroup(gameID);
            //    }
        }
        public void StartGetNotConfirmedValue(int gameID,int userID)
        {
            //    var group = _numbersGroup.FirstOrDefault(g => g.Owner == Context.ConnectionId && !g.HasFinished && !g.HasStarted);
            //    if (group != null)
            //    {
            //        group.HasStarted = true;
            BroadcastNotConfirmedValues(gameID, userID);
            //    }
        }
        public void Start()
        {
            //var group = _groups.FirstOrDefault(g => g.Owner == Context.ConnectionId && !g.HasFinished && !g.HasStarted);
            //if (group != null)
            //{
            //    group.HasStarted = true;
                BroadcastGroup();
            //}
        }

        //public void Drink()
        //{
        //    var group = _numbersGroup.FirstOrDefault(g => !g.HasFinished && g.HasStarted && g.Glasses.Any(gl => gl.ConnectionId == Context.ConnectionId));
        //    if (group != null)
        //    {
        //        var glass = group.Glasses.First(g => g.ConnectionId == Context.ConnectionId);
        //        glass.Value--;
        //        if (glass.Value <= 0)
        //        {
        //            group.HasFinished = true;
        //            group.WinnerConnectionId = Context.ConnectionId;
        //            group.WinnerEmail = glass.Email;
        //        }
        //        BroadcastNumberGroup(group);
        //        if (group.HasFinished)
        //        {
        //            _numbersGroup.Remove(group);
        //        }
        //    }
        //}

        private void BroadcastGroup()
        {
            //var clients = group.Glasses.Select(g => g.ConnectionId).ToList();
            Clients.All.SendAsync("UserList", GetAll());
        }
        //private void BroadcastNumberGroup(NumbersGroup group, bool removing = false)
        //{
        //    var clients = group.Glasses.Select(g => g.ConnectionId).ToList();
        //    Clients.Clients(clients).SendAsync("Group", removing ? null : group);
        //}

        private void BroadcastGameNumberGroup(int gameID)
        {
            // var clients = group.Glasses.Select(g => g.ConnectionId).ToList();
            GameFinal _gameFinal = new GameFinal();
            _gameFinal.gameValues = GetValues(gameID);
            _gameFinal.gameWinner = GetGameWinnerName(gameID);
            Clients.All.SendAsync("GameAllValues", _gameFinal);
        }
        private void BroadcastNotConfirmedValues(int gameID,int userID)
        {
            Clients.All.SendAsync("GetNotConfirmedValue", GetNotConfirmedValue(gameID, userID));
        }
        public string GetGameWinnerName(int gameID)
        {
            string winnerName = string.Empty;
            string sQry = "SELECT ISNULL(U.FirstName +' '+U.LastName,'') WinnerName FROM dbo.tblGames G JOIN dbo.tblUser U ON g.WinnerID=u.UserID WHERE G.[GameID]="+ gameID;
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                winnerName = Convert.ToString(dtUserGameInfo.Rows[0]["WinnerName"]);
            }
            return winnerName;
        }
        public IEnumerable<tblUser> GetAll()
        {
            List<tblUser> userInfos = null;
            string sQry = "SELECT * FROM [tblUser]";
            DataTable dtUserInfo = ExecuteQuery(sQry);
            if (dtUserInfo != null)
            {
                //userInfos = new List<tblUser>();
                userInfos = (from DataRow dr in dtUserInfo.Rows
                             select new tblUser()
                             {
                                 UserID = Convert.ToInt32(dr["UserID"]),
                                 UserName = dr["UserName"].ToString(),
                                 FirstName = dr["FirstName"].ToString(),
                                 LastName = dr["LastName"].ToString(),
                                 Email = dr["Email"].ToString(),
                                 IsUserOnline = Convert.ToBoolean(dr["IsUserOnline"].ToString())
                             }).ToList();

                // userInfos = ConvertDataTable<tblUser>(dtUserInfo);
            }
            return userInfos;
        }
        public int[] GetAllValue(int gameID)
        {
            int[] userGameInfos = null;
            string sQry = "SELECT * FROM [tblUserGame] WHERE [GameID]=" + gameID + " AND IsNull(IsDeleted,0)=0";
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                userGameInfos = dtUserGameInfo.Rows.OfType<DataRow>().Select(k => Convert.ToInt32(k[3].ToString())).ToArray();

            }
            return userGameInfos;
        }
        public int[] GetNotConfirmedValue(int gameID, int userID)
        {
            int[] userGameInfos = null;
            string sQry = "SELECT * FROM [tblUserGame] WHERE [GameID]=" + gameID + " AND [UserID]=" + userID + " AND IsNull(IsDeleted,0)=0 AND IsNull(IsConfirmed,0)=0";
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                userGameInfos = dtUserGameInfo.Rows.OfType<DataRow>().Select(k => Convert.ToInt32(k[3].ToString())).ToArray();

            }
            return userGameInfos;
        }
        public IEnumerable<tblUserGame> GetValues(int gameID)
        {
            List<tblUserGame> userGameInfos = null;
            string sQry = "SELECT * FROM [tblUserGame] WHERE [GameID]=" + gameID + " AND IsNull(IsDeleted,0)=0";
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                userGameInfos = (from DataRow dr in dtUserGameInfo.Rows
                             select new tblUserGame()
                             {
                                 UserID = Convert.ToInt32(dr["UserID"]),
                                 GameID = Convert.ToInt32(dr["GameID"]),
                                 Value = Convert.ToInt32(dr["Value"]),
                                 IsConfirmed = Convert.ToBoolean(dr["IsConfirmed"])
                             }).ToList();

            }
            return userGameInfos;
        }



        private DataTable ExecuteQuery(string strSql)
        {
            SqlConnection conn = null;
            DataTable dt = null;
            try
            {
                conn = new SqlConnection(_connectionStrings.connectionStr);
                SqlCommand cmd = new SqlCommand(strSql, conn);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                dt = new DataTable();
                //Fill the dataset
                da.Fill(dt);
                if (!(dt.Rows.Count > 0)) dt = null;
            }
            catch { dt = null; }
            finally { if (conn.State != 0) conn.Close(); }
            return dt;
        }
    }

    public class GameFinal
    {
        public IEnumerable<tblUserGame> gameValues { get; set; }
        public string gameWinner { get; set; }
    }

    public class NumbersGroup
    {
        public int GameID { get; set; }
        public bool HasStarted { get; set; }
        public bool HasFinished { get; set; }
        public string Key { get; set; }
        public string WinnerConnectionId { get; set; }
        public string WinnerEmail { get; set; }
        //ConnectionId
        public string Owner { get; set; }
        public List<Glass> Glasses { get; set; } = new List<Glass>();
    }

    public class Glass
    {
        public bool HasLeft { get; set; }
        public string ConnectionId { get; set; }
        public string Email { get; set; }
        public int Value { get; set; } = 100;
    }
}
