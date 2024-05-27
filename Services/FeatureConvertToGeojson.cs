using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace BasarSoftTask3_API.Services
{
    public class FeatureConvertToGeojson
    {
        public string ConvertToGeoJson(IEnumerable<LocAndUsers> locations)
        {
            var features = locations.Select(location =>
            {
                //var geometry = ParseWkt(location.WKT); // WKT'yi geometriye dönüştür
                //var geometry = new
                //{
                //    type = location.Geometry.GeometryType.ToLower(), // Geometry tipini al
                //    coordinates = GetCoordinates(location.Geometry) // Koordinatları al
                //};
                var geometry = new GeoJsonWriter().Write(location.Geometry);
                var geometryObject = JsonConvert.DeserializeObject(geometry); // JSON string'ini nesneye dönüştür
                return new
                {
                    type = "Feature",
                    properties = new
                    {
                        ID = location.ID,
                        Name = location.Name,
                    },
                    //geometry = new GeoJsonWriter().Write(location.Geometry)
                    geometry = geometryObject


                };
            });

            var featureCollection = new
            {
                type = "FeatureCollection",
                features = features
            };

            return JsonConvert.SerializeObject(featureCollection, Formatting.Indented); // Daha okunabilir bir çıktı için Formatting.Indented kullanırız
        }
    }
    //private Geometry ParseWkt(string wkt)
    //{
    //    var reader = new WKTReader();
    //    return reader.Read(wkt);
    //}
}

