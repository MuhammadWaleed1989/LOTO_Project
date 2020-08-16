using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TestWebApi.helper;
using TestWebApi.Models;
using System.Data;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;


namespace TestWebApi.Hubs
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
        //public override Task OnDisconnectedAsync(Exception exception)
        //{
        //    if (_groups.Any(g => g.UserName == Context.User))
        //    {
        //        var gs = _groups.Where(g => g.Owner == Context.ConnectionId).ToList();
        //        for (int i = 0; i < gs.Count; i++)
        //        {
        //            BroadcastGroup(gs[i], true);
        //            _groups.Remove(gs[i]);
        //        }
        //    }
        //    return base.OnDisconnectedAsync(exception);
        //}

        //public void CreateOrJoin(string key, string email)
        //{
        //    var group = _groups.FirstOrDefault(g => g.Key == key);
        //    if (group == null)
        //    {
        //        group = new DrinkingGroup { Key = key, Owner = Context.ConnectionId };
        //        _groups.Add(group);
        //    }

        //    if (group.HasFinished || group.HasStarted)
        //    {
        //        throw new Exception("You cannot join a group which has started or finished");
        //    }

        //    group.Glasses.Add(new Glass { ConnectionId = Context.ConnectionId, Email = email });

        //    BroadcastGroup(group);
        //}

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
        //    var group = _groups.FirstOrDefault(g => !g.HasFinished && g.HasStarted && g.Glasses.Any(gl => gl.ConnectionId == Context.ConnectionId));
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
        //        BroadcastGroup(group);
        //        if (group.HasFinished)
        //        {
        //            _groups.Remove(group);
        //        }
        //    }
        //}

        private void BroadcastGroup()
        {
            //var clients = group.Glasses.Select(g => g.ConnectionId).ToList();
            Clients.All.SendAsync("UserList", GetAll());
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
    public class DrinkingGroup
    {
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
