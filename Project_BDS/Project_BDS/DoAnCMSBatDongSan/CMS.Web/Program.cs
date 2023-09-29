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
CDSContext context = new CDSContext();

Random rd = new Random();
//for(int i = 1; i <= 50; i++)
//{
//    Team team = new Team
//    {
//        Code = "abcd" + i.ToString(),
//        CreateTime = DateTime.Now,
//        Description = "description" + i.ToString(),
//        Member = 0,
//        Name = "Phòng " + i.ToString(),
//        Sologan = "abc",
//        StatusId = rd.Next(1, 2),
//        TruongPhongId  = rd.Next(5, 15),
//        UpdateTime = DateTime.Now,
//    };
//    context.teams.Add(team);
//}
//context.SaveChanges();

//for (int i = 1; i <= 50; i++)
//{
//    User user = new User
//    {
//        CreateTime = DateTime.Now,
//        DateOfBirth = new DateTime(2000, 1, 1),
//        Email = $"abcd{i}@gmail.com",
//        Name = "Nguyễn Văn A" + i.ToString(),
//        Password = BcryptNet.HashPassword("abcd"),
//        PhoneNumber = "0123456789" + i.ToString(),
//        RoleId = rd.Next(2, 6),
//        StatusId = 2,
//        TeamId = rd.Next(3, 50),
//        UserName = "abcd" + i.ToString(),
//        IsActive = true
//    };
//    context.users.Update(user);
//}
//context.SaveChanges();
//for(int i = 1; i <= 50; i++)
//{
//    double phanTramHoaHong = 0.01 * i;
//    double giaBan = 2000000 + i;
//    Product product = new Product
//    {
//        Address = "address" + i.ToString(),
//        BatDauBan = DateTime.Now,
//        Build = new DateTime(2022, 1, 1),
//        CertificateOfLand1 = "string",
//        CertificateOfLand2 = "string",
//        DauChuId = rd.Next(16, 26),
//        IsActive = true,
//        GiaBan = giaBan,
//        HostName = "Nguyễn Văn B" + i.ToString(),
//        HoaHong = giaBan * phanTramHoaHong,
//        HostPhoneNumber = "0234567819" + i.ToString(),
//        PhanTramChiaNV = phanTramHoaHong,
//        StatusId = rd.Next(1, 2)
//    };
//    context.products.Add(product);
//}
//context.SaveChanges();

//for(int i = 1; i <= 50; i++)
//{
//    PhieuXemNha phieuXemNha = new PhieuXemNha
//    {
//        BanThanhCong = true,
//        IsActive = true,
//        CreateTime = new DateTime(1970 + i, 1, 1),
//        CustumerId = i.ToString(),
//        CustumerIdImg1 = i.ToString(),
//        CustumerIdImg2 = i.ToString(),
//        CustumerName = "Trần Văn A" + i.ToString(),
//        CustumerPhoneNumber = "0987654321" + i.ToString(),
//        Desciption = "abcd" + i.ToString(),
//        NhaId = rd.Next(3, 40),
//        NhanVienId = rd.Next(2, 30)
//    };
//    context.Add(phieuXemNha);
//}
//context.SaveChanges();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPhieuXemNhaService, PhieuXemNhaService>();
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
