using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


#if UNITY_IPHONE

public enum P31RemoteNotificationType
{
	None = 0,
	Badge = 1 << 0,
	Sound = 1 << 1,
	Alert = 1 << 2
};


public enum UIInterfaceOrientation
{
   Portrait = 1,
   PortraitUpsideDown = 2,
   LandscapeLeft = 4,
   LandscapeRight = 3
};


public enum PhotoPromptType
{
	Camera = 0,
	Album,
	CameraAndAlbum
};


public class EtceteraBinding
{
	// Takes a screenshot and puts it in the Application.persistentDataPath directory (which is Documents on iOS)
    public static IEnumerator takeScreenShot( string filename )
    {
    	yield return new WaitForEndOfFrame();

		Application.CaptureScreenshot( filename );
		
		// wait for the file to get written or 300 frames max (5 - 10 seconds)
		var path = Application.persistentDataPath + "/" + filename;
		var frame = 0;
		while( !System.IO.File.Exists( path ) && frame < 300 )
		{
			yield return null;
			++frame;
		}
    }

    
	
	#region Language
	
    [DllImport("__Internal")]
    private static extern string _etceteraGetCurrentLanguage();
 
	// Returns the locale currently set on the device
    public static string getCurrentLanguage()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _etceteraGetCurrentLanguage();
		return "en";
    }
	
	
    [DllImport("__Internal")]
    private static extern string _etceteraGetLocalizedString( string key, string defaultValue );
 
	// Uses the Xcode Localizable.strings system to get a localized version of the given string
    public static string getLocalizedString( string key, string defaultValue )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _etceteraGetLocalizedString( key, defaultValue );
		return string.Empty;
    }
	
	#endregion;
	
	
	#region UIAlertView and P31AlertView
	
	// Shows a standard Apple alert with the given title, message and buttonTitle
	[System.Obsolete( "Use the _etceteraShowAlertWithTitleMessageAndButtons. This method will be removed." )]
    public static void showAlertWithTitleMessageAndButton( string title, string message, string buttonTitle )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			showAlertWithTitleMessageAndButtons( title, message, new string[] { buttonTitle } );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraShowAlertWithTitleMessageAndButtons( string title, string message, string buttons );
 
	// Shows a standard Apple alert with the given title, message and an array of buttons. At least one button must be included.
    public static void showAlertWithTitleMessageAndButtons( string title, string message, string[] buttons )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			var buttonArrayList = new ArrayList( buttons );
			_etceteraShowAlertWithTitleMessageAndButtons( title, message, Prime31.MiniJSON.jsonEncode( buttonArrayList ) );
		}
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraSetPromptColors( uint borderColor, uint gradientStopOne, uint gradientStopTwo );
 
	// Sets the colors for all calls to showPrompt*.  Colors should be hex with alpha.  Ex of orange full opacity: 0xFF0000FF
    public static void setPromptColors( uint borderColor, uint gradientStopOne, uint gradientStopTwo )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraSetPromptColors( borderColor, gradientStopOne, gradientStopTwo );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraShowPromptWithOneField( string title, string message, string placeHolder, bool autocomplete );
 
	// Shows a prompt with one text field
    public static void showPromptWithOneField( string title, string message, string placeHolder, bool autocomplete )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraShowPromptWithOneField( title, message, placeHolder, autocomplete );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraShowPromptWithTwoFields( string title, string message, string placeHolder1, string placeHolder2, bool autocomplete );
 
	// Shows a prompt with two text fields
    public static void showPromptWithTwoFields( string title, string message, string placeHolder1, string placeHolder2, bool autocomplete )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraShowPromptWithTwoFields( title, message, placeHolder1, placeHolder2, autocomplete );
    }
	
	#endregion;
	
	
	#region Web and Mail
	
    [DllImport("__Internal")]
    private static extern void _etceteraShowWebPage( string url, bool showControls );
 
	// Opens a web view with the url (Url can either be a resource on the web or a local file) and optional controls (back, forward, copy, open in Safari)
    public static void showWebPage( string url, bool showControls )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraShowWebPage( url, showControls );
    }
    
    
    [DllImport("__Internal")]
    private static extern bool _etceteraIsEmailAvailable();
 
	// Checks to see if an email account is setup on the device
    public static bool isEmailAvailable()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _etceteraIsEmailAvailable();
		return false;
    }


    [DllImport("__Internal")]
    private static extern bool _etceteraIsSMSAvailable();
 
	// Checks to see if SMS is available on the current device
    public static bool isSMSAvailable()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _etceteraIsSMSAvailable();
		return false;
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraShowMailComposer( string toAddress, string subject, string body, bool isHTML );
 
	// Opens the mail composer with the given information
    public static void showMailComposer( string toAddress, string subject, string body, bool isHTML )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraShowMailComposer( toAddress, subject, body, isHTML );
    }
	

	// Opens the mail composer with a screenshot of the current state of the game attached
    public static IEnumerator showMailComposerWithScreenshot( MonoBehaviour mono, string toAddress, string subject, string body, bool isHTML )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			// grab the screeny and give it a unique name
			var tempName = System.Guid.NewGuid() + ".png";
			
			yield return mono.StartCoroutine( takeScreenShot( tempName ) );

			var path = Application.persistentDataPath + "/" + tempName;
			
			// open the composer
			showMailComposerWithAttachment( path, "image/png", "screenshot.png", toAddress, subject, body, isHTML );
		}
    }

	
	[DllImport("__Internal")]
    private static extern void _etceteraShowMailComposerWithAttachment( string filePathToAttachment, string attachementMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML );
 
	// Opens the mail composer with the given attachment.  The attachment data must be stored in a file on disk.
    public static void showMailComposerWithAttachment( string filePathToAttachment, string attachmentMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraShowMailComposerWithAttachment( filePathToAttachment, attachmentMimeType, attachmentFilename, toAddress, subject, body, isHTML );
    }


    [DllImport("__Internal")]
    private static extern void _etceteraShowSMSComposer( string body );
 
	// Opens the sms composer with the given body
    public static void showSMSComposer( string body )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraShowSMSComposer( body );
    }
	
	#endregion;
	
	
	#region Activity View
	
    [DllImport("__Internal")]
    private static extern void _etceteraShowActivityView();
 
	// Shows a simple native spinner.  You must call hideActivityView to hide it
    public static void showActivityView()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraShowActivityView();
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraHideActivityView();
 
	// Hides any activity views that are showing
    public static void hideActivityView()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraHideActivityView();
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraShowBezelActivityViewWithLabel( string label );
 
	// Shows a bezel activity view with a label
    public static void showBezelActivityViewWithLabel( string label )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraShowBezelActivityViewWithLabel( label );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraShowBezelActivityViewWithImage( string label, string imagePath );
 
	// Shows a bezel activity view with a label and image
    public static void showBezelActivityViewWithImage( string label, string imagePath )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraShowBezelActivityViewWithImage( label, imagePath );
    }
	
	#endregion;
	
	
	#region Ask For Review, Photo and Push Notifications
	
    [DllImport("__Internal")]
    private static extern void _etceteraAskForReview( int launchCount, float hoursBetweenPrompts, string title, string message, string iTunesUrl );
 
	// Opens the ask for review dialogue only if the game has been launched 'launchCount' times, the user did not request to not
	// be asked again, the user has not previously reviewed this version of the game and at least 'hoursBetweenPrompts' has passed
	// since the last prompt
    public static void askForReview( int launchCount, float hoursBetweenPrompts, string title, string message, string iTunesUrl )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraAskForReview( launchCount, hoursBetweenPrompts, title, message, iTunesUrl );
    }


    [DllImport("__Internal")]
    private static extern void _etceteraAskForReviewImmediately( string title, string message, string iTunesUrl );
 
	// Opens the ask for review dialogue immediately
    public static void askForReview( string title, string message, string iTunesUrl )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraAskForReviewImmediately( title, message, iTunesUrl );
    }
	

    [DllImport("__Internal")]
    private static extern void _etceteraSetPopoverPoint( float xPos, float yPos );
 
	// Sets the position from which the popover for prompting for a photo will show when on an iPad
    public static void setPopoverPoint( float xPos, float yPos )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraSetPopoverPoint( xPos, yPos );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraPromptForPhoto( float scaledToSize, int promptType );
 
	// for backwards compatibility
	public static void promptForPhoto( float scaledToSize )
	{
		promptForPhoto( scaledToSize, PhotoPromptType.CameraAndAlbum );
	}
	
	// Prompts the user to either take a photo or choose from the photo library.  scaledToSize should be set
	// less than 1.0f in most cases to avoid getting a huge image from the camera or photo library
    public static void promptForPhoto( float scaledToSize, PhotoPromptType promptType )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraPromptForPhoto( scaledToSize, (int)promptType );
    }

    
    [DllImport("__Internal")]
    private static extern void _etceteraResizeImageAtPath( string filePath, float width, float height );
 
	// Resizes and optionally crops the image at the given path
    public static void resizeImageAtPath( string filePath, float width, float height )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraResizeImageAtPath( filePath, width, height );
    }


    [DllImport("__Internal")]
    private static extern string _etceteraGetImageSize( string filePath );
 
	// Gets the size of the image at the given path.  Returns 0,0 for invalid paths
    public static Vector2 getImageSize( string filePath )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			var res = _etceteraGetImageSize( filePath );
			var parts = res.Split( new char[] { ',' } );
			if( parts.Length == 2 )
				return new Vector2( float.Parse( parts[0] ), float.Parse( parts[1] ) );
		}
		
		return Vector2.zero;
    }

	
	[DllImport("__Internal")]
    private static extern void _etceteraSaveImageToPhotoAlbum( string filePath );
 
	// Writes the given image to the users photo album
    public static void saveImageToPhotoAlbum( string filePath )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraSaveImageToPhotoAlbum( filePath );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _etceteraSetUrbanAirshipCredentials( string appKey, string appSecret, string alias );
 
	// Sets the Urban Airship credentials and optionally the alias.  Set these before calling registerForRemoteNotifications
	public static void setUrbanAirshipCredentials( string appKey, string appSecret )
	{
		setUrbanAirshipCredentials( appKey, appSecret, null );
	}
	
    public static void setUrbanAirshipCredentials( string appKey, string appSecret, string alias )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraSetUrbanAirshipCredentials( appKey, appSecret, alias );
    }
    
	
    [DllImport("__Internal")]
    private static extern void _etceteraRegisterForRemoteNotifications( int types );
 
	// Registers the game for remote (push) notifications.  types is a bitmask
    public static void registerForRemoteNotifcations( P31RemoteNotificationType types )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraRegisterForRemoteNotifications( (int)types );
    }
	
	
    [DllImport("__Internal")]
    private static extern int _etceteraGetEnabledRemoteNotificationTypes();
 
	// Gets the bitmasked notification types the user has registered for
    public static P31RemoteNotificationType getEnabledRemoteNotificationTypes()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return ( P31RemoteNotificationType )_etceteraGetEnabledRemoteNotificationTypes();
		return P31RemoteNotificationType.None;
    }
    
    
    [DllImport("__Internal")]
    private static extern int _etceteraGetBadgeCount();
 
	// Gets the current application badge count
    public static int getBadgeCount()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _etceteraGetBadgeCount();
		return 0;
    }
    
    
    [DllImport("__Internal")]
    private static extern void _etceteraSetBadgeCount( int badgeCount );
 
	// Sets the current application badge count
    public static void setBadgeCount( int badgeCount )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraSetBadgeCount( badgeCount );
    }


    [DllImport("__Internal")]
    private static extern int _etceteraGetStatusBarOrientation();
 
	// Gets the current UIApplication's status bar orientation
    public static UIInterfaceOrientation getStatusBarOrientation()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return (UIInterfaceOrientation)_etceteraGetStatusBarOrientation();
		return UIInterfaceOrientation.Portrait;
    }
	
	#endregion;
	
	
	#region UDID
	
	[DllImport("__Internal")]
    private static extern string _etceteraUniqueDeviceIdentifier();
 
	// Returns a unique identifier for the current device and application
    public static string uniqueDeviceIdentifier()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _etceteraUniqueDeviceIdentifier();
		return string.Empty;
    }
	
	
    [DllImport("__Internal")]
    private static extern string _etceteraUniqueGlobalDeviceIdentifier();
 
	// Returns a unique identifier for the current device
    public static string uniqueGlobalDeviceIdentifier()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _etceteraUniqueGlobalDeviceIdentifier();
		return string.Empty;
    }
	
	#endregion
	
	
	#region Inline web view
	
	[DllImport("__Internal")]
	private static extern void _etceteraInlineWebViewShow( int x, int y, int width, int height );

	// Shows the inline web view. Remember, iOS uses points not pixels for positioning and layout!
	public static void inlineWebViewShow( int x, int y, int width, int height )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraInlineWebViewShow( x, y, width, height );
	}


	[DllImport("__Internal")]
	private static extern void _etceteraInlineWebViewClose();

	// Closes the inline web view
	public static void inlineWebViewClose()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraInlineWebViewClose();
	}


	[DllImport("__Internal")]
	private static extern void _etceteraInlineWebViewSetUrl( string url );

	// Sets the current url for the inline web view
	public static void inlineWebViewSetUrl( string url )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraInlineWebViewSetUrl( url );
	}


	[DllImport("__Internal")]
	private static extern void _etceteraInlineWebViewSetFrame( int x, int y, int width, int height );

	// Sets the current frame for the inline web view
	public static void inlineWebViewSetFrame( int x, int y, int width, int height )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_etceteraInlineWebViewSetFrame( x, y, width, height );
	}
	
	#endregion

}
#endif