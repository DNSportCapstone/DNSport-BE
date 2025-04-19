using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;

namespace DataAccess.Services.Implement
{
    public class FieldService : IFieldService
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly Db12353Context _dbcontext;
        private readonly IEmailSender _emailSender;

        public FieldService(Db12353Context dbcontext, IFieldRepository fieldRepository, IEmailSender emailSender)
        {
            _dbcontext = dbcontext;
            _fieldRepository = fieldRepository;
            _emailSender = emailSender;
        }
        public async Task<List<GetFieldResponse>> GetAllFieldsAsync()
        {
            var fields = await _fieldRepository.GetAllFieldsAsync();
            return fields.Where(f => f.Status == "Active").Select(f => new GetFieldResponse
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
        public async Task<GetFieldResponse> GetFieldByIdAsync(int fieldId)
        {
            var field = await _fieldRepository.GetFieldByIdAsync(fieldId);
            if (field == null)
            {
                return null; // Hoặc throw lỗi 404
            }

            return new GetFieldResponse
            {
                FieldId = field.FieldId,
                StadiumId = field.StadiumId ?? 0,
                SportId = field.SportId ?? 0,
                Description = field.Description,
                DayPrice = field.DayPrice ?? 0,
                NightPrice = field.NightPrice ?? 0,
                Status = field.Status,
                ImageUrls = field.Images?.Select(i => i.Url).ToList() ?? new List<string>()
            };
        }

        public async Task<RegisterFieldResponse> RegisterFieldAsync(RegisterFieldRequest request)
        {
            var field = new Field
            {
                SportId = request.SportId,
                StadiumId = request.StadiumId,
                Description = request.Description,
                DayPrice = request.DayPrice,
                NightPrice = request.NightPrice,
                Status = request.Status
            };

            await _fieldRepository.AddAsync(field);
            await _dbcontext.SaveChangesAsync();

            if (request.ImageUrls?.Any() == true)
            {
                await _fieldRepository.AddImagesToFieldAsync(field.FieldId, request.ImageUrls);
            }
            return new RegisterFieldResponse
            {
                FieldId = field.FieldId,
                Message = "Field registered successfully."
            };
        }

        public async Task<UpdateFieldResponse> EditFieldAsync(EditFieldRequest request)
        {
            var field = await _fieldRepository.GetFieldByIdAsync(request.FieldId);
            if (field == null)
            {
                return new UpdateFieldResponse { Message = "Field not found." };
            }

            field.SportId = request.SportId;
            field.StadiumId = request.StadiumId;
            field.Description = request.Description;
            field.DayPrice = request.DayPrice;
            field.NightPrice = request.NightPrice;
            field.Status = request.Status;

            await _fieldRepository.UpdateFieldAsync(field);
            if (request.ImageUrls?.Any() == true)
            {
                await _fieldRepository.AddImagesToFieldAsync(field.FieldId, request.ImageUrls);
            }          
            return new UpdateFieldResponse
            {
                FieldId = field.FieldId,
                Message = "Field updated successfully."
            };
        }

        public async Task<List<FieldModel>> GetFieldsByStadiumId(int stadiumId)
        {
            return await _fieldRepository.GetFieldsByStadiumId(stadiumId);

        }

        public async Task<int> SetFieldStatus(FieldStatusRequest request)
        {
            var result = await _fieldRepository.SetFieldStatus(request);
            if (request.Status == "disable")
            {
                var owner = await _fieldRepository.GetFieldOwner(request.FieldId);
                var field = await _fieldRepository.GetFieldByIdAsync(request.FieldId);
                await _emailSender.SendEmailAsync(owner.Email, $"Thông báo về việc tạm thời vô hiệu hóa {field.Description}",
                                                  $@"
                                                    <p>Chào bạn,</p>

                                                    <p>Chúng tôi rất tiếc khi phải thông báo rằng <strong>{field.Description}</strong> của bạn đã tạm thời bị đình chỉ hoạt động do nhận được nhiều phản ánh từ người dùng.</p>

                                                    <p>Hiện tại, chúng tôi đang tiến hành kiểm tra và xác minh các phản ánh này để đảm bảo tính minh bạch và công bằng.</p>

                                                    <p>Trong thời gian này, bạn sẽ không thể sử dụng chức năng liên quan đến <strong>{field.Description}</strong>. Chúng tôi sẽ liên hệ lại với bạn ngay khi quá trình kiểm tra hoàn tất hoặc khi cần thêm thông tin.</p>

                                                    <p>Nếu bạn có bất kỳ thắc mắc nào hoặc cần hỗ trợ thêm, vui lòng phản hồi email này hoặc liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>

                                                    <p>Trân trọng,<br>
                                                    <em>Đội ngũ hỗ trợ</em></p>
                                                  ");
            }
            return result;
        }
        public async Task<FieldModel> GetFieldById(int fieldId)
        {
            return await _fieldRepository.GetFieldById(fieldId);

        }
    }
}
