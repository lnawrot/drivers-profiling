using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    /*
     * Project contstants
     */
    public static class CONSTANTS {
        public const int NUMBER_OF_TRIPS = 200;
        public const double ACCELETRATION_TRESHOLD = 0.15;
        public const double ANGLE_TRESHOLD = 60;
    }

    /*
     * Utilities class
     */
    public class Utils {
        /*
         * Calculating distance between two points
         */
        public static double Distance( Point from, Point to ) {
            double result = 0;
            result += Math.Pow( to.X - from.X, 2 );
            result += Math.Pow( to.Y - from.Y, 2 );

            return Math.Sqrt(result);
        }

        /*
         * Calculating angle between points
         */
        public static double Angle( Point from, Point to ) {
            double angle = Math.Atan2( to.Y - from.Y, to.X - from.X );
            return angle * 180 / Math.PI;
        }

        public static double Angle( Point vector ) {
            double angle = Math.Atan2( vector.Y, vector.X );
            return angle * 180 / Math.PI;
        }


        public static double GetPercentile( List<double> data, int p ) {
            data.Sort();
            int index = (int)Math.Floor((decimal)data.Count() * p / 100);
            return data[index];
        }

        /*
         * Get all percentiles that can be divided by 5 (so 5th, 10th, 15th ... )
         */
        public static List<double> GetPercentiles( List<double> data ) {
            if (data.Count() == 0) {
                return data;
            } 

            data.Sort();
            List<double> result = new List<double>();
            for (var i = 5; i <= 95; i += 5) {
                result.Add( Utils.GetPercentile( data, i ) );
            }
            
            return result;
        }

        /*
         * Filtering data
         */
        public static List<double> Filter( List<double> data ) {
            List<double> copy = data.Select( s => s ).ToList();
            double p5 = Utils.GetPercentile( copy, 10 );
            double p95 = Utils.GetPercentile( copy, 90 );
            
            List<double> result = data.Where(d => d >= p5 && d <= p95).ToList();
            return result;
        }

        public static List<Point> Filter( List<Point> data ) {        
            List<Point> copy = data.Select( s => s ).ToList();
            double x95 = Utils.GetPercentile( copy.Select( p => p.X ).ToList(), 95 ),
                   y95 = Utils.GetPercentile( copy.Select( p => p.Y ).ToList(), 95 );

            List<Point> result = data.Where( d => d.X < x95 && d.Y < y95 ).ToList();

            //result = MovingAverage( result );
            
            return result;
        }

        /*
         * Calculate moving average
         */
        public static List<Point> MovingAverage( List<Point> data ) {
            int size = Parameters.MovingAverageSize;
            double tmpValX = 0, tmpValY = 0;
            List<Point> tmpList;
            Point tmpPoint;
            List<Point> result = new List<Point>();

            for( int i = 0; i < data.Count; i++ ) {
                if( i < size ) {
                    tmpList = data.GetRange( 0, i + 1 );
                    tmpPoint = new Point( tmpList.Select( x => x.X ).Average(), tmpList.Select( x => x.Y ).Average() );
                    result.Add( tmpPoint );
                    tmpValX = tmpPoint.X;
                    tmpValY = tmpPoint.Y;
                }
                else if( i < (data.Count - size) ){
                    tmpValX += data[i].X / size;
                    tmpValX -= data[i - size].X / size;
                    tmpValY += data[i].Y / size;
                    tmpValY -= data[i - size].Y / size;
                    result.Add( new Point(tmpValX, tmpValY) );
                }
                else {
                    tmpList = data.GetRange( i, data.Count - i );
                    tmpPoint = new Point( tmpList.Select( x => x.X ).Average(), tmpList.Select( x => x.Y ).Average() );
                    result.Add( tmpPoint );
                }
            }

            return result;
        }
    }

    /*
     * Extending List of doubles with methods for calculating Mean, Variance, Median and Standard Deviation.
     */
    public static class ListMathExtensions {
        public static double Mean( this List<double> values ) {
            return values.Sum() / values.Count;
        }

        public static double Variance( this List<double> values ) {
            double variance = 0;
            double mean = values.Mean();

            for (int i = 0; i < values.Count; i++) {
                variance += Math.Pow( (values[i] - mean), 2 );
            }

            return variance / (values.Count - 1);
        }

        public static double StandardDeviation( this List<double> values ) {
            double variance = values.Variance();

            return Math.Sqrt( variance );
        }

        public static double QuantileDeviation( this List<double> values ) {
            double median = values.Median();
            double m1 = values.Where( m => m < median ).ToList().Median();
            double m2 = values.Where( m => m >= median ).ToList().Median();

            return m2 - m1;
        }

        public static double Median( this List<double> values ) {
            if( values.Count() == 0 )
                return 0;

            List<double> copy = values.Select( s => s ).ToList();

            copy.Sort();
            double[] sorted = copy.ToArray();

            int mid = values.Count / 2;
            double median = (values.Count % 2 != 0) ? sorted[mid] : (sorted[mid] + sorted[mid - 1]) / 2;

            return median;
        }
    }
}
