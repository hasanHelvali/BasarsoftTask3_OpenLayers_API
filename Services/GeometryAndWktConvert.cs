using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite;

namespace BasarSoftTask2_API.Services
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
    }
}
