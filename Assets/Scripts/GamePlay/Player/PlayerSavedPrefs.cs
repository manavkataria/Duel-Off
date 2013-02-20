using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/** Player Info and Preferences Object
 *  -- Passed around to different objects to read information about the player.
 *  -- There should really only be one instance of this class unless we put in
 *  -- the option for multiple users / log-ins, as well as passing information
 *  -- for multiplayer
 */ 
public class PlayerSavedPrefs {
	
	public delegate void OptionsUpdateHandler(_OptionsType optionType, bool isTrue, MainMenuController.MenuState state, _ControlScheme scheme );
    public static event OptionsUpdateHandler onOptionsUpdate;
	
	public delegate void OnHealthPackHandler();
	public static event OnHealthPackHandler onHealthPackUse;
	
	public delegate void BroadcastSupplyUpdateHandler();
	public static event BroadcastSupplyUpdateHandler onBroadcastUpdate;
	
	public enum _OptionsType { None, Sounds, Music, Controls };
	
	public enum _ControlScheme { Accelerometer, Gyroscope };
    private _ControlScheme _controlScheme;
	
	#region Properties
	
	private int _level;
	private int _exp;
	private int _gold;
	private string _firstName;
	private string _lastName;
	private int _isSoundOn;
	private int _isMusicOn;
	private int _controls;
	private float _health;
	private int _playCount;
	private int _wins;
	private int _losses;
	
	private List<HealthPack> _hPacks = new List<HealthPack>();
	private List<Gun> _allGuns = new List<Gun>();
	private List<HealthPack> _allHPacks = new List<HealthPack>();
	private Gun _userGun;
	
	private string _enemyName;
	private Dictionary<int,int> _expRequired = new Dictionary<int, int>();
	
	#endregion
	
	#region Accessors
	
	public int Level
	{
		get { return _level; }
		set 
		{
			if( value > 20 )
				_level = 20;
			else
				_level = value;
		}
	}
	
	public int Exp
	{
		get { return _exp; }
		set { _exp = value; }
	}
	
	public int Gold
	{
		get { return _gold; }
		set { _gold = value; }
	}
	
	public string PlayerFirstName
	{
		get { return _firstName; }
		set { _firstName = value; }
	}
	
	public string PlayerLastName
	{
		get { return _lastName; }
		set { _lastName = value; }
	}
	
	public bool isSoundOn
	{
		get 
		{ 
			return ( _isSoundOn == 1 ) ? true : false;
		}
		set 
		{ 
			_isSoundOn = (value == true) ? 1 : 0;
			if( onOptionsUpdate != null )
				onOptionsUpdate(_OptionsType.Sounds, value, MainMenuController.instance.menuState, ControlScheme);
		}
	}
	
	public bool isMusicOn
	{
		get 
		{ 
			return ( _isMusicOn == 1 ) ? true : false;
		}
		set 
		{ 
			_isMusicOn = (value == true) ? 1 : 0;
			if( onOptionsUpdate != null )
				onOptionsUpdate(_OptionsType.Music, value, MainMenuController.instance.menuState, ControlScheme);
		}
	}
	
	public _ControlScheme ControlScheme
	{
		get 
		{ 
			return ( _controls == 0 ) ? _ControlScheme.Accelerometer : _ControlScheme.Gyroscope;
		}
		set 
		{ 
			_controls = ( value == _ControlScheme.Accelerometer ) ? 0 : 1;
			if( onOptionsUpdate != null )
				onOptionsUpdate(_OptionsType.Controls, false, MainMenuController.instance.menuState, ControlScheme );
		}
	}
	
	public List<HealthPack> hPacks
	{
		get { return _hPacks; }
		set { _hPacks = value; }
	}
	
	public List<Gun> allGuns
	{
		get { return _allGuns; }
		set { _allGuns = value; }
	}
	
	public List<HealthPack> allHPacks
	{
		get { return _allHPacks; }
		set { _allHPacks = value; }
	}
	
