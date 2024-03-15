using BasarSoftTask2_API.DTOs;
using BasarSoftTask2_API.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.IO;
using System.Xml;
namespace BasarSoftTask2_API.Context
{
    public class MapContext:DbContext
    {

        public DbSet<LocAndUsers> LocsAndUsers { get; set; }

        private readonly IConfiguration _configuration;

        public MapContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreDbConnectionString"), x => x.UseNetTopologySuite());
        }

    }
}
