using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google.PolylineDecoder
{
    class Program
    {
        static void Main(string[] args)
        {
            var encodedPolyLine = "_p~iF~ps|U";

            Console.WriteLine("Encoded polyline: {0}", encodedPolyLine);

            var polyLine = new Polyline();

            var line = polyLine.Decode(encodedPolyLine);

            foreach (var geoPoint in line)
            {
                Console.WriteLine("Lat: {0}, Lng: {1}", geoPoint.Latitude, geoPoint.Longitude);
            }

            Console.WriteLine("completed line");
            Console.ReadLine();
        }
    }

    public class Polyline
    {

        //public string Encode(List<GeoPoint> polyline)
        //{
        //    var sb = new StringBuilder();

        //    foreach (var geoPoint in polyline)
        //    {
        //        sb.Append(EncodeCoordinate(geoPoint.Latitude));
        //        sb.Append(EncodeCoordinate(geoPoint.Longitude));
        //    }

        //    return sb.ToString();
        //}

        //private string EncodeCoordinate(double coordinate)
        //{
        //    var temp = Math.Round((coordinate*100000.0));

        //    var tempByte = (byte) temp;
           
        //    if (temp < 0)
        //    {
        //        //invert and add 1 
        //        tempByte = (byte)(~tempByte + 1);
        //    }

        //    var shifted = tempByte << 1;

        //    if (coordinate < 0)
        //        shifted = ~shifted;

            

        //}

        public List<GeoPoint> Decode(string encodedPolyline)
        {
            if (String.IsNullOrEmpty(encodedPolyline)) return null;

            var polyline = new List<GeoPoint>();

            var polylineChars = encodedPolyline.ToCharArray();

            var index = 0;
            var sum = 0;
            var next5Bits = 0;
            var shifter = 0;
            var currentLat = 0;
            var currentLong = 0;

            while (index < polylineChars.Length)
            {
                sum = 0;
                shifter = 0;
                do
                {
                    next5Bits = (int) polylineChars[index++] - 63;
                    Console.WriteLine(next5Bits);
                    sum |= (next5Bits & 31) << shifter;
                    Console.WriteLine("\t" + Convert.ToString(sum, 2));
                    shifter += 5;
                } while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5Bits = (int) polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                } while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5Bits >= 32)
                    break;

                currentLong += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                
                var p = new GeoPoint();
                p.Latitude = Convert.ToDouble(currentLat)/100000.0;
                p.Longitude = Convert.ToDouble(currentLong)/100000.0;
                polyline.Add(p);
            }

            return polyline;
        } 
    }

    public class GeoPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}
