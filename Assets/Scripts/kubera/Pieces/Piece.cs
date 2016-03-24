using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public enum EPieceType
{
	NONE,
	AQUA,
	BLUE,
	GREEN,
	MAGENTA,
	RED,
	YELLOW,
	GREY,
	BLACK,
	LETTER,
	LETTER_OBSTACLE,
	LETTER_DESTROY_POWERUP,
	LETTER_ROTATE_POWERUP,
	LETTER_WILD_POWERUP,
	LETTER_BLOCK_POWERUP
}

public class Piece : MonoBehaviour 
{
	private static int ISNTANCE_COUNT = 0;

	public Color COLOR_AQUA				= new Color(0.376f, 0.698f, 0.639f); 
	public Color COLOR_BLACK			= new Color(0.180f, 0.188f, 0.192f);
	public Color COLOR_LETTER_OBSTACLE	= new Color(0.180f, 0.188f, 0.192f);
	public Color COLOR_BLUE				= new Color(0.133f, 0.565f, 0.945f);
	public Color COLOR_GREEN			= new Color(0.192f, 0.545f, 0.263f);
	public Color COLOR_GREY				= new Color(0.392f, 0.514f, 0.584f);
	public Color COLOR_MAGENTA			= new Color(0.643f, 0.059f, 0.482f);
	public Color COLOR_RED				= new Color(0.965f, 0.282f, 0.427f);
	public Color COLOR_YELLOW			= new Color(0.976f, 0.627f, 0.000f);
	public Color COLOR_NONE				= new Color(1, 1, 1);

	public GameObject[] squares;
	public EPieceType starterType;

	protected EPieceType _currentType;
	protected Color rendererColor;

	[HideInInspector]public GameObject parent;

	public int rotateTimes = 0;

	[HideInInspector]
	public int rotateCount = 0;

	[HideInInspector]
	public int createdIndex = 0;

	//[HideInInspector]
	//public  Guid guid;
	[HideInInspector]
	public int id;

	[HideInInspector]
	public Vector3 positionOnScene;

	[HideInInspector]
	public Vector3 initialPieceScale;

	void Start () 
	{
		squares = new GameObject[transform.childCount];

		for(int i=0; i<transform.childCount; i++)
		{
			squares [i] = transform.GetChild (i).gameObject;
		}
	
		currentType = starterType;
	}

	//HACK: esta fallando GetInstanceID gracias a que las piezas se clonan con Resources.Load
	//por ahora dejamos GUID (que ademas si se inicializa en la declaracion sufre del mismo bug de repeticion)
	/*public void initializeId()
	{
		guid = System.Guid.NewGuid();
	}*/

	//HACK: Que onda con GetInstanceID
	public void setUniqueId()
	{
		id = ++ISNTANCE_COUNT;
	}

	public void printID()
	{
		Debug.Log("ID: "+this.GetInstanceID());
	}

	protected void updateColorBasedOnType()
	{
		if(currentType == EPieceType.LETTER_OBSTACLE)
		{
			return;
		}

		Color color = getColorOfType(currentType);
			
		foreach(GameObject piece in squares)
		{
			piece.GetComponent<SpriteRenderer>().color = color;
		}
	}

	public Color getColorOfType(EPieceType type)
	{
		switch(type)
		{
		case EPieceType.AQUA:
			return COLOR_AQUA;
		case EPieceType.BLACK:
			return COLOR_BLACK;
		case EPieceType.LETTER_OBSTACLE:
			return COLOR_LETTER_OBSTACLE;
		case EPieceType.BLUE:
			return COLOR_BLUE;
		case EPieceType.GREEN:
			return COLOR_GREEN;
		case EPieceType.GREY:
			return COLOR_GREY;
		case EPieceType.MAGENTA:
			return COLOR_MAGENTA;
		case EPieceType.RED:
			return COLOR_RED;
		case EPieceType.YELLOW:
			return COLOR_YELLOW;
		}

		return COLOR_NONE;
	}
		
	public EPieceType currentType
	{
		get{return _currentType;}

		set
		{
			starterType = value;//Para evitar que el Start() modifique este llamado
			_currentType = value;

			updateColorBasedOnType();
		}
	}
}
