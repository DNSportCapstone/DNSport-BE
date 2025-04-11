using AutoMapper;
using BusinessObject.Models;
using DataAccess.DAO;
using DataAccess.Mapper;
using DataAccess.Model;
using DataAccess.Repositories.Implement;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Implement;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using VNPAY.NET;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Db12353Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers()
    .AddOData(opt => opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(int.MaxValue));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{   
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "DNSport API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAutoMapper(typeof(ApplicationMapper));


// DAO
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<UserDetailDAO>();
builder.Services.AddScoped<BookingDAO>();
builder.Services.AddScoped<StadiumDAO>();

// Repository
builder.Services.AddScoped<IFieldRepository,FieldRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserDetailRepository, UserDetailRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IStadiumRepository, StadiumRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IVnpay, Vnpay>();
builder.Services.AddTransient<VnpayPayment>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<IRefundRepository, RefundRepository>();


// Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.Configure<MailSetting>(configuration.GetSection("MaillSettings"));
builder.Services.AddSingleton<IEmailSender, SendMailServices>();
builder.Services.AddHttpClient<IGoMapsService, GoMapsService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IStadiumService, StadiumService>();


var corsSettings = builder.Configuration.GetSection("CORS");
var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(option => option.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyOrigin",
        builder => builder.WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? string.Empty)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"]
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DNSport API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowMyOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
