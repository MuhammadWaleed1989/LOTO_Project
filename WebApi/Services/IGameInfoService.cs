using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IGameInfoService
    {
        int Add(tblGames gameInfo);
        IEnumerable<tblGames> GetAll();
        tblGames GetById(int id);

        tblGames Find(int id);
        int Update(int gameId, tblGames gameInfo);
        //int Remove(int id);
        //int Update(PlaceInfo placeInfo);
    }
}
