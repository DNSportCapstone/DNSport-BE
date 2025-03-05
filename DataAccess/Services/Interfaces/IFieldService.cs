using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using System.Threading.Tasks;

namespace DataAccess.Services.Interfaces
{
    public interface IFieldService
    {
        Task<List<GetFieldResponse>> GetAllFieldsAsync();
        Task<int> RegisterFieldAsync(RegisterFieldRequest request);
        Task<EditFieldResponse> EditFieldAsync(EditFieldRequest request);
    }
}
