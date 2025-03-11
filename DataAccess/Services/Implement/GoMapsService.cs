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
        private readonly string GoMapsApiKey = "AlzaSykNj3eIXCztTfYvjRIEk6n10fCPBkwwUqq";
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

            var stadiumList = new List<StadiumDistanceModel>();

            foreach (var stadium in stadiums)
            {
                var distanceData = await GetDistanceMatrixAsync(userLocation, stadium.Address);
                var distanceInfo = JsonSerializer.Deserialize<DistanceMatrixResponse>(distanceData);

                var distanceText = distanceInfo?.Rows?.FirstOrDefault()?.Elements?.FirstOrDefault()?.Distance?.Text ?? "N/A";
                var duration = distanceInfo?.Rows?.FirstOrDefault()?.Elements?.FirstOrDefault()?.Duration?.Text ?? "N/A";

                // Chuyển distance từ string (ví dụ: "5.4 km") sang số (5.4)
                double distanceValue = 0;
                if (distanceText != "N/A")
                {
                    double.TryParse(Regex.Match(distanceText, @"\d+(\.\d+)?").Value, out distanceValue);
                }

                stadiumList.Add(new StadiumDistanceModel
                {
                    StadiumId = stadium.StadiumId,
                    StadiumName = stadium.StadiumName,
                    Address = stadium.Address,
                    Image = stadium.Image,
                    Distance = distanceText,
                    Duration = duration,
                    DistanceValue = distanceValue // Thêm thuộc tính DistanceValue để sắp xếp
                });
            }

            // Sắp xếp theo DistanceValue từ nhỏ tới lớn
            return stadiumList.OrderBy(s => s.DistanceValue).ToList();
        }


    }
}
