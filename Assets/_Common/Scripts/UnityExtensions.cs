using UnityEngine;
using System.Collections;

//Version: 1.0.0

/// <summary>
/// Extensions for Unity classes, this class contains functions that work as extensions and help to encapsulate
/// common used functionalities.
/// </summary>
public static class UnityExtensions
{
	public static GameObject GetRootGameObject(this GameObject gameObject)
	{
		Transform t = gameObject.transform.GetRootTransform();
		return t.gameObject;
	}

	public static Transform GetRootTransform(this Transform transform)
	{
		Transform root = transform.parent;
		if(root == null)
		{
			return transform;
		}
		else
		{
			while(root.parent != null)
			{
				root = root.parent;
			}
			return root;
		}
	}

	/// <summary>
	/// Forces the GameObject to activate/deactivate recursively itself and all its children.
	/// </summary>
	/// <param name="gameObject">Game objectransform.</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public static void ForceActivateRecursively(this GameObject gameObject, bool enable)
	{
		gameObject.transform.ForceActivateRecursively(enable);
	}

	/// <summary>
	/// Forces the Transform to activate/deactivate recursively itself and all its children.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public static void ForceActivateRecursively(this Transform transform, bool enable)
	{
		transform.gameObject.SetActive(enable);
		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).ForceActivateRecursively(enable);
		}
	}

	/// <summary>
	/// Sets the position x.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">X coordinate New value.</param>
	public static void SetPositionX(this Transform transform,float newValue)
	{
		transform.position = new Vector3(newValue, transform.position.y, transform.position.z);
	}

	/// <summary>
	/// Sets the local position x.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">X coordinate New value.</param>
	public static void SetLocalPositionX(this Transform transform,float newValue)
	{
		transform.localPosition = new Vector3(newValue, transform.localPosition.y, transform.localPosition.z);
	}

	/// <summary>
	/// Sets the position y.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">Y coordinate New value.</param>
	public static void SetPositionY(this Transform transform,float newValue)
	{
		transform.position = new Vector3(transform.position.x, newValue, transform.position.z);
	}

	/// <summary>
	/// Sets the local position y.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">Y coordinate New value.</param>
	public static void SetLocalPositionY(this Transform transform,float newValue)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, newValue, transform.localPosition.z);
	}

	/// <summary>
	/// Sets the position z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">Z coordinate New value.</param>
	public static void SetPositionZ(this Transform transform,float newValue)
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, newValue);
	}

	/// <summary>
	/// Sets the local position z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">Z coordinate New value.</param>
	public static void SetLocalPositionZ(this Transform transform,float newValue)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newValue);
	}

	/// <summary>
	/// Sets the position in X and Y.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	public static void SetPositionXY(this Transform transform,float newValueX, float newValueY)
	{
		transform.position = new Vector3(newValueX, newValueY, transform.position.z);
	}

	/// <summary>
	/// Sets the local position in X and Y.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	public static void SetLocalPositionXY(this Transform transform,float newValueX, float newValueY)
	{
		transform.localPosition = new Vector3(newValueX, newValueY, transform.localPosition.z);
	}

	/// <summary>
	/// Sets the position in X and Z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetPositionXZ(this Transform transform,float newValueX, float newValueZ)
	{
		transform.position = new Vector3(newValueX, transform.position.y, newValueZ);
	}

	/// <summary>
	/// Sets the local position in X and Z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalPositionXZ(this Transform transform,float newValueX, float newValueZ)
	{
		transform.localPosition = new Vector3(newValueX, transform.localPosition.y, newValueZ);
	}

	/// <summary>
	/// Sets the position in Y and Z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetPositionYZ(this Transform transform,float newValueY, float newValueZ)
	{
		transform.position = new Vector3(transform.position.x, newValueY, newValueZ);
	}

	/// <summary>
	/// Sets the local position in Y and Z.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalPositionYZ(this Transform transform,float newValueY, float newValueZ)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, newValueY, newValueZ);
	}

	/// <summary>
	/// Sets the position in all axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetPosition(this Transform transform, float newValueX,float newValueY, float newValueZ)
	{
		transform.position = new Vector3(newValueX, newValueY, newValueZ);
	}

	/// <summary>
	/// Sets the local position in all axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalPosition(this Transform transform, float newValueX, float newValueY, float newValueZ)
	{
		transform.localPosition = new Vector3(newValueX, newValueY, newValueZ);
	}

	/// <summary>
	/// Sets the rotation for X axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetRotationX(this Transform transform,float newValue)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	newValue, 
													transform.rotation.eulerAngles.y, 
													transform.rotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the local rotation for X axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetLocalRotationX(this Transform transform,float newValue)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	newValue, 
														transform.localRotation.eulerAngles.y, 
														transform.localRotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the rotation for Y axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetRotationY(this Transform transform,float newValue)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	transform.rotation.eulerAngles.x, 
													newValue,
													transform.rotation.eulerAngles.z));
	}
		
	/// <summary>
	/// Sets the local rotation for Y axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetLocalRotationY(this Transform transform,float newValue)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	transform.localRotation.eulerAngles.x, 
														newValue,
														transform.localRotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the rotation for Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetRotationZ(this Transform transform,float newValue)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	transform.rotation.eulerAngles.x, 
													transform.rotation.eulerAngles.y,
													newValue));
	}

	/// <summary>
	/// Sets the local rotation for Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValue">New value.</param>
	public static void SetLocalRotationZ(this Transform transform,float newValue)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	transform.localRotation.eulerAngles.x, 
														transform.localRotation.eulerAngles.y,
														newValue));
	}

	/// <summary>
	/// Sets the rotation for X and Y axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	public static void SetRotationXY(this Transform transform,float newValueX, float newValueY)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	newValueX, 
													newValueY, 
													transform.rotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the local rotation for X and Y axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	public static void SetLocalRotationXY(this Transform transform,float newValueX,float newValueY)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	newValueX, 
														newValueY, 
														transform.localRotation.eulerAngles.z));
	}

	/// <summary>
	/// Sets the rotation for X and Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetRotationXZ(this Transform transform,float newValueX, float newValueZ)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	newValueX, 
													transform.rotation.eulerAngles.y,
													newValueZ));
	}

	/// <summary>
	/// Sets the local rotation for X and Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalRotationXZ(this Transform transform,float newValueX,float newValueZ)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	newValueX, 
														transform.localRotation.eulerAngles.y,
														newValueZ));
	}

	/// <summary>
	/// Sets the rotation for Y and Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetRotationYZ(this Transform transform,float newValueY, float newValueZ)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	transform.rotation.eulerAngles.x,
													newValueY, 
													newValueZ));
	}

	/// <summary>
	/// Sets the local rotation for Y and Z axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalRotationYZ(this Transform transform,float newValueY,float newValueZ)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	transform.localRotation.eulerAngles.x,
														newValueY, 
														newValueZ));
	}

	/// <summary>
	/// Sets the rotation for all axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetRotation(this Transform transform,float newValueX, float newValueY, float newValueZ)
	{
		transform.rotation = Quaternion.Euler(new Vector3(	newValueX, newValueY, newValueZ));
	}

	/// <summary>
	/// Sets the local rotation for all axis.
	/// </summary>
	/// <param name="transform">Transform.</param>
	/// <param name="newValueX">New value x.</param>
	/// <param name="newValueY">New value y.</param>
	/// <param name="newValueZ">New value z.</param>
	public static void SetLocalRotation(this Transform transform, float newValueX,float newValueY,float newValueZ)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(	newValueX, newValueY, newValueZ));
	}
		
	/// <summary>
	/// Reset the specified transform to 0's on Local position, rotation and scale of 1's.
	/// </summary>
	/// <param name="transform">Transform.</param>
	public static void Reset(this Transform transform)
	{
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	/// <summary>
	/// Resets the velocity (also the angular velocity).
	/// </summary>
	/// <param name="rigidbody">Rigidbody.</param>
	public static void ResetVelocity(this Rigidbody rigidbody)
	{
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
	}

	public static Rect RectTransformToScreenSpace(this RectTransform transform )
	{
		Vector2 size = Vector2.Scale( transform.rect.size, transform.lossyScale );
		return new Rect( transform.position.x, Screen.height - transform.position.y, size.x, size.y );
	}

	public static void  CopyRectTransform(this RectTransform rTransform, RectTransform otherRT)
	{
		rTransform.anchoredPosition = otherRT.anchoredPosition;
		rTransform.anchorMax = otherRT.anchorMax;
		rTransform.anchorMin = otherRT.anchorMin;
		rTransform.sizeDelta = otherRT.sizeDelta;
		rTransform.offsetMax = otherRT.offsetMax;
		rTransform.offsetMin = otherRT.offsetMin;
		rTransform.localScale = otherRT.localScale;
		rTransform.eulerAngles = otherRT.eulerAngles;

	}

	public static void  SetOffsetsToZero(this RectTransform rTransform)
	{
		rTransform.offsetMin = Vector2.zero;
		rTransform.offsetMax = Vector2.zero;
	}


}
