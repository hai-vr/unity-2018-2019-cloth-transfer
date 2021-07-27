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

using HaiClothTransfer.HctPartialKdTreeLib.Math;

namespace HaiClothTransfer.HctPartialKdTreeLib
{
    public struct HyperRect
	{
		private double[] minPoint;
		public double[] MinPoint
		{
			get
			{
				return minPoint;
			}
			set
			{
				minPoint = new double[value.Length];
				value.CopyTo(minPoint, 0);
			}
		}

		private double[] maxPoint;
		public double[] MaxPoint
		{
			get
			{
				return maxPoint;
			}
			set
			{
				maxPoint = new double[value.Length];
				value.CopyTo(maxPoint, 0);
			}
		}

		public static HyperRect Infinite(int dimensions, DoubleMath math)
		{
            var rect = new HyperRect
            {
                MinPoint = new double[dimensions],
                MaxPoint = new double[dimensions]
            };

            for (var dimension = 0; dimension < dimensions; dimension++)
			{
				rect.MinPoint[dimension] = math.NegativeInfinity;
				rect.MaxPoint[dimension] = math.PositiveInfinity;
			}

			return rect;
		}

		public double[] GetClosestPoint(double[] toPoint, DoubleMath math)
		{
			double[] closest = new double[toPoint.Length];

			for (var dimension = 0; dimension < toPoint.Length; dimension++)
			{
				if (math.Compare(minPoint[dimension], toPoint[dimension]) > 0)
				{
					closest[dimension] = minPoint[dimension];
				}
				else if (math.Compare(maxPoint[dimension], toPoint[dimension]) < 0)
				{
					closest[dimension] = maxPoint[dimension];
				}
				else
					// Point is within rectangle, at least on this dimension
					closest[dimension] = toPoint[dimension];
			}

			return closest;
		}

		public HyperRect Clone()
		{
            var rect = new HyperRect
            {
                MinPoint = MinPoint,
                MaxPoint = MaxPoint
            };
            return rect;
		}
	}
}
