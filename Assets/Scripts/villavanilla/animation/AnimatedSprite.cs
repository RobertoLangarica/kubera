using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TransformInfo),typeof(SpriteRenderer))]
public class AnimatedSprite: MonoBehaviour 
{
	public int currentSequenceIndex;
	public bool autoUpdate = false;
	public List<SerializedSequence> sequences;

	public SpriteRenderer spriteRenderer;
	[HideInInspector]public TransformInfo transformation;

	void Awake()
	{
		this.transformation = GetComponent<TransformInfo>();

		spriteRenderer = GetComponent<SpriteRenderer>();
		//Se convierten en una lista de frames las secuencias
		for(int i = 0; i < sequences.Count; i++)
		{
			sequences[i].imageReference = spriteRenderer;
			sequences[i].FPSUpdate();
			sequences[i].readAndFillFrames();
		}

		//Inicializamos la imagen con el primer frame de la primer secuencia
		this.sequences[currentSequenceIndex].updateImage(this.transformation);

	}
		
	public void updateSequence(float time)
	{
		sequences[currentSequenceIndex].update(time,transformation);
	}

	public void changeSequence(int newSequenceIndex, bool resetCurrentSequence = true)
	{
		if(newSequenceIndex != currentSequenceIndex)
		{
			if(resetCurrentSequence)
			{
				sequences[currentSequenceIndex].reset();
			}

			currentSequenceIndex = newSequenceIndex;
			//Inicializamos la imagen con el primer frame de la primer secuencia
			this.sequences[currentSequenceIndex].updateImage(this.transformation);
		}
	}

	void Update()
	{
		if(autoUpdate)
		{
			updateSequence(Time.deltaTime);	
		}
	}
}
