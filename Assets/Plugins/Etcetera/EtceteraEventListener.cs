using UnityEngine;
using System.Collections;


public class EtceteraEventListener : MonoBehaviour
{
#if UNITY_IPHONE
	void OnEnable()
	{
		// Listen to all events for illustration purposes
		EtceteraManager.dismissingViewControllerEvent += dismissingViewControllerEvent;
		EtceteraManager.imagePickerCancelled += imagePickerCancelled;
		EtceteraManager.imagePickerChoseImage += imagePickerChoseImage;
		EtceteraManager.alertButtonClicked += alertButtonClicked;
		
		EtceteraManager.promptCancelled += promptCancelled;
		EtceteraManager.singleFieldPromptTextEntered += singleFieldPromptTextEntered;
		EtceteraManager.twoFieldPromptTextEntered += twoFieldPromptTextEntered;
		
		EtceteraManager.remoteRegistrationSucceeded += remoteRegistrationSucceeded;
		EtceteraManager.remoteRegistrationFailed += remoteRegistrationFailed;
		EtceteraManager.urbanAirshipRegistrationSucceeded += urbanAirshipRegistrationSucceeded;
		EtceteraManager.urbanAirshipRegistrationFailed += urbanAirshipRegistrationFailed;
		EtceteraManager.remoteNotificationReceived += remoteNotificationReceived;
		
		EtceteraManager.mailComposerFinished += mailComposerFinished;
		EtceteraManager.smsComposerFinished += smsComposerFinished;
	}
	
	
	void OnDisable()
	{
		// Remove all event handlers
		EtceteraManager.dismissingViewControllerEvent += dismissingViewControllerEvent;
		EtceteraManager.imagePickerCancelled -= imagePickerCancelled;
		EtceteraManager.imagePickerChoseImage -= imagePickerChoseImage;
		EtceteraManager.alertButtonClicked -= alertButtonClicked;
		
		EtceteraManager.promptCancelled -= promptCancelled;
		EtceteraManager.singleFieldPromptTextEntered -= singleFieldPromptTextEntered;
		EtceteraManager.twoFieldPromptTextEntered -= twoFieldPromptTextEntered;
		
		EtceteraManager.remoteRegistrationSucceeded -= remoteRegistrationSucceeded;
		EtceteraManager.remoteRegistrationFailed -= remoteRegistrationFailed;
		EtceteraManager.urbanAirshipRegistrationSucceeded -= urbanAirshipRegistrationSucceeded;
		EtceteraManager.urbanAirshipRegistrationFailed -= urbanAirshipRegistrationFailed;
		
		EtceteraManager.mailComposerFinished -= mailComposerFinished;
		EtceteraManager.smsComposerFinished -= smsComposerFinished;
	}
	
	
	void dismissingViewControllerEvent()
	{
		Debug.Log( "dismissingViewControllerEvent" );
	}
	
	
	void imagePickerCancelled()
	{
		Debug.Log( "imagePickerCancelled" );
	}
	

	void imagePickerChoseImage( string imagePath )
	{
		Debug.Log( "image picker chose image: " + imagePath );
	}
	
	
	void alertButtonClicked( string text )
	{
		Debug.Log( "alert button clicked: " + text );
	}
	
	
	void promptCancelled()
	{
		Debug.Log( "promptCancelled" );
	}
	
	
	void singleFieldPromptTextEntered( string text )
	{
		Debug.Log( "field : " + text );
	}
	
	
	void twoFieldPromptTextEntered( string textOne, string textTwo )
	{
		Debug.Log( "field one: " + textOne + ", field two: " + textTwo );
	}
	
	
	void remoteRegistrationSucceeded( string deviceToken )
	{
		Debug.Log( "remoteRegistrationSucceeded with deviceToken: " + deviceToken );
	}
	
	
	void remoteRegistrationFailed( string error )
	{
		Debug.Log( "remoteRegistrationFailed : " + error );
	}
	
	
	void urbanAirshipRegistrationSucceeded()
	{
		Debug.Log( "urbanAirshipRegistrationSucceeded" );
	}
	
	
	void urbanAirshipRegistrationFailed( string error )
	{
		Debug.Log( "urbanAirshipRegistrationFailed : " + error );
	}
	
	
	void remoteNotificationReceived( Hashtable notification )
	{
		Debug.Log( "remoteNotificationReceived" );
		foreach( DictionaryEntry kv in notification )
			Debug.Log( string.Format( "{0}: {1}", kv.Key, kv.Value ) );
	}
	
	
	void mailComposerFinished( string result )
	{
		Debug.Log( "mailComposerFinished : " + result );
	}
	
	
	void smsComposerFinished( string result )
	{
		Debug.Log( "smsComposerFinished : " + result );
	}
#endif
}
