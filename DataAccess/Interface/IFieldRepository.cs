using DataAccess.Model;

namespace DataAccess.Interface
{
    public interface IFieldRepository
    {
        Task<List<FieldHomeModel>> GetFieldHomeData();
    }
}
