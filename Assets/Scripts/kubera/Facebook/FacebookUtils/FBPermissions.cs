using UnityEngine;
using System;
using System.Collections.Generic;
using Facebook.Unity;

public class FBPermissions : MonoBehaviour {

	// Constants for the list of permissions we are requesting when prompting for Facebook Login
	// Read permissions and publish permissions should be requested seperatly and within context
	// See more: https://developers.facebook.com/docs/facebook-login/permissions/overview
	private static readonly List<string> readPermissions    = new List<string> {"public_profile","user_friends"};
	public static readonly List<string> publishPermissions = new List<string> {"publish_actions"};

	// Prompt the player to authenticate Friend Smash! with Facebook Login
	//
	// By default FB.LogInWithReadPermissions will attempt to authenticate the user with only
	// the basic permissions. If you need one or more additional permissions, pass in a list
	// of the permissions you wish to request from the player.
	//
	// In Friend Smash, only user_friends is required to enable access to friends, so that the game can show friends's
	// profile picture to make the experience more personal and engaging.
	//
	public static void PromptForLogin (Action callback = null)
	{
		// Login for read permissions
		// https://developers.facebook.com/docs/unity/reference/current/FB.LogInWithReadPermissions
		FB.LogInWithReadPermissions(readPermissions, delegate (ILoginResult result)
			{
				Debug.Log("LoginCallback");
				if (FB.IsLoggedIn)
				{
					Debug.Log("Logged in with ID: " + AccessToken.CurrentAccessToken.UserId +
						"\nGranted Permissions: " + AccessToken.CurrentAccessToken.Permissions.ToCommaSeparateList());
				}
				else
				{
					if (result.Error != null)
					{
						Debug.LogError(result.Error);
					}
					Debug.Log("Not Logged In");
				}
				if (callback != null)
				{
					callback();
				}
			});
	}

	// Prompt the player to grant publish permissions with Facebook Login
	// Publish permissions allow seemless publishing of content to Facebook on behalf of a player,
	// such as open graph stories, scores, and achievements.
	// https://developers.facebook.com/docs/facebook-login/permissions#reference-publish_actions
	// 
	// Access to the 'publish_actions' permissions requires that your app goes through a lightweight review with Facebook
	// See more about Login Review here: https://developers.facebook.com/docs/facebook-login/review/faqs#what_is_review
	//
	// Reminder: 'publish_actions' is not required for FB.ShareLink or FB.FeedShare, it is only required for API based sharing
	//
	public static void PromptForPublish (Action callback = null)
	{
		// Login for publish permissions
		// https://developers.facebook.com/docs/unity/reference/current/FB.LogInWithPublishPermissions

		FB.LogInWithPublishPermissions(publishPermissions, delegate (ILoginResult result)
			{
				Debug.Log("LoginCallback");
				if (FB.IsLoggedIn)
				{
					Debug.Log("Logged in with ID: " + AccessToken.CurrentAccessToken.UserId +
						"\nGranted Permissions: " + AccessToken.CurrentAccessToken.Permissions.ToCommaSeparateList());
				}
				else
				{
					if (result.Error != null)
					{
						Debug.LogError(result.Error);
					}
					Debug.Log("Not Logged In");
				}
				if (callback != null)
				{
					callback();
				}
			});
	}

	#region Util
	// Helper function to check whether the player has granted 'publish_actions'
	public static bool HavePublishActions
	{
		get {
			return (FB.IsLoggedIn &&
				(AccessToken.CurrentAccessToken.Permissions as List<string>).Contains("publish_actions")) ? true : false;
		}
		private set {}
	}
	#endregion
}
