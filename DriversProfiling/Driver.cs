using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    /*
     * Class storing driver data (all trips, profile and ids)
     */
    public class Driver {
        public int Id;
        public DriverProfile Profile;
        public List<Trip> Trips = new List<Trip>();

        public Driver( int id ) {
            Id = id;
            LoadTrips();
            CalculateProfile();
        }

        /*
         * Calculating profile of a driver
         */
        private void CalculateProfile() {
            CalculateSpeed();
            CalculateAcceleration();
            CalculateDeceleration();
            CalculateGForce();
            CalculateDistance();
            CalculateDuration();
        }

        #region calculate helpers
        private void Calculate(List<double> values, string property) {
            FieldInfo propertyInfo = Profile.GetType().GetField( property );
            Type stats = propertyInfo.GetValue(Profile).GetType();

            stats.GetField( "Mean" ).SetValue( propertyInfo.GetValue( Profile ), values.Mean() );
            stats.GetField( "Median" ).SetValue( propertyInfo.GetValue( Profile ), values.Median() );
            stats.GetField( "StdDev" ).SetValue( propertyInfo.GetValue( Profile ), values.StandardDeviation() );
        }

        private void CalculateSpeed() {
            List<double> mean = Utils.Filter(Trips.Select( t => t.Profile.Speed.Mean ).ToList());
            List<double> median = Utils.Filter(Trips.Select( t => t.Profile.Speed.Median ).ToList());

            Profile.Speed.Mean = mean.Mean();
            Profile.Speed.Median = median.Mean();
            Profile.Speed.StdDev = mean.StandardDeviation();
        }

        private void CalculateAcceleration() {
            List<double> mean = Utils.Filter(Trips.Select( t => t.Profile.Acceleration.Mean ).ToList());
            List<double> median = Utils.Filter(Trips.Select( t => t.Profile.Acceleration.Median ).ToList());

            Profile.Acceleration.Mean = mean.Mean();
            Profile.Acceleration.Median = median.Mean();
            Profile.Acceleration.StdDev = mean.StandardDeviation();
        }

        private void CalculateDeceleration() {
            List<double> mean = Utils.Filter( Trips.Select( t => t.Profile.Deceleration.Mean ).ToList() );
            List<double> median = Utils.Filter( Trips.Select( t => t.Profile.Deceleration.Median ).ToList() );

            Profile.Deceleration.Mean = mean.Mean();
            Profile.Deceleration.Median = median.Mean();
            Profile.Deceleration.StdDev = mean.StandardDeviation();
        }

        private void CalculateGForce() {
            List<double> mean = Utils.Filter(Trips.Select( t => t.Profile.GForce.Mean ).ToList());
            List<double> median = Utils.Filter(Trips.Select( t => t.Profile.GForce.Median ).ToList());

            Profile.GForce.Mean = mean.Mean();
            Profile.GForce.Median = median.Mean();
            Profile.GForce.StdDev = mean.StandardDeviation();
        }

        private void CalculateDistance() {
            List<double> distances = Trips.Select( t => t.Profile.Distance ).ToList();
            distances = Utils.Filter( distances );

            Profile.Distance.Mean = distances.Mean();
            Profile.Distance.Median = distances.Median();
            Profile.Distance.StdDev = distances.StandardDeviation();
        }

        private void CalculateDuration() {
            List<double> durations = Trips.Select( t => System.Convert.ToDouble(t.Profile.Duration) ).ToList();
            durations = Utils.Filter( durations );

            Profile.Duration.Mean = durations.Mean();
            Profile.Duration.Median = durations.Median();
            Profile.Duration.StdDev = durations.StandardDeviation();
        }
        #endregion

        #region trip helpers
        public void LoadTrips() {
            for (int i = 1; i <= Parameters.TripsCountToCalculateProfile && i <= 200; i++) {
                Trip trip = new Trip( Id, i );
                trip.SetPoints( CSVReader.Read( Parameters.Path + Id + "/" + i + ".csv" ) );
                trip.Calculate();
                AddTrip( trip );
            }
        }

        public void AddTrip( Trip trip ) {
            Trips.Add( trip );
        }

        /*
         * Creating output data for drivers (listing every trip and information if it belongs to this driver or not)
         */
        public string GetTripsOutput() {
            //var data = Checker.Check( this );
            //StringBuilder sb = new StringBuilder( "", 1000 );
            //foreach (var t in data) {
            //    sb.AppendFormat( "{0}_{1},{2}\n", Id, t.Key, t.Value ).AppendLine();
            //}

            //return sb.ToString();
            return "";
        }
        #endregion
    }

    /**
     * Struct for storing calculated profile of a driver.
     **/
    public struct DriverProfile {
        public Stats Distance;
        public Stats Duration;
        public Stats Speed;
        public Stats Acceleration;
        public Stats Deceleration;
        public Stats GForce;

        public string ToString() {
            StringBuilder sb = new StringBuilder( "", 5000 );
            sb.AppendFormat( "Profile (mean, median, std-dev)" ).AppendLine();
            sb.AppendFormat( "Speed:        {0,5} | {1,5} | {2,5}", Math.Round( Speed.Mean, 3 ), Math.Round( Speed.Median, 3 ), Math.Round( Speed.StdDev, 3 ) ).AppendLine();
            sb.AppendFormat( "Acceleration: {0,5} | {1,5} | {2,5}", Math.Round(Acceleration.Mean, 3), Math.Round(Acceleration.Median, 3), Math.Round(Acceleration.StdDev, 3) ).AppendLine();
            sb.AppendFormat( "Deceleration: {0,5} | {1,5} | {2,5}", Math.Round(Deceleration.Mean, 3), Math.Round(Deceleration.Median, 3), Math.Round(Deceleration.StdDev, 3) ).AppendLine();
            sb.AppendFormat( "GForce:       {0,5} | {1,5} | {2,5}", Math.Round(GForce.Mean, 3), Math.Round(GForce.Median, 3), Math.Round(GForce.StdDev, 3) ).AppendLine();
            sb.AppendFormat( "Distance:     {0,5} | {1,5} | {2,5}", Math.Round(Distance.Mean, 3), Math.Round(Distance.Median, 3), Math.Round(Distance.StdDev, 3) ).AppendLine();
            sb.AppendFormat( "Duration:     {0,5} | {1,5} | {2,5}", Math.Round(Duration.Mean, 3),Math.Round( Duration.Median, 3), Math.Round(Duration.StdDev, 3) ).AppendLine();

            return sb.ToString();
        }        
    };
}
