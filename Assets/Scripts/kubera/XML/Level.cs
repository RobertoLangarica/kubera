using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Level
{

	/**
	 * Nombre que tendra el nivel con el siguiente formato:
	 * Numero de 4 digitos minimo
	 * ej. 0001
	 **/ 
	[XmlAttribute("name")]public string name; 

	/**
	 * Mundo al que pertenece el nivel
	 **/ 
	[XmlAttribute("world")]public int world;

	[XmlAttribute("isBoss")]public bool isBoss;

	[XmlAttribute("friendsNeeded")]	public int friendsNeeded;
	[XmlAttribute("starsNeeded")]	public int starsNeeded;
	[XmlAttribute("gemsNeeded")]	public int gemsNeeded;

	[XmlAttribute("goal")]public string goal;

	/**
	 * Cantidad de movimientos que se tienen para el nivel
	 **/ 
	[XmlAttribute("moves")]public int moves;

	/**
	 * Puntos para obtener una estrella al pasar el nivel
	 **/ 
	[XmlAttribute("scoreToStar1")]public int scoreToStar1;

	/**
	 * Puntos para obtener una estrella al pasar el nivel
	 **/ 
	[XmlAttribute("scoreToStar2")]public int scoreToStar2;

	/**
	 * Puntos para obtener una estrella al pasar el nivel
	 **/ 
	[XmlAttribute("scoreToStar3")]public int scoreToStar3;

	[XmlAttribute("unblockWordHint")]public bool unblockWordHint;
	[XmlAttribute("unblockBomb")]public bool unblockBomb;
	[XmlAttribute("unblockBlock")]public bool unblockBlock;
	[XmlAttribute("unblockRotate")]public bool unblockRotate;
	[XmlAttribute("unblockDestroy")]public bool unblockDestroy;
	[XmlAttribute("unblockWildcard")]public bool unblockWildcard;


	/**
	 * Listado de piezas separado por ',' con el siguiente formato:
	 * (Cantidad)_Pieza
	 * ej. 2_A0
	 * */
	[XmlAttribute("pieces")]public string pieces;


	/**
	 * Cadena de enteros separdos por ',' que representan la grid de juego.
	 * Hay que consultar a Cell para saber que significan los enteros
	 **/ 
	[XmlAttribute("grid")]public string grid;

	/**
	 * Letras separadas por ',' con el siguiente formato:
	 * (Cantidad en numero)_(Letra)_(Valor en numero y letra)_(Tipo)
	 * ej. 5_b_1_3,2_a_x2_5
	 **/ 
	[XmlAttribute("lettersPool")]public string lettersPool;

	//Pool de letras obstaculo. Tienen el mismo formato que las letras
	[XmlAttribute("obstacleLettersPool")]public string obstacleLettersPool;

	[XmlAttribute("tutorialLettersPool")]public string tutorialLettersPool;

	[XmlAttribute("difficulty")]public int difficulty;//Dificultad que de momento no se utiliza

	public int index{get{return int.Parse(name);}}

}
