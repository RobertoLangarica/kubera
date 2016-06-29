using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extentions 
{
	
	public static void Adjust(this List<RectTransform> rectList)
	{
		foreach(RectTransform rectT in rectList){
			adjustRectTransform(rectT);
		}

	}

	public static void adjustRectTransform(this RectTransform rectT)
	{
		if(rectT == null || rectT.parent == null){
			return;
		}

		Bounds parentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectT.parent);

		Vector2 parentSize = new Vector2(parentBounds.size.x, parentBounds.size.y);
		// convert anchor ration in to pixel position
		Vector2 posMin = new Vector2(parentSize.x * rectT.anchorMin.x, parentSize.y * rectT.anchorMin.y);
		Vector2 posMax = new Vector2(parentSize.x * rectT.anchorMax.x, parentSize.y * rectT.anchorMax.y);

		// add offset
		posMin = posMin + rectT.offsetMin;
		posMax = posMax + rectT.offsetMax;

		// convert from pixel position to anchor ratio again
		posMin = new Vector2(posMin.x / parentBounds.size.x, posMin.y / parentBounds.size.y);
		posMax = new Vector2(posMax.x / parentBounds.size.x, posMax.y / parentBounds.size.y);

		rectT.anchorMin = posMin;
		rectT.anchorMax = posMax;

		rectT.offsetMin = Vector2.zero;
		rectT.offsetMax = Vector2.zero;
	}
}
