﻿using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface IFieldRepository
    {
        Task<List<FieldHomeModel>> GetFieldHomeData();
        Task<List<Field>> GetAllFieldsAsync();
        Task<int> AddAsync(Field field);
        Task<Field?> GetFieldByIdAsync(int fieldId);
        Task UpdateFieldAsync(Field field);
        Task AddImagesToFieldAsync(int fieldId, List<string> imageUrls);
        Task UpdateFieldImagesAsync(int fieldId, List<string> imageUrls);
        Task<List<FieldModel>> GetFieldsByStadiumId(int stadiumId);
        Task<int> SetFieldStatus(FieldStatusRequest request);
        Task<User> GetFieldOwner(int fieldId);
        Task<Field> GetFieldsByBookingId(int bookingId);
        Task<List<FieldReportModel>> GetFieldReportList();
        Task<FieldModel> GetFieldById(int fieldId);
        Task<List<FieldModel>> GetActiveFields();
        Task<List<FieldModel>> GetActiveFieldsByStadiumId(int stadiumId);
    }
}
