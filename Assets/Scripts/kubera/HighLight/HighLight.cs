using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HighLight : MonoBehaviour 
{
	public enum EHIGHLIGHPARENTTYPE
	{
		NONE,
		CELL,
		HUD
	}

	public Image image;
	public SpriteRenderer sprite_renderer;
	public Cell cellParent;

	public float minAlpha = 0.5f;
	public float animStep = 0.02f;

	public bool hasBeat;
	public bool lastForever;
	public float beatTime = 1;
	public float beatStrength = 0.1f;
	public int beatVibrato = 1;
	public float beatRandom = 0;

	public GameObject borderStars;

	public EHIGHLIGHPARENTTYPE parentType;

	protected bool isScaled;
	protected bool isDescending;
	protected bool startAnim;

	protected ParticleSystem particles;

	protected Vector3 previousScale;

	protected bool isActive;
	protected List<HighLightManager.EHighLightType> suscribedTypes = new List<HighLightManager.EHighLightType> ();
	protected List<HighLightManager.EHighLightStatus> suscribedStatus = new List<HighLightManager.EHighLightStatus> ();

	void Start()
	{
		//particles = borderStars.GetComponent<ParticleSystem> ();
		//borderStars.SetActive (true);

		if (lastForever) 
		{
			initAnim ();
		}
	}

	void Update()
	{
		if (startAnim) 
		{
			if (isDescending) 
			{
				if (getModifiedAlpha(-animStep) <= minAlpha) 
				{
					isDescending = false;
				}
			} 
			else 
			{

				if (getModifiedAlpha(animStep) >= 1) 
				{
					isDescending = true;
				}
			}
		}
	}

	public bool activateHighLight(HighLightManager.EHighLightType type,HighLightManager.EHighLightStatus status)
	{
		int index = suscribedTypes.IndexOf (type);
		previousScale = transform.localScale;

		if (index < 0) 
		{
			if (suscribedTypes.Count == 0) 
			{
				gameObject.SetActive (true);

				if (hasBeat) 
				{
					Invoke ("highLightBeat", 2);
				}
			}

			suscribedTypes.Add (type);
			suscribedStatus.Add (status);

			updateColor ();

			if (!lastForever) 
			{
				initAnim ();
			}

			return true;
		}
		return false;
	}

	protected virtual void updateColor()
	{
		Color temp = Color.white;
		//Image tempImg = null;
		//SpriteRenderer tempSpt = null;

		//HACK revisar
		/*if (particles != null) 
		{
			//borderStars.SetActive (true);
			particles.Play();
		}*/

		//Obtenemos el color desde el manager dependiendo del tipo
		switch (suscribedStatus[suscribedStatus.Count -1]) 
		{
		case(HighLightManager.EHighLightStatus.NORMAL):
			temp = HighLightManager.GetInstance().normalHighLight;
			break;
		case(HighLightManager.EHighLightStatus.WRONG):
			temp = HighLightManager.GetInstance().wrongHighLight;
			//HACK revisar
			/*if (particles != null) 
			{
				//borderStars.SetActive (false);
				particles.Clear();
				particles.Stop();
			}*/
			break;
		case(HighLightManager.EHighLightStatus.HINT):
			temp = HighLightManager.GetInstance().hintHighLight;
			break;
		}

		//Asignamos el color dependiendo si es de canvas o de sprite normal
		if(image != null)
		{
			image.color = temp;
		}

		if(sprite_renderer)
		{
			sprite_renderer.color = temp;
		}

		/*tempImg = gameObject.GetComponent<Image>();

		if (tempImg != null) 
		{
			tempImg.color = temp;
		} 
		else 
		{
			tempSpt = gameObject.GetComponent<SpriteRenderer> ();
			if (tempSpt != null) 
			{
				tempSpt.color = temp;
			}
		}*/

		if (!isScaled) 
		{
			setScale ();
		}

	}

	protected void initAnim()
	{
		if (!startAnim)
		{
			isDescending = true;
		}

		startAnim = true;
	}

	protected void finishAnim()
	{
		if (!lastForever) 
		{
			startAnim = false;

			getModifiedAlpha (0, true);
		}
	}

	protected float getModifiedAlpha(float value,bool reset = false)
	{
		SpriteRenderer tempSpt = GetComponent<SpriteRenderer>();
		Image tempImg = GetComponent<Image>();

		Color tempCol = Color.white;

		if (tempSpt != null) 
		{
			if (!reset) 
			{
				tempCol = new Color (tempSpt.color.r, tempSpt.color.g, tempSpt.color.b, tempSpt.color.a + value);
			}

			tempSpt.color = tempCol;
		} 
		else if (tempImg != null) 
		{
			if (!reset) 
			{
				tempCol = new Color (tempImg.color.r, tempImg.color.g, tempImg.color.b, tempImg.color.a + value);
			} 

			tempImg.color = tempCol;

		}

		return tempCol.a;
	}

	public bool deactivateType(HighLightManager.EHighLightType type)
	{
		int index = suscribedTypes.IndexOf (type);

		if (index >= 0) 
		{
			suscribedTypes.RemoveAt (index);
			suscribedStatus.RemoveAt (index);

			if (suscribedTypes.Count == 0) 
			{
				gameObject.SetActive (false);

				//HACK revisar
				/*
				if (particles != null) 
				{
					particles.Clear();
					particles.Stop ();
					//borderStars.SetActive (false);
				}*/

				finishAnim ();

				if (hasBeat) 
				{
					transform.localScale = previousScale;
					DOTween.Kill ("HighLightBeat",true);	
				}

				return true;
			} 
			else 
			{
				updateColor ();
				return false;
			}
		}
		return false;
	}

	public void completlyDeactivate()
	{
		suscribedTypes.Clear ();
		suscribedStatus.Clear ();

		gameObject.SetActive (false);
		//HACK revisar
		/*if (particles != null) 
		{
			particles.Clear();
			particles.Stop ();
			//borderStars.SetActive (false);
		}*/

		finishAnim ();
	}

	protected void setScale()
	{
		switch (parentType) 
		{
		case(EHIGHLIGHPARENTTYPE.CELL):
			scaleSpriteToFather ();
			break;
		}
	}

	public void scaleSpriteToFather()
	{

		float percent = (CellsManager.instance.cellSize / sprite_renderer.bounds.size.x);
		//float percent = (FindObjectOfType<CellsManager>().cellSize / GetComponent<SpriteRenderer> ().bounds.size.x);
		percent *= 1.18f;

		transform.localScale = new Vector3 (percent, percent, percent);


		transform.position = cellParent.transform.position + (new Vector3 (cellParent.sprite_renderer.bounds.extents.x,
			-cellParent.sprite_renderer.bounds.extents.x, 0));

		//borderStars.transform.position = transform.position;

		isScaled = true;
	}

	protected void highLightBeat()
	{
		if (suscribedTypes.Count == 0) 
		{
			return;
		}

		transform.parent.DOShakeScale (beatTime,beatStrength,beatVibrato,beatRandom).OnComplete(
			()=>
			{
				if (suscribedTypes.Count != 0) 
				{
					Invoke("highLightBeat",1);
				}
			}
		).SetId ("HighLightBeat");
	}
}