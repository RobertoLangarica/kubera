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
	public const string ATTEMPT_PER_LEVEL					 = "attemptsPerLevel";
	public const string POWERUPS_USED						 = "powerUpsUsed";
	public const string REGISTER_WORD						 = "registerWord";
	public const string ZERO_LIFES_LEVEL					 = "zeroLifesLevel";
	public const string LAST_LEVEL_BEFORE_PAUSE_APP			 = "lastLevelBeforePauseApp";
	public const string MUSIC_TURNED_OFF					 = "musicTurnedOff";
	public const string FX_TURNED_OFF						 = "fxTurnedOff";
	public const string GEMS_USED_ON_LEVEL					 = "gemsUsedOnLevel";
	public const string GEMS_USED_IN_LIFES					 = "gemUsedInLifes";
	public const string GEMS_USED_FOR_FIRST_TIME			 = "gemsUsedForFirstTime";
	public const string GEMS_USED_AFTER_FIRST_PURCHASE		 = "gemsUsedAfterFirstPurchased";
	public const string GEMS_FIRST_PURCHASE					 = "gemsFirstPurchase";
	public const string BOSS_REACHED						 = "bossReached";
	public const string FACEBOOK_KEY_REQUEST				 = "facebookKeyRequest";
	public const string FACEBOOK_FIRST_LIFE_REQUEST			 = "facebookFirstLifeRequest";
	public const string FACEBOOK_LOGIN						 = "facebookLogin";

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

	void Start()
	{
		switchForUnityAnalytics (true);
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus) 
		{
			registerEventWithParameters (LAST_LEVEL_BEFORE_PAUSE_APP,
				new Dictionary<string, string> () {
					{ "Level",PersistentData.GetInstance().lastLevelReachedName},
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

	public void registerFirstWinAttempts(string level,int attempts)
	{
		registerEventWithParameters(ATTEMPTS_TO_WIN_LEVEL,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"Attempts",attempts.ToString()}});
	}

	public void registerLevelAttempts(string level,int attempts)
	{
		registerEventWithParameters(ATTEMPT_PER_LEVEL,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"Attempts",attempts.ToString()}});
	}

	public void registerPowerUpsUse(string level,Dictionary<string,int> powerUps,int attempt)
	{
		Dictionary<string,string> parameters = new Dictionary<string, string>{
			{ "Level",level },
			{ "User",DataManagerKubera.GetInstance ().currentUserId },
			{"Attempts",attempt.ToString()}};

		foreach(KeyValuePair<string,int> val in powerUps)
		{
			parameters.Add (val.Key,val.Value.ToString());
		}

		registerEventWithParameters (POWERUPS_USED,parameters);
	}

	public void registerCreatedWord(string level,string word,int length)
	{
		if (level == "0001" && word == "FELIZ") 
		{
			return;
		}

		if (level == "0003" && word == "YEN") 
		{
			return;
		}

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

	public void registerForBossReached(string level,int currentStars)
	{
		float percent = (currentStars / (int.Parse(level) * 3)) * 100;

		registerEventWithParameters(BOSS_REACHED,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"StarsEarned",currentStars.ToString()},
				{"PercentOfStars",percent.ToString()}});
	}

	public void registerFacebookKeyRequest(string level)
	{
		registerEventWithParameters(FACEBOOK_KEY_REQUEST,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId}});
	}

	public void registerFacebookFirstLifeRequest(string level)
	{
		registerEventWithParameters(FACEBOOK_FIRST_LIFE_REQUEST,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId}});
	}

	public void registerFaceBookLogin()
	{
		registerEventWithParameters(FACEBOOK_LOGIN,
			new Dictionary<string, string>() {
				{"User",DataManagerKubera.GetInstance().currentUserId}});
	}

	public void registerGemsUsedOnLevel(string level,int gems)
	{
		registerEventWithParameters(GEMS_USED_ON_LEVEL,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId},
				{"Gems",gems.ToString()}});
	}

	public void registerGemsUsedOnLifes(string level)
	{
		registerEventWithParameters(GEMS_USED_IN_LIFES,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId}});
	}

	public void registerGemsUsedForFirstTime(string level)
	{
		registerEventWithParameters(GEMS_USED_FOR_FIRST_TIME,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId}});
	}

	public void registerGemsUsedAfterFirstPurchase(string level)
	{
		registerEventWithParameters(GEMS_USED_AFTER_FIRST_PURCHASE,
			new Dictionary<string, string>() {
				{"Level",level},
				{"User",DataManagerKubera.GetInstance().currentUserId}});
	}

	public void registerGemsFirstPurchase(string level)
	{
		registerEventWithParameters(GEMS_FIRST_PURCHASE,
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