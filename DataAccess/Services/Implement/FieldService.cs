using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using System.Threading.Tasks;

namespace DataAccess.Services.Implement
{
    public class FieldService : IFieldService
    {
        private readonly IFieldRepository _fieldRepository;

        public FieldService(IFieldRepository fieldRepository)
        {
            _fieldRepository = fieldRepository;
        }
        public async Task<List<GetFieldResponse>> GetAllFieldsAsync()
        {
            var fields = await _fieldRepository.GetAllFieldsAsync();
            return fields.Select(f => new GetFieldResponse
            {
                FieldId = f.FieldId,
                StadiumId = f.StadiumId ?? 0, 
                SportId = f.SportId ?? 0,     
                Description = f.Description,
                DayPrice = f.DayPrice ?? 0,   
                NightPrice = f.NightPrice ?? 0,
                ImageUrls = f.Images.Select(i => i.Url).ToList()
            }).ToList();
        }

        public async Task<int> RegisterFieldAsync(RegisterFieldRequest request)
        {
            var field = new Field
            {
                StadiumId = request.StadiumId,
                SportId = request.SportId,
                Description = request.Description,
                DayPrice = request.DayPrice,
                NightPrice = request.NightPrice,
                Status = request.Status
            };

            return await _fieldRepository.AddAsync(field);
        }
        public async Task<EditFieldResponse> EditFieldAsync(EditFieldRequest request)
        {
            var field = await _fieldRepository.GetFieldByIdAsync(request.FieldId);
            if (field == null)
            {
                throw new KeyNotFoundException("Field not found.");
            }

            // Cập nhật thông tin chung
            field.Description = request.Description;
            field.DayPrice = request.DayPrice ?? 0;   // Ép kiểu từ nullable decimal? -> decimal
            field.NightPrice = request.NightPrice ?? 0;
            // Chỉ Admin mới có thể thay đổi trạng thái
            //if (userRole == "Admin" && !string.IsNullOrEmpty(request.Status))
            //{
            //    field.Status = request.Status;
            //}

            // Xử lý upload ảnh (nếu có)
            //if (request.Image != null)
            //{
            //    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/fields");
            //    Directory.CreateDirectory(uploadsFolder);
            //    string uniqueFileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
            //    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            //
            //    using (var fileStream = new FileStream(filePath, FileMode.Create))
            //   {
            //        await request.Image.CopyToAsync(fileStream);
            //    }

            //    field.ImageUrl = $"/images/fields/{uniqueFileName}";
            //}

            await _fieldRepository.UpdateFieldAsync(field);

            return new EditFieldResponse
            {
                FieldId = field.FieldId,
                Description = field.Description,
                DayPrice =  field.DayPrice ?? 0,
                NightPrice = field.NightPrice ?? 0,
                Status = field.Status,
                //ImageUrl = field.ImageUrl ?? string.Empty,
                Message = "Field updated successfully."
            };
        }

    }
}
