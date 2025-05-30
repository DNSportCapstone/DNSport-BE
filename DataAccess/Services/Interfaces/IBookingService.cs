﻿using DataAccess.DTOs.Request;
using DataAccess.Model;

namespace DataAccess.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<RevenueReportModel>> GetRevenueReport();
        Task<List<BookingReportModel>> GetBookingReport();
        Task<List<DenounceModel>> GetAllDenounce();
        Task<BookingInvoiceModel> GetBookingInvoice(int id);
        Task<int> CreateMultipleBookings(MultipleBookingsRequest request);
        Task<int> CreateBookingReport(ReportRequest bookingReport);
        Task<List<TransactionLogModel>> GetTransactionLog(int userId);
        Task<int> SetReportStatus(int id, string status);
        Task<List<FieldReportModel>> GetFieldReportList();
        Task<List<BookingHistoryModel>> GetBookingHistory(int userId);
        bool UpdateBookingStatus(int bookingId, string status);
        void AddTransactionLogAndRevenueTransaction(int bookingId);
        Task<int> CreateRecurringBookings(RecurringBookingRequest request);
        decimal GetTotalPriceWithVoucher(int bookingId);
    }
}
