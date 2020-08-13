using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebApi.Models;

namespace TestWebApi.Services
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
        //int Remove(int id);
        //int Update(PlaceInfo placeInfo);
    }
}
