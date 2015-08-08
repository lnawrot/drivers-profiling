using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversProfiling {
    class CSVReader {
        /*
         * Reading contents of file with GPS data
         */
        public static List<Point> Read( string path ) {
            List<Point> result = new List<Point>();
            try {
                string[] lines = System.IO.File.ReadAllLines( @path );

                foreach (string line in lines) {
                    if (line != "x,y" && line != "") {
                        string[] coordinates = line.Split( ',' );
                        result.Add( new Point( float.Parse( coordinates[0], CultureInfo.InvariantCulture ), float.Parse( coordinates[1], CultureInfo.InvariantCulture ) ) );
                    }
                }
            }
            catch (FileNotFoundException ex) {
                System.Console.WriteLine( "Given file not found!" );
            }

            return result;
        }
    }
}
