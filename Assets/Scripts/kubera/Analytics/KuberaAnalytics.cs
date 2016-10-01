using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KHD;
using Kubera.Data;

public class KuberaAnalytics : Manager<KuberaAnalytics> 
{
	public const string BEFORE_BONIFICATION_POINTS_MOVEMENTS = "beforeBonificationPointsAndMovements";
	public const string FIRST_WIN_STARS						 = "firstWinStars";
	public const string ATTEMPTS_TO_WIN_LEVEL				 = "attemptsToWinLevel";
	public const string POWERUPS_USED						 = "powerUpsUsed";
	public const string REGISTER_WORD						 = "registerWord";
	public const string ZERO_LIFES_LEVEL					 = "zeroLifesLevel";
	public const string LAST_LEVEL_BEFORE_PAUSE_APP			 = "lastLevelBeforePauseApp";
	public const string MUSIC_TURNED_OFF					 = "musicTurnedOff";
	public const string FX_TURNED_OFF						 = "fxTurnedOff";

	protected DateTime epoch = new DateTime(1970,1,1,0,0,0,DateTimeKind.Local);

	protected void registerSimpleEvent(string eventName)
	{
		FlurryAnalytics.Instance.LogEvent (eventName);
	}

	protected void registerEventWithParameters(string eventName,Dictionary<string,string> parameters)
	{
		FlurryAnalytics.Instance.LogEventWithParameters (eventName, parameters);
	}

	protected void startTimerEvent(string eventName)
	{
		FlurryAnalytics.Instance.LogEvent (eventName, true);
	}

	protected void finishTimerEvent(string eventName)
	{
		FlurryAnalytics.Instance.EndTimedEvent (eventName);
	}

	protected void switchForUnityAnalytics(bool activate)
	{
		FlurryAnalytics.Instance.replicateDataToUnityAnalytics = activate;
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus) 
		{
			registerEventWithParameters (LAST_LEVEL_BEFORE_PAUSE_APP,
				new Dictionary<string, string> () {
					{ "Level","" },
					{ "User",DataManagerKubera.GetInstance ().currentUserId }
				});
		}
	}

	public void registerFirstWinStars(string level,int stars)
	{
		registerEventWithParameters(FIRST_WIN_STARS,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"Stars",stars.ToString()}});
	}

	public void registerForBeforeBonification(string level,int points,int movements)
	{
		registerEventWithParameters(BEFORE_BONIFICATION_POINTS_MOVEMENTS,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"Points",points.ToString()},
				{"Movements",movements.ToString()}});
	}

	public void registerNewAttempt(string level,bool isBeforeFirstWin)
	{
		registerEventWithParameters(ATTEMPTS_TO_WIN_LEVEL,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"BeforeFirstWin",isBeforeFirstWin.ToString()}});
	}

	public void registerPowerUpsUse(string level,Dictionary<string,int> powerUps)
	{
		Dictionary<string,string> parameters = new Dictionary<string, string>{
			{ "Level",level },
			{ "User",DataManagerKubera.GetInstance ().currentUserId }};

		foreach(KeyValuePair<string,int> val in powerUps)
		{
			parameters.Add (val.Key,val.Value.ToString());
		}

		registerEventWithParameters (POWERUPS_USED,parameters);
	}

	public void registerCreatedWord(string level,string word,int length)
	{
		registerEventWithParameters(REGISTER_WORD,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"Length",length.ToString()},
				{"Word",word.ToString()}});		
	}

	public void registerLevelWhereReached0Lifes(string level)
	{
		registerEventWithParameters(ZERO_LIFES_LEVEL,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId}});
	}

	public void registerMusicTurnedOff()
	{
		registerEventWithParameters(MUSIC_TURNED_OFF,
			new Dictionary<string, string>() {
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"DateInMiliSeconds",generateNowTimestamp()}});	
	}

	public void registerFXTurnedOff()
	{
		registerEventWithParameters(FX_TURNED_OFF,
			new Dictionary<string, string>() {
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"DateInMiliSeconds",generateNowTimestamp()}});	
	}

	public string generateNowTimestamp()
	{
		return ((DateTime.Now - epoch).TotalMilliseconds).ToString ();
	}
}