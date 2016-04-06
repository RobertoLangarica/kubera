using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoalManager : MonoBehaviour 
{
	public const string LETTERS		= "letters";//Listado de letras a utilizar
	public const string WORDS_COUNT	= "words";//Cantidad de palabras
	public const string WORD		= "word";//Palabra que completar
	public const string SYNONYMOUS	= "sin";//Un sinonimo de la lista
	public const string ANTONYMS	= "ant";//Un antonimo de la lista
	public const string OBSTACLES	= "obstacles";//Obstaculos que remover
	public const string POINTS		= "points";//Una cantidad de puntos que alcanzar

	public string currentCondition { get; }

	public List<string> goalLetters { get; }
	public List<string> usedLetters { get; }

	public int goalObstacles { get; }
	public int obstaclesCount;

	public int goalPoints { get; }
	public int pointsCount;

	public int goalWordsCount { get; }
	public int wordsCount { get; }

	public List<string> goalWords { get; }
	public List<string> goalWordsToShow { get; }

	public delegate void DOnGoalAchieved();
	public DOnGoalAchieved OnGoalAchieved;

	public void initializeFromString(string goal)
	{
		string[] data = goal.Split('-');
		string condition = data[0];
		string parameters= data[1];

		currentCondition = condition;

		switch(condition)
		{
		case LETTERS:
			configureForLetters(parameters);
			break;
		case OBSTACLES:
			configureForObstacles(parameters);
			break;
		case POINTS:
			configureForPoints(parameters);
			break;
		case WORDS_COUNT:
			configureForWordsCount(parameters);
			break;
		case WORD:
		case SYNONYMOUS:
		case ANTONYMS:
			configureForWords(parameters);
			break;
		}
	}

	private void configureForLetters(string parameters)
	{
		string[] letters = parameters.Split (',');
		string[] data;
		int letterCount;

		goalLetters = new List<string>();
		usedLetters = new List<string>();

		for(int i = 0; i< letters.Length; i++)
		{
			data = letters[i].Split ('_');
			letterCount = int.Parse (data[0]);
			addMultipleTimes<string>(letterCount,data[1],goalLetters);
		}
	}

	private void addMultipleTimes<T>(int times,T item,List<T> target)
	{
		for(int i = 0; i < times; i++)
		{
			target.Add(item);
		}
	} 

	private void configureForObstacles(string parameters)
	{
		goalObstacles = int.Parse(parameters);
	}

	private void configureForPoints(string parameters)
	{
		goalPoints = int.Parse(parameters);
	}

	private void configureForWordsCount(string parameters)
	{
		goalWordsCount = int.Parse(parameters);
	}

	/**
	 * @param parameters: csv palabraAcentuada_sinAcentuar , palabraAcentuada_sinAcentuar...
	 **/ 
	private void configureForWords(string parameters)
	{
		string[] words = parameters.Split(',');

		goalWords = new List<string>(words.Length);
		goalWordsToShow = new List<string>(words.Length);

		for(int i = 0; i < words.Length; i++)
		{
			string[] data = words[i].Split('_');

			goalWordsToShow.Add(data[0]);
			goalWords.Add(data[1]);
		}
	}

	public void submitPoints(int points)
	{
		pointsCount += points;

		if(pointsCount >= goalPoints)
		{
			if(OnGoalAchieved != null)
			{
				OnGoalAchieved();
			}
		}
	}

	public void submitWord(List<Letter> word)
	{
		switch(currentCondition)
		{
		case OBSTACLES:
			processWordForObstacles(word);
			break;
		case LETTERS:
			processWordForLetters(word);
			break;
		case WORDS_COUNT:
			incrementWords();
			break;
		case WORD:
		case SYNONYMOUS:
		case ANTONYMS:
			processWordForGoalWords();
			break; 
		}
	}

	private void processWordForObstacles(List<Letter> word)
	{
		int count = 0;

		foreach(Letter letter in word)
		{
			if(letter.type == Letter.EType.OBSTACLE)
			{
				count++;
			}
		}

		submitObstacles(count);
	}

	private void submitObstacles(int obstacles)
	{
		obstaclesCount += obstacles;

		if(obstaclesCount >= goalPoints)
		{
			if(OnGoalAchieved != null)
			{
				OnGoalAchieved();
			}
		}
	}

	private void processWordForLetters(List<Letter> word)
	{
		int index;

		for(int i = 0; i < word.Count; i++)
		{
			index = goalLetters.IndexOf(word[i].abcChar.character);

			if(index != -1)
			{
				usedLetters.Add(goalLetters[index]);
				goalLetters.RemoveAt(index);

				if(goalLetters.Count == 0)
				{
					if(OnGoalAchieved != null)
					{
						OnGoalAchieved();
					}
					break;
				}
			}
		}
	}

	private void incrementWords()
	{
		if(++wordsCount >= goalWordsCount)
		{
			if(OnGoalAchieved != null)
			{
				OnGoalAchieved();
			}
		}
	}

	private void processWordForGoalWords(List<Letter> word)
	{
		string sWord = getStringFromWord(word);

		if(goalWords.IndexOf(sWord) != -1)
		{
			if(OnGoalAchieved != null)
			{
				OnGoalAchieved();
			}
		}
	}

	private string getStringFromWord(List<Letter> word)
	{
		string result = "";

		foreach(Letter letter in word)
		{
			result = result + letter.abcChar.character;
		}

		result.ToLower();
	}
}
