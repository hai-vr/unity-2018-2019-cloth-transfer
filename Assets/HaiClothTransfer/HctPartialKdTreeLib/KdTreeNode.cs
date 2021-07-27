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
using System.Text;

namespace HaiClothTransfer.HctPartialKdTreeLib
{
    [Serializable]
	public class KdTreeNode<TValue>
	{
		public KdTreeNode()
		{
		}

		public KdTreeNode(double[] point, TValue value)
		{
			Point = point;
			Value = value;
		}

		public double[] Point;
		public TValue Value = default(TValue);

		internal KdTreeNode<TValue> LeftChild = null;
		internal KdTreeNode<TValue> RightChild = null;

		internal KdTreeNode<TValue> this[int compare]
		{
			get
			{
				if (compare <= 0)
					return LeftChild;
				else
					return RightChild;
			}
			set
			{
				if (compare <= 0)
					LeftChild = value;
				else
					RightChild = value;
			}
		}

		public bool IsLeaf
		{
			get
			{
				return (LeftChild == null) && (RightChild == null);
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			for (var dimension = 0; dimension < Point.Length; dimension++)
			{
				sb.Append(Point[dimension].ToString() + "\t");
			}

			if (Value == null)
				sb.Append("null");
			else
				sb.Append(Value.ToString());

			return sb.ToString();
		}
	}
}
