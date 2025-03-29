using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Interfaces
{
    public interface IGoMapsService
    {
        Task<string> GetCoordinatesAsync(string address);
        Task<string> GetDistanceMatrixAsync(string origin, string destinations);
        Task<string> GetNearestRoadAsync(string points);
        Task<List<StadiumDistanceModel>> GetNearbyStadiumsAsync(string userLocation);
    }
}
