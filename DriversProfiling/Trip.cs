using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    /*
     * Class representing single trip of driver, calculating profile and storing trip points.
     */
    public class Trip {
        public int Id;
        public int Driver;
        private List<Point> Points;

        // trip profile
        public Results Profile;

        public Trip( int driver, int id ) {
            Id = id;
            Driver = driver;
        }

        public Trip( int driver, int id, List<Point> points ) {
            Id = id;
            Driver = driver;
            Points = points;
        }

        public void SetPoints( List<Point> points ) {
            Points = points;
        }

        /*
         * Calculating trip profile
         */
        public void Calculate() {
            Profile.Distance = CalcDistance();
            Profile.Duration = CalcDuration();

            var velocity = CalcVelocity();

            Profile.Speed.Mean = Profile.Distance / Profile.Duration;
            
            var resultantVelocity = velocity.Select( x => Math.Sqrt( Math.Pow( x.X, 2 ) + Math.Pow( x.Y, 2 ) ) ).ToList();

            Profile.Speed.Median = resultantVelocity.Median();

            var accelerationPeaks = CalcAcceleration( resultantVelocity );
            
            var accelerations = accelerationPeaks.Where( i => i > 0 ).ToList();
            if( accelerations.Count > 0 ) {
                Profile.Acceleration.Mean = accelerations.Mean();
                Profile.Acceleration.Median = accelerations.Median();
                Profile.Acceleration.StdDev = accelerations.StandardDeviation();
            }

            var decelerations = accelerationPeaks.Where( i => i < 0 ).ToList();
            if( decelerations.Count > 0 ) {
                Profile.Deceleration.Mean = decelerations.Mean();
                Profile.Deceleration.Median = decelerations.Median();
                Profile.Deceleration.StdDev = decelerations.StandardDeviation();
            }

            var gForce = CalcGForce( velocity );
            if( gForce.Count > 0 ) {
                Profile.GForce.Mean = gForce.Mean();
                Profile.GForce.Median = gForce.Median();
                Profile.GForce.StdDev = gForce.StandardDeviation();
            }
        }

        #region calculation helpers
        private double CalcDistance() {
            double result = 0;
            for(int i = 1; i < Points.Count; i++) {
                result += Utils.Distance( Points[i], Points[i - 1] );
            }

            return result;
        }

        private int CalcDuration() {
            return Points.Count;
        }

        private List<Point> CalcVelocity() {
            List<Point> velocity = new List<Point>();

            for (int i = 2; i < Points.Count; i++) {
                Point speed = new Point(Points[i].X - Points[i-1].X, Points[i].Y - Points[i-1].Y);
                velocity.Add( speed );
            }

            velocity = Utils.Filter( velocity );

            return velocity;
        }

        private List<double> CalcAcceleration( List<double> velocity ) {
            List<double> acceleration = new List<double>(),
                         accelerationPeaks = new List<double>(),
                         tmpList = new List<double>();

            int k = 0;

            for( int i = 2; i < velocity.Count; i++ ) {
                acceleration.Add( velocity[i] - velocity[i-1] );
            }

            foreach (double val in acceleration) {
                if( Math.Abs( val ) > CONSTANTS.ACCELETRATION_TRESHOLD ) {
                    if( k == 0 ) {
                        k = ( val > 0 ) ? 1 : -1;
                        tmpList.Add( val );
                        continue;
                    }
                    else if( (k == -1 && val < 0) || ( k == 1 && val > 0 ) ) {
                        tmpList.Add( val );
                        continue;
                    }
                    else {
                        //do not continue
                    }
                }
                if( k == 1 ) {
                    accelerationPeaks.Add( tmpList.Max() );
                }
                else if( k == -1 ) {
                    accelerationPeaks.Add( tmpList.Min() );
                }

                tmpList.Clear();
                k = 0;
            }
            
            return accelerationPeaks;
        }

        public List<double> CalcGForce( List<Point> velocity ) {
            List<double> gForcePeaks = new List<double>(),
                         velocityAngle = new List<double>(),
                         accAngle = new List<double>(),
                         accValue = new List<double>(),
                         tmpList = new List<double>();
           
            for( int i = 2; i < velocity.Count; i++ ) {
                var a = new Point( velocity[i].X - velocity[i - 1].X, velocity[i].Y - velocity[i - 1].Y );
                accValue.Add( Math.Sqrt( Math.Pow( a.X, 2 ) + Math.Pow( a.Y, 2 ) ) );
                accAngle.Add( Utils.Angle( a ) );
                velocityAngle.Add( Utils.Angle( velocity[i] ) );
            }

            int k = 0;

            for( int i = 0; i < accValue.Count; i++ ){
                var val = accValue[i];
                if( Math.Abs( val ) > CONSTANTS.ACCELETRATION_TRESHOLD ) {
                    var angleDiff = Math.Abs( accAngle[i] - velocityAngle[i] );
                    if( angleDiff > 180 )
                        angleDiff = 360 - angleDiff;

                    if( angleDiff > CONSTANTS.ANGLE_TRESHOLD ) {
                        if( k == 0 ) {
                            k = 1;
                            tmpList.Add( accValue[i] );
                            continue;
                        }
                        else {
                            tmpList.Add( accValue[i] );
                            continue;
                        }
                    }              
                }
                if( k == 1 ) {
                    gForcePeaks.Add( tmpList.Max() );
                }
                tmpList.Clear();
                k = 0;
            }

            return gForcePeaks;
        }
        #endregion

        #region helpers
        private List<double> GetDistances() {
            List<double> result = new List<double>();
            for (int i = 1; i < Points.Count; i++) {
                result.Add( Utils.Distance( Points[i], Points[i - 1] ) );
            }

            return result;
        }
        #endregion
    }

    /**
     * Struct for storing calculated features of a trip.
     **/
    public struct Results {
        public double Distance;
        public int Duration;

        public Stats Speed;
        public Stats Acceleration;
        public Stats Deceleration;
        public Stats GForce;
    };

    /**
     * Struct for storing mean, median and standard deviation of a trip or driver feature.
     **/
    public struct Stats {
        public double Mean;
        public double Median;
        public double StdDev;
    };
}
