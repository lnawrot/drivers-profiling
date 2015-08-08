using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    class Program {
        /*
         * arguments in order:
         *  - path to folder with drivers trips
         *  - number of drivers to check
         *  - number of trips of these drivers to consider when calculating profile and checking
         *  - number of drivers to test (when testing profile of current driver)
         *  - number of trips for each test driver
         */
        
        static void Main( string[] args ) {
            // checking if program parameters should be changed
            if (args.Length > 0) {
                Parameters.Path = args[0];
                if (args.Length > 1)
                    Parameters.DriversCount = Convert.ToInt32( args[1] );

                if (args.Length > 2)
                    Parameters.TripsCountToCalculateProfile = Convert.ToInt32( args[2] );

                if (args.Length > 3)
                    Parameters.DriversCountToTest = Convert.ToInt32( args[3] );

                if (args.Length > 4)
                    Parameters.TripsCountToTest = Convert.ToInt32( args[4] );
            }
            // it there are no invocation params passed then calculate with default parameters

            // loop through all drivers files
            List<Driver> drivers = new List<Driver>();
            int[] driversIds;
            try {
                driversIds = Directory.GetDirectories( @Parameters.Path ).Select( d => Int32.Parse( new DirectoryInfo( d ).Name ) ).ToArray();
            }
            catch (IOException ex) {
                System.Console.WriteLine( ex.Message );
                return;
            }
            

            // initiate drivers and calculate profiles
            Console.WriteLine( "Calculating drivers profiles..." );
            for (int i = 0; i < Parameters.DriversCount && i < driversIds.Length; i++) {
                Driver driver = new Driver( driversIds[i] );
                Output.WriteDriverInfo( driver );

                drivers.Add(driver);
            }

            Random rnd = new Random();

            List<Checker.CheckResult> result;
            List<double> selfChecks = new List<double>();
            List<double> otherChecks = new List<double>();

            // checking drivers profiles accuracy
            foreach( Driver driverToCheck in drivers ) {
                result = Checker.Check( driverToCheck, drivers.OrderBy( x => rnd.Next() ).Take( Parameters.DriversCountToTest ).Where( d => d.Id != driverToCheck.Id ).ToList() );
                Output.WriteDriverStats( driverToCheck, result );
                selfChecks.Add( 100 * result[0].falseTrips / result[0].tripsNo );
                result = result.Where( x => x.driverId != driverToCheck.Id ).ToList();
                otherChecks.Add( 100 * result.Select( x => x.falseTrips ).Average() / result[0].tripsNo );
            }

            Console.WriteLine(String.Format("Self checks average: {0}%     Others check average: {1}", selfChecks.Average(), otherChecks.Average() ));
            
            System.Console.ReadKey();
        }
    }
}
