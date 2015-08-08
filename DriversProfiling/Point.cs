using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    /*
     * Helper class for storing X and Y coordinates
     */
    public class Point {
        public double X;
        public double Y;

        public Point() {
        }

        public Point( double x, double y ) {
            X = x;
            Y = y;
        }
    }
}
