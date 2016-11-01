using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

public class Feedback : MonoBehaviour {

	protected string m_mailSubject ="";
	protected string m_htmlMailBody ="";
	protected string[] m_mailToRecipients = new string[1];
	protected string[] m_mailCCRecipients;
	protected string[] m_mailBCCRecipients;

	protected string CompanyEmail = "developers@villavanilla.com";
	public Action OnFinishedSharing;

	void Start()
	{
		m_mailToRecipients [0] = CompanyEmail;
		m_mailSubject = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FEEDBACK_SUBJECT);
		m_htmlMailBody =  MultiLanguageTextManager.instance.multipleReplace(MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.FEEDBACK_CONTENT),
			new string[1]{ "/n" }, new string[1]{Environment.NewLine });
	}

	public void SendHTMLTextMail () 
	{
		// Create composer
		MailShareComposer	_composer	= new MailShareComposer();
		_composer.Subject				= m_mailSubject;
		_composer.Body					= m_htmlMailBody;
		_composer.IsHTMLBody			= true;
		_composer.ToRecipients			= m_mailToRecipients;
		_composer.CCRecipients			= m_mailCCRecipients;
		_composer.BCCRecipients			= m_mailBCCRecipients;

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}

	private void FinishedSharing (eShareResult _result)
	{
		if (OnFinishedSharing != null) 
		{
			OnFinishedSharing ();
		}
	}
}
