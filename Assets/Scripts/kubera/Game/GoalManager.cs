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

	protected string _currentCondition;
	public string currentCondition { get{return _currentCondition;} protected set{_currentCondition = value;}}

	protected List<string> _goalLetters;
	public List<string> goalLetters { get{return _goalLetters;} protected set{_goalLetters = value;}}

	protected List<string> _usedLetters;
	public List<string> usedLetters { get{return _usedLetters;} protected set{_usedLetters = value;}}

	protected int _goalObstacles;
	public int goalObstacles  { get{return _goalObstacles;} protected set{_goalObstacles = value;}}
	public int obstaclesCount;

	protected int _goalPoints;
	public int goalPoints  { get{return _goalPoints;} protected set{_goalPoints = value;}}
	public int pointsCount;

	protected int _goalWordsCount;
	public int goalWordsCount  { get{return _goalWordsCount;} protected set{_goalWordsCount = value;}}

	protected int _wordsCount;
	public int wordsCount  { get{return _wordsCount;} protected set{_wordsCount = value;}}

	protected List<string> _goalWords;
	public List<string> goalWords  { get{return _goalWords;} protected set{_goalWords = value;}}

	protected List<string> _goalWordsToShow;
	public List<string> goalWordsToShow  { get{return _goalWordsToShow;} protected set{_goalWordsToShow = value;}}

	public delegate void DOnGoalAchieved();
	public DOnGoalAchieved OnGoalAchieved;

	public delegate void DOnLetterFound(string letter);
	public DOnLetterFound OnLetterFound;

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

		if(pointsCount >= goalPoints && currentCondition == POINTS)
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
			processWordForGoalWords(word);
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

		if(obstaclesCount >= goalObstacles)
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
				OnLetterFound (word[i].abcChar.character);

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

		return result;
	}

	public object getGoalConditionParameters()
	{
		
		switch (currentCondition) 
		{
		case LETTERS:
			return goalLetters;
		case OBSTACLES:
			return goalObstacles;
		case POINTS:
			return goalPoints;
		case WORDS_COUNT:
			return goalWordsCount;
		case SYNONYMOUS:			
		case WORD:			
		case ANTONYMS:
			return goalWordsToShow;
		default:
			return null;
		}
	}
}
