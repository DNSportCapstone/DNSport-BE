using DataAccess.Implement;
using DataAccess.Interface;
using DataAccess.Model;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var MaillSettings = configuration.GetSection("MaillSettings");
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IFieldRepository,FieldRepository>();
builder.Services.AddScoped<IStadium,StadiumService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<IUser,UserService>();
builder.Services.Configure<MailSetting>(MaillSettings);
builder.Services.AddSingleton<IEmailSender, SendMailServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
