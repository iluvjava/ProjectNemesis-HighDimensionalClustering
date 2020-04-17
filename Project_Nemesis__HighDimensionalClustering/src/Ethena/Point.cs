using System;
using System.Collections.Generic;
using System.Text;

namespace Chaos.src.Ethena
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
            
            if (a.GetType() == typeof(TextAnalysis) && b.GetType() == typeof(TextAnalysis))
            {
                TextAnalysis aa = (TextAnalysis)a, bb = (TextAnalysis)b;
                return TextAnalysis.Dis(aa, bb); 
            }

            if (a.GetType() == typeof(SpacialPoint) && b.GetType() == typeof(SpacialPoint))
            {
                SpacialPoint aa = (SpacialPoint)a, bb = (SpacialPoint)b;
                return SpacialPoint.Dis(aa, bb); 
            }

            throw new NotImplementedException();

        }
    }

    /// <summary>
    /// This class is really for test full graph, that is all. 
    /// But it's important cause debugging on matrix dimension is almost impossible. 
    /// </summary>
    public class SpacialPoint : Point
    {
        double[] coord;
        int dim;

        public SpacialPoint(double[] coords)
        {
            if (coords.Length == 0) throw new ArgumentException();
            dim = coords.Length;
            coord = coords;
        }

        public SpacialPoint(int[] coords)
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
        public static double Dis(SpacialPoint a, SpacialPoint b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                throw new ArgumentException("Can't take null. ");
            if (a.dim != b.dim)
                throw new ArgumentException("Points dimension mismatched.");

            double sum = 0;
            for (int I = 0; I < a.dim; sum += Math.Pow(a.coord[I] - b.coord[I], 2.0), I++) ;
            return Math.Sqrt(sum);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("(");
            for (int I = 0; I < coord.Length - 1; I++)
            {
                sb.Append(coord[I] + ", ");
            }
            sb.Append(coord[coord.Length - 1] + ")");
            return sb.ToString();
        }



    }





}
