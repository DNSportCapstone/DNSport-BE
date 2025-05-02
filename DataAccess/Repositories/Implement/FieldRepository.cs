using AutoMapper;
using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implement
{
    public class FieldRepository : IFieldRepository
    {
        private readonly Db12353Context _dbcontext = new();
        private readonly IMapper _mapper;
        public FieldRepository(Db12353Context dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
        }
        public async Task<List<FieldHomeModel>> GetFieldHomeData()
        {
            var result = await (from f in _dbcontext.Fields
                                join stadium in _dbcontext.Stadiums on f.StadiumId equals stadium.StadiumId
                                join s in _dbcontext.Sports on f.SportId equals s.SportId
                                join i in _dbcontext.Images on f.FieldId equals i.FieldId
                                select new FieldHomeModel
                                {
                                    FieldId = f.FieldId,
                                    FieldName = stadium.StadiumName ?? string.Empty,
                                    SportName = s.SportName ?? string.Empty,
                                    ImagePath = i.Url ?? string.Empty
                                }).AsNoTracking().ToListAsync();
            return result;
        }
        //Get All Field 
        public async Task<List<Field>> GetAllFieldsAsync()
        {
            return await _dbcontext.Fields
                .Include(f => f.Images) // Load danh sách ảnh của sân
                .ToListAsync();
        }
        //Register New Field
        public async Task<int> AddAsync(Field field)
        {
            await _dbcontext.Fields.AddAsync(field);
            await _dbcontext.SaveChangesAsync();
            return field.FieldId;
        }
        public async Task<Field?> GetFieldByIdAsync(int fieldId)
        {
            return await _dbcontext.Fields.Include(f => f.Stadium).Include(f => f.Images).FirstOrDefaultAsync(f => f.FieldId == fieldId);
        }
        //Update New Field 
        public async Task UpdateFieldAsync(Field field)
        {
            _dbcontext.Fields.Update(field);
            await _dbcontext.SaveChangesAsync();
        }
        public async Task AddImagesToFieldAsync(int fieldId, List<string> imageUrls)
        {
            var images = imageUrls.Select(url => new Image
            {
                FieldId = fieldId,
                Url = url,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7)
            }).ToList();

            await _dbcontext.Images.AddRangeAsync(images);
            await _dbcontext.SaveChangesAsync();
        }
        public async Task UpdateFieldImagesAsync(int fieldId, List<string> imageUrls)
        {
            var existingImages = _dbcontext.Images.Where(img => img.FieldId == fieldId);
            _dbcontext.Images.RemoveRange(existingImages); // Xóa ảnh cũ

            await AddImagesToFieldAsync(fieldId, imageUrls); // Thêm ảnh mới
        }

        public async Task<List<FieldModel>> GetFieldsByStadiumId(int stadiumId)
        {
            try
            {
                var fields = await _dbcontext.Fields
                    .Where(f => f.StadiumId == stadiumId)
                    .Select(f => new FieldModel
                    {
                        FieldId = f.FieldId,
                        StadiumName = f.Stadium.StadiumName,
                        FieldName = f.FieldName,
                        Description = f.Description ?? string.Empty,
                        DayPrice = f.DayPrice ?? 0,
                        NightPrice = f.NightPrice ?? 0,
                        MaximumPeople = f.MaximumPeople ?? 0,
                        Status = f.Status,

                        Thumbnail = f.Images
                                     .OrderBy(img => img.ImageId)
                                     .Select(img => img.Url)
                                     .FirstOrDefault(),

                        BookingFields = f.BookingFields.Select(bf => new BookingFieldModel
                        {
                            BookingFieldId = bf.BookingFieldId,
                            StartTime = bf.StartTime,
                            EndTime = bf.EndTime,
                            Price = bf.Price,
                            Date = bf.Date
                        }).ToList()
                    })
                    .ToListAsync();

                return fields;
            }
            catch
            {
                return new List<FieldModel>();
            }
        }


        public async Task<int> SetFieldStatus(FieldStatusRequest request)
        {
            var field = await _dbcontext.Fields.FirstOrDefaultAsync(f => f.FieldId == request.FieldId);
            if (field == null)
            {
                return 0;
            }
            field.Status = request.Status;
            await _dbcontext.SaveChangesAsync();
            return 1;
        }

        public async Task<User> GetFieldOwner(int fieldId)
        {
            var user = await _dbcontext.Fields
                .Where(f => f.FieldId == fieldId)
                .Select(f => f.Stadium.User)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<Field> GetFieldsByBookingId(int bookingId)
        {
            var field = await _dbcontext.BookingFields
                .Where(bf => bf.BookingId == bookingId)
                .Select(bf => bf.Field)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return field;
        }

        public async Task<List<FieldReportModel>> GetFieldReportList()
        {
            var fieldReportModels = await (from f in _dbcontext.Fields
                                           join s in _dbcontext.Stadiums on f.StadiumId equals s.StadiumId into stadiumGroup
                                           from stadium in stadiumGroup.DefaultIfEmpty()
                                           join u in _dbcontext.Users on stadium.UserId equals u.UserId into userGroup
                                           from user in userGroup.DefaultIfEmpty()
                                           join ud in _dbcontext.UserDetails on user.UserId equals ud.UserId into userDetailGroup
                                           from userDetail in userDetailGroup.DefaultIfEmpty()
                                           select new FieldReportModel
                                           {
                                               FieldId = f.FieldId,
                                               FieldName = f.Description ?? string.Empty,
                                               StadiumName = stadium.StadiumName ?? string.Empty,
                                               OwnerEmail = user.Email ?? string.Empty,
                                               OwnerName = userDetail.FullName ?? string.Empty,
                                               IsActive = f.Status == "Active" ? true : false,
                                               ViolationCount = (
                                                   from bf in _dbcontext.BookingFields
                                                   join b in _dbcontext.Bookings on bf.BookingId equals b.BookingId
                                                   join d in _dbcontext.Denounces on b.BookingId equals d.BookingId
                                                   where bf.FieldId == f.FieldId
                                                   select d
                                               ).Count()
                                           }
                                        ).ToListAsync();

            return fieldReportModels;
        }


        public async Task<FieldModel> GetFieldById(int fieldId)
        {
            try
            {
                var fields = await _dbcontext.Fields
                            .Where(f => f.FieldId == fieldId)
                            .Select(f => new FieldModel
                            {
                                FieldId = f.FieldId,
                                Description = f.Description ?? string.Empty,
                                DayPrice = f.DayPrice ?? 0,
                                NightPrice = f.NightPrice ?? 0,
                                Images = f.Images.Select(i => new ImageModel
                                {
                                    ImageId = i.ImageId,
                                    FieldId = i.FieldId ?? 0,
                                    Url = i.Url ?? string.Empty,
                                }).ToList(),
                                BookingFields = f.BookingFields.Select(bf => new BookingFieldModel
                                {
                                    BookingFieldId = bf.BookingFieldId,
                                    StartTime = bf.StartTime,
                                    EndTime = bf.EndTime,
                                    Price = bf.Price,
                                    Date = bf.Date
                                }).ToList(),
                                Sport = new SportModel
                                {
                                    SportId = f.SportId ?? 0,
                                    SportName = f.Sport != null ? f.Sport.SportName : string.Empty,
                                },
                                Stadium = new StadiumModel
                                {
                                    StadiumName = f.Stadium != null ? f.Stadium.StadiumName : string.Empty,
                                    Address = f.Stadium != null ? f.Stadium.Address : string.Empty,
                                }
                            })
                            .FirstOrDefaultAsync();
                return fields;
            }
            catch
            {
                return new FieldModel();
            }
        }
    }
}
