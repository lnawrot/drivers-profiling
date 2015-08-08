using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    /*
     * Class for showing and saving results to file and showing on console
     */
    public class Output {
        private static string resultsFile = "results.txt";
        private static bool cleared = false;

        /*
         * Saving one driver profile output and show it to console
         */
        public static void WriteDriverInfo( Driver driver ) {
            StringBuilder sb = new StringBuilder( "", 5000 );
            sb.AppendFormat( "Driver #{0}", driver.Id ).AppendLine();
            sb.AppendFormat( "{0}\n", driver.Profile.ToString() ).AppendLine();

            Console.WriteLine( "Driver #" + driver.Id );
            Console.WriteLine( driver.Profile.ToString() + "\n" );

            WriteToResults( sb.ToString() );
        }

        /*
         * Saving one driver trips output
         */
        public static void WriteDriver( Driver driver ) {
            StringBuilder sb = new StringBuilder( "", 5000 );
            sb.AppendFormat( "{0}\n", driver.GetTripsOutput() ).AppendLine();

            WriteToResults( sb.ToString() );
        }

        /*
         * Writing driver statistics (number of false trips, and profile matches with other drivers trips) to results file and to console
         */
        public static void WriteDriverStats( Driver driver, List<Checker.CheckResult> list ) {
            StringBuilder sb = new StringBuilder( "", 5000 );
            sb.AppendFormat( "Driver #{0} checking trips", driver.Id ).AppendLine();
            Console.WriteLine( "Driver #" + driver.Id + " checking trips" );
            List<double> results = new List<double>();
            foreach (Checker.CheckResult result in list) {
                sb.Append( result.ToString() );
                Console.Write( "   " + result.ToString() );
                if( result.driverId != driver.Id ) {
                    results.Add( 100 * result.falseTrips / result.tripsNo );
                }
            }
            double driverPercent = 100 * list[0].falseTrips / list[0].tripsNo;
            sb.AppendFormat( "Driver #{0} false trips = {1}%, others = {2}%", driver.Id, driverPercent, results.Mean() ).AppendLine();
            Console.WriteLine( String.Format("   Driver #{0} false trips = {1}%, others = {2}%", driver.Id, driverPercent,Math.Round(results.Mean(), 1)) );


            WriteToResults( sb.ToString() );
        }

        #region helpers
        public static void WriteToResults( string text ) {
            WriteToFile( text, resultsFile );
        }

        /*
         * Writing data to file
         */
        private static void WriteToFile( string text, string path ) {
            if (!cleared) {
                Clear(path);
            }
            File.AppendAllText( path, text );
        }

        /*
         * Clearing results file
         */
        private static void Clear(string path) {
            File.WriteAllText( @path, string.Empty );
            cleared = true;
        }
        #endregion
    }
}
