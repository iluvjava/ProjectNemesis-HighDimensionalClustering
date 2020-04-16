using System;
using System.Collections.Generic;
using System.Text;
using static Chaos.src.Util.Basic;

namespace Chaos.src.Util
{
    /// <summary>
    /// Given a string, the function will return a transition matrix 
    /// for a markov chain, it really depends on the context 
    /// on which markov chain is returned.
    /// </summary>
    /// <param name="s">
    /// String. 
    /// </param>
    /// <returns>
    /// A 2d array full of double. 
    /// </returns>
    public delegate double[,] MatrixGenFxn(string s);
    public delegate double MatrixDisFxn(double[,] a, double[,] b);


    /// <summary>
    /// Singleton Design. 
    /// </summary>
    public class SettingsManager
    {


        private SettingsManager()
        {

        }

        public static MatrixGenFxn DisPatchMatrixGenFxn()
        {
            MatrixGenFxn handler = delegate(string s) 
            {
                return GetTM27(s);
            };
            return handler;
        }


        public static MatrixDisFxn DispatchMatrixDisFxn()
        {
            MatrixDisFxn TwoNorm = delegate (double[,] a, double[,] b)
            {
                return Matrix2NormDistance(a, b);
            };
            return TwoNorm;
        }



    }


    public enum MatrixMetric 
    {
        TwoNorm,
        VecOneNorm,
    }

    public enum MatrixType
    { 
        Tm27, 
        SecondOrder27, 
    }


}
