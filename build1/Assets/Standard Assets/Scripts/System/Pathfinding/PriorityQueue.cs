using System.Collections.Generic;
using System;

/// <summary>
/// very simple prioirty queue, not optimized at all
/// </summary>
/// <exception cref='InvalidOperationException'>
/// Is thrown when the attempting to dequeue an empty priority queue.
/// </exception>
public class PriorityQueue<T>
{
	// using dictionary for fast access & easy coding
	private Dictionary<int, List<T>> objects = new Dictionary<int, List<T>>();
	private int lowestPriority = int.MaxValue; // init to max
	private int count = 0;
	
	public PriorityQueue()
	{
		// N/A
	}
	
	/// <summary>
	/// Enqueue the specified object with the given priority
	/// </summary>
	/// <param name='priority'>
	/// Priority.
	/// </param>
	/// <param name='obj'>
	/// Object.
	/// </param>
	public void enqueue(int priority, T obj) {
		if (priority < lowestPriority) {
			lowestPriority = priority;
		}
		if (objects.ContainsKey(priority)) {
			objects[priority].Add(obj);
		} else {
			objects.Add(priority, new List<T>());	
			objects[priority].Add(obj);
		}
		count++;
	}
	
	/// <summary>
	/// Dequeue this instance.
	/// </summary>
	/// <exception cref='InvalidOperationException'>
	/// Is thrown when the attempting to dequeue an empty priority queue.
	/// </exception>
	public T dequeue() {
		if (isEmpty()) {
			throw new InvalidOperationException("Empty queue");	
		}
		count--;
		// store the object to dequeue and remove from list
		T objToReturn = objects[lowestPriority][0];
		objects[lowestPriority].Remove(objToReturn);
		
		// check if any more obj in queue with same priority
		if (objects[lowestPriority].Count == 0) {
			// no more, so let's remove that
			objects.Remove(lowestPriority);
			
			// restore lowest priority to max int value
			lowestPriority = int.MaxValue;	
			if (!isEmpty()) {
			 	// find the next lowest priority item
				foreach(int p in objects.Keys) {
					if (p < lowestPriority) {
						lowestPriority = p;
					}
				}
			}
			
		}
		
		return objToReturn;
	}
	
	/// <summary>
	/// Is the priority queue empty.
	/// </summary>
	/// <returns>
	/// True if empty, false otherwise.
	/// </returns>
	public bool isEmpty() {
		return (count == 0);	
	}

}

