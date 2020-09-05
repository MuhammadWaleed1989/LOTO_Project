using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Services
{
    public interface IGameInfoService
    {
        IEnumerable<tblGames> GetAll();
        int Add(tblGames gameInfo);
        int Update(int gameId, tblGames gameInfo);
        int UpdateWinnigValues(tblGames gameInfo);
        
        tblGames GetById(int id);
        tblGames Find(int id);
        int Remove(int id);
        int InsertUserOfGame(tblUserOfGame usersOfGame);

    }
}
