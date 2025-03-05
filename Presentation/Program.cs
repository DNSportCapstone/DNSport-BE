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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VNPAY.NET;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var MaillSettings = configuration.GetSection("MaillSettings");

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

// Mapper
builder.Services.AddScoped<IMapper, Mapper>();

// Repository
builder.Services.AddScoped<IFieldRepository,FieldRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserDetailRepository, UserDetailRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IFieldRepository, FieldRepository>();
builder.Services.AddScoped<IStadiumRepository, StadiumRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddSingleton<IVnpay, Vnpay>();
builder.Services.AddTransient<VnpayPayment>();

// Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.Configure<MailSetting>(MaillSettings);
builder.Services.AddSingleton<IEmailSender, SendMailServices>();

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


builder.Services.AddDbContext<Db12353Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowMyOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
