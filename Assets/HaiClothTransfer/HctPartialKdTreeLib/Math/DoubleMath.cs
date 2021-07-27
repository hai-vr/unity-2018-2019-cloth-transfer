// The MIT License (MIT)
//
// Copyright (c) 2013 codeandcats
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// ---
//
// Original source: https://github.com/codeandcats/KdTree
// KdTree has been modified for use in HaiClothTransfer.

using System;

namespace HaiClothTransfer.HctPartialKdTreeLib.Math
{
    [Serializable]
    public class DoubleMath : TypeMath<double>
    {
        public override int Compare(double a, double b)
        {
            return a.CompareTo(b);
        }

        public override bool AreEqual(double a, double b)
        {
            return a == b;
        }

        public override double MinValue
        {
            get { return double.MinValue; }
        }

        public override double MaxValue
        {
            get { return double.MaxValue; }
        }

        public override double Zero
        {
            get { return 0; }
        }

        public override double NegativeInfinity { get { return double.NegativeInfinity; } }

        public override double PositiveInfinity { get { return double.PositiveInfinity; } }

        public override double Add(double a, double b)
        {
            return a + b;
        }

        public override double Subtract(double a, double b)
        {
            return a - b;
        }

        public override double Multiply(double a, double b)
        {
            return a * b;
        }

        public override double DistanceSquaredBetweenPoints(double[] a, double[] b)
        {
            double distance = Zero;
            int dimensions = a.Length;

            // Return the absolute distance bewteen 2 hyper points
            for (var dimension = 0; dimension < dimensions; dimension++)
            {
                double distOnThisAxis = Subtract(a[dimension], b[dimension]);
                double distOnThisAxisSquared = Multiply(distOnThisAxis, distOnThisAxis);

                distance = Add(distance, distOnThisAxisSquared);
            }

            return distance;
        }
    }

    // Algebraic!
    [Serializable]
    public abstract class TypeMath<T>
    {
        #region ITypeMath<T> members

        public abstract int Compare(T a, T b);

        public abstract bool AreEqual(T a, T b);

        public virtual bool AreEqual(T[] a, T[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (var index = 0; index < a.Length; index++)
            {
                if (!AreEqual(a[index], b[index]))
                    return false;
            }

            return true;
        }

        public abstract T MinValue { get; }

        public abstract T MaxValue { get; }

        public T Min(T a, T b)
        {
            if (Compare(a, b) < 0)
                return a;
            else
                return b;
        }

        public T Max(T a, T b)
        {
            if (Compare(a, b) > 0)
                return a;
            else
                return b;
        }

        public abstract T Zero { get; }

        public abstract T NegativeInfinity { get; }

        public abstract T PositiveInfinity { get; }

        public abstract T Add(T a, T b);

        public abstract T Subtract(T a, T b);

        public abstract T Multiply(T a, T b);

        public abstract T DistanceSquaredBetweenPoints(T[] a, T[] b);

        #endregion
    }
}
