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
    public class UserGameService : IUserGameService
    {
        private readonly AppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;
        public UserGameService(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionStrings)
        {
            _appSettings = appSettings.Value;
            _connectionStrings = connectionStrings.Value;
        }
        #region Main functions
        public IEnumerable<tblGameValues> GetGameValues()
        {
            List<tblGameValues> gameValues = null;
            string sQry = "SELECT * FROM [tblGameValues]";
            DataTable dtGameValues = ExecuteQuery(sQry);
            if (dtGameValues != null)
            {
                gameValues = (from DataRow dr in dtGameValues.Rows
                              select new tblGameValues()
                              {
                                  GameValueID = Convert.ToInt32(dr["GameValueID"]),
                                  RowNum1 = Convert.ToInt32(dr["RowNum1"]),
                                  RowNum2 = Convert.ToInt32(dr["RowNum2"]),
                                  RowNum3 = Convert.ToInt32(dr["RowNum3"]),
                                  RowNum4 = Convert.ToInt32(dr["RowNum4"]),
                                  RowNum5 = Convert.ToInt32(dr["RowNum5"]),
                                  RowNum6 = Convert.ToInt32(dr["RowNum6"]),
                              }).ToList();
            }
            return gameValues;
        }
        public int Add(tblUserGame userGamedata)
        {
                string sQry = "INSERT INTO [tblUserGame] ([UserID],[GameID],[GameValueID],[IsConfirmed],[IsDeleted]) " +
                  "VALUES('" + userGamedata.UserID + "','" + userGamedata.GameID + "','" + userGamedata.GameValueID + "','" + userGamedata.IsConfirmed + "','"+false+"');SELECT SCOPE_IDENTITY();";
                int retVal = ExecuteCRUDByQuery(sQry);          
            return retVal;
        }
        public int Update(List<tblUserGame> userGamedata)
        {
            int retVal = 0;
            for (int i = 0; i < userGamedata.Count; i++)
            {
                string sQry = "UPDATE [tblUserGame] SET [IsConfirmed]='" + userGamedata[i].IsConfirmed + "',[IsDeleted]='" + userGamedata[i].IsDeleted + "' WHERE [GameID]=" + userGamedata[i].GameID + " AND [GameValueID]=" + userGamedata[i].GameValueID + " AND [UserID]=" + userGamedata[i].UserID;
                retVal = ExecuteUpdateQuery(sQry); 
            }
            return retVal;
        }
        public int Update(int gameId, tblGames gameInfo)
        {
            string sQry = "UPDATE [tblGames] SET [IsGameStart]='" + gameInfo.IsGameStart + "',[IsGamePause]='" + gameInfo.IsGamePause + "',[IsGameFinish]='" + gameInfo.IsGameFinish + "' WHERE [GameID]=" + gameId;
            int retVal = ExecuteUpdateQuery(sQry);
            return retVal;
        }

        public tblUserGame GetById(int gameID)
        {
            return Find(gameID);
        }

        public tblUserGame Find(int gameID)
        {
            tblUserGame userGameData = new tblUserGame();
            string sQry = "SELECT * FROM [tblUserGame] WHERE [GameID]=" + gameID;
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                DataRow dr = dtUserGameInfo.Rows[0];
                userGameData = GetGameInfoByRow(dr);
                
            }
            return userGameData;
        }

        public int[] GetAll(int? gameID, int? userID)
        {
            int[] userGameInfos = null;
            string sQry = "SELECT * FROM [tblUserGame] WHERE [GameID]=" + gameID + "AND [UserID]=" + userID;
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                userGameInfos = dtUserGameInfo.Rows.OfType<DataRow>().Select(k => Convert.ToInt32(k[2].ToString())).ToArray();
            }
            return userGameInfos;
        }
        public int[] GetAllValue(int gameID)
        {
            int[] userGameInfos = null;
            string sQry = "SELECT * FROM [tblUserGame] WHERE [GameID]=" + gameID;
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                userGameInfos = dtUserGameInfo.Rows.OfType<DataRow>().Select(k => Convert.ToInt32(k[2].ToString())).ToArray();
               
            }
            return userGameInfos;
        }
        #endregion

        #region Functions to Convert Data Row to Class

        private tblUserGame GetGameInfoByRow(DataRow dr)
        {
            tblUserGame userGame = new tblUserGame();
            userGame.GameID = Convert.ToInt32(dr["GameID"]);
            userGame.UserID = Convert.ToInt32(dr["UserID"]);
            userGame.GameValueID = Convert.ToInt32(dr["GameValueID"]);
            userGame.IsConfirmed = Convert.ToBoolean(dr["IsConfirmed"]);
            return userGame;
        }

        #endregion

        #region Supporting Functions

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

        #endregion

    }
}
