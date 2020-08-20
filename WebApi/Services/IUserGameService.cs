using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IUserGameService
    {
        int Add(tblUserGame userGameInfo);
        IEnumerable<tblUserGame> GetAll();
        tblUserGame GetById(int id);
        tblUserGame Find(int id);
        int Update(int gameId, tblUserGame gameInfo);
    }
}
