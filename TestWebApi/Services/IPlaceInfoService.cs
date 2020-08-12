using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebApi.Models;

namespace TestWebApi.Services
{
    public interface IPlaceInfoService
    {
        int Add(PlaceInfo placeInfo);
        int AddRange(IEnumerable<PlaceInfo> places);
        IEnumerable<PlaceInfo> GetAll();
        PlaceInfo Find(int id);
        int Remove(int id);
        int Update(PlaceInfo placeInfo);
    }
}
