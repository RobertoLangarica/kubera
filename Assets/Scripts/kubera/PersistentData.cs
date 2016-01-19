﻿using UnityEngine;
using System.Collections;

/**
 * clase que ponemos en un prefab que va existir 
 * desde el inicio del juego con informacion que debe ser permanente o compartida sin 
 * necesidad de ir directamente dirigida a alguien.
 * 
 * */
public class PersistentData : MonoBehaviour 
{
	//Solo existe una sola instancia en todo el juego de este objeto
	public static PersistentData instance;

	[HideInInspector]
	public int currentLevel;
	[HideInInspector]
	public Levels data;

	//El idioma en que se encuentra configurado actualmente el juego
	private string currentLanguage;
	private bool destroyed = false;//Indica si el objeto ya se destruyo

	void Awake() 
	{
		if(instance)
		{
			//No se si al mandar destroyed en el awake llegue entrar a start pero no corremos riesgos
			destroyed = true;
			Destroy(instance);
			return;
		}

		instance = this;

		DontDestroyOnLoad(this);

		TextAsset tempTxt = (TextAsset)Resources.Load ("levels");
		data = Levels.LoadFromText(tempTxt.text);
		FindObjectOfType<ABCDataStructure>().InitializeAlfabet(tempTxt.text);
	}

	void Start()
	{
		if(destroyed)
		{
			return;
		}


		TextAsset abc = Resources.Load("ABCData/ABC_spanish") as TextAsset;
		//Debug.Log(abc);
	}
}