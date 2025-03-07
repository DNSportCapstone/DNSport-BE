using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumController : ControllerBase
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IStadiumRepository _stadiumService;
        private readonly Cloudinary _cloudinary;

        public StadiumController(
            IFieldRepository fieldRepository,
            IStadiumRepository stadiumService,
            IOptions<CloudinarySettings> config)
        {
            _fieldRepository = fieldRepository;
            _stadiumService = stadiumService;
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet]
        public async Task<IActionResult> GetStadiumData()
        {
            var result = await _stadiumService.GetStadiumData();
            return Ok(result);
        }

        [HttpPost("Stadium")]
        public async Task<IActionResult> AddStadium([FromBody] StadiumRequestModel model)
        {
            var result = await _stadiumService.AddStadium(model);
            return Ok(result);
        }

        [HttpGet("Field")]
        public async Task<IActionResult> GetFieldHomeData()
        {
            var result = await _fieldRepository.GetFieldHomeData();
            return Ok(result);
        }


        //Upload ảnh lên Cloudinary
        [HttpPost("{id}/UploadImage")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "stadium" // Ảnh lưu vào folder "stadium" trên Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
                return BadRequest(uploadResult.Error.Message);

            var imageUrl = uploadResult.SecureUrl.ToString();
            var success = await _stadiumService.UpdateStadiumImage(id, imageUrl);

            if (!success)
                return StatusCode(500, "Could not save image URL to database.");

            return Ok(new { ImageUrl = imageUrl });
        }
    }
}
