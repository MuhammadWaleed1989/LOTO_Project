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
    public class UserInfoService : IUserInfoService
    {
        public int Add(tblUser userInfo)
        {

            var saltedpassword = helper.Helper.ComputeHash(userInfo.Password, "SHA512", null);
            string sQry = "INSERT INTO [tblUser] ([UserName],[Email],[FirstName],[LastName],[Password],[IsDeleted],[IsAdmin],[IsUserOnline]) " +
                "VALUES('" + userInfo.UserName + "','" + userInfo.Email + "','" + userInfo.FirstName + "','" + 
                userInfo.LastName + "','" + saltedpassword + "','" + false + "','" + false + "','" + false + "')";
            int retVal=ExecuteCRUDByQuery(sQry);
            return retVal;
        }
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<tblUser> _users = new List<tblUser>
        {
            new tblUser { UserID = 1, FirstName = "Test", LastName = "User", UserName = "test", Password = "test" }
        };

        private readonly AppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;

        public UserInfoService(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionStrings)
        {
            _appSettings = appSettings.Value;
            _connectionStrings = connectionStrings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {

            tblUser _user = new tblUser();
            _user = FindUserByName(model);

            // return null if user not found
            if (_user == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(_user);

            return new AuthenticateResponse(_user, token);

        }

        //public IEnumerable<tblUser> GetAll()
        //{
        //    return _users;
        //}

        public tblUser GetById(int id)
        {
            return _users.FirstOrDefault(x => x.UserID == id);
        }

        // helper methods

        private string generateJwtToken(tblUser user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserID.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        //public int AddRange(IEnumerable<PlaceInfo> places)
        //{
        //    string sQry = "INSERT INTO [BillGatesPlaceInfo] ([Place],[About],[City],[State],[Country]) VALUES";
        //    string sVal = "";
        //    foreach(var place in places)            
        //      sVal+= "('" + place.Place + "','" + place.About + "','" + place.City + "','" + place.State + "','" + place.Country + "'),";
        //    sVal = sVal.TrimEnd(',');
        //    sQry = sQry + sVal;
        //    int retVal=ExecuteCRUDByQuery(sQry);
        //    return retVal;
        //}

        public tblUser Find(int id)
        {
            tblUser userInfo = null;
            string sQry = "SELECT * FROM [tblUser] WHERE [UserID]=" + id;
            DataTable dtPlaceInfo = ExecuteQuery(sQry);
            if (dtPlaceInfo != null)
            {
                DataRow dr = dtPlaceInfo.Rows[0];
                userInfo = GetPlaceInfoByRow(dr);
            }
            return userInfo;
        }

        public tblUser FindUserByName(AuthenticateRequest model)
        {
            tblUser userInfo = null;
            
            string sQry = "SELECT * FROM [tblUser] WHERE [email]='" + model.Username + "'";
            DataTable dtUserInfo = ExecuteQuery(sQry);
            if (dtUserInfo != null)
            {
                DataRow dr = dtUserInfo.Rows[0];
                userInfo = GetPlaceInfoByRow(dr);
            }
            bool saltedpassword = false;
            if (userInfo != null) {
                saltedpassword = helper.Helper.VerifyHash(model.Password, "SHA512", userInfo.Password);
                UpdateOnlineStatus(userInfo.UserID, true);
            }
            if (!saltedpassword) { userInfo = null; }
            return userInfo;
        }
        public int UpdateOnlineStatus(int userID, bool isOnline)
        {
            string sQry = "UPDATE [tblUser] SET [IsUserOnline]='" + isOnline + "' WHERE [UserID]=" + userID;
            int retVal = ExecuteCRUDByQuery(sQry);
            return retVal;
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
        //public int Remove(int id)
        //{
        //    string sQry = "DELETE FROM [BillGatesPlaceInfo] WHERE [Id]=" + id;
        //    int retVal=ExecuteCRUDByQuery(sQry);
        //    return retVal;
        //}

        //public int Update(PlaceInfo placeInfo)
        //{
        //    string sQry = "UPDATE [BillGatesPlaceInfo] SET [Place]='" + placeInfo.Place + "',[About]='" + placeInfo.About + "',[City]='" + placeInfo.City + "',[State]='" + placeInfo.State + "',[Country]='" + placeInfo.Country + "' WHERE [Id]=" + placeInfo.Id;
        //    int retVal=ExecuteCRUDByQuery(sQry);
        //    return retVal;            
        //}


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
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
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

        private tblUser GetPlaceInfoByRow(DataRow dr)
        {
            tblUser userInfo = new tblUser();
            userInfo.UserID = Convert.ToInt32(dr["UserID"]);
            userInfo.UserName = dr["UserName"].ToString();
            userInfo.FirstName = dr["FirstName"].ToString();
            userInfo.LastName = dr["LastName"].ToString();
            userInfo.Email = dr["Email"].ToString();
            userInfo.Password = dr["Password"].ToString();
            userInfo.IsAdmin = Convert.ToBoolean(dr["IsAdmin"].ToString());
            return userInfo;
        }

    }
}
