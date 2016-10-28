using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomPool<T> 
{
	private List<T> original;
	private List<T> randomized;
	private List<T> ordered;

	public RandomPool(List<T> source)
	{
		original = new List<T>(source);//clone
		randomized = randomizeList(original);
		ordered = new List<T> (original);
	}

	public T getNext()
	{
		if(ordered.Count == 0)
		{
			ordered = new List<T> (original);
		}

		T temporal = ordered[0];
		ordered.RemoveAt(0);

		return temporal;
	}

	public T getNextRandomized()
	{
		if(randomized.Count == 0)
		{
			randomized = randomizeList(original);
		}

		T temporal = randomized[0];
		randomized.RemoveAt(0);

		return temporal;
	}

	protected List<T> randomizeList(List<T> target)
	{
		List<T> result = new List<T>();
		List<T> temporal = new List<T>(target);
		int index;

		while(temporal.Count > 0)
		{
			index = Random.Range(0,temporal.Count);
			result.Add(temporal[index]);
			temporal.RemoveAt(index);
		}

		return result;
	}
}
