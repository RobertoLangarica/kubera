using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AnimatedSprite))]
[CanEditMultipleObjects]
public class AnimatedSpriteEditor : Editor 
{
	protected AnimatedSprite animatedSprite;

	public void OnEnable()
	{
		animatedSprite = (AnimatedSprite)target;

		//Sprite
		if(animatedSprite.gameObject.GetComponent<SpriteRenderer>() == null)
		{
			animatedSprite.gameObject.AddComponent<SpriteRenderer>();
			EditorUtility.SetDirty(animatedSprite);
		}

		//TransformInfo
		if(animatedSprite.gameObject.GetComponent<TransformInfo>() == null)
		{
			animatedSprite.transformation = animatedSprite.gameObject.AddComponent<TransformInfo>();
			EditorUtility.SetDirty(animatedSprite);
		}
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		serializedObject.Update();

		bool dirty = false;

		//Actualizamos las secuencias si se encuentran en los valores por default

		//Buscamos las secuencias ya existentes que no se van a tocar
		SerializedProperty seqs = serializedObject.FindProperty("sequences");
		for(int i = 0; i < seqs.arraySize; i++)
		{
			//FPS
			//SerializedProperty sequence = seqs.GetArrayElementAtIndex(i);

			/*int fps = sequence.FindPropertyRelative("FPS").intValue;
			int calculatedFPS = sequence.FindPropertyRelative("_currentFPSCalculated").intValue;

			Debug.Log(fps+"_"+calculatedFPS);

			if(calculatedFPS != fps)
			{
				sequence.FindPropertyRelative("_currentFPSCalculated").intValue = fps;
				sequence.FindPropertyRelative("inverseFPS").floatValue = 1.0f/fps;
				dirty = true;
			}
			*/

			/*SerializedProperty frames = sequence.FindPropertyRelative("frames");

			for(int j = 0; j < frames.arraySize; j++)
			{
				SerializedProperty frame = frames.GetArrayElementAtIndex(j);
				if(frame.FindPropertyRelative("frameLength").intValue == 0)
				{
					frame.FindPropertyRelative("frameLength").intValue = 1;
					dirty = true;
				}

				if(frame.FindPropertyRelative("id").intValue == 0)
				{
					frame.FindPropertyRelative("id").intValue = j;
					dirty = true;
				}
			}*/
		}

		if(dirty)
		{
			EditorUtility.SetDirty(target);
		}
		serializedObject.ApplyModifiedProperties();
		//serializedObject.Update();
	}
}
