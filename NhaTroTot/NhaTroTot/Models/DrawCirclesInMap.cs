using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media;

namespace NhaTroTot.Model
{
    public class DrawCirclesInMap
    {
        private Data_Geo _data;
        public Data_Geo GetData_Geo { get { return _data; } }
        private Data_Geo SetData_Geo { set { _data = value; } }
        public static GeoCoordinate GetAtDistanceBearing(GeoCoordinate point, double distance, double bearing)
        {
          const double degreesToRadian = Math.PI / 180.0;
          const double radianToDegrees = 180.0 / Math.PI;
          const double earthRadius = 6378137.0;

          var latA = point.Latitude * degreesToRadian;
          var lonA = point.Longitude * degreesToRadian;
          var angularDistance = distance / earthRadius;
          var trueCourse = bearing * degreesToRadian;

          var lat = Math.Asin(
              Math.Sin(latA) * Math.Cos(angularDistance) +
              Math.Cos(latA) * Math.Sin(angularDistance) * Math.Cos(trueCourse));

          var dlon = Math.Atan2(
              Math.Sin(trueCourse) * Math.Sin(angularDistance) * Math.Cos(latA),
              Math.Cos(angularDistance) - Math.Sin(latA) * Math.Sin(lat));

          var lon = ((lonA + dlon + Math.PI) % (Math.PI * 2)) - Math.PI;

          var result = new GeoCoordinate { Latitude = lat * radianToDegrees, Longitude = lon * radianToDegrees };
          return result;
        }

        public static IList<GeoCoordinate> GetCirclePoints(GeoCoordinate center, double radius, int nrOfPoints = 50)
        {
            var angle = 360.0 / nrOfPoints;
            var locations = new List<GeoCoordinate>();
            for (var i = 0; i <= nrOfPoints; i++)
            {
                locations.Add(GetAtDistanceBearing(center,radius, angle * i));
            }
            return locations;
        }

        public void DrawCircles(Map map, GeoCoordinate location)
        {
            var stroke = Colors.Purple;
            var fill = Colors.Transparent;
            int distance = 0;
            if (map.ZoomLevel <= 15)
                distance = 200;
            else
                if (map.ZoomLevel <= 18)
                    distance = 50;
                else
                    distance = 25;
            stroke.A = 80;
            var circle = new MapPolygon
            {
                StrokeThickness = 2,
                StrokeColor = stroke,
                FillColor=fill,
                StrokeDashed = false
            };
            foreach (var p in GetCirclePoints(location,distance))
            {
                circle.Path.Add(p);
            }
            Data_Geo da = new Data_Geo() { I = location, R = distance };
            SetData_Geo = da;
            map.MapElements.Add(circle);
        }

        public double rad2deg(double rad)
        {
            return (rad / (Math.PI * 180.0));
        }
        public double deg2rad(double deg)
        {
            return ((Math.PI * deg) / 180.0);
        }
        public double distance2M(GeoCoordinate point1, GeoCoordinate point2)
        {

            double R = 6371; // Radius of the earth in km
            var dLat = deg2rad(point2.Latitude - point1.Latitude);  // deg2rad below
            var dLon = deg2rad(point2.Longitude - point1.Longitude);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(point1.Latitude)) * Math.Cos(deg2rad(point2.Latitude)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d*1000;
        }
  }
    public class Data_Geo
    {
        public GeoCoordinate I { get; set; }
        public double R { get; set; }
    }
}
