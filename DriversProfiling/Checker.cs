using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    /*
     * Class responsible for checking which trips match driver profile
     */
    public class Checker {
        private const int MATCH_TRESHOLD = 2;

        /*
         * Weights determining importance of given profile feature
         */
        private static Dictionary<string, int> Weights = new Dictionary<string, int>() {
            { "Speed", 10 },
            { "Acceleration", 30 },
            { "Deceleration", 35 },
            { "GForce", 20 },
            { "Distance", 1 },
            { "Duration", 1 }
        };
        private static Dictionary<string, double> Result;
        private static DriverProfile Driver;
        private static Results Trip;


        /*
         * Method returning trips matches
         */
        public static List<double> CalculateMatch( Driver driver, List<Trip> trips ) {
            // calculate difference between trip and driver profile
            List<double> result = new List<double>();

            foreach( Trip trip in trips ) {
                int weightsSum = Weights.Select( w => w.Value ).Sum();
                Result = new Dictionary<string, double>();
                Driver = driver.Profile;
                Trip = trip.Profile;
                RunChecks();

                double mismatch = Result.Select( r => r.Value ).Sum();
                result.Add( mismatch );
            }

            return result;
        }

        /*
         * Calculating count of self false trips and false trips of other drivers matching profile of current driver
         */
        public static List<CheckResult> Check( Driver driver, List<Driver> driversToCheck ) {
            List<CheckResult> result = new List<CheckResult>();
            Random rnd = new Random();
            CheckResult tmpResult;
            
            List<double> selfMatches = CalculateMatch(driver, driver.Trips);

            var selfMatchesFiltered = Utils.Filter( selfMatches );
                    
            tmpResult = new CheckResult();
            tmpResult.driverId = driver.Id;
            tmpResult.meanMatchValue = selfMatches.Mean();
            tmpResult.tripsNo = driver.Trips.Count();

            double matchLimit = selfMatchesFiltered.Mean() + selfMatchesFiltered.StandardDeviation() / 2;

            tmpResult.falseTrips = selfMatches.Where( x => x > matchLimit ).Count();

            result.Add( tmpResult );

            foreach( Driver currDriver in driversToCheck ) {
                List<double> matches = CalculateMatch(driver, currDriver.Trips.OrderBy(x => rnd.Next()).Take(Parameters.TripsCountToTest).ToList() );
            
                tmpResult = new CheckResult();
                tmpResult.driverId = currDriver.Id;
                tmpResult.meanMatchValue = matches.Mean();

                tmpResult.tripsNo = Parameters.TripsCountToTest;
                tmpResult.falseTrips = matches.Where( x => x > matchLimit ).Count();
                result.Add( tmpResult );
            }

            return result;
        } 

        public class CheckResult {
            public int driverId;
            public int tripsNo;
            public int falseTrips;
            public double meanMatchValue;

            public String ToString() {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat( "Driver #{0,3}: {1,3}/{2,3} ({3,2}%) false trips", driverId, falseTrips, tripsNo, 100*falseTrips/tripsNo ).AppendLine();
                return sb.ToString();
            }
        }


        #region checker helpers
        private static void RunChecks() {
            CheckSpeed();
            CheckAcceleration();
            CheckDeceleration();
            CheckGForce();
            CheckDistance();
            CheckDuration();
        }

        private static void CheckSpeed() {
            Result.Add( "Speed", GetPercentageChange( Driver.Speed.Mean, Trip.Speed.Mean ) * Weights["Speed"] );
            //Result.Add( "Speed.Median", GetPercentageChange( Driver.Speed.Median, Trip.Speed.Median) * Weights["Speed"] );
        }

        private static void CheckAcceleration() {
            Result.Add( "Acceleration", GetPercentageChange( Driver.Acceleration.Mean, Trip.Acceleration.Mean ) * Weights["Acceleration"] );
            //Result.Add( "Acceleration.Median", GetPercentageChange( Driver.Acceleration.Mean, Trip.Acceleration.Median ) * Weights["Acceleration"] / 2 );
        }

        private static void CheckDeceleration() {
            Result.Add( "Deceleration", GetPercentageChange( Driver.Deceleration.Mean, Trip.Deceleration.Mean ) * Weights["Deceleration"] );
            //Result.Add( "Deceleration.Median", GetPercentageChange( Driver.Deceleration.Mean, Trip.Deceleration.Median ) * Weights["Deceleration"] / 2 );
        }

        private static void CheckGForce() {
            Result.Add( "GForce", GetPercentageChange( Driver.GForce.Mean, Trip.GForce.Mean ) * Weights["GForce"] );
            //Result.Add( "GForce.Median", GetPercentageChange( Driver.GForce.Mean, Trip.GForce.Median ) * Weights["GForce"] / 2 );
        }

        private static void CheckDistance() {
            Result.Add( "Distance", GetPercentageChange( Driver.Distance.Mean, Trip.Distance ) * Weights["Distance"] );
        }

        private static void CheckDuration() {
            Result.Add( "Duration", GetPercentageChange( Driver.Duration.Mean, Trip.Duration ) * Weights["Duration"] );
        }
        /*
         * Getting percentage change between two values
         */
        private static double GetPercentageChange( double v1, double v2 ) {
            if( v1 == 0 ) {
                return 0;
            }

            return Math.Abs( ( v2 / v1 ) - 1 );
        }
        #endregion
    }
}
