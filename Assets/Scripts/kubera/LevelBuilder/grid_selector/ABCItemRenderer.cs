using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LevelBuilder
{
	public class ABCItemRenderer : GridItemRenderer 
	{
		public Text txtPoints;
		public Text txtX2Count;
		public Text txtX3Count;

		public void addPoints()
		{
			data.points++;
			showData();
		}

		public void substractPoints()
		{
			data.points = data.points == 0 ? 0 : data.points-1;
			showData();
		}

		public void addX2()
		{
			data.x2Count++;
			showData();
		}

		public void substractX2()
		{
			data.x2Count = data.x2Count == 0 ? 0 : data.x2Count-1;
			showData();
		}

		public void addX3()
		{
			data.x3Count++;
			showData();
		}

		public void substractX3()
		{
			data.x3Count = data.x3Count == 0 ? 0 : data.x3Count-1;
			showData();
		}
		
		public override void showData()
		{
			base.showData();

			if(txtPoints != null)txtPoints.text = data.points.ToString("00");
			if(txtX2Count != null)txtX2Count.text = data.x2Count.ToString("00");
			if(txtX3Count != null)txtX3Count.text = data.x3Count.ToString("00");
		}

		public int getPoints()
		{
			return data.points;
		}

		public int getX2Count()
		{
			return data.x2Count;
		}

		public int getX3Count()
		{
			return data.x3Count;
		}
	}
}