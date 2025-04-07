using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
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
    }
}
