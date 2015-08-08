using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    public class Analyzer {
        /*
         * Analyzing profiles of all drivers in order to choose proper weights to trip features
         */
        public static DriverProfile AllDrivers( List<Driver> drivers ) {
            DriverProfile profile;
            
            #region calculations
            #region speed
            List<double> mean = drivers.Select( d => d.Profile.Speed.Mean ).ToList();
            List<double> median = drivers.Select( d => d.Profile.Speed.Median ).ToList();
            List<double> stddev = drivers.Select( d => d.Profile.Speed.StdDev ).ToList();

            profile.Speed.Mean = mean.Mean();
            profile.Speed.Median = median.Mean();
            profile.Speed.StdDev = mean.StandardDeviation();
            #endregion

            #region acceleration
            mean = drivers.Select( d => d.Profile.Acceleration.Mean ).ToList();
            median = drivers.Select( d => d.Profile.Acceleration.Median ).ToList();
            stddev = drivers.Select( d => d.Profile.Acceleration.StdDev ).ToList();

            profile.Acceleration.Mean = mean.Mean();
            profile.Acceleration.Median = median.Mean();
            profile.Acceleration.StdDev = mean.StandardDeviation();
            #endregion

            #region deceleration
            mean = drivers.Select( d => d.Profile.Deceleration.Mean ).ToList();
            median = drivers.Select( d => d.Profile.Deceleration.Median ).ToList();
            stddev = drivers.Select( d => d.Profile.Deceleration.StdDev ).ToList();

            profile.Deceleration.Mean = mean.Mean();
            profile.Deceleration.Median = median.Mean();
            profile.Deceleration.StdDev = mean.StandardDeviation();
            #endregion

            #region gforce
            mean = drivers.Select( d => d.Profile.GForce.Mean ).ToList();
            median = drivers.Select( d => d.Profile.GForce.Median ).ToList();
            stddev = drivers.Select( d => d.Profile.GForce.StdDev ).ToList();

            profile.GForce.Mean = mean.Mean();
            profile.GForce.Median = median.Mean();
            profile.GForce.StdDev = mean.StandardDeviation();
            #endregion

            #region distance
            mean = drivers.Select( d => d.Profile.Distance.Mean ).ToList();
            median = drivers.Select( d => d.Profile.Distance.Median ).ToList();
            stddev = drivers.Select( d => d.Profile.Distance.StdDev ).ToList();

            profile.Distance.Mean = mean.Mean();
            profile.Distance.Median = median.Mean();
            profile.Distance.StdDev = mean.StandardDeviation();
            #endregion

            #region duration
            mean = drivers.Select( d => d.Profile.Duration.Mean ).ToList();
            median = drivers.Select( d => d.Profile.Duration.Median ).ToList();
            stddev = drivers.Select( d => d.Profile.Duration.StdDev ).ToList();

            profile.Duration.Mean = mean.Mean();
            profile.Duration.Median = median.Mean();
            profile.Duration.StdDev = mean.StandardDeviation();
            #endregion
            #endregion

            return profile;
        }
    }
}
