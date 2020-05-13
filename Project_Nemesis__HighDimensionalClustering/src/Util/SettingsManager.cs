using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections.Generic;
using System.Text;
using static Chaos.src.Util.Basic;

namespace Chaos.src.Util
{
    /// <summary>
    ///     Given a string, the function will return a transition matrix 
    ///     for a markov chain, it really depends on the context 
    ///     on which markov chain is returned.
    /// </summary>
    /// <param name="s">
    ///     String. 
    /// </param>
    /// <returns>
    ///     A 2d array full of double. 
    /// </returns>
    public delegate double[,] MatrixGenFxn(string s);

    /// <summary>
    ///     A function that return the distance between 2 matrices for a 
    ///     certain metric space. 
    /// </summary>
    /// <param name="a">
    ///     matrix, uniform 2d double array. 
    /// </param>
    /// <param name="b">
    ///     matrix, uniform 2d double array. 
    /// </param>
    /// <returns></returns>
    public delegate double MatrixDisFxn(double[,] a, double[,] b);


    /// <summary>
    ///     Singleton Design. 
    /// </summary>
    public class SettingsManager
    {
        public static MatrixMetric MtxMetric = MatrixMetric.TwoNorm;
        public static MatrixType MtxType = MatrixType.Tm27;
        private SettingsManager()
        {

        }

        /// <summary>
        ///     Let the setting manager dispatch a delegate 
        ///     that generate TM27 Transition Matrix. 
        /// </summary>
        public static void SetTM27ForTransitionMatrix()
        {
            MtxType = MatrixType.Tm27; 
        }

        /// <summary>
        ///     Let the setting manager dispatches a delegate 
        ///     that generate 2nd27TM matrix. 
        /// </summary>
        public static void Set2ndTM27ForTransitionMatrix()
        {
            MtxType = MatrixType.SecondOrder27; 
        }

        public static void SetDisFxnTo2Norm()
        {
            MtxMetric = MatrixMetric.TwoNorm; 
        }

        public static void SetDisFxnToVecNorm()
        {
            MtxMetric = MatrixMetric.VecOneNorm;
        }

        public static MatrixGenFxn DisPatchMatrixGenFxn()
        {
            switch (MtxType)
            {
                case MatrixType.Tm27:
                {
                    MatrixGenFxn handler = delegate (string s)
                    {
                        return GetTM27(s);
                    };
                    return handler;
                }
                case MatrixType.SecondOrder27:
                {
                    MatrixGenFxn handler = delegate (string s)
                    {
                        return Get2ndTM27(s);
                    };
                    return handler;
                }
            }

            throw new Exception("This shouldn't happen, please go check source codes.");
        }


        public static MatrixDisFxn DispatchMatrixDisFxn()
        {
            switch (MtxMetric)
            {
                case MatrixMetric.TwoNorm:
                {
                    MatrixDisFxn TwoNorm = delegate (double[,] a, double[,] b)
                    {
                        return Matrix2NormDistance(a, b);
                    };
                    return TwoNorm;
                }
                case MatrixMetric.VecOneNorm:
                {
                    MatrixDisFxn VectorizedNorm = delegate (double[,] a, double[,] b)
                    {
                        return MatrixVectorizedOneNorm(a, b);
                    };
                    return VectorizedNorm; 
                }
            }

            throw new Exception("This shouldn't happen, please go check source codes.");
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
