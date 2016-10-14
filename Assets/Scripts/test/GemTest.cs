﻿using UnityEngine;
using System.Collections;
using utils.gems;

public class GemTest : MonoBehaviour {

	public void simulateUserLoggedIn()
	{
		ShopikaManager.GetCastedInstance<ShopikaManager>().OnUserLoggedIn("db42ee60-2427-11e6-a38f-6b42b8609e79","yHPCOfglVnAfjOqTJTTrwTj3QIOskkAom97HFw72Vz7mJFDMjO92t0vs78BfjvrB");
	}

	public void simulateGemsChanged()
	{
		ShopikaManager.GetCastedInstance<ShopikaManager>().OnGemsRemotleyChanged();
	}
}