	public Gun userGun
	{
		get 
		{ 
			return ( _userGun != null ) ? _userGun : new Gun(DBAccess.instance.getGunStats("Colt_1911") ); 
		}
		set { _userGun = value; }
	}
	
	public float health
	{
		get { return _health; }
		set 
		{ 
			if( value > 100f )
				_health = 100f;
			else if( value < 0f )
				_health = 0f;
			else
				_health = value;
		}
	}
	
	public int playCount
	{
		get { return _playCount; }
		set { _playCount = value; }
	}
	
	public int wins
	{
		get { return _wins; }
		set { _wins = value; }
	}
	
	public int losses
	{
		get { return _losses; }
		set { _losses = value; }
	}
	
	public string enemyName
	{
		get { return _enemyName; }
		set { _enemyName = value; }
	}
	
	public Dictionary<int,int> expRequired
	{
		get{ return _expRequired; }
		set{ _expRequired = value; }
	}
	
	#endregion
	
	#region CONSTANTS
	
	public const string HPACK_ID_COLUMN 		= "Pack_ID";
	public const string GUN_ID_COLUMN 			= "Gun_ID";
	
	public const string HPACK_TABLE_NAME		= "HealthPacks";
	public const string GUN_TABLE_NAME			= "Weapons";
	
	#endregion
	
	#region Constructors
	
	public PlayerSavedPrefs(System.Object[] objArray )
    {
		_level = Convert.ToInt32(objArray[0]);
		_exp = Convert.ToInt32(objArray[1]);
		_firstName = Convert.ToString(objArray[2]);
		_lastName = Convert.ToString(objArray[3]);
		_isSoundOn = Convert.ToInt32(objArray[4]);
		_isMusicOn = Convert.ToInt32(objArray[5]);
		_controls = Convert.ToInt32(objArray[6]);
		_gold = Convert.ToInt32(objArray[7]);
		_playCount = Convert.ToInt32(objArray[8]);
		_userGun = new Gun( DBAccess.instance.getGunStats(Convert.ToString(objArray[9]) ) );
		_wins = Convert.ToInt32(objArray[10]);
		_losses = Convert.ToInt32(objArray[11]);
		
		populateExpRequired();
		populateAllGuns();
		populateAllHPacks();
		
		// Broadcast updated options to listeners upon construction
		if( onOptionsUpdate != null )
		{
			onOptionsUpdate(_OptionsType.Controls, false, MainMenuController.instance.menuState, ControlScheme );
			onOptionsUpdate(_OptionsType.Music, isMusicOn, MainMenuController.instance.menuState, ControlScheme);
			onOptionsUpdate(_OptionsType.Sounds, isSoundOn, MainMenuController.instance.menuState, ControlScheme);
		}
		
		Debug.Log( "USER GUN: " + _userGun.model );
    }
	
	// Use this constructor for Debug purposes and populating information manually
	public PlayerSavedPrefs() { }
	
	void populateExpRequired()
	{
		System.Object[] items = DBAccess.instance.getExpRequired();
		
		for( int i = 1; i < 26; i++ )
		{
			_expRequired.Add( i, (int)items[i-1] );
		}
	}
	
	void populateAllGuns()
	{
		_allGuns.Insert( 0, new Gun(DBAccess.instance.getGunStats("Colt_1911") ) );
		_allGuns.Insert( 1, new Gun(DBAccess.instance.getGunStats("Winchester_1912")));
		_allGuns.Insert( 2, new Gun(DBAccess.instance.getGunStats("AK-47")));
	}
	
	void populateAllHPacks()
	{
		List<System.Object[]> read = DBAccess.instance.getPurchasedHealthpacks();
			
		for( int i = 0; i < read.Count; i++ )
		{
			HealthPack h = new HealthPack( read[i] );
			allHPacks.Insert( i, h );
		}
	}
	
	#endregion
	
	#region Main Methods
	
	public void useHealthPack( int index )
	{
		if( health < 100 )
		{
			health += hPacks[index].power;
			hPacks[index].quantity--;
		}
		else
			return;
		
		if( onHealthPackUse != null )
			onHealthPackUse();
	}
	
