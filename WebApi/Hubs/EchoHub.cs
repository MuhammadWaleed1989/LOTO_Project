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
       
        private static List<tblUser> _groups = new List<tblUser>();
    
        
        private readonly AppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;

        public EchoHub(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionStrings)
        {
            _appSettings = appSettings.Value;
            _connectionStrings = connectionStrings.Value;
        }
        
        public void GameStart(int gameID, int userID)
        {
  
            InsertUserOfGame(gameID, userID);
            BroadcastGameNumberGroup(gameID,userID);
 
        }
        public int InsertUserOfGame(int gameID, int userID)
        {
            IEnumerable<tblGamesUsers> usersOfGame = null;
            int retVal = 0;
            usersOfGame = GetGameUsers(gameID);
            if (usersOfGame == null)
            {
                string sQry = "INSERT INTO [tblUserOfGame] ([UserID],[GameID],[ConnectionId],[IsDeleted]) " +
                  "VALUES('" + userID + "','" + gameID + "','" + Context.ConnectionId + "','" + false + "');SELECT SCOPE_IDENTITY();";
                retVal = ExecuteCRUDByQuery(sQry);
            }
            else
            {
                var result = usersOfGame.Where(g => g.ConnectionId == Context.ConnectionId && g.UserID == userID).ToList();
                if (!result.Any())
                {
                    string sQry = "INSERT INTO [tblUserOfGame] ([UserID],[GameID],[ConnectionId],[IsDeleted]) " +
                       "VALUES('" + userID + "','" + gameID + "','" + Context.ConnectionId + "','" + false + "');SELECT SCOPE_IDENTITY();";
                    retVal = ExecuteCRUDByQuery(sQry);

                }
            }
           
            return retVal;
        }
        public void StartGetNotConfirmedValue(int gameID,int userID)
        {

            BroadcastNotConfirmedValues(gameID, userID);

        }
        public void Start()
        {
                BroadcastGroup();
        }

        

        private void BroadcastGroup()
        {
            Clients.All.SendAsync("UserList", GetAll());
        }


        private void BroadcastGameNumberGroup(int gameID,int userID)
        {
             var clients = GetGameUsers(gameID);
            var gameClients = clients.Select(g => g.ConnectionId).ToList();
            GameFinal _gameFinal = new GameFinal();
            _gameFinal.gameInfo = GetGameInfo(gameID);
            _gameFinal.gameValues = GetValues(gameID);
            _gameFinal.userInfo = GetCurrentUser(userID);
            Clients.Clients(gameClients).SendAsync("GetGameInfo", _gameFinal);
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
        public tblUser GetCurrentUser(int userID)
        {
            List<tblUser> userInfos = null;
            string sQry = "DECLARE @COINPRICE INT =100; SET @COINPRICE = ISNULL((SELECT Coinprice FROM dbo.tblAdminConfig),100) SELECT U.[UserID],U.[UserName],U.[Email] ,U.[FirstName],U.[LastName] ,U.[Phone] ,U.[Password] ,U.[IsDeleted] ,U.[IsAdmin] ,U.[IsUserOnline] , ";
            sQry += " U.[CoinsCost] ,CAST(U.CoinsCost-(COUNT(UG.GameValueID) *@COINPRICE) AS BIGINT) AS RemainingCoins, ";
            sQry += " CAST((COUNT(UG.GameValueID) * 100) AS BIGINT) AS UsedCoins  FROM dbo.[tblUser] U ";
            sQry += " Left JOIN [tblUserGame]  UG ON UG.UserID= u.UserID ";
            sQry += " WHERE ISNULL(U.IsDeleted,0)=0 AND ISNULL(UG.IsDeleted,0)=0 AND U.UserID=" + userID;
            sQry += " GROUP BY U.[UserID],U.[UserName],U.[Email] ,U.[FirstName],U.[LastName] ,U.[Phone] ,U.[Password] ,U.[IsDeleted] ,U.[IsAdmin] ,U.[IsUserOnline] ,U.[CoinsCost] ";
            DataTable dtUserInfo = ExecuteQuery(sQry);
            if (dtUserInfo != null)
            {
                userInfos = (from DataRow dr in dtUserInfo.Rows
                             select new tblUser()
                             {
                                 UserID = Convert.ToInt32(dr["UserID"]),
                                 UserName = dr["UserName"].ToString(),
                                 FirstName = dr["FirstName"].ToString(),
                                 LastName = dr["LastName"].ToString(),
                                 Email = dr["Email"].ToString(),
                                 Phone = dr["Phone"].ToString(),
                                 IsUserOnline = Convert.ToBoolean(dr["IsUserOnline"].ToString()),
                                 CoinsCost = Convert.ToInt32(dr["CoinsCost"]),
                                 UsedCoins = Convert.ToInt32(dr["UsedCoins"]),
                                 RemainingCoins = Convert.ToInt32(dr["RemainingCoins"]),
                             }).ToList();

                // userInfos = ConvertDataTable<tblUser>(dtUserInfo);
            }
            tblUser tblUserInfo = new tblUser();
            if (userInfos != null)
            {
                if (userInfos.Any()) { tblUserInfo = userInfos[0]; }
            }
            
            
            return tblUserInfo;
        }
        public IEnumerable<tblUser> GetAll()
        {
            List<tblUser> userInfos = null;
            string sQry = "DECLARE @COINPRICE INT =100;SET @COINPRICE = ISNULL((SELECT Coinprice FROM dbo.tblAdminConfig),100) SELECT U.[UserID],U.[UserName],U.[Email] ,U.[FirstName],U.[LastName] ,U.[Phone] ,U.[Password] ,U.[IsDeleted] ,U.[IsAdmin] ,U.[IsUserOnline] , ";
            sQry += " U.[CoinsCost] ,CAST(U.CoinsCost-(COUNT(UG.GameValueID) *@COINPRICE) AS BIGINT) AS RemainingCoins, ";
            sQry += " CAST((COUNT(UG.GameValueID) * 100) AS BIGINT) AS UsedCoins  FROM dbo.[tblUser] U ";
            sQry += " LEFT JOIN [tblUserGame]  UG ON UG.UserID= u.UserID AND UG.GameID= 2 ";
            sQry += " WHERE ISNULL(U.IsDeleted,0)=0 AND ISNULL(UG.IsDeleted,0)=0 ";
            sQry += " GROUP BY U.[UserID],U.[UserName],U.[Email] ,U.[FirstName],U.[LastName] ,U.[Phone] ,U.[Password] ,U.[IsDeleted] ,U.[IsAdmin] ,U.[IsUserOnline] ,U.[CoinsCost] ";
            DataTable dtUserInfo = ExecuteQuery(sQry);
            if (dtUserInfo != null)
            {
                userInfos = (from DataRow dr in dtUserInfo.Rows
                             select new tblUser()
                             {
                                 UserID = Convert.ToInt32(dr["UserID"]),
                                 UserName = dr["UserName"].ToString(),
                                 FirstName = dr["FirstName"].ToString(),
                                 LastName = dr["LastName"].ToString(),
                                 Email = dr["Email"].ToString(),
                                 Phone = dr["Phone"].ToString(),
                                 IsUserOnline = Convert.ToBoolean(dr["IsUserOnline"].ToString()),
                                 CoinsCost = Convert.ToInt32(dr["CoinsCost"]),
                                 UsedCoins= Convert.ToInt32(dr["UsedCoins"]),
                                 RemainingCoins = Convert.ToInt32(dr["RemainingCoins"]),
                             }).ToList();

                // userInfos = ConvertDataTable<tblUser>(dtUserInfo);
            }
            return userInfos;
        }
        public IEnumerable<tblGamesUsers> GetGameUsers(int gameID)
        {
            List<tblGamesUsers> userInfos = null;
            string sQry = "SELECT U.*, UG.ConnectionId FROM [tblUser] U JOIN [tblUserOfGame] UG ON UG.UserID=u.UserID AND UG.GameID=" + gameID+" WHERE ISNULL(U.IsDeleted,0)= 0 AND ISNULL(UG.IsDeleted,0)= 0";
            DataTable dtUserInfo = ExecuteQuery(sQry);
            if (dtUserInfo != null)
            {
                //userInfos = new List<tblUser>();
                userInfos = (from DataRow dr in dtUserInfo.Rows
                             select new tblGamesUsers()
                             {
                                 UserID = Convert.ToInt32(dr["UserID"]),
                                 UserName = dr["UserName"].ToString(),
                                 FirstName = dr["FirstName"].ToString(),
                                 LastName = dr["LastName"].ToString(),
                                 Email = dr["Email"].ToString(),
                                 Phone = dr["Phone"].ToString(),
                                 ConnectionId=dr["ConnectionId"].ToString(),
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
        public IEnumerable<tblGames> GetGameInfo(int gameID)
        {
            List<tblGames> gameInfos = null;
            string sQry = "SELECT * FROM [tblGames] WHERE [GameID]=" + gameID + " AND IsNull(IsDeleted,0)=0";
            DataTable dtGameInfo = ExecuteQuery(sQry);
            if (dtGameInfo != null)
            {
                gameInfos = (from DataRow dr in dtGameInfo.Rows
                             select new tblGames()
                             {
                                 GameID = Convert.ToInt32(dr["GameID"]),
                                 WinValue1 = Convert.ToInt32(dr["WinValue1"]),
                                 WinValue2 = Convert.ToInt32(dr["WinValue2"]),
                                 WinValue3 = Convert.ToInt32(dr["WinValue3"]),
                                 WinValue4 = Convert.ToInt32(dr["WinValue4"]),
                                 WinValue5 = Convert.ToInt32(dr["WinValue5"]),
                                 WinValue6 = Convert.ToInt32(dr["WinValue6"]),
                                 IsGameStart = Convert.ToBoolean(dr["IsGameStart"]),
                                 IsGamePause = Convert.ToBoolean(dr["IsGamePause"]),
                                 IsGameFinish = Convert.ToBoolean(dr["IsGameFinish"]),
                                 StartDate = Convert.ToDateTime(dr["StartDate"]),
                                 StartTime = Convert.ToDateTime(dr["StartTime"]),
                                 EndDate = Convert.ToDateTime(dr["EndDate"]),
                                 EndTime = Convert.ToDateTime(dr["EndTime"]),
                                 StartDateAndTime = Convert.ToDateTime(dr["StartDate"]).ToString("MM/dd/yyyy") + " " + Convert.ToDateTime(dr["StartTime"]).ToString("hh:mm tt"),
                                 EndDateAndTime = Convert.ToDateTime(dr["EndDate"]).ToString("MM/dd/yyyy") + " " + Convert.ToDateTime(dr["EndTime"]).ToString("hh:mm tt"),
                                 WinValueList = new int[6] { Convert.ToInt32(dr["WinValue1"].ToString()), Convert.ToInt32(dr["WinValue2"].ToString()), Convert.ToInt32(dr["WinValue3"].ToString()), Convert.ToInt32(dr["WinValue4"].ToString()), Convert.ToInt32(dr["WinValue5"].ToString()), Convert.ToInt32(dr["WinValue6"].ToString()) },
                             }).ToList();

            }
            return gameInfos;
        }
        public IEnumerable<tblGameWithValues> GetValues(int gameID)
        {
            List<tblGameWithValues> userGameInfos = null;
            string sQry = "SELECT U.UserID,UG.IsConfirmed,G.*,U.FirstName +' '+ u.LastName AS UserName FROM dbo.tblUserGame UG JOIN dbo.tblGameValues G ON G.GameValueID= UG.GameValueID";
            sQry += " JOIN dbo.tblUser U ON U.UserID = UG.UserID WHERE [GameID]=" + gameID + " AND ISNULL(UG.IsDeleted,0)= 0 ORDER BY u.UserID,UG.GameValueID";
            //string sQry = "SELECT * FROM [tblUserGame] WHERE [GameID]=" + gameID + " AND IsNull(IsDeleted,0)=0";
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                userGameInfos = (from DataRow dr in dtUserGameInfo.Rows
                                 select new tblGameWithValues()
                                 {
                                     UserID = Convert.ToInt32(dr["UserID"]),
                                     GameValueID = Convert.ToInt32(dr["GameValueID"]),
                                     RowNum1 = Convert.ToInt32(dr["RowNum1"]),
                                     RowNum2 = Convert.ToInt32(dr["RowNum2"]),
                                     RowNum3 = Convert.ToInt32(dr["RowNum3"]),
                                     RowNum4 = Convert.ToInt32(dr["RowNum4"]),
                                     RowNum5 = Convert.ToInt32(dr["RowNum5"]),
                                     RowNum6 = Convert.ToInt32(dr["RowNum6"]),
                                     UserName = Convert.ToString(dr["UserName"]),
                                     IsConfirmed = Convert.ToBoolean(dr["IsConfirmed"])
                                 }).ToList();

            }
            return userGameInfos;
        }

        public bool CalculateResult(int gameID)
        {
            List<GameUsers> gameUsers = null;
            List<GameInfos> gameInfos = null;
            string gameUserQry = "SELECT U.UserID,G.*,U.FirstName + ' ' + u.LastName AS UserName FROM dbo.tblUserGame UG ";
            gameUserQry += "JOIN dbo.tblGameValues G ON G.GameValueID = UG.GameValueID JOIN dbo.tblUser U ON U.UserID = UG.UserID WHERE[GameID] = " + gameID;
            gameUserQry += " AND ISNULL(UG.IsDeleted,0)= 0 AND ISNULL(UG.IsConfirmed,0)= 1 ORDER BY u.UserID,UG.GameValueID";
            DataTable dtGameUsers = ExecuteQuery(gameUserQry);
            if (dtGameUsers != null)
            {
                gameUsers = (from DataRow dr in dtGameUsers.Rows
                                 select new GameUsers()
                                 {
                                     UserID = Convert.ToInt32(dr["UserID"]),
                                     GameValueID = Convert.ToInt32(dr["GameValueID"]),
                                     RowNum1 = Convert.ToInt32(dr["RowNum1"]),
                                     RowNum2 = Convert.ToInt32(dr["RowNum2"]),
                                     RowNum3 = Convert.ToInt32(dr["RowNum3"]),
                                     RowNum4 = Convert.ToInt32(dr["RowNum4"]),
                                     RowNum5 = Convert.ToInt32(dr["RowNum5"]),
                                     RowNum6 = Convert.ToInt32(dr["RowNum6"]),
                                     UserName = Convert.ToString(dr["UserName"]),
                                     RowValueList = new int[6] { Convert.ToInt32(dr["RowNum1"].ToString()), Convert.ToInt32(dr["RowNum2"].ToString()), Convert.ToInt32(dr["RowNum3"].ToString()), Convert.ToInt32(dr["RowNum4"].ToString()), Convert.ToInt32(dr["RowNum5"].ToString()), Convert.ToInt32(dr["RowNum6"].ToString()) },
                                 }).ToList();

            }
            string gameQry = "SELECT GameID,WinValue1,WinValue2,WinValue3,WinValue4,WinValue5,WinValue6 FROM  dbo.tblGames WHERE ISNULL(IsDeleted, 0) = 0 AND ISNULL(IsGameFinish,0)= 1 ";
            gameQry += " AND[GameID] = "+gameID;
            DataTable dtGameInfos = ExecuteQuery(gameQry);
            if (dtGameInfos != null)
            {
                gameInfos = (from DataRow dr in dtGameInfos.Rows
                             select new GameInfos()
                             {
                                 GameID = Convert.ToInt32(dr["GameID"]),
                                 WinValue1 = Convert.ToInt32(dr["RowNum1"]),
                                 WinValue2 = Convert.ToInt32(dr["RowNum2"]),
                                 WinValue3 = Convert.ToInt32(dr["RowNum3"]),
                                 WinValue4 = Convert.ToInt32(dr["RowNum4"]),
                                 WinValue5 = Convert.ToInt32(dr["RowNum5"]),
                                 WinValue6 = Convert.ToInt32(dr["RowNum6"]),
                                 WinValueList = new int[6] { Convert.ToInt32(dr["WinValue1"].ToString()), Convert.ToInt32(dr["WinValue2"].ToString()), Convert.ToInt32(dr["WinValue3"].ToString()), Convert.ToInt32(dr["WinValue4"].ToString()), Convert.ToInt32(dr["WinValue5"].ToString()), Convert.ToInt32(dr["WinValue6"].ToString()) },
                             }).ToList();

            }
            return true;
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
        private int ExecuteCRUDByQuery(string strSql)
        {
            SqlConnection conn = null;
            int iR = 0;
            try
            {
                conn = new SqlConnection(_connectionStrings.connectionStr);
                SqlCommand cmd = new SqlCommand(strSql, conn);
                cmd.CommandType = CommandType.Text;
                conn.Open();
                //Execute the command
                iR = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch { iR = 0; }
            finally { if (conn.State != 0) conn.Close(); }
            return iR;
        }
    }

    public class tblGamesUsers
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public bool? IsDeleted { get; set; }
        public string Phone { get; set; }
        public string ConnectionId { get; set; }
        
        public bool? IsAdmin { get; set; }
        public bool? IsUserOnline { get; set; }

    }
    public class GameFinal
    {
        public IEnumerable<tblGameWithValues> gameValues { get; set; }
        public IEnumerable<tblGames> gameInfo { get; set; }
        public tblUser userInfo { get; set; }
    }

 

    public class tblGameWithValues
    {
        public int? UserID { get; set; }
        public int? GameValueID { get; set; }
        public bool? IsConfirmed { get; set; }

        public int? RowNum1 { get; set; }

        public int? RowNum2 { get; set; }

        public int? RowNum3 { get; set; }

        public int? RowNum4 { get; set; }

        public int? RowNum5 { get; set; }

        public int? RowNum6 { get; set; }
        public string UserName { get; set; }

    }

    public class GameUsers
    {
        public int? UserID { get; set; }
        public int? GameValueID { get; set; }

        public int? RowNum1 { get; set; }

        public int? RowNum2 { get; set; }

        public int? RowNum3 { get; set; }

        public int? RowNum4 { get; set; }

        public int? RowNum5 { get; set; }

        public int? RowNum6 { get; set; }
        public int[] RowValueList { get; set; }
        public string UserName { get; set; }

    }

    public class GameInfos
    {
        public int GameID { get; set; }

        public int? WinValue1 { get; set; }

        public int? WinValue2 { get; set; }

        public int? WinValue3 { get; set; }

        public int? WinValue4 { get; set; }

        public int? WinValue5 { get; set; }

        public int? WinValue6 { get; set; }
        public int[] WinValueList { get; set; }

    }
}
