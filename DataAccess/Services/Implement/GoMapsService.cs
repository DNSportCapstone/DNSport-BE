using BusinessObject.Models;
using DataAccess.Model;
using DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.Services.Implement
{
    public class GoMapsService : IGoMapsService
    {
        private readonly string GoMapsApiKey = "AlzaSykNj3eIXCztTfYvjRIEk6n10fCPBkwwUqq";
        private const string GoMapsBaseUrl = "https://maps.gomaps.pro/maps/api";
        private readonly string RoadsBaseUrl = "https://roads.gomaps.pro/v1";
        private readonly HttpClient _httpClient;
        private readonly Db12353Context _dbcontext;

        public GoMapsService(Db12353Context context,HttpClient httpClient)
        {
            _dbcontext = context;
            _httpClient = httpClient;
        }

        //Lấy tọa độ từ địa chỉ (Geocoding API)
        public async Task<string> GetCoordinatesAsync(string address)
        {
            string encodedAddress = Uri.EscapeDataString(address);
            string requestUrl = $"{GoMapsBaseUrl}/geocode/json?address={encodedAddress}&key={GoMapsApiKey}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return $"Lỗi: {response.StatusCode}";
            }

            return await response.Content.ReadAsStringAsync();
        }

        //Tính khoảng cách giữa một điểm và nhiều điểm khác (Distance Matrix API)
        public async Task<string> GetDistanceMatrixAsync(string origin, string destinations)
        {
            string encodedOrigin = Uri.EscapeDataString(origin);
            string encodedDestinations = Uri.EscapeDataString(destinations);
            string requestUrl = $"{GoMapsBaseUrl}/distancematrix/json?origins={encodedOrigin}&destinations={encodedDestinations}&key={GoMapsApiKey}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return $"Lỗi: {response.StatusCode}";
            }

            return await response.Content.ReadAsStringAsync();
        }

        //Tìm con đường gần nhất với tập hợp tọa độ (Nearest Roads API)
        public async Task<string> GetNearestRoadAsync(string points)
        {
            string requestUrl = $"{RoadsBaseUrl}/nearestRoads?points={points}&key={GoMapsApiKey}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return $"Lỗi: {response.StatusCode}";
            }

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<List<StadiumDistanceModel>> GetNearbyStadiumsAsync(string userLocation)
        {
            var stadiums = await _dbcontext.Stadiums
                .Select(s => new
                {
                    s.StadiumId,
                    s.StadiumName,
                    s.Address,
                    s.Image
                })
                .ToListAsync();

            var stadiumList = new List<StadiumDistanceModel>();

            foreach (var stadium in stadiums)
            {
                var distanceData = await GetDistanceMatrixAsync(userLocation, stadium.Address);
                var distance = ExtractValue(distanceData, "\"text\" : \"(.*?) km\"");
                var duration = ExtractValue(distanceData, "\"text\" : \"(.*?) mins\"");

                stadiumList.Add(new StadiumDistanceModel
                {
                    StadiumId = stadium.StadiumId,
                    StadiumName = stadium.StadiumName,
                    Address = stadium.Address,
                    Image = stadium.Image,
                    Distance = distance + " km",
                    Duration = duration + " mins"
                });
            }

            return stadiumList;
        }

        // Hàm trích xuất dữ liệu từ chuỗi JSON (dùng Regex)
        private string ExtractValue(string response, string pattern)
        {
            var match = Regex.Match(response, pattern);
            return match.Success ? match.Groups[1].Value : "N/A";
        }



    }
}