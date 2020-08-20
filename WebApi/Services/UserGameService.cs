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
        public int Add(tblUserGame userGamedata)
        {
            string sQry = "INSERT INTO [tblUserGame] ([UserID],[GameID],[Value1],[Value2],[Value3],[Value4],[Value5],[Value6]) " +
                "VALUES('" + userGamedata.UserID + "','" + userGamedata.GameID + "','" + userGamedata.Value1 + "','" +
                userGamedata.Value2 + "','" + userGamedata.Value3 + "','" + userGamedata.Value4 + "','" + userGamedata.Value5 + "','" + userGamedata.Value6 + "');SELECT SCOPE_IDENTITY();";
            int retVal = ExecuteCRUDByQuery(sQry);
            //retVal = AddGameDetails(gamedata.gameDetail, retVal);
            return retVal;
        }

        public tblUserGame GetById(int id)
        {
            return Find(id);
        }

        public tblUserGame Find(int id)
        {
            tblUserGame userGameData = new tblUserGame();
            string sQry = "SELECT * FROM [tblUserGame] WHERE [GameID]=" + id;
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                DataRow dr = dtUserGameInfo.Rows[0];
                userGameData = GetGameInfoByRow(dr);
                
            }
            return userGameData;
        }

        public int Update(int gameId, tblUserGame userGameInfo)
        {
            string sQry = "UPDATE [tblUserGame] SET [Value1]='" + userGameInfo.Value1 + "',[Value2]='" + userGameInfo.Value2 + "',[Value3]='" + userGameInfo.Value3 + "',[Value4]='" + userGameInfo.Value4 + "',[Value5]='" + userGameInfo.Value5 + "',[Value6]='" + userGameInfo.Value6 + "' WHERE [GameID]=" + gameId;
            int retVal = ExecuteCRUDByQuery(sQry);
            return retVal;
        }

        public IEnumerable<tblUserGame> GetAll()
        {
            List<tblUserGame> userGameInfos = null;
            string sQry = "SELECT * FROM [tblUserGame]";
            DataTable dtUserGameInfo = ExecuteQuery(sQry);
            if (dtUserGameInfo != null)
            {
                userGameInfos = (from DataRow dr in dtUserGameInfo.Rows
                             select new tblUserGame()
                             {
                                 GameID = Convert.ToInt32(dr["GameID"]),
                                 UserID = Convert.ToInt32(dr["UserID"]),
                                 Value1 = Convert.ToInt32(dr["Value1"]),
                                 Value2 = Convert.ToInt32(dr["Value2"]),
                                 Value3 = Convert.ToInt32(dr["Value3"]),
                                 Value4 = Convert.ToInt32(dr["Value4"]),
                                 Value5 = Convert.ToInt32(dr["Value5"]),
                                 Value6 = Convert.ToInt32(dr["Value6"]),
                             }).ToList();
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
            userGame.Value1 = Convert.ToInt32(dr["Value1"]);
            userGame.Value2 = Convert.ToInt32(dr["Value2"]);
            userGame.Value3 = Convert.ToInt32(dr["Value3"]);
            userGame.Value4 = Convert.ToInt32(dr["Value4"]);
            userGame.Value5 = Convert.ToInt32(dr["Value5"]);
            userGame.Value6 = Convert.ToInt32(dr["Value6"]);
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
