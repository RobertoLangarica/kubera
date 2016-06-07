using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SerializedSequence
{
	public 	string sname = "";

	[System.Serializable]
	public class MySprite
	{
		public Sprite sprite;

		public int frameLength;
		[HideInInspector]public int id;
		public Vector3 pivot;
		public Vector3 offset;
		
		/*public ExtendedSprite(float offsetX,float offsetY, float pivotX
	                      , float pivotY, int frameLength = 1,int id = -1)*/
		public MySprite()
		{
			/*offset = Camera.main.ScreenToWorldPoint(new Vector3(offsetX,offsetY,0));
		pivot = Camera.main.ScreenToWorldPoint(new Vector3(pivotX,pivotY,0));
		this.frameLength 		= frameLength < 1 ? 1 : frameLength;
		this.id					= id;*/
			
			offset = Vector3.zero;
			pivot = Vector2.zero;
			frameLength = 1;
		} 
	}
	
	public	bool isLoop	= false;
	public int FPS;
	protected int _currentFPSCalculated;
	protected float inverseFPS;

	public delegate void DOnFinish();
	public delegate void DOnFrame();
	public DOnFinish onSequenceEnd;
	public DOnFrame onFrame;
	public	string fx =  "";
	public int fxFrame;
	public SpriteRenderer imageReference;//Imagen de refeencia en la que se actualiza la textura
	public int currentFrame = 0;//Indice del frame de la secuencia
	public MySprite[] frames;//Informacion de los frames
	protected MySprite[] completeFrames;
	[HideInInspector]public MySprite currentSprite;


	protected float frameTime = 0.0f;
	protected float frameIncrement;
	
	public SerializedSequence()
	{
		//que no truene llamar el delegate sin ninguna inicializacion
		onSequenceEnd += foo;
		onFrame += foo;

		//Sprite foo default
		currentSprite = new MySprite();
		currentSprite.id = -1;
	}

	public SerializedSequence(SerializedSequence copyFrom)
	{
		//que no truene llamar el delegate sin ninguna inicializacion
		onSequenceEnd += foo;
		onFrame += foo;

		FPS = copyFrom.FPS;
		FPSUpdate();

		//Sprite foo default
		currentSprite = new MySprite();
		currentSprite.id = -1;

		imageReference = copyFrom.imageReference;

		frames = (MySprite[])copyFrom.frames.Clone();
		readAndFillFrames();
	}
	
	void foo(){}
	
	public void FPSUpdate()
	{
		_currentFPSCalculated = FPS;
		inverseFPS = 1.0f/FPS;
	}

	public void setFramesOffset(Vector3 offset,int initialIndex = 0,int count = -1)
	{
		count = count < 0 ? frames.Length : initialIndex + count;

		for(int i = initialIndex; i < count; i++)
		{
			frames[i].offset = offset;
		}

		//Actualizamos los frames individuales
		readAndFillFrames();
	}

	public void setFramesOffsetX(float offsetX,int initialIndex = 0,int count = -1)
	{
		count = count < 0 ? frames.Length : initialIndex + count;
		
		for(int i = initialIndex; i < count; i++)
		{
			frames[i].offset.x = offsetX;
		}
		
		//Actualizamos los frames individuales
		readAndFillFrames();
	}

	public void setFramesOffsetY(float offsetY,int initialIndex = 0,int count = -1)
	{
		count = count < 0 ? frames.Length : initialIndex + count;
		
		for(int i = initialIndex; i < count; i++)
		{
			frames[i].offset.y = offsetY;
		}
		
		//Actualizamos los frames individuales
		readAndFillFrames();
	}
	
	public void readAndFillFrames()
	{
		int count = 0;

		for(int i= 0; i < frames.Length; i++)
		{
			frames[i].frameLength = frames[i].frameLength == 0 ? 1: frames[i].frameLength;

			count += frames[i].frameLength;
		}

		completeFrames = new MySprite[count];
		int c = 0;

		for(int i= 0; i < frames.Length; i++)
		{
			frames[i].id = i;

			for(int j = 0; j < frames[i].frameLength; j++)
			{
				
				completeFrames[c++] = frames[i];
			}
		}
	}
	
	/**
	 * Actualiza el frame de la animacion 
	 * @param deltaTime
	 * 
	 */
	public void update(float deltaTime,TransformInfo transformInfo)
	{
		#if UNITY_EDITOR
		if(_currentFPSCalculated != FPS)
		{
			FPSUpdate();
		}
		#endif

		onFrame();

		frameTime += deltaTime;

		if(frameTime > inverseFPS)
		{
			//Hay que cambiar de frame
			frameIncrement = frameTime*FPS;
			
			if(fx.Length > 0)
			{
				if(currentFrame < fxFrame && currentFrame + frameIncrement >= fxFrame)
				{
					//TODO Sonidos
					//Config.playFx(fx);
				}
			}
			
			frameTime = frameTime % inverseFPS;
			
			if(currentFrame == completeFrames.Length-1)
			{
				//Termino?
				currentFrame += Mathf.FloorToInt(frameIncrement);
				
				if(isLoop)
				{
					currentFrame = currentFrame % completeFrames.Length;
				}
				else
				{
					currentFrame = completeFrames.Length-1;
					frameTime = 0;					
				}
				
				onSequenceEnd();
			}
			else
			{
				//Si no es loop va forzar el ultimo frame
				currentFrame += Mathf.FloorToInt(frameIncrement);		
				
				if(currentFrame >= completeFrames.Length)
				{
					if(isLoop)
					{
						currentFrame = currentFrame % completeFrames.Length;
						
						onSequenceEnd();
					}
					else
					{
						currentFrame = completeFrames.Length-1;
					}
				}
			}

			//el transform que siempre se actualice
			if(currentSprite.id != completeFrames[currentFrame].id)
			{
				updateImage(transformInfo);
			}	
		}
	}
	
	public void updateImage(TransformInfo transformInfo)
	{
		updateImage ();

		Vector3 pos = (currentSprite.pivot+currentSprite.offset);
		pos.x *= transformInfo.scale.x + transformInfo.offset.x;
		pos.y *= transformInfo.scale.y + transformInfo.offset.y;
		pos.z = imageReference.gameObject.transform.position.z;
		
		//imageReference.gameObject.transform.localPosition = pos;
		//imageReference.gameObject.transform.localScale = transformInfo.scale;
	}

	public void updateImage()
	{
		imageReference.sprite = completeFrames[currentFrame].sprite;
		currentSprite = completeFrames[currentFrame];
	}

	public void reset()
	{
		currentFrame = 0;
		frameTime = 0;
	}
}
