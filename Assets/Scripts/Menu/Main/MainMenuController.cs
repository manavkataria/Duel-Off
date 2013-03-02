using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Main Menu Controller
 * -- Attached to UI_Main in Hierarchy
 * -- Controls movement / logic between Pages / Scripts:
 * -- PlayCrate, OptionsOmniCrate, and StoreCrate
 */
public class MainMenuController : MonoSingleton<MainMenuController> {

    public delegate void StoreClickHandler(SuppliesUIObject clicked, MenuState state, GameObject hit);
    public static event StoreClickHandler onStoreClick;
	
	public delegate void PurchaseClickHandler(GameObject clicked);
	public static event PurchaseClickHandler onPurchasableClick;

    public delegate void StashClickHandler(SuppliesUIObject clicked, MenuState state, GameObject hit);
    public static event StashClickHandler onStashClick;
	
	public delegate void InfoUpdateHandler();
	public static event InfoUpdateHandler updateInfo;
	
	public delegate void OnCharacterArrowHandler( bool isRight );
	public static event OnCharacterArrowHandler onCharacterArrow;

    public enum MenuState { InTransition, Main, Options, Character, CharPageOne, CharPageTwo, Store, GoldStore }
    public MenuState menuState = MenuState.InTransition;
    
    public GameObject thisGO;
    public GameObject env;
    public GameObject UI;

    public Vector3 envOffset = Vector3.zero;
	
	public CameraFade[] cameraFades;
	
	public AudioSource mainMenuMusic;
	
	#region Data / CONSTANTS
	
	public const string GOLD_200 				= "Cons_Gold_200";
	public const string GOLD_500				= "Cons_Gold_500";
	public const string GOLD_4000				= "Cons_Gold_4000";
	
	public const string PRIME31_DIRECTORY		= "Prefabs/Prime31/";
	
	public const string CHARTBOOST_APPID		= "511b29e416ba47a033000011";
	public const string CHARTBOOST_SIGNATURE	= "dc2a47421b79f150a6e471f017dc9ef59ed4f991";
	
	public string[] PRODUCT_IDENTIFIERS	= new string[3];
	
	#endregion
	
	public override void Init() 
	{ 
		PRODUCT_IDENTIFIERS[0] = GOLD_200;
		PRODUCT_IDENTIFIERS[1] = GOLD_500;
		PRODUCT_IDENTIFIERS[2] = GOLD_4000;
		
		Application.targetFrameRate = 45; 
		
		GameObject gcm;
		GameObject skm;
		GameObject etcm;
		GameObject etcl;
		GameObject cbm;
		
		if( GameObject.Find("GameCenterManager") == null && GameObject.Find("GameCenterManager(Clone)") == null )
			gcm = (GameObject)Instantiate(Resources.Load( PRIME31_DIRECTORY + "GameCenterManager" ) );
		if( GameObject.Find("StoreKitManager") == null && GameObject.Find("StoreKitManager(Clone)" ) == null )
			skm = (GameObject)Instantiate(Resources.Load( PRIME31_DIRECTORY + "StoreKitManager" ) );
		if( GameObject.Find("EtceteraManager") == null && GameObject.Find("EtceteraManager(Clone)" ) == null )
			etcm = (GameObject)Instantiate(Resources.Load( PRIME31_DIRECTORY + "EtceteraManager" ) );
		if( GameObject.Find("EtceteraEventListener") == null && GameObject.Find("EtceteraEventListener(Clone)" ) == null )
			etcl = (GameObject)Instantiate(Resources.Load(PRIME31_DIRECTORY + "EtceteraEventListener" ) );
		if( GameObject.Find("ChartBoostManager") == null && GameObject.Find("ChartBoostManager(Clone)" ) == null )
			cbm = (GameObject)Instantiate(Resources.Load(PRIME31_DIRECTORY + "ChartBoostManager" ) );
			
		
#if UNITY_IPHONE	
		ChartBoostBinding.init( CHARTBOOST_APPID, CHARTBOOST_SIGNATURE );
		StoreKitBinding.requestProductData( PRODUCT_IDENTIFIERS );
		
		if( GameCenterBinding.isGameCenterAvailable() )
			GameCenterBinding.authenticateLocalPlayer();
#endif
	}
    
	private RevMob revmob;
 	
	private static readonly Dictionary<string, string> appIds = new Dictionary<string, string>() {
		{ "Android", "4f56aa6e3dc441000e005a20"},	//Invalid AppId. Dummy.
		{ "IOS", "5130f2e2424648d21e000025" }
    };
	
    void Awake() {
		//Debug.Log("RevMob Start");
		//revmob = RevMob.Start(appIds);
		//DontDestroyOnLoad (this);
    }
	
    //Display RevMob Ad
	public void DisplayAd() {
#if UNITY_IPHONE
		if (revmob != null) {
			Debug.Log("Displaying REVMOB FullScreen Ad");
			revmob.ShowFullscreen();
		} else {
			Debug.Log("REVMOB is NULL!");
		}
#endif
	}
	
	IEnumerator Start()
    {
		while( !DBAccess.instance.isDbInit ) { yield return null; }
		
		if( DBAccess.instance.userPrefs.isMusicOn )
			mainMenuMusic.Play();
		
		// Fade Camera in.
		cameraFades[2].enabled = true;
		cameraFades[2].SetScreenOverlayColor( Color.black );
		cameraFades[2].StartFade( Color.clear, 2f );
		
		// Play opening animation and then set for user interaction.
		animation.Play("MainIntro");
        StartCoroutine
        (
            animation.PlayWithOptions("MainStart", animation["MainIntro"].length,
                () => { instance.applyOffset(); },
                () => 
				{ 
					instance.menuState = MenuState.Main; 
					animation.Play("MainIdle");
					updateInfo();
					DBAccess.instance.userPrefs.triggerOptionsUpdate();
				}
            )
        );
		yield return new WaitForSeconds(2f);
		
		cameraFades[2].enabled = false;
    }
	
