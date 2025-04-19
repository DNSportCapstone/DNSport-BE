    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessObject.Models;
using DataAccess.DTOs.Request;
using DataAccess.DTOs.Response;
using DataAccess.Model;

namespace DataAccess.Mapper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<UserDetail, UserDetailModel>().ReverseMap();
            CreateMap<BankingAccount, BankAccountModel>().ReverseMap();
            CreateMap<Field, FieldModel>().ForMember(dest => dest.BookingFields, opt => opt.MapFrom(src => src.BookingFields)).ReverseMap();
            CreateMap<BookingField, BookingFieldModel>().ReverseMap();
            CreateMap<Voucher, VoucherModel>().ReverseMap();
            CreateMap<CreateVoucherRequest, VoucherModel>().ReverseMap();
            CreateMap<UpdateVoucherRequest, VoucherModel>().ReverseMap();
            CreateMap<VoucherModel, VoucherResponse>().ReverseMap();
            CreateMap<VoucherModel, CreateVoucherResponse>();
            CreateMap<Voucher, VoucherResponse>().ReverseMap();
            CreateMap<Voucher, CreateVoucherResponse>().ReverseMap();
        }
    }
}
