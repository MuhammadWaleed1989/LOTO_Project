using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TestWebApi.Models;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TestWebApi.helper;
using System.Reflection;

namespace TestWebApi.Services
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
        public int Add(tblGames gameInfo)
        {
            string sQry = "INSERT INTO [tblGames] ([GameName],[Cell1Value],[Cell2Value],[Cell3Value],[Cell4Value],[IsDeleted]) " +
                "VALUES('" + gameInfo.GameName + "','" + gameInfo.Cell1Value + "','" + gameInfo.Cell2Value + "','" +
                gameInfo.Cell3Value + "','" + gameInfo.Cell4Value + "','" + false + "')";
            int retVal=ExecuteCRUDByQuery(sQry);
            return retVal;
        }

        public tblGames GetById(int id)
        {
            return Find(id);
        }

        public tblGames Find(int id)
        {
            tblGames gameInfo = null;
            string sQry = "SELECT * FROM [tblGames] WHERE [GameID]=" + id;
            DataTable dtGameInfo = ExecuteQuery(sQry);
            if (dtGameInfo != null)
            {
                DataRow dr = dtGameInfo.Rows[0];
                gameInfo = GetPlaceInfoByRow(dr);
            }
            return gameInfo;
        }

        public int Update(int gameId, tblGames gameInfo)
        {
            string sQry = "UPDATE [tblGames] SET [GameName]='" + gameInfo.GameName + "',[Cell1Value]='" + gameInfo.Cell1Value + "',[Cell2Value]='" + gameInfo.Cell2Value + "',[Cell3Value]='" + gameInfo.Cell3Value + "',[Cell4Value]='" + gameInfo.Cell4Value + "' WHERE [GameID]=" + gameId;
            int retVal = ExecuteCRUDByQuery(sQry);
            return retVal;
        }

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
                                   Cell1Value = Convert.ToInt32(dr["Cell1Value"].ToString()),
                                   Cell2Value = Convert.ToInt32(dr["Cell2Value"].ToString()),
                                   Cell3Value = Convert.ToInt32(dr["Cell3Value"].ToString()),
                                   Cell4Value = Convert.ToInt32(dr["Cell4Value"].ToString())
                               }).ToList();
            }
            return gameInfos;
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

        private tblGames GetPlaceInfoByRow(DataRow dr)
        {
            tblGames gameInfo = new tblGames();
            gameInfo.GameID = Convert.ToInt32(dr["GameID"]);
            gameInfo.GameName = dr["GameName"].ToString();
            gameInfo.Cell1Value = Convert.ToInt32(dr["Cell1Value"].ToString());
            gameInfo.Cell2Value = Convert.ToInt32(dr["Cell2Value"].ToString());
            gameInfo.Cell3Value = Convert.ToInt32(dr["Cell3Value"].ToString());
            gameInfo.Cell4Value = Convert.ToInt32(dr["Cell4Value"].ToString());
            return gameInfo;
        }

    }
}
