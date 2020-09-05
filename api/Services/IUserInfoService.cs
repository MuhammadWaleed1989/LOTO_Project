using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Services
{
    public interface IUserInfoService
    {
        int Add(tblUser userInfo);
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<tblUser> GetAll();
        tblUser GetById(int id);
        //int AddRange(IEnumerable<PlaceInfo> places);
        //IEnumerable<PlaceInfo> GetAll();
        tblUser Find(int id);
        int UpdateOnlineStatus(int userID, bool isOnline);
        //int Remove(int id);
        int Update(int userID, tblUser userInfo);
        int UpdateUserPassword(int userID, tblUser userInfo);
        
    }
}
