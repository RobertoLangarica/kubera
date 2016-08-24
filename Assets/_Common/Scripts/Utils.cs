using UnityEngine;
using System.Collections;

//Version: 1.0.0

/// <summary>
/// A variety of utils for different functionalities.
/// </summary>
public class Utils
{
	/// <summary>
	/// Gets the random index from the float array, this array must have values between 0-1 that all summed give 1
	///  and sorted in ascendant order, the Value will be taken randomly as a Roulette or by weigth.
	/// The min value must be 0.01, as it needs to represent at least 1%.
	/// </summary>
	/// <returns>The random index from roulette.</returns>
	/// <param name="roulette">Roulette.</param>
	public static int GetRandomIndexFromRoulette(ref float[] roulette)
	{
		float randomValue = Random.Range(0.01f,1.0f);
		float Accumulated = 0;
		for(int i = 0 ; i < roulette.Length; i++)
		{
			Accumulated += roulette[i];
			if( randomValue <= Accumulated)
			{
				return i;
			}
		}
		return 0;
	}

	/// <summary>
	/// Creates a probability roulette from an array of weights. For this to generate a useful array the minimum value
	/// must be at least 1% of the summed values.
	/// </summary>
	/// <returns>The probabilities roulette.</returns>
	/// <param name="weights">The weights array.</param>
	public static float[] CreateRouletteFromProbabilities(ref int[] weights)
	{
		float[] roulette = new float[weights.Length];
		float total = 0.0f;
		for(int i = 0; i < weights.Length; i++)
		{
			total += weights[i];
		}
		for(int i = 0; i < weights.Length; i++)
		{
			roulette[i] = (float)weights[i]/total;
		}

		return roulette;
	}

}
