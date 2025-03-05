using BusinessObject.Models;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataAccess.Repositories.Implement
{
    public class FieldRepository : IFieldRepository
    {
        private Db12353Context _dbcontext = new();
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
            return await _dbcontext.Fields.FindAsync(fieldId);
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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


    }
}
