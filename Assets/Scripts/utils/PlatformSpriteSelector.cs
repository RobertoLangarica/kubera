using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlatformSpriteSelector : MonoBehaviour 
{
	public Image target;

	public Sprite androidSprite;
	public Sprite iosSprite;
	public Sprite defaultSprite;


	void Start () 
	{
		
		#if UNITY_IOS
		target.sprite = iosSprite;
		#elif UNITY_ANDROID
		target.sprite = androidSprite;
		#else
		target.sprite = defaultSprite;
		#endif
	}
	
}
