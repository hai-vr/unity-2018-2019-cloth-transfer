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
using HaiClothTransfer.HctPartialKdTreeLib.Math;

namespace HaiClothTransfer.HctPartialKdTreeLib
{
	public class NearestNeighbourList<TItem>
	{
		public NearestNeighbourList(int maxCapacity, DoubleMath distanceMath)
		{
			this.maxCapacity = maxCapacity;
			this.distanceMath = distanceMath;

			queue = new PriorityQueue<TItem>(maxCapacity, distanceMath);
		}

		private PriorityQueue<TItem> queue;

		private DoubleMath distanceMath;

		private int maxCapacity;
		public int MaxCapacity { get { return maxCapacity; } }

		public int Count { get { return queue.Count; } }

		public bool Add(TItem item, double distance)
		{
			if (queue.Count >= maxCapacity)
			{
				// If the distance of this item is less than the distance of the last item
				// in our neighbour list then pop that neighbour off and push this one on
				// otherwise don't even bother adding this item
				if (distanceMath.Compare(distance, queue.GetHighestPriority()) < 0)
				{
					queue.Dequeue();
					queue.Enqueue(item, distance);
					return true;
				}
				else
					return false;
			}
			else
			{
				queue.Enqueue(item, distance);
				return true;
			}
		}

		public double GetFurtherestDistance()
		{
			if (Count == 0)
				throw new Exception("List is empty");
			else
				return queue.GetHighestPriority();
		}

		public TItem RemoveFurtherest()
		{
			return queue.Dequeue();
		}

		public bool IsCapacityReached
		{
			get { return Count == MaxCapacity; }
		}
	}
}
