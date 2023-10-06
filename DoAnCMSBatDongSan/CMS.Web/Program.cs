using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;
using System.Text;
using CMS_Design.IService;
using CMS_ActionLayer.Service;
using CMS_Design.Payloads.Responses;
using CMS_Design.Payloads.Converters;
using CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam;
using CMS_Design.Payloads.DTOs.DataResponseOfSalesAndProfitOfCompany;
using CMS_Design.Payloads.DTOs.DataResponseTeam;
using CMS_Design.Payloads.DTOs.DataResponseToken;
using CMS_Design.Payloads.DTOs.DataResponseUser;
using CMS_Design.Payloads.DTOs.DataResponseProduct;
using CMS_ActionLayer.DAO;
using CMS_Design.Entities;
using Microsoft.AspNetCore.Identity;
using CMS_Design.Payloads.DTOs.DataResponsePhieuXemNha;
using BCrypt.Net;
using BcryptNet = BCrypt.Net.BCrypt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Auth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Làm theo mẫu này. Example: Bearer {Token} ",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    x.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPhieuXemNhaService, PhieuXemNhaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<PhieuXemNhaConverter>();
builder.Services.AddSingleton<ResponseObject<PhieuXemNhaDTO>>();
builder.Services.AddSingleton<PhieuXemNhaConverter>();
builder.Services.AddSingleton<ResponseObject<SalesStatisticsDTO>>();
builder.Services.AddSingleton<DocumentService>();
builder.Services.AddCors();
builder.Services.AddSingleton<ResponseObject<SalesStatisticsOfTeamDTO>>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ProductConverter>();
builder.Services.AddSingleton<ResponseObject<ProductDTO>>();
builder.Services.AddSingleton<ResponseObject<TeamDTO>>();
builder.Services.AddSingleton<ResponseObject<UserDTO>>();
builder.Services.AddSingleton<ResponseObject<TokenDTO>>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ProductImgConverter>();
builder.Services.AddSingleton<UserConverter>();
builder.Services.AddControllers().AddJsonOptions(options =>

{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection("AppSettings:SecretKey").Value!))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();
app.UseCors("AllowAllOrigins");

app.MapControllers();

app.Run();
