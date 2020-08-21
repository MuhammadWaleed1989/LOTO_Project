using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IGameInfoService
    {
        int Add(GameData gameInfo);
        IEnumerable<tblGames> GetAll();
        GameData GetById(int id);

        GameData Find(int id);
        //int Update(int gameId, tblGames gameInfo);
        int Remove(int id);
        int UpdateWinner(tblUserGame winnerOfGame);
        //int Update(PlaceInfo placeInfo);
    }
}
