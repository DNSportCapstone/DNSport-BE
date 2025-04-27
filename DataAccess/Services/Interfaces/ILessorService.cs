using DataAccess.Model;

namespace DataAccess.Services.Interfaces
{
    public interface ILessorService
    {
        Task<int> CreateLessorContact(LessorContactModel lessorContact);
        Task<int> AcceptLessorContact(int id);
        Task<List<LessorContactModel>> GetAllLessorContact();
        Task<int> RejectLessorContact(int id);
    }
}