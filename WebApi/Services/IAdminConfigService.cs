using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IAdminConfigService
    {
        int Add(tblAdminConfig userInfo);
        IEnumerable<tblAdminConfig> GetAll();
        tblAdminConfig GetById(int id);

        tblAdminConfig Find(int id);
        int Update(int userID, tblAdminConfig userInfo);
        
    }
}
