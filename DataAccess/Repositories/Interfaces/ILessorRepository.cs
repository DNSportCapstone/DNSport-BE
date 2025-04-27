using DataAccess.Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface ILessorRepository
    {
        Task<int> CreateLessorContact(LessorContactModel lessorContact);
        Task<int> AcceptLessorContact(int id);
        Task<List<LessorContactModel>> GetAllLessorContact();
        Task<int> RejectLessorContact(int id);
    }
}
