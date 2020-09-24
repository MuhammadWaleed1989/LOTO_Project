using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.helper;
using System.Reflection;

namespace WebApi.Services
{
    public class GameInfoService : IGameInfoService
    {


        private readonly AppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;

        public GameInfoService(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionStrings)
        {
            _appSettings = appSettings.Value;
            _connectionStrings = connectionStrings.Value;
        }
        #region Main Functions
        public IEnumerable<tblGames> GetAll()
        {
            List<tblGames> gameInfos = null;
            string sQry = "SELECT * FROM [tblGames]";
            DataTable dtGameInfo = ExecuteQuery(sQry);
            if (dtGameInfo != null)
            {
                gameInfos = (from DataRow dr in dtGameInfo.Rows
                             select new tblGames()
                             {
                                 GameID = Convert.ToInt32(dr["GameID"]),
                                 GameName = dr["GameName"].ToString(),
                                 WinValue1 = Convert.ToInt32(dr["WinValue1"].ToString()),
                                 WinValue2 = Convert.ToInt32(dr["WinValue2"].ToString()),
                                 WinValue3 = Convert.ToInt32(dr["WinValue3"].ToString()),
                                 WinValue4 = Convert.ToInt32(dr["WinValue4"].ToString()),
                                 WinValue5 = Convert.ToInt32(dr["WinValue5"].ToString()),
                                 WinValue6 = Convert.ToInt32(dr["WinValue6"].ToString()),
                                 StartDate = Convert.ToDateTime(dr["StartDate"]),
                                 StartTime = Convert.ToDateTime(dr["StartTime"]),
                                 EndDate = Convert.ToDateTime(dr["EndDate"]),
                                 EndTime = Convert.ToDateTime(dr["EndTime"]),
                                 StartDateAndTime = Convert.ToDateTime(dr["StartDate"]).ToString("MM/dd/yyyy") + " " +  Convert.ToDateTime(dr["StartTime"]).ToString("hh:mm tt"),
                                 EndDateAndTime = Convert.ToDateTime(dr["EndDate"]).ToString("MM/dd/yyyy") + " " + Convert.ToDateTime(dr["EndTime"]).ToString("hh:mm tt"),
                                 WinValueList = new int[6] { Convert.ToInt32(dr["WinValue1"].ToString()), Convert.ToInt32(dr["WinValue2"].ToString()), Convert.ToInt32(dr["WinValue3"].ToString()),Convert.ToInt32(dr["WinValue4"].ToString()), Convert.ToInt32(dr["WinValue5"].ToString()), Convert.ToInt32(dr["WinValue6"].ToString()) },
                             }).ToList();
            }
            return gameInfos;
        }
        public tblGames GetLastWinner()
        {
            tblGames gameData = new tblGames();
            string sQry = "SELECT TOP 1 * FROM dbo.tblGames WHERE ISNULL(winValue1,-1)<>-1 ORDER BY EndDate DESC ";
            DataTable dtGameInfo = ExecuteQuery(sQry);
            if (dtGameInfo != null)
            {
                DataRow dr = dtGameInfo.Rows[0];
                gameData = GetGameInfoByRow(dr);
            }
            return gameData;
        }
        public int Add(tblGames gameInfo)
        {
            string sQry = "INSERT INTO [tblGames] ([GameName],[WinValue1],[WinValue2],[WinValue3],[WinValue4],[WinValue5],[WinValue6],[IsDeleted],[IsGameStart],[IsGamePause],[IsGameFinish],[StartDate],[StartTime],[EndDate],[EndTime]) " +
                "VALUES('" + gameInfo.GameName + "','" + gameInfo.WinValue1 + "','" + gameInfo.WinValue2 + "','" +
                gameInfo.WinValue3 + "','" + gameInfo.WinValue4 + "','" + gameInfo.WinValue5 + "','" + gameInfo.WinValue6 + "','" + false + "','" + gameInfo.IsGameStart + "','" + gameInfo.IsGamePause + "','" + gameInfo.IsGameFinish + "','" + gameInfo.StartDate + "','" + gameInfo.StartTime + "','" + gameInfo.EndDate + "','" + gameInfo.EndTime + "');SELECT SCOPE_IDENTITY();";
            int retVal = ExecuteCRUDByQuery(sQry);
            return retVal;
        }
        public int InsertUserOfGame(tblUserOfGame usersOfGame)
        {
            string sQry = "INSERT INTO [tblUserOfGame] ([UserID],[GameID],[IsDeleted]) " +
                "VALUES('" + usersOfGame.UserID + "','" + usersOfGame.GameID + "','" + false + "');SELECT SCOPE_IDENTITY();";
            int retVal = ExecuteCRUDByQuery(sQry);
            return retVal;
        }
        public int Update(int gameId, tblGames gameInfo)
        {
            string sQry = "UPDATE [tblGames] SET [GameName]='" + gameInfo.GameName + "',[StartDate]='" + gameInfo.StartDate + "',[StartTime]='" + gameInfo.StartTime + "',[EndDate]='" + gameInfo.EndDate + "',[EndTime]='" + gameInfo.EndTime + "' WHERE [GameID]=" + gameId;
            int retVal = ExecuteUpdateQuery(sQry);
            return retVal;
        }
        public int UpdateWinnigValues(tblGames gameInfo)
        {
            string sQry = "UPDATE [tblGames] SET [WinValue1]='" + gameInfo.WinValue1 + "',[WinValue2]='" + gameInfo.WinValue2 + "',[WinValue3]='" + gameInfo.WinValue3 + "',[WinValue4]='" + gameInfo.WinValue4 + "',[WinValue5]='" + gameInfo.WinValue5 + "',[WinValue6]='" + gameInfo.WinValue6 + "' WHERE [GameID]=" + gameInfo.GameID;
            int retVal = ExecuteUpdateQuery(sQry);
            return retVal;
        }
        public int UpdateWinnerImage(string winnerImage, int gameID)
        {
            string sQry = "UPDATE [tblGames] SET [WinnerImage]='" + winnerImage + "' WHERE [GameID]=" + gameID;
            int retVal = ExecuteUpdateQuery(sQry);
            return retVal;
        }
        public tblGames GetById(int id)
        {
            return Find(id);
        }
        public tblGames Find(int id)
        {
            tblGames gameData = new tblGames();
            string sQry = "SELECT * FROM [tblGames] WHERE [GameID]=" + id;
            DataTable dtGameInfo = ExecuteQuery(sQry);
            if (dtGameInfo != null)
            {
                DataRow dr = dtGameInfo.Rows[0];
                gameData = GetGameInfoByRow(dr);
            }
            return gameData;
        }
        public int Remove(int id)
        {
            string sQry = "DELETE FROM [tblGames] WHERE [GameID]=" + id;
            int retVal = ExecuteCRUDByQuery(sQry);
            return retVal;
        }
        #endregion
        #region Functions to Convert Data Row to Class
        private tblGames GetGameInfoByRow(DataRow dr)
        {
            tblGames gameInfo = new tblGames();
            gameInfo.GameID = Convert.ToInt32(dr["GameID"]);
            gameInfo.GameName = Convert.ToString(dr["GameName"]);
            gameInfo.WinValue1 = Convert.ToInt32(dr["WinValue1"]);
            gameInfo.WinValue2 = Convert.ToInt32(dr["WinValue2"]);
            gameInfo.WinValue3 = Convert.ToInt32(dr["WinValue3"]);
            gameInfo.WinValue4 = Convert.ToInt32(dr["WinValue4"]);
            gameInfo.WinValue5 = Convert.ToInt32(dr["WinValue5"]);
            gameInfo.WinValue6 = Convert.ToInt32(dr["WinValue6"]);
            gameInfo.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);
            gameInfo.WinnerImage = Convert.ToString(dr["WinnerImage"]);
            gameInfo.WinValueList = new int[6] { Convert.ToInt32(dr["WinValue1"].ToString()), Convert.ToInt32(dr["WinValue2"].ToString()), Convert.ToInt32(dr["WinValue3"].ToString()), Convert.ToInt32(dr["WinValue4"].ToString()), Convert.ToInt32(dr["WinValue5"].ToString()), Convert.ToInt32(dr["WinValue6"].ToString()) };
            return gameInfo;
        }
        #endregion

        #region Supporting Function
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
        private int ExecuteUpdateQuery(string strSql)
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
                iR = cmd.ExecuteNonQuery();
            }
            catch { iR = 0; }
            finally { if (conn.State != 0) conn.Close(); }
            return iR;
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        #endregion

    }
}
