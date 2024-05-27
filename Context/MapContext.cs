using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nest;
using NetTopologySuite;
using NetTopologySuite.IO;
using System.Xml;
namespace BasarSoftTask3_API.Context
{
    public class MapContext:IdentityDbContext 
    {

        public DbSet<LocAndUsers> LocsAndUsers { get; set; }
        public DbSet<UserRegister> UserRegisters { get; set; }
        public DbSet<GeographyAuthority> GeographyAuthorities { get; set; }

        private readonly IConfiguration _configuration;

        public MapContext(DbContextOptions<MapContext> option, IConfiguration configuration) : base(option)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreDbConnectionString"), x => x.UseNetTopologySuite());
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=BasarSoftMapDB;Username=postgres;Password=postgres;", x => x.UseNetTopologySuite());
        }

        
    }
}
