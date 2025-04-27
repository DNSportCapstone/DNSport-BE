using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject.Models;

public partial class Db12353Context : DbContext
{
    public Db12353Context()
    {
    }

    public Db12353Context(DbContextOptions<Db12353Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BankingAccount> BankingAccounts { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingField> BookingFields { get; set; }

    public virtual DbSet<BookingFieldService> BookingFieldServices { get; set; }

    public virtual DbSet<Denounce> Denounces { get; set; }

    public virtual DbSet<DenounceImage> DenounceImages { get; set; }

    public virtual DbSet<Field> Fields { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<LessorContact> LessorContacts { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<RevenueSharing> RevenueSharings { get; set; }

    public virtual DbSet<RevenueTransaction> RevenueTransactions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }

    public virtual DbSet<Sport> Sports { get; set; }

    public virtual DbSet<Stadium> Stadiums { get; set; }

    public virtual DbSet<TransactionLog> TransactionLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserVoucher> UserVouchers { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=db12353.public.databaseasp.net;uid=db12353;pwd=doan2024;database=db12353;Encrypt=false;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<BankingAccount>(entity =>
        {
            entity.HasKey(e => e.BankingAccountId).HasName("PK__BankingA__A23083059413AF4D");

            entity.ToTable("BankingAccount");

            entity.HasOne(d => d.User).WithMany(p => p.BankingAccounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__BankingAc__UserI__7E37BEF6");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AED65198C2F");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Booking__UserId__5EBF139D");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__Booking__Voucher__5FB337D6");
        });

        modelBuilder.Entity<BookingField>(entity =>
        {
            entity.HasKey(e => e.BookingFieldId).HasName("PK__BookingF__92EF916D1CFDA4C7");

            entity.ToTable("BookingField");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingFields)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingFi__Booki__6D0D32F4");

            entity.HasOne(d => d.Field).WithMany(p => p.BookingFields)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK__BookingFi__Field__6E01572D");
        });

        modelBuilder.Entity<BookingFieldService>(entity =>
        {
            entity.HasKey(e => new { e.BookingFieldId, e.ServiceId }).HasName("PK__BookingF__2EBE2A6DA7DEDF3F");

            entity.ToTable("BookingFieldService");

            entity.Property(e => e.Price).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.BookingField).WithMany(p => p.BookingFieldServices)
                .HasForeignKey(d => d.BookingFieldId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingFi__Booki__70DDC3D8");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingFieldServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingFi__Servi__71D1E811");
        });

        modelBuilder.Entity<Denounce>(entity =>
        {
            entity.HasKey(e => e.DenounceId).HasName("PK__Denounce__9352008A640CEF8D");

            entity.ToTable("Denounce");

            entity.Property(e => e.DenounceTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Receive).WithMany(p => p.DenounceReceives)
                .HasForeignKey(d => d.ReceiveId)
                .HasConstraintName("FK__Denounce__Receiv__02FC7413");

            entity.HasOne(d => d.Send).WithMany(p => p.DenounceSends)
                .HasForeignKey(d => d.SendId)
                .HasConstraintName("FK__Denounce__SendId__02084FDA");
        });

        modelBuilder.Entity<DenounceImage>(entity =>
        {
            entity.HasKey(e => e.DenounceImageId).HasName("PK__Denounce__A3097B1F7F156A8A");

            entity.ToTable("DenounceImage");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Denounce).WithMany(p => p.DenounceImages)
                .HasForeignKey(d => d.DenounceId)
                .HasConstraintName("FK__DenounceI__Denou__07C12930");
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.FieldId).HasName("PK__Field__C8B6FF07F67E7AC6");

            entity.ToTable("Field");

            entity.Property(e => e.DayPrice).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.NightPrice).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Sport).WithMany(p => p.Fields)
                .HasForeignKey(d => d.SportId)
                .HasConstraintName("FK__Field__SportId__5BE2A6F2");

            entity.HasOne(d => d.Stadium).WithMany(p => p.Fields)
                .HasForeignKey(d => d.StadiumId)
                .HasConstraintName("FK__Field__Status__5AEE82B9");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Image__7516F70CB7544563");

            entity.ToTable("Image");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Field).WithMany(p => p.Images)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK__Image__FieldId__0C85DE4D");
        });

        modelBuilder.Entity<LessorContact>(entity =>
        {
            entity.HasKey(e => e.ContactId);

            entity.ToTable("LessorContact");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1257B3F8A9");

            entity.ToTable("Notification");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__4F7CD00D");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A380B13BBE3");

            entity.ToTable("Payment");

            entity.Property(e => e.Deposit).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PaymentTime).HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Payment__Booking__628FA481");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Rating__FCCDF87C1B1759F4");

            entity.ToTable("Rating");

            entity.Property(e => e.ReplyTime).HasColumnType("datetime");
            entity.Property(e => e.Time).HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Rating__BookingI__75A278F5");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Rating__UserId__74AE54BC");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.RefundId).HasName("PK__Refund__725AB9208953C112");

            entity.ToTable("Refund");

            entity.Property(e => e.RefundAmount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Time).HasColumnType("datetime");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__Refund__PaymentI__66603565");

            entity.HasOne(d => d.User).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Refund__UserId__656C112C");
        });

        modelBuilder.Entity<RevenueSharing>(entity =>
        {
            entity.ToTable("RevenueSharing");

            entity.Property(e => e.LessorPercentage).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Stadium).WithMany(p => p.RevenueSharings)
                .HasForeignKey(d => d.StadiumId)
                .HasConstraintName("FK_RevenueSharing_Stadium");
        });

        modelBuilder.Entity<RevenueTransaction>(entity =>
        {
            entity.ToTable("RevenueTransaction");

            entity.Property(e => e.AdminAmount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.OwnerAmount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.RevenueTransactionDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.RevenueTransactions)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_RevenueTransaction_Booking");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A4AEFB3C0");

            entity.ToTable("Role");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB00A7C487FD9");

            entity.ToTable("Service");

            entity.Property(e => e.Price).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Service__Categor__6A30C649");

            entity.HasOne(d => d.Field).WithMany(p => p.Services)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK__Service__FieldId__693CA210");
        });

        modelBuilder.Entity<ServiceCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ServiceC__19093A0B6FA8686F");

            entity.ToTable("ServiceCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Sport>(entity =>
        {
            entity.HasKey(e => e.SportId).HasName("PK__Sport__7A41AF3C2CC67200");

            entity.ToTable("Sport");

            entity.Property(e => e.SportName).HasMaxLength(100);
        });

        modelBuilder.Entity<Stadium>(entity =>
        {
            entity.HasKey(e => e.StadiumId).HasName("PK__Stadium__ED8330580C3226B0");

            entity.ToTable("Stadium");

            entity.HasOne(d => d.User).WithMany(p => p.Stadia)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Stadium__UserId__5812160E");
        });

        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Transact__5E54864805C85499");

            entity.ToTable("TransactionLog");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TimeSlot).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.TransactionLogs)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Transacti__Booki__7B5B524B");

            entity.HasOne(d => d.User).WithMany(p => p.TransactionLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Transacti__UserI__7A672E12");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C1CDB61F9");

            entity.ToTable("User");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReceiveNotification).HasDefaultValue(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__RoleId__48CFD27E");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserDeta__1788CC4C7CF06C6E");

            entity.ToTable("UserDetail");

            entity.Property(e => e.UserId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.User).WithOne(p => p.UserDetail)
                .HasForeignKey<UserDetail>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserDetai__UserI__4BAC3F29");
        });

        modelBuilder.Entity<UserVoucher>(entity =>
        {
            entity.HasKey(e => e.UserVoucherId).HasName("PK__UserVouc__8017D499C480913F");

            entity.ToTable("UserVoucher");

            entity.HasOne(d => d.User).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserVouch__IsUse__5441852A");

            entity.HasOne(d => d.Voucher).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__UserVouch__Vouch__5535A963");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Voucher__3AEE7921E995711E");

            entity.ToTable("Voucher");

            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
