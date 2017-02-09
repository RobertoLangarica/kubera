using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ParticleScaler : MonoBehaviour 
{
	public ParticleSystem particles;
	public RectTransform canvasTransform;

	void Start () 
	{
		Vector3 box = new Vector3(canvasTransform.sizeDelta.x,canvasTransform.sizeDelta.y);
		ParticleSystem.ShapeModule shape = particles.shape;
		shape.box = box;
	}
}
