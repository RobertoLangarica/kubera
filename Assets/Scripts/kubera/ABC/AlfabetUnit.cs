using UnityEngine;
using System.Collections;

namespace ABC
{
	public class AlfabetUnit
	{
		public int ivalue;
		public char cvalue;


		//Constructores por comodidad de manejo
		public AlfabetUnit(int intValue, char charValue)
		{
			ivalue = intValue;
			cvalue = charValue;
		}

		public AlfabetUnit(int intValue, string character)
		{
			ivalue = intValue;
			cvalue = character.ToCharArray()[0];
		}

		public AlfabetUnit(){}
	}
}