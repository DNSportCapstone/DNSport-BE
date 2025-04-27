using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;

namespace DataAccess.Services.Implement
{
    public class LessorService : ILessorService
    {
        private readonly ILessorRepository _lessorRepository;
        public LessorService(ILessorRepository lessorRepository)
        {
            _lessorRepository = lessorRepository;
        }

        public async Task<int> CreateLessorContact(LessorContactModel lessorContact)
        {
            return await _lessorRepository.CreateLessorContact(lessorContact);
        }

        public async Task<int> AcceptLessorContact(int id)
        {
            return await _lessorRepository.AcceptLessorContact(id);
        }
        public async Task<List<LessorContactModel>> GetAllLessorContact()
        {
            return await _lessorRepository.GetAllLessorContact();
        }

        public async Task<int> RejectLessorContact(int id)
        {
            return await _lessorRepository.RejectLessorContact(id);
        }
    }
}
