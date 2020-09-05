using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Services
{
    public interface IUserGameService
    {
        IEnumerable<tblGameValues> GetGameValues();
        int Add(tblUserGame userGameInfo);
        int[] GetAll(int? gameID,int? userID);
        int[] GetAllValue(int gameID);
        tblUserGame GetById(int gameID);
        tblUserGame Find(int gameID);
        int Update(List<tblUserGame> userGamedata);
        int Update(int gameId, tblGames gameInfo);
    }
}
