using BusinessObject.Models;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Implement;
using DataAccess.Repositories.Interfaces;
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
        private readonly string GoMapsApiKey = "AlzaSyUfzCeRZ3dFPIhYaQkL1lEPICq4WRjGQPT";
        private const string GoMapsBaseUrl = "https://maps.gomaps.pro/maps/api";
        private readonly string RoadsBaseUrl = "https://roads.gomaps.pro/v1";
        private readonly HttpClient _httpClient;
        private readonly IStadiumRepository _stadiumRepository;

        public GoMapsService(IStadiumRepository stadiumRepository, HttpClient httpClient)
        {
            _stadiumRepository = stadiumRepository;
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
            var stadiums = await _stadiumRepository.GetStadiumData();

            var distanceTasks = stadiums.Select(stadium => GetDistanceMatrixAsync(userLocation, stadium.Address ?? string.Empty));
            var distanceResults = await Task.WhenAll(distanceTasks);

            var stadiumList = stadiums.Select((stadium, index) =>
            {
                var distanceInfo = JsonSerializer.Deserialize<DistanceMatrixResponse>(distanceResults[index]);
                var distanceText = distanceInfo?.Rows?.FirstOrDefault()?.Elements?.FirstOrDefault()?.Distance?.Text ?? "N/A";
                var duration = distanceInfo?.Rows?.FirstOrDefault()?.Elements?.FirstOrDefault()?.Duration?.Text ?? "N/A";

                double.TryParse(Regex.Match(distanceText, @"\d+(\.\d+)?").Value, out double distanceValue);

                return new StadiumDistanceModel
                {
                    StadiumId = stadium.StadiumId,
                    StadiumName = stadium.StadiumName ?? string.Empty,
                    Address = stadium.Address ?? string.Empty,
                    Image = stadium.Image ?? string.Empty,
                    Distance = distanceText,
                    Duration = duration,
                    DistanceValue = distanceValue
                };
            }).OrderBy(s => s.DistanceValue).ToList();

            return stadiumList;
        }
    }
}
