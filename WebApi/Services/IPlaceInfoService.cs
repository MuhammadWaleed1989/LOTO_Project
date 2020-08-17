using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
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
