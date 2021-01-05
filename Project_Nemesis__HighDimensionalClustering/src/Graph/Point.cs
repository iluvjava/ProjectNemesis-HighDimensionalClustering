using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using TheBase.src.Util;

namespace TheBase.src.Core
{

    /// <summary>
    /// This interface marks a type of thing, and it 
    /// contains a lot of helpful static functions for it. 
    /// </summary>
    public interface Point
    {

        /// <summary>
        /// Assume non null frrom a, b
        /// </summary>
        /// <param name="a">
        ///     point a
        /// </param>
        /// <param name="b">
        ///     point b
        /// </param>
        /// <returns>
        ///     Distance between the points, 
        ///     * exact implementations depends on runtime-type of the 
        ///     point object. 
        /// </returns>
        public static double Dis(Point a, Point b)
        {
            
            if (a is Text && b is Text)
            {
                Text aa = (Text)a, bb = (Text)b;
                return Text.Dis(aa, bb); 
            }

            if (a is SpatialPoint && b is SpatialPoint)
            {
                SpatialPoint aa = a as SpatialPoint, bb = b as SpatialPoint;
                return SpatialPoint.Dis(aa, bb); 
            }

            throw new NotImplementedException();

        }

        /// <summary>
        ///     <para>
        ///         Using the dynamic type of the inputs arrays of points, call it's desinator
        ///         and return the centroid of all the points. 
        ///     </para>
        /// </summary>
        /// <param name="points">
        /// List of points. 
        /// </param>
        /// <returns>
        ///     A point, of the same type as the points in the list. 
        /// </returns>
        public static Point CentroidOf(Point[] points)
        {
            if (points.Length == 0) throw new Exception("Can't find the centroid of an empty points[]."); 

            if (points[0] is SpatialPoint)
            {
                SpatialPoint[] p = new SpatialPoint[points.Length];
                for (int I = 0; I < p.Length; I++) p[I] = (SpatialPoint)points[I]; 
                return SpatialPoint.TakeAverage(p);
            }

            if (points[0] is Text)
            {
                Text[] T = Array.ConvertAll(points, item => (Text)item);
                return Text.GetCentroidAmong(T);
            }

            throw new NotImplementedException(); // TODO: NOT IMPLEMENTED YET. 
        }

    }

    /// <summary>
    /// This class is really for test full graph, that is all. 
    /// But it's important cause debugging on matrix dimension is almost impossible. 
    /// </summary>
    public class SpatialPoint : Point
    {
        protected double[] coord ;
        protected int dim;

        protected SpatialPoint(int dimension)
        {
            // internal constructor, origin is created. 
            coord = new double[dimension];
            dim = dimension;
        }

        public SpatialPoint(double[] coords)
        {
            if (coords.Length == 0) throw new ArgumentException();
            dim = coords.Length;
            coord = coords;
        }

        public SpatialPoint(int[] coords)
        { 
            if (coords.Length == 0) throw new ArgumentException();
            coord = new double[coords.Length]; 
            for (int I = 0; I < coords.Length; I++) coord[I] = coords[I];
            dim = coords.Length;
        }

        /// <summary>
        ///     * Get the distance between 2 points in the space. 
        ///     * Euclidean distance, with the square root. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Dis(SpatialPoint a, SpatialPoint b)
        {
            if (a is null || b is null)
                throw new ArgumentException("Can't take null. ");
            if (a.dim != b.dim)
                throw new ArgumentException("Points dimension mismatched.");

            double sum = 0;
            for (int I = 0; I < a.dim; sum += Math.Pow(a.coord[I] - b.coord[I], 2.0), I++) ;
            return Math.Sqrt(sum);
        }
        
        override
        public string ToString()
        {
            StringBuilder sb = new StringBuilder("(");
            for (int I = 0; I < coord.Length - 1; I++)
            {
                sb.Append(coord[I] + ", ");
            }
            sb.Append(coord[coord.Length - 1] + ")");
            return sb.ToString();
        }

        public static Point[] NormalRandomPoints(double[] mu, double sd, int N)
        {
            SpatialPoint[] res = new SpatialPoint[N];
            int dimension = mu.Length;
            Normal[] Generators = new Normal[dimension];
            
            for (int I = 0; I < dimension; Generators[I] = new Normal(mu[I], sd), I++);
            
            for (int I = 0; I < N; I++)
            {
                res[I] = new SpatialPoint(dimension); 
            }
            for (int I = 0; I < dimension; I++)
            {
                double[] RandSamples = new double[N]; 
                Generators[I].Samples(RandSamples);
                for (int J = 0; J < N; J++)
                {
                    res[J].coord[I] = RandSamples[J];
                }
            }
            
            return res;
            
        }

        /// <summary>
        ///     Take the average of all spacial points from a list of points. 
        /// </summary>
        /// <returns></returns>
        public static Point TakeAverage(SpatialPoint[] points)
        {
            double[][] thecoords = new double[points.Length][];
            for (int I = 0; I < points.Length; I++)
            {
                thecoords[I] = points[I].coord; 
            }

            return new SpatialPoint(Basic.EntrywiseAverage(thecoords));
        }

        /// <summary>
        ///     Index the number of a certain coordinate. 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double this[int i]
        {
            get {
                return coord[i];
            }
        }

    }

}
