using BasarSoftTask3_API.Context;
using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.IRepository;
using BasarSoftTask3_API.Repository;
using BasarSoftTask3_API.Services;
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
    /*Jwt yapoýsýnýn Program.cs tarafýndaki konfigurasyonu bu kadardýr. Bu konfigurasyon uzeirne bir class bina edip
     *bu class ý controller tarafýnda kullanabiliriz.*/
});

builder.Services.AddIdentity<UserRegister,IdentityRole>().AddEntityFrameworkStores<MapContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<UserRegister>>();
builder.Services.AddScoped<SignInManager<UserRegister>>();

builder.Services.AddCors(configuration => configuration.AddDefaultPolicy(policiy =>
policiy.WithOrigins("http://localhost:4200", "https://localhost:4200")
.AllowAnyHeader().AllowAnyMethod().AllowCredentials()));


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();
