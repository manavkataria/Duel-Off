using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Prime31;


// Any methods that Obj-C calls back using UnitySendMessage should be present here
public class EtceteraManager : MonoBehaviour
{
#if UNITY_IPHONE
	// Fired whenever any full screen view controller is dismissed
	public static event Action dismissingViewControllerEvent;
	
	// Fired when the user cancels the image picker
	public static event Action imagePickerCancelled;
	
	// Fired when the user selects or takes a photo
	public static event Action<string> imagePickerChoseImage;
	
	// Delegate for handling loading a texture dynamically from the file system
	public delegate void EceteraTextureDelegate( Texture2D texture );
	
	// Delegate failure handler for loading a texture dynamically
	public delegate void EceteraTextureFailedDelegate( string error );
	
	// Fired when the user touches a button on the alert view
	public static event Action<string> alertButtonClicked;
	
	// Fired when the user touches the cancel button on a prompt
	public static event Action promptCancelled;
	
	// Fired when the user finishes entering text in the prompt
	public static event Action<string> singleFieldPromptTextEntered;
	
	// Fired when the user finishes entering text in a two field prompt
	public static event Action<string, string> twoFieldPromptTextEntered;
	
	// Fired when remote notifications are successfully registered for
	public static event Action<string> remoteRegistrationSucceeded;
	
	// Fired when remote notification registration fails
	public static event Action<string> remoteRegistrationFailed;
	
	// Fired when Urban Airship registration succeeds
	public static event Action urbanAirshipRegistrationSucceeded;
	
	// Fired when Urban Airship registration fails
	public static event Action<string> urbanAirshipRegistrationFailed;
	
	// Fired when a remote notification is received or your game was launched from a remote notification
	public static event Action<Hashtable> remoteNotificationReceived;
	
	// Fired when the mail composer is dismissed
	public static event Action<string> mailComposerFinished;
	
	// Fired when the SMS composer is dismissed
	public static event Action<string> smsComposerFinished;
	
	
    void Awake()
    {
		// Set the GameObject name to the class name for easy access from Obj-C
		gameObject.name = this.GetType().ToString();
		DontDestroyOnLoad( this );
    }
	
	
	public void dismissingViewController()
	{
		if( dismissingViewControllerEvent != null )
			dismissingViewControllerEvent();
	}
	
	
	#region Image picker
	
	public void imagePickerDidCancel( string empty )
	{
		if( imagePickerCancelled != null )
			imagePickerCancelled();
	}
	
	
	public void imageSavedToDocuments( string filePath )
	{
		if( imagePickerChoseImage != null )
			imagePickerChoseImage( filePath );
	}

	
	// Loads up a Texture2D with the image at the given path
	public static IEnumerator textureFromFileAtPath( string filePath, EceteraTextureDelegate del, EceteraTextureFailedDelegate errorDel )
	{
		using( WWW www = new WWW( filePath ) )
		{
			yield return www;
			
			if( www.error != null )
			{
				if( errorDel != null )
					errorDel( www.error );
			}
			
			// Assign the texture to a local variable to avoid leaking it (Unity bug)
			Texture2D tex = www.texture;
	
			if( tex != null )
				del( tex );
		}
	}
	
	#endregion;
	
	
	#region Alert and Prompt
	
	public void alertViewClickedButton( string buttonTitle )
	{
		if( alertButtonClicked != null )
			alertButtonClicked( buttonTitle );
	}
	
	
	public void alertPromptCancelled( string empty )
	{
		if( promptCancelled != null )
			promptCancelled();
	}
	
	
	public void alertPromptEnteredText( string text )
	{
		// Was this one prompt or 2?
		string[] promptText = text.Split( new string[] {"|||"}, StringSplitOptions.None );
		if( promptText.Length == 1 )
		{
			if( singleFieldPromptTextEntered != null )
				singleFieldPromptTextEntered( promptText[0] );
		}
		
		if( promptText.Length == 2 )
		{
			if( twoFieldPromptTextEntered != null )
				twoFieldPromptTextEntered( promptText[0], promptText[1] );
		}
	}
	
	#endregion;
	
	
	#region Remote Notifications
	
	public void remoteRegistrationDidSucceed( string deviceToken )
	{
		if( remoteRegistrationSucceeded != null )
			remoteRegistrationSucceeded( deviceToken );
	}
	
	
	public void remoteRegistrationDidFail( string error )
	{
		if( remoteRegistrationFailed != null )
			remoteRegistrationFailed( error );
	}
	
	
	public void urbanAirshipRegistrationDidSucceed( string empty )
	{
		if( urbanAirshipRegistrationSucceeded != null )
			urbanAirshipRegistrationSucceeded();
	}
	
	
	public void urbanAirshipRegistrationDidFail( string error )
	{
		if( urbanAirshipRegistrationFailed != null )
			urbanAirshipRegistrationFailed( error );
	}
	
	
	public void remoteNotificationWasReceived( string json )
	{
		if( remoteNotificationReceived != null )
			remoteNotificationReceived( json.hashtableFromJson() );
	}
	
	#endregion;
	
	
	public void mailComposerFinishedWithResult( string result )
	{
		if( mailComposerFinished != null )
			mailComposerFinished( result );
	}
	
	
	public void smsComposerFinishedWithResult( string result )
	{
		if( smsComposerFinished != null )
			smsComposerFinished( result );
	}

#endif
}