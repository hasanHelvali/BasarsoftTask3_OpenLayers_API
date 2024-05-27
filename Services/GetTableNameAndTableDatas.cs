using BasarSoftTask3_API.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nest;
using NetTopologySuite.Geometries;

namespace BasarSoftTask3_API.Services
{
    public class GetTableNameAndTableDatas
    {
        private readonly MapContext _context;
        private readonly DbContextOptions _dbContextOptions;


        public GetTableNameAndTableDatas(MapContext context, DbContextOptions dbContextOptions)
        {
            _context = context;
            _dbContextOptions = dbContextOptions;
        }

        public List<string> GetGeometryColumnTableNames()
        {
            //var geometryColumnTables = _context.Model.GetEntityTypes()
            //    .Where(e => e.GetProperties().Any(p => p.PropertyInfo.PropertyType == typeof(Point)))
            //    .Select(e => e.GetTableName())
            //    .ToList();

            //return geometryColumnTables;

            //var geometryColumnTables = _context.Model.GetEntityTypes()
            //    .Where(e => e.GetProperties().Any(p => p.PropertyInfo.PropertyType == typeof(Geometry)))
            //    .Select(e => e.GetTableName())
            //    .ToList();

            //Console.WriteLine("Tables with a geometry column:");
            //foreach (var tableName in geometryColumnTables)
            //{
            //    Console.WriteLine(tableName);
            //}

            //using (var dbContext = new MapContext(_dbContextOptions))
            //{
            //    var tableNames = await dbContext.Database.ExecuteSqlRawAsync<string>(
            //        "SELECT table_name " +
            //        "FROM information_schema.columns " +
            //        "WHERE data_type = 'USER-DEFINED' AND udt_name = 'geometry'");

            //    return tableNames.ToList();
            //}


            //        var geometryColumnTables = _context.Model.GetEntityTypes()
            //.Where(e => e.GetProperties().Any(p => p.Name == "Geometry"))
            //.Select(e => e.GetTableName())
            //.ToList();

            //return geometryColumnTables;

            
            var geometryColumnTables = _context.Model.GetEntityTypes()//Tablolar nesneleri burada elde ediliyor.
            .Where(e => e.GetProperties().Any(p => p.Name == "Geometry"))//Tabloların kolonları kontrol ediliyor ve Geometry olanlar seciliyor.
            .Select(e => e.GetTableName())//Secilen tabloların isimleri seciliyor.
            .ToList();//Lİst olarak geri donduruluyor.
            
            return geometryColumnTables;
        }
    }
}
