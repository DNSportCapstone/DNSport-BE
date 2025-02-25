using DataAccess.Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface IFieldRepository
    {
        Task<List<FieldHomeModel>> GetFieldHomeData();
    }
}
