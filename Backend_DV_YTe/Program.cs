using Backend_DV_YTe;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository;
using Backend_DV_YTe.Repository.Interface;
using Backend_DV_YTe.Service;
using Backend_DV_YTe.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
builder.Services.Configure<AppSettingModel>(configuration.GetSection("AppSettings"));
 
var secretKey = configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(opt =>
               {
                   opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                   {
                       //tự cấp token
                       ValidateIssuer = false,
                       ValidateAudience = false,

                       //ký vào token
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                       ClockSkew = TimeSpan.Zero
                   };
               });
// Add services to the container.
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddDistributedMemoryCache();
//=======================
builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<INhaCungCapRepository, NhaCungCapRepository>();
builder.Services.AddScoped<INhanVienRepository, NhanVienRepository>();
builder.Services.AddScoped<INhapThuocRepository, NhapThuocRepository>();
builder.Services.AddScoped<ILoaiThuocRepository, LoaiThuocRepository>();
builder.Services.AddScoped<IThuocRepository, ThuocRepository>();
builder.Services.AddScoped<ICTNhapThuocRepository, CTNhapThuocRepository>();
builder.Services.AddScoped<IXuatThuocRepository, XuatThuocRepository>();
builder.Services.AddScoped<ICTXuatThuocRepository, CTXuatThuocRepository>();
builder.Services.AddScoped<INhapThietBiYTeRepository, NhapThietBiYTeRepository>();
builder.Services.AddScoped<IThietBiYteRepository, ThietBiYTeRepository>();
builder.Services.AddScoped<ICTNhapThietBiYTeRepository, CTNhapThietBiYTeRepository>();
builder.Services.AddScoped<IXuatThietBiYTeRepository, XuatThietBiYTeRepository>();
builder.Services.AddScoped<ICTXuatThietBiYTeRepository, CTXuatThietBiYTeRepository>();
builder.Services.AddScoped<ILoaiDichVuRepository, LoaiDichVuRepository>();
builder.Services.AddScoped<ILoaiThietBiRepository, LoaiThietBiRepository>();
builder.Services.AddScoped<IDichVuRepository, DichVuRepository>();
builder.Services.AddScoped<IBacSiRepository, BacSiRepository>();
builder.Services.AddScoped<IKetQuaDichVuRepository, KetQuaDichVuRepository>();
builder.Services.AddScoped<ILichHenRepository, LichHenRepository>();
builder.Services.AddScoped<IDanhGiaRepository, DanhGiaRepository>();
builder.Services.AddScoped<IChuyenKhoaRepository, ChuyenKhoaRepository>();
builder.Services.AddScoped<ICTBacSiRepository, CTBacSiRepository>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();
builder.Services.AddScoped<ICTMuaThuocRepository, CTMuaThuocRepository>();
builder.Services.AddScoped<ICTMuaThietBiYTeRepository, CTMuaThietBiYTeRepository>();
//======================================

//======================================
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
 

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger eShop Solution", Version = "v1" });

    // Thay đổi cấu hình bảo mật để chỉ nhập chuỗi token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. Enter only the token value.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


//==========================================================

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();// de ra 401
app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowAll");

app.Run();