    void Update()
    {
		// Raycast checking for where to route information and what events / SendMessages to use.
        if (Input.GetMouseButtonDown(0))
        {
            if (instance.menuState != MenuState.InTransition)
            {
                Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 3f);
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.collider.tag == "MainUI")
                        hit.collider.gameObject.SendMessage("onMenuClick", instance, SendMessageOptions.DontRequireReceiver);
                    else if (hit.collider.gameObject.layer == 11 && instance.menuState == MainMenuController.MenuState.Character )
					{
						DBAccess.instance.userPrefs.enemyName = findTaggedParent(hit.collider.transform).tag;
						DBAccess.instance.enemyStats = findTaggedParent(hit.collider.transform).GetComponent<EnemySelectStats>().stats;
						
						if( DBAccess.instance.userPrefs.playCount == 0 )
							StartCoroutine( loadNextLevel(1) );		//Tutorial Scene
						else
							StartCoroutine( loadNextLevel(2) ); 	//Demo Scene
					}
				
                    else if (hit.collider.tag == "StoreUI")
                    {
                        onStoreClick(hit.collider.GetComponent<SuppliesUIObject>(), instance.menuState, hit.collider.gameObject);
                    }
					else if (hit.collider.tag == "BuyButton")
					{
						onPurchasableClick(hit.collider.gameObject);
						
						if( Application.platform == RuntimePlatform.IPhonePlayer )
							ChartBoostBinding.trackEvent( "Buy Item Attempt" );
					}
					else if(hit.collider.tag == "GoldButton")
					{
						onPurchasableClick(hit.collider.gameObject);
					}
                    else if (hit.collider.tag == "StashUI")
                    {
                        onStashClick(hit.collider.GetComponent<SuppliesUIObject>(), instance.menuState, hit.collider.gameObject);
                    }
                    else if (hit.collider.tag == "OptionsUI")
                    {
                        GameObject clicked = hit.collider.gameObject;

                        if (clicked.name.Contains("Sound"))
                            clicked.SendMessage("hitByRayCast", SendMessageOptions.DontRequireReceiver);
                        else if (clicked.name.Contains("Music"))
                            clicked.SendMessage("hitByRayCast", SendMessageOptions.DontRequireReceiver);
                        else if (clicked.name.Contains("Control"))
                            clicked.SendMessage("hitByRayCast", SendMessageOptions.DontRequireReceiver);
						else if (clicked.name.Contains("GameCenter"))
							clicked.SendMessage("hitByRayCast", SendMessageOptions.DontRequireReceiver);
						else if (clicked.name.Contains("Play"))
							StartCoroutine( loadNextLevel(1) );
						else if (clicked.name.Contains("Background More Apps"))
						{
							Debug.Log( "Options More Apps" );
							if( Application.platform == RuntimePlatform.IPhonePlayer )
							{
								ChartBoostBinding.showMoreApps();
								ChartBoostBinding.trackEvent( "Options More Apps Click" );
							}
						}
                    }
					else if (hit.collider.tag == "CharacterUI")
					{
						EnemySelectPopulator e = GameObject.Find("EnemyGroupingParent").GetComponent<EnemySelectPopulator>();
						
						if( hit.collider.name == "Arrow Right" && e.characterIndex < 4)
						{
							instance.menuState = MainMenuController.MenuState.InTransition;
							
							hit.collider.SendMessage("hitByRayCast",hit.collider.gameObject,SendMessageOptions.DontRequireReceiver);
							
							if( onCharacterArrow != null )
								onCharacterArrow( true );
						}
						if( hit.collider.name == "Arrow Left" && e.characterIndex > 0)
						{
							instance.menuState = MainMenuController.MenuState.InTransition;
							
							hit.collider.SendMessage("hitByRayCast",hit.collider.gameObject,SendMessageOptions.DontRequireReceiver);

							if( onCharacterArrow != null )
								onCharacterArrow( false );
						}
						if( hit.collider.name.Contains("Background More Apps"))
						{
							Debug.Log("Character More Apps");
							
							if( Application.platform == RuntimePlatform.IPhonePlayer )
							{
								ChartBoostBinding.showMoreApps();
								ChartBoostBinding.trackEvent( "Character Select More Apps Click" );
							}
						}
					}
                }
            }
        }
    }
	
	
	/// <summary>
	/// Plays the MainStart anim on UI_Main and then plays MainIdle.
	/// </summary>
    public void playMenuStart()
    {
        StartCoroutine(
            instance.thisGO.animation.PlayWithOptions("MainStart",
                () => 
				{ 
					instance.menuState = MenuState.Main; 
					animation.Play("MainIdle");
					Debug.Log("playMenuStart");
				}
            )
        );
    }
	
	/// <summary>
	/// Finds the tagged parent for Characters
	/// </summary>
	/// <returns>
	/// The tagged parent.
	/// </returns>
	/// <param name='hit'>
	/// Hit.
	/// </param>
	Transform findTaggedParent( Transform hit )
	{
		Transform parent = hit.transform.parent;
		
		while( parent != null )
		{
			if( parent.tag.Contains("Character") )
				break;
			else
			{
				parent = parent.parent;
				continue;
			}
		}
		
		return parent;		
	}
	
	IEnumerator loadNextLevel(int lvlToLoad)
	{
		if( mainMenuMusic != null )
			iTween.AudioTo( gameObject, iTween.Hash(
				"audiosource", mainMenuMusic,
				"volume", 0,
				"time", 2,
				"easetype", iTween.EaseType.linear
				)
			);
		
		cameraFades[2].enabled = true;
		cameraFades[2].SetScreenOverlayColor( Color.clear );
		cameraFades[2].StartFade( Color.black, 2f );
		
		yield return new WaitForSeconds(2f);
		
		DBAccess.instance.userPrefs.CommitToDB(false);
		
		yield return new WaitForSeconds(1f);
		
		Application.LoadLevel(lvlToLoad);		
	}

    // State change of menu to prevent double tapping of buttons, or early tapping of buttons before
    // animation finishes
    public void menuIsInTransition() { instance.menuState = MenuState.InTransition; }

    // Set offset of UI_Main > FG to match UI_Main > AnimRoot in Hierarchy
    public void setOffset() { instance.envOffset.y = instance.env.transform.eulerAngles.y; }

    // Have UI_Main > FG offset follow UI_Main > AnimRoot offset in Hierarchy
    public void applyOffset() { instance.env.transform.eulerAngles = instance.UI.transform.eulerAngles + instance.envOffset; }
	
	public void triggerUpdateInfo() { updateInfo(); }
	
}
