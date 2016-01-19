﻿using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Level
{

	/**
	 * Nombre que tendra el nivel con el siguiente formato:
	 * Numero de 4 digitos minimo
	 * ej. 00001
	 **/ 
	[XmlAttribute("name")]public string name; 

	[XmlAttribute("difficulty")]public int difficulty;//Dificultad que de momento no se utiliza
	
	/**
	 * Letras separadas por ',' con el siguiente formato:
	 * (Cantidad en numero)_(Letra)_(Valor en numero y letra)_(Tipo)
	 * ej. 5_b_1_3,2_a_x2_5
	 **/ 
	[XmlAttribute("lettersPool")]public string lettersPool;

	//Pool de letras obstaculo. Tienen el mismo formato que las letras
	[XmlAttribute("obstacleLettersPool")]public string obstacleLettersPool;


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
}
