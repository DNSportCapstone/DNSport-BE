using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using System.Threading.Tasks;

namespace DataAccess.Services.Interfaces
{
    public interface IFieldService
    {
        Task<List<GetFieldResponse>> GetAllFieldsAsync();
        Task<RegisterFieldResponse> RegisterFieldAsync(RegisterFieldRequest request);
        Task<UpdateFieldResponse> EditFieldAsync(EditFieldRequest request);
        Task<List<FieldModel>> GetFieldsByStadiumId(int stadiumId);
    }
}
