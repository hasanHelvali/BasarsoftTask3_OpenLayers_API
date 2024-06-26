using BasarSoftTask3_API.Context;
using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.Filters;
using BasarSoftTask3_API.IRepository;
using BasarSoftTask3_API.Logging;
using BasarSoftTask3_API.Middlewares;
using BasarSoftTask3_API.Repository;
using BasarSoftTask3_API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MapContext>(o=>o.UseNpgsql("Host=localhost;Port=5432;Database=BasarSoftMapDB;Username=postgres;Password=postgres;"));


builder.Services.AddScoped(typeof(IMapRepository<>), typeof(MapRepository<>));
//builder.Services.AddScoped(typeof(IMapRepository<>), typeof(MapRepository<>));

builder.Services.AddSingleton<LogProducer>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey=true,
        ValidateLifetime=true,
        ValidIssuer = builder.Configuration["Token:Issuer"],
        ValidAudience = builder.Configuration["Token:Audience"],//denetleneip denetlenmeyecegini sorar. Denetlensin diyorum. True da diyebilriim.
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
        ClockSkew =TimeSpan.Zero
    };
    /*Jwt yapo�s�n�n Program.cs taraf�ndaki konfigurasyonu bu kadard�r. Bu konfigurasyon uzeirne bir class bina edip
     *bu class � controller taraf�nda kullanabiliriz.*/
});



builder.Services.AddIdentity<UserRegister,IdentityRole>().AddEntityFrameworkStores<MapContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<UserRegister>>();
builder.Services.AddScoped<SignInManager<UserRegister>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();

builder.Services.AddScoped<GetTableNameAndTableDatas>();
builder.Services.AddScoped<FeatureConvertToGeojson>();

//angular cors
//builder.Services.AddCors(configuration => configuration.AddDefaultPolicy(policiy =>
//policiy.WithOrigins("http://localhost:4200", "https://localhost:4200")
//.AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

//react cors
builder.Services.AddCors(configuration => configuration.AddDefaultPolicy(policiy =>
policiy.WithOrigins("http://localhost:3000", "https://localhost:3000")
.AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
    {
        policy.RequireRole("User");
    });

    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireRole("Admin");
    });

    options.AddPolicy("SuperAdmin", policy =>
    {
        policy.RequireRole("SuperAdmin");
    });
});
//Uygulamada gecerli olan yetkilendirme politikalar� bu sekilde verilmistir.
//builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions,>

//builder.Services.AddControllers(options =>
//{
//options.Filters.Add<ValidationFilter>();
//options.Filters.Add<RoleAuthorizationFilter>();
//Ilgili middleware i pipeline a ekledik.
//});

builder.Services.AddHttpClient();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseMiddleware<LoggingMiddleware>();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
