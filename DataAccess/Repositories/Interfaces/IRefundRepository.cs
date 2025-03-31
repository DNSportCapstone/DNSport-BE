using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IRefundRepository
    {
        Task<IEnumerable<RefundModel>> GetAllRefundsAsync();
        Task<RefundModel> GetRefundByIdAsync(int refundId);
        Task<bool> CanRequestRefundAsync(int bookingId);
        Task<RefundResponseModel> CreateRefundRequestAsync(RefundRequestModel refundRequest); // trả về RefundResponseModel
        Task<RefundResponseModel> PreviewRefundRequestAsync(int bookingId);
        Task UpdateRefundStatusAsync(int refundId, string status);
        Task<bool> SaveChangesAsync();
        Task DeleteRefundAsync(int refundId);
    }
}
