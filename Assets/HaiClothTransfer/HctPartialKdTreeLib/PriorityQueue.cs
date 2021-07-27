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
	struct ItemPriority<TItem>
	{
		public TItem Item;
		public double Priority;
	}

	public class PriorityQueue<TItem>
	{
		public PriorityQueue(int capacity, DoubleMath priorityMath)
		{
			if (capacity <= 0)
				throw new ArgumentException("Capacity must be greater than zero");

			this.capacity = capacity;
			queue = new ItemPriority<TItem>[capacity];

			this.priorityMath = priorityMath;
		}

		///<remarks>
		///This constructor will use a default capacity of 4.
		///</remarks>
		public PriorityQueue(DoubleMath priorityMath)
		{
			this.capacity = 4;
			queue = new ItemPriority<TItem>[capacity];

			this.priorityMath = priorityMath;
		}

		private DoubleMath priorityMath;

		private ItemPriority<TItem>[] queue;

		private int capacity;

		private int count;
		public int Count { get { return count; } }

		// Try to avoid unnecessary slow memory reallocations by creating your queue with an ample capacity
		private void ExpandCapacity()
		{
			// Double our capacity
			capacity *= 2;

			// Create a new queue
			var newQueue = new ItemPriority<TItem>[capacity];

			// Copy the contents of the original queue to the new one
			Array.Copy(queue, newQueue, queue.Length);

			// Copy the new queue over the original one
			queue = newQueue;
		}

		public void Enqueue(TItem item, double priority)
		{
			if (++count > capacity)
				ExpandCapacity();

			int newItemIndex = count - 1;

			queue[newItemIndex] = new ItemPriority<TItem> { Item = item, Priority = priority };

			ReorderItem(newItemIndex, -1);
		}

		public TItem Dequeue()
		{
			TItem item = queue[0].Item;

			queue[0].Item = default(TItem);
			queue[0].Priority = priorityMath.MinValue;

			ReorderItem(0, 1);

			count--;

			return item;
		}

		private void ReorderItem(int index, int direction)
		{
			if ((direction != -1) && (direction != 1))
				throw new ArgumentException("Invalid Direction");

			var item = queue[index];

			int nextIndex = index + direction;

			while ((nextIndex >= 0) && (nextIndex < count))
			{
				var next = queue[nextIndex];

				int compare = priorityMath.Compare(item.Priority, next.Priority);

				// If we're moving up and our priority is higher than the next priority then swap
				// Or if we're moving down and our priority is lower than the next priority then swap
				if (
					((direction == -1) && (compare > 0))
					||
					((direction == 1) && (compare < 0))
				)
				{
					queue[index] = next;
					queue[nextIndex] = item;

					index += direction;
					nextIndex += direction;
				}
				else
					break;
			}
		}

		public TItem GetHighest()
		{
			if (count == 0)
				throw new Exception("Queue is empty");
			else
				return queue[0].Item;
		}

		public double GetHighestPriority()
		{
			if (count == 0)
				throw new Exception("Queue is empty");
			else
				return queue[0].Priority;
		}
	}
}
