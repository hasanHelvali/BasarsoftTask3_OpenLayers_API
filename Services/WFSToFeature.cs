using BasarSoftTask3_API.Entities;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;

namespace BasarSoftTask3_API.Services
{
    public static class WFSToFeature
    {
        public static Object WMSToFeatureConvert(dynamic jsonData)
        {
            Console.WriteLine(jsonData);
            //var featureJson = JArray.Parse(jsonData)[0];
            //var geometryJson = featureJson["geometry"];
            //var propertiesJson = featureJson["properties"];

            return new
            {
                ID = (string)jsonData["features"][0]["id"],
                Type = (string)jsonData["features"][0]["geometry"]["type"],
                Name = (string)jsonData["features"][0]["properties"]["Name"],
                Wkt = GeometryAndWktConvert.ArrayToWkt(jsonData["features"][0]["geometry"]["coordinates"][0])

                //WKT = ()GeometryAndWktConvert.ArrayToWkt(jsonData["features"][0]["geometry"][coordinates])
            };
            //var wkt = WktConverter.ConvertToWkt(featureModel.Coordinates);
            //var id= (string)jsonData["features"][0]["id"];
            //var id= (string)jsonData["features"][0]["geometry"]
        }
    }
}
/*
"""features"": [
  {
    ""type"": ""Feature"",
    ""id"": ""LocsAndUsers.67"",
    ""geometry"": {
        ""type"": ""Polygon"",
      
    },
    ""geometry_name"": ""Geometry"",
    ""properties"": {
        ""Name"": ""Kıbrıs Eyaleti"",
      ""Type"": ""POLYGON""
    }
}
]"
    */