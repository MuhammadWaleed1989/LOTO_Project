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


namespace TestWebApi.Services
{
    public class UserInfoService : IUserInfoService
    {
        public int Add(tblUser userInfo, string connectionString)
        {

            var saltedpassword = helper.Helper.ComputeHash(userInfo.Password, "SHA512", null);
            string sQry = "INSERT INTO [tblUser] ([UserName],[Email],[FirstName],[LastName],[Password],[IsDeleted],[IsAdmin]) " +
                "VALUES('" + userInfo.UserName + "','" + userInfo.Email + "','" + userInfo.FirstName + "','" + 
                userInfo.LastName + "','" + saltedpassword + "','" + false + "','" + false + "')";
            int retVal=ExecuteCRUDByQuery(sQry, connectionString);
            return retVal;
        }
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<tblUser> _users = new List<tblUser>
        {
            new tblUser { UserID = 1, FirstName = "Test", LastName = "User", UserName = "test", Password = "test" }
        };

        private readonly AppSettings _appSettings;

        public UserInfoService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _users.SingleOrDefault(x => x.UserName == model.Username && x.Password == model.Password);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<tblUser> GetAll()
        {
            return _users;
        }

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

        //public PlaceInfo Find(int id)
        //{
        //    PlaceInfo placeInfo = null;
        //    string sQry = "SELECT * FROM [BillGatesPlaceInfo] WHERE [Id]="+id;
        //    DataTable dtPlaceInfo = ExecuteQuery(sQry);
        //    if (dtPlaceInfo != null)
        //    {
        //        DataRow dr = dtPlaceInfo.Rows[0];
        //        placeInfo = GetPlaceInfoByRow(dr);
        //    }
        //    return placeInfo;
        //}

        //public IEnumerable<PlaceInfo> GetAll()
        //{
        //    List<PlaceInfo> placeInfos = null;
        //    string sQry = "SELECT * FROM [BillGatesPlaceInfo]";
        //    DataTable dtPlaceInfo = ExecuteQuery(sQry);           
        //    if (dtPlaceInfo != null)
        //    {
        //        placeInfos = new List<PlaceInfo>();
        //        foreach (DataRow dr in dtPlaceInfo.Rows)
        //          placeInfos.Add(GetPlaceInfoByRow(dr));
        //    }
        //    return placeInfos;
        //}

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


        private int ExecuteCRUDByQuery(string strSql, string connectionString)
        {
            SqlConnection conn = null;
            int iR = 0;
            try
            {
                conn = new SqlConnection(connectionString);
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
        

    
    //private DataTable ExecuteQuery(string strSql)
    //{
    //    string sConStr = "Data Source=.\\SQLExpress;Initial Catalog=BillGatesMoney;Integrated Security=True";
    //    SqlConnection conn = null;
    //    DataTable dt = null;
    //    try
    //    {
    //        conn = new SqlConnection(sConStr);                
    //        SqlCommand cmd = new SqlCommand(strSql,conn);
    //        cmd.CommandType = CommandType.Text;
    //        SqlDataAdapter da = new SqlDataAdapter(cmd);
    //        conn.Open();
    //        dt = new DataTable();
    //        //Fill the dataset
    //        da.Fill(dt);
    //        if (!(dt.Rows.Count > 0)) dt = null;
    //    }
    //    catch { dt = null;  }
    //    finally { if (conn.State != 0) conn.Close(); }
    //    return dt;
    //}

    //private PlaceInfo GetPlaceInfoByRow(DataRow dr)
    //{
    //    PlaceInfo placeInfo = new PlaceInfo();
    //    placeInfo.Id = Convert.ToInt32(dr["Id"]);
    //    placeInfo.Place = dr["Place"].ToString();
    //    placeInfo.About = dr["About"].ToString();
    //    placeInfo.City = dr["City"].ToString();
    //    placeInfo.State = dr["State"].ToString();
    //    placeInfo.Country = dr["Country"].ToString();
    //    return placeInfo;
    //}

}
}
