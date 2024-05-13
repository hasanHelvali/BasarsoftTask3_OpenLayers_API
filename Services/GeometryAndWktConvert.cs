using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite;
using System.Drawing;
using System.Text;

namespace BasarSoftTask3_API.Services
{
    public static class GeometryAndWktConvert
    {
        public static Geometry WktToGeometrys(this string _wkt)
        {
            WKTReader wKTReader = new WKTReader(new NtsGeometryServices(CoordinateArraySequenceFactory.Instance, new PrecisionModel(), 4326))
            {
                IsOldNtsCoordinateSyntaxAllowed = false
            };
            return wKTReader.Read(_wkt);
            /*Burada yapılan isleem alınan wkt verisini geometry tipine cevirmesidir. Daha onceden alınan veriler de geometry tipine cast edilebiliryordu. 
             * Burada yapılan silemin farkı ise  geometry verisini 4326 seklinde haritada goruntulenecek bir formata cevirmesidir. Bu yapı cok onemlidir.*/
        }
        public static string GetWktType(string wkt)
        {
            if(wkt.StartsWith("POINT"))
                return "POINT";
            else if(wkt.StartsWith("LINESTRING"))
                return "LINESTRING";
            else if (wkt.StartsWith("POLYGON"))
                return "POLYGON";
            else { return "POINT"; }
        }


        public static string ArrayToWkt(dynamic coordinates)
        {
            StringBuilder wkt = new StringBuilder();
            wkt.Append("POLYGON (");

            // Koordinatları döngüyle işleyerek WKT formatına dönüştür
            foreach (var coordSet in coordinates)
            {
                wkt.Append("(");
               wkt.Append(coordSet[0] + " " + coordSet[1] + ",");
                // İlk koordinatı tekrar ekle (çokgenin kapanması için)
                var firstCoord = coordSet[0];
                wkt.Append(firstCoord[0] + " " + firstCoord[1]);
                wkt.Append("),");
            }

            // Son virgülü kaldır ve WKT formatını kapat
            wkt.Length--; // Son virgülü kaldır
            wkt.Append(")");

            return wkt.ToString();
        }
    }
}