	public void triggerHealthPackUpdate()
	{
		if( onHealthPackUse != null )
			onHealthPackUse();
	}
	
	public void triggerOptionsUpdate()
	{
		if( onOptionsUpdate != null )
		{
			onOptionsUpdate(_OptionsType.Controls, false, MainMenuController.instance.menuState, ControlScheme );
			onOptionsUpdate(_OptionsType.Sounds, isSoundOn, MainMenuController.instance.menuState, ControlScheme );
			onOptionsUpdate(_OptionsType.Music, isMusicOn, MainMenuController.instance.menuState, ControlScheme );
		}
	}
	
	public void findAmmoAndSetAsPurchased( Ammo a )
	{
		Debug.Log("findAmmoAndSetAsPurchased");
		for( int i = 0; i < _allGuns.Count; i++ )
		{
			for( int j = 0; j < _allGuns[i].compatibleAmmo.Count; j++ )
			{
				if( a.ammoID == _allGuns[i].compatibleAmmo[j].ammoID )
				{
					_allGuns[i].compatibleAmmo[j].isPurchased = true;
					_allGuns[i].compatibleAmmo[j].amountOwned++;
					_allGuns[i].compatibleAmmo[j].CommitToDB();
					CommitToDB(true);
				}
			}
		}
	}
	
	public void findGunAndSetAsPurchased( Gun g )
	{
		Debug.Log("findGunAndSetAsPurhcased");
		for( int i = 0; i < _allGuns.Count; i++ )
		{
			if( g.gunID == _allGuns[i].gunID )
			{
				_allGuns[i].isPurchased = true;
				CommitToDB(true);
			}
		}
	}
	
	public void findHealthPackAndSetAsPurchased( HealthPack hp )
	{
		Debug.Log("findHealthPackAndSetAsPurchased");
		for( int i = 0; i < _allHPacks.Count; i++ )
		{
			if( hp.hpackID == _allHPacks[i].hpackID )
			{
				hPacks.Add(_allHPacks[i]);
				hPacks[hPacks.IndexOf(_allHPacks[i])].quantity++;
				CommitToDB(true);
			}
		}
	}
	
	public void broadcastSupplyUpdate()
	{
		Debug.Log("broadcastSupplyUpdate");
		onBroadcastUpdate();
	}
	
	#endregion
	
	#region Database Methods
	
	public void CommitToDB( bool shouldBroadcast )
	{
		Debug.Log( "PlayerSavedPrefs: Committing Prefs." );
		
		// Prepare query values
		System.Object[] prefsValues = new System.Object[12];
		
		prefsValues[0] = _level;
		prefsValues[1] = _exp;
		prefsValues[2] = _firstName;
		prefsValues[3] = _lastName;
		prefsValues[4] = _isSoundOn;
		prefsValues[5] = _isMusicOn;
		prefsValues[6] = _controls;
		prefsValues[7] = _playCount;
		prefsValues[8] = _gold;
		prefsValues[9] = _userGun;
		prefsValues[10] = _wins;
		prefsValues[11] = _losses;
		
		// Write changes for HealthPacks to DB
		for( int i = 0; i < hPacks.Count; i++ )
			hPacks[i].CommitToDB();
		
		for( int i = 0; i < allHPacks.Count; i++)
		{
			allHPacks[i].commitUnlockStatus( 
				HPACK_TABLE_NAME,
				HPACK_ID_COLUMN,
				allHPacks[i].hpackID,
				allHPacks[i].IsPurchased()
			);
		}
				
		// Write changes for equipped Gun to DB
		userGun.CommitToDB();
		
		// Write changes for 
		for( int i = 0; i < _allGuns.Count; i++ )
		{
			_allGuns[i].commitUnlockStatus(
				GUN_TABLE_NAME,
				GUN_ID_COLUMN,
				_allGuns[i].gunID,
				_allGuns[i].IsPurchased()
			);
		}
		
		// Write this PlayerSavedPrefs to DB
		DBAccess.instance.setPlayerPrefs( prefsValues, shouldBroadcast );
	}

	#endregion
}
