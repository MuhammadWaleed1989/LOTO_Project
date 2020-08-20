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
        public int Add(GameData gamedata)
        {
            string sQry = "INSERT INTO [tblGames] ([GameName],[WinValue1],[WinValue2],[WinValue3],[WinValue4],[WinValue5],[WinValue6],[IsDeleted]) " +
                "VALUES('" + gamedata.gameInfo.GameName + "','" + gamedata.gameInfo.WinValue1 + "','" + gamedata.gameInfo.WinValue2 + "','" +
                gamedata.gameInfo.WinValue3 + "','" + gamedata.gameInfo.WinValue4 + "','" + gamedata.gameInfo.WinValue5 + "','" + gamedata.gameInfo.WinValue6 + "','" + false + "');SELECT SCOPE_IDENTITY();";
            int retVal=ExecuteCRUDByQuery(sQry);
            retVal = AddGameDetails(gamedata.gameDetail, retVal);
            return retVal;
        }
        public int AddGameDetails(List<tblGamesDetail> gameDetail, int gameID)
        {
            var retValFinal = 0;
            for (int i = 0; i < gameDetail.Count; i++)
            {
                string sQry = "INSERT INTO [tblGamesDetail] ([GameID],[ValueOfRow1],[ValueOfRow2],[ValueOfRow3],[ValueOfRow4],[ValueOfRow5],[ValueOfRow6]) " +
                "VALUES('" + gameID + "','" + gameDetail[i].ValueOfRow1 + "','" + gameDetail[i].ValueOfRow2 + "','" +
                gameDetail[i].ValueOfRow3 + "','" + gameDetail[i].ValueOfRow4 + "','" + gameDetail[i].ValueOfRow5 + "','" + gameDetail[i].ValueOfRow6 + "');SELECT SCOPE_IDENTITY();";
                retValFinal = ExecuteCRUDByQuery(sQry);
            }
            return retValFinal;
        }

        public GameData GetById(int id)
        {
            return Find(id);
        }

        public GameData Find(int id)
        {
            GameData gameData = new GameData();
            string sQry = "SELECT * FROM [tblGames] WHERE [GameID]=" + id;
            DataTable dtGameInfo = ExecuteQuery(sQry);
            if (dtGameInfo != null)
            {
                DataRow dr = dtGameInfo.Rows[0];
                gameData.gameInfo = GetGameInfoByRow(dr);
                string gameDetailQry = "SELECT GD.* FROM tblGamesDetail GD JOIN [tblGames] G  ON GD.[GameID]= G.[GameID] WHERE GD.[GameID] =" + id + " AND ISNULL(G.IsDeleted,0)=0";
                List<tblGamesDetail> gameDetailsInfos = new List<tblGamesDetail>();
                DataTable dtGameDetail = ExecuteQuery(gameDetailQry);
                if (dtGameDetail != null)
                {
                    gameDetailsInfos = (from DataRow dRow in dtGameDetail.Rows
                                        select new tblGamesDetail()
                                        {
                                            GameDetailID = Convert.ToInt32(dRow["GameDetailID"]),
                                            GameID = Convert.ToInt32(dRow["GameID"]),
                                            ValueOfRow1 = Convert.ToInt32(dRow["ValueOfRow1"].ToString()),
                                            ValueOfRow2 = Convert.ToInt32(dRow["ValueOfRow2"].ToString()),
                                            ValueOfRow3 = Convert.ToInt32(dRow["ValueOfRow3"].ToString()),
                                            ValueOfRow4 = Convert.ToInt32(dRow["ValueOfRow4"].ToString()),
                                            ValueOfRow5 = Convert.ToInt32(dRow["ValueOfRow5"].ToString()),
                                            ValueOfRow6 = Convert.ToInt32(dRow["ValueOfRow6"].ToString())
                                        }).ToList();
                }
                gameData.gameDetail = gameDetailsInfos;
            }
            return gameData;
        }

        //public int Update(int gameId, tblGames gameInfo)
        //{
        //    string sQry = "UPDATE [tblGames] SET [GameName]='" + gameInfo.GameName + "',[Cell1Value]='" + gameInfo.Cell1Value + "',[Cell2Value]='" + gameInfo.Cell2Value + "',[Cell3Value]='" + gameInfo.Cell3Value + "',[Cell4Value]='" + gameInfo.Cell4Value + "' WHERE [GameID]=" + gameId;
        //    int retVal = ExecuteCRUDByQuery(sQry);
        //    return retVal;
        //}

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

        private tblGames GetGameInfoByRow(DataRow dr)
        {
            tblGames gameInfo = new tblGames();
            gameInfo.GameID = Convert.ToInt32(dr["GameID"]);
            gameInfo.GameName = dr["GameName"].ToString();
            gameInfo.WinValue1 = Convert.ToInt32(dr["WinValue1"].ToString());
            gameInfo.WinValue2 = Convert.ToInt32(dr["WinValue2"].ToString());
            gameInfo.WinValue3 = Convert.ToInt32(dr["WinValue3"].ToString());
            gameInfo.WinValue4 = Convert.ToInt32(dr["WinValue4"].ToString());
            gameInfo.WinValue5 = Convert.ToInt32(dr["WinValue5"].ToString());
            gameInfo.WinValue6 = Convert.ToInt32(dr["WinValue6"].ToString());
            gameInfo.IsDeleted = Convert.ToBoolean(dr["IsDeleted"].ToString());
            return gameInfo;
        }

        private tblGamesDetail GetGameDetailInfoByRow(DataRow dr)
        {
            tblGamesDetail gameDetail = new tblGamesDetail();
            gameDetail.GameDetailID = Convert.ToInt32(dr["GameDetailID"]);
            gameDetail.GameID = Convert.ToInt32(dr["GameID"]);
            gameDetail.ValueOfRow1 = Convert.ToInt32(dr["ValueOfRow1"].ToString());
            gameDetail.ValueOfRow2 = Convert.ToInt32(dr["ValueOfRow2"].ToString());
            gameDetail.ValueOfRow3 = Convert.ToInt32(dr["ValueOfRow3"].ToString());
            gameDetail.ValueOfRow4 = Convert.ToInt32(dr["ValueOfRow4"].ToString());
            gameDetail.ValueOfRow5 = Convert.ToInt32(dr["ValueOfRow5"].ToString());
            gameDetail.ValueOfRow6 = Convert.ToInt32(dr["ValueOfRow6"].ToString());
            return gameDetail;
        }

    }
}
