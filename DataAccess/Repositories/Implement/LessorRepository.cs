using BusinessObject.Models;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implement
{
    public class LessorRepository : ILessorRepository
    {
        private readonly Db12353Context _dbContext;
        private readonly IUserDetailRepository _userDetailRepository;
        public LessorRepository(Db12353Context dbContext, IUserDetailRepository userDetailRepository)
        {
            _dbContext = dbContext;
            _userDetailRepository = userDetailRepository;
        }

        public async Task<int> CreateLessorContact(LessorContactModel lessorContact)
        {
            var contact = new LessorContact
            {
                Email = lessorContact.Email,
                FullName = lessorContact.FullName,
                PhoneNumber = lessorContact.PhoneNumber,
                Address = lessorContact.Address,
                CreateAt = DateTime.Now,
                Status = "Pending"
            };
            await _dbContext.LessorContacts.AddAsync(contact);
            await _dbContext.SaveChangesAsync();
            return contact.ContactId;
        }

        public async Task<int> AcceptLessorContact(int id)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var contact = await _dbContext.LessorContacts.FirstOrDefaultAsync(x => x.ContactId == id);
                if (contact == null) throw new Exception("Contact not found");

                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == contact.Email);
                if (user is not null)
                {
                    user.RoleId = 2;
                }
                else
                {
                    var lessor = new User
                    {
                        Email = contact.Email,
                        RoleId = 2,
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow.AddHours(7)
                    };
                    await _dbContext.Users.AddAsync(lessor);
                    await _dbContext.SaveChangesAsync();

                    var lessorDetail = new UserDetailModel
                    {
                        UserId = lessor.UserId,
                        FullName = contact.FullName,
                        PhoneNumber = contact.PhoneNumber,
                        Address = contact.Address,
                    };
                    await _userDetailRepository.AddNewUserDetail(lessorDetail);
                }

                _dbContext.LessorContacts.Remove(contact);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return contact.ContactId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<int> RejectLessorContact(int id)
        {
            var contact = await _dbContext.LessorContacts.FirstOrDefaultAsync(x => x.ContactId == id);
            if (contact != null)
            {
                _dbContext.LessorContacts.Remove(contact);
                await _dbContext.SaveChangesAsync();
            }
            return contact.ContactId;
        }

        public async Task<List<LessorContactModel>> GetAllLessorContact()
        {
            var result = await _dbContext.LessorContacts.Where(c => c.Status == "Pending")
                                                        .Select(x => new LessorContactModel
                                                        {
                                                            ContactId = x.ContactId,
                                                            Email = x.Email,
                                                            FullName = x.FullName,
                                                            PhoneNumber = x.PhoneNumber,
                                                            Address = x.Address,
                                                            CreateAt = x.CreateAt,
                                                            Status = x.Status
                                                        }).AsNoTracking()
                                                          .ToListAsync();
            return result;
        }
    }
}
