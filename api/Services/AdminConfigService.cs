using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.helper;
using System.Reflection;

namespace api.Services
{
    public class AdminConfigService : IAdminConfigService
    {
        private readonly AppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;

        public AdminConfigService(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionStrings)
        {
            _appSettings = appSettings.Value;
            _connectionStrings = connectionStrings.Value;
        }
        public int Add(tblAdminConfig configInfo)
        {

            string sQry = "INSERT INTO [tblAdminConfig] ([Coinprice],[confirmseconds]) " +
                "VALUES('" + configInfo.Coinprice + "','" + configInfo.confirmseconds + "')";
            int retVal=ExecuteCRUDByQuery(sQry);
            return retVal;
        }
        public int Update(int Adminid, tblAdminConfig configInfo)
        {
            string sQry = "UPDATE [tblAdminConfig] SET [Coinprice]='" + configInfo.Coinprice + "',[confirmseconds]='" + configInfo.confirmseconds + "' WHERE [Adminid]=" + Adminid;
            int retVal = ExecuteUpdateQuery(sQry);
            return retVal;
        }

        
   

        public tblAdminConfig GetById(int id)
        {
            return Find(id);
        }


        public tblAdminConfig Find(int id)
        {
            tblAdminConfig configInfo = null;
            string sQry = "SELECT * FROM [tblAdminConfig] WHERE [Adminid]=" + id;
            DataTable dtConfigInfo = ExecuteQuery(sQry);
            if (dtConfigInfo != null)
            {
                DataRow dr = dtConfigInfo.Rows[0];
                configInfo = GetPlaceInfoByRow(dr);
            }
            return configInfo;
        }

      

        public IEnumerable<tblAdminConfig> GetAll()
        {
            List<tblAdminConfig> configInfo = null;
            string sQry = "SELECT * FROM [tblAdminConfig]";
            DataTable dtConfigInfo = ExecuteQuery(sQry);
            if (dtConfigInfo != null)
            {
                configInfo = (from DataRow dr in dtConfigInfo.Rows
                               select new tblAdminConfig()
                               {
                                   Adminid = Convert.ToInt32(dr["Adminid"]),
                                   Coinprice = Convert.ToInt32(dr["Coinprice"]),
                                   confirmseconds = Convert.ToInt32(dr["confirmseconds"]),
            
                               }).ToList();
            }
            return configInfo;
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

        private tblAdminConfig GetPlaceInfoByRow(DataRow dr)
        {
            tblAdminConfig configInfo = new tblAdminConfig();
            configInfo.Coinprice = Convert.ToInt32(dr["Coinprice"]);
            configInfo.confirmseconds = Convert.ToInt32(dr["confirmseconds"]);
            return configInfo;
        }

    }
}
