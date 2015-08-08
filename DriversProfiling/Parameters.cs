using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    class Parameters {
        // how many drivers will be calculated
        public static int DriversCount = 20;
        // how many trips for each driver will be calculated
        public static int TripsCountToCalculateProfile = 200;
        // how many test drivers for each calculated driver should be checked
        public static int DriversCountToTest = 20;
        // how many trips from test drivers should be checked
        public static int TripsCountToTest = 100;
        // path o driver trips files
        public static string Path = "{PATH}";
        // size of moving average window
        public static int MovingAverageSize = 1;
    }
}
