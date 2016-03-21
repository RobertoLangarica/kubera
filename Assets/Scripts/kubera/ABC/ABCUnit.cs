using UnityEngine;
using System.Collections;

namespace ABC
{
	public class ABCUnit
	{
		public int ivalue;
		public char cvalue;

		public ABCUnit(int intValue, char charValue)
		{
			ivalue = intValue;
			cvalue = charValue;
		}

		public ABCUnit(int intValue, string character)
		{
			ivalue = intValue;
			cvalue = character.ToCharArray()[0];
		}

		public ABCUnit(){}
	}
}