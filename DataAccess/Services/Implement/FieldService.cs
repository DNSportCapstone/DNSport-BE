﻿using AutoMapper;
using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataAccess.Services.Implement
{
    public class FieldService : IFieldService
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly Db12353Context _dbcontext;
        private readonly IMapper _mapper;


        public FieldService(Db12353Context dbcontext, IFieldRepository fieldRepository, IMapper mapper)
        {
            _dbcontext = dbcontext;
            _fieldRepository = fieldRepository;
            _mapper = mapper;
        }
        public async Task<List<GetFieldResponse>> GetAllFieldsAsync()
        {
            var fields = await _fieldRepository.GetAllFieldsAsync();
            return fields
                .Where(f => f.StadiumId != null) 
                .Select(f => new GetFieldResponse
                {
                    FieldId = f.FieldId,
                    StadiumId = f.StadiumId.Value,
                    SportId = f.SportId ?? 0,
                    Description = f.Description ?? "",
                    DayPrice = f.DayPrice ?? 0,
                    NightPrice = f.NightPrice ?? 0,
                    ImageUrls = f.Images?.Select(i => i.Url).ToList() ?? new List<string>()
                })
                .ToList();
        }
        public async Task<List<GetFieldResponse>>GetFieldsByStadiumIdAsync(int stadiumId)
        {
            var fields = await _fieldRepository.GetFieldsByStadiumIdAsync(stadiumId);
            return _mapper.Map<List<GetFieldResponse>>(fields);
        }

        public async Task<RegisterFieldResponse> RegisterFieldAsync(RegisterFieldRequest request)
        {
            var field = new Field
            {
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

            field.Description = request.Description;
            field.DayPrice = request.DayPrice;
            field.NightPrice = request.NightPrice;
            field.Status = request.Status;

            await _fieldRepository.UpdateFieldAsync(field);

            if (request.ImageUrls?.Any() == true)
            {
                await _fieldRepository.UpdateFieldImagesAsync(field.FieldId, request.ImageUrls);
            }

            return new UpdateFieldResponse
            {
                FieldId = field.FieldId,
                Message = "Field updated successfully."
            };
        }

    }
}
