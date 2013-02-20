using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

/** DBAccess Singleton **
 * 
 * Used to initialize and access database that has stored game information
 * -- This singleton is populated upon a DBaccess.Instance... call.  It is ok
 * -- to create manually, but automation from MonoSingleton is better suited.
 * 
 * -- Object arrays were used for population instead of Generic Dictionary 
 * -- due to Dictionary not being thread safe for synchronous reads from
 * -- persistant Data Path.
 */ 
public class DBAccess : MonoSingleton<DBAccess> {
	
	public static bool debug = false;
	
	public bool isDbInit = false;

    private SQLiteDB db;

    private string dbFileName = "duelOffDB.db";
	
	// DBAccess persists across all levels, placed here instead of creating another singleton
	public PlayerSavedPrefs userPrefs;

	// DBAccess persists across all levels, placed here instead of creating another singleton
	public EnemyStats enemyStats;
	
	private const string GUN_NODE_TRANSFORM_PATH		= "/Bone/Bone Pelvis/Bone Spine/Bone R Clavicle/Bone R UpperArm/Bone R Forearm/Bone R Hand/R Hand";
	private const string GUN_RESOURCES_PATH				= "Models/Guns/";
	
	public static void Log (string log) 
	{ 
		if (debug) UnityEngine.Debug.Log(log);
	}

    // Persist across all levels for easy and constant access of DB 
    public override void Init() 
	{ 
		if( instance != null )
			initDB();
		DontDestroyOnLoad(instance.gameObject); 
	}
	
	void OnEnable()
	{
		if( Application.loadedLevel == 1 )
			instance.isDbInit = true;
		if( Application.loadedLevel == 2 )
		{
			DBAccess.Log(instance.userPrefs.enemyName);
			GameObject g = (GameObject)Instantiate( Resources.Load("Prefabs/Character/InGame/" + instance.userPrefs.enemyName), new Vector3( -6.7f, 0f, 14f ), Quaternion.identity );
			
			GameObject eGun;
			
			switch( DBAccess.instance.enemyStats.enemyGunModel )
			{
			case "Colt1911Enemy":
				eGun = (GameObject)Instantiate( Resources.Load( GUN_RESOURCES_PATH + "Colt_1911/FBX/" + DBAccess.instance.enemyStats.enemyGunModel ) );
				break;
			case "EnemyWinchester":
				eGun = (GameObject)Instantiate( Resources.Load( GUN_RESOURCES_PATH + "Winchester_1912/FBX/" + DBAccess.instance.enemyStats.enemyGunModel ) );
				break;
			case "Ak47Enemy":
				eGun = (GameObject)Instantiate( Resources.Load( GUN_RESOURCES_PATH + "AK-47/FBX/" + DBAccess.instance.enemyStats.enemyGunModel ) );
				break;
			default:
				eGun = (GameObject)Instantiate( Resources.Load( GUN_RESOURCES_PATH + "Colt_1911/FBX/" + DBAccess.instance.enemyStats.enemyGunModel ) );
				break;
			}
						
			eGun.transform.parent = g.transform.FindChild( instance.userPrefs.enemyName + GUN_NODE_TRANSFORM_PATH);
			eGun.transform.localPosition = Vector3.zero;
			
			eGun.transform.localEulerAngles = Vector3.zero;
			
			EnemyBase b = g.GetComponent<EnemyBase>();
			
			DBAccess.Log( DBAccess.instance.userPrefs.enemyName );
									
			b.gunActual = g.transform.FindChild( DBAccess.instance.userPrefs.enemyName + GUN_NODE_TRANSFORM_PATH + "/" + DBAccess.instance.enemyStats.enemyGunModel + "(Clone)" ).gameObject;
			
			isDbInit = true;
		}
	}
	
	// Set initialization state to false on Level Change for next scene setup
	// public void OnDisable() { instance.isDbInit = false; }
	
	#region Read Methods
	
	#region Characters
	
	public System.Object[] getPlayerSavedPrefs()
	{
		instance.isDbInit = false;
		float startTime = Time.realtimeSinceStartup;
		
		SQLiteQuery query;
		query = new SQLiteQuery( instance.db, "SELECT * FROM PlayerSavedPrefs" );
		
		System.Object[] returnArray = new System.Object[12];
		
		DBAccess.Log("SQLiteKit: Sending getPlayerSavedPrefs query to Database.");
		while (query.Step())
		{
			returnArray[0] = query.GetInteger("Level");
			returnArray[1] = query.GetInteger("Experience");
			returnArray[2] = query.GetString("First_Name");
			returnArray[3] = query.GetString("Last_Name");
			returnArray[4] = query.GetInteger("Sound");
			returnArray[5] = query.GetInteger("Music");
			returnArray[6] = query.GetInteger("Controls");
			returnArray[7] = query.GetInteger("Gold");
			returnArray[8] = query.GetInteger("Play_Count");
			returnArray[9] = query.GetString("Preferred_Gun");
			returnArray[10] = query.GetInteger("Wins");
			returnArray[11] = query.GetInteger("Losses");
		}
		
		DBAccess.Log("SQLiteKit: String of queries finished. Process time: " + ( Time.realtimeSinceStartup - startTime ) );
		query.Release();
		instance.isDbInit = true;
		
		return returnArray;
	}
	
    public System.Object[] getEnemyStats( int level )
    {
		float startTime = Time.realtimeSinceStartup;
		
        SQLiteQuery query;
        System.Object[] returnArray = new System.Object[11];

        DBAccess.Log("SQLiteKit: Sending getEnemyStats query to Database.");
        query = new SQLiteQuery( instance.db, "SELECT * FROM EnemyStats WHERE Level = " + level.ToString());
        while (query.Step())
        {
			returnArray[0] = query.GetInteger("Level");
			returnArray[1] = query.GetDouble("HitRate");
			returnArray[2] = query.GetDouble("HideRate");
			returnArray[3] = query.GetDouble("MinHide");
			returnArray[4] = query.GetDouble("MaxHide");
			returnArray[5] = query.GetDouble("Run Speed");
			returnArray[6] = query.GetDouble("MinShotFreq");
			returnArray[7] = query.GetDouble("MaxShotFreq");
			returnArray[8] = query.GetDouble("MinStillRate");
			returnArray[9] = query.GetDouble("MaxStillRate");
			returnArray[10] = query.GetDouble("Damage");
        }
        DBAccess.Log("SQLiteKit: String of queries finished. Process time: " + ( Time.realtimeSinceStartup - startTime ) );
        query.Release();

        return returnArray;
    }
	
	public System.Object[,] getRandomEnemyName()
	{
		SQLiteQuery query;
		System.Object[,] returnArray = new System.Object[2,1];
		
		query = new SQLiteQuery( instance.db, "SELECT * FROM Enemy_Names" );
		
		int stepCount = 0;
		
		int firstCount = UnityEngine.Random.Range( 1, 99 );
		int secondCount = UnityEngine.Random.Range( 1, 99 );
		
		while( query.Step() )
		{
			if( firstCount == stepCount )
				returnArray[0,0] = query.GetString("First");
			if( secondCount == stepCount )
				returnArray[1,0] = query.GetString("Last");
			
			stepCount++;
		}
		
		query.Release();
		
		return returnArray;
	}
	
	public System.Object[] getExpRequired()
	{
		SQLiteQuery query;
		System.Object[] returnArray = new System.Object[25];
		
		query = new SQLiteQuery( instance.db, "SELECT * FROM EnemyStats" );
		
		int stepCount = 0;
		while( query.Step() )
		{
			returnArray[stepCount] = query.GetInteger("Exp_Required");
			
			stepCount++;
		}
		
		query.Release();
		
		return returnArray;
	}
	
	#endregion
	
	#region Items
	
	public System.Object[] getGunStats( string gunDBName )
	{
		float startTime = Time.realtimeSinceStartup;
		
		SQLiteQuery query;
		System.Object[] returnArray = new System.Object[8];
		
		DBAccess.Log("SQLiteKit: Sending getGunStats query to Database.");
		query = new SQLiteQuery( instance.db, "SELECT * FROM Weapons WHERE Model = '" + gunDBName + "'" );
		while(query.Step())
		{
			returnArray[0] = query.GetInteger("Gun_ID");
			returnArray[1] = query.GetString("Model");
			returnArray[2] = query.GetDouble("Firing_Rate");
			returnArray[3] = query.GetDouble("Recoil");
			returnArray[4] = query.GetDouble("Power");
			returnArray[5] = query.GetInteger("Price");
			returnArray[6] = query.GetInteger("isPurchased");
			returnArray[7] = query.GetInteger("Unlock_Level");
		}
		DBAccess.Log("SQLiteKit: String of queries finished. Process time: " + ( Time.realtimeSinceStartup - startTime ) );
		query.Release();
		
		return returnArray;
	}
	
	public System.Object[,] getCompatibleAmmo( int gunID )
	{
		float startTime = Time.realtimeSinceStartup;
		
		SQLiteQuery query;
		
		System.Object[,] ammoArray = new System.Object[4,10];
		
		DBAccess.Log("SQLiteKit: Sending getComopatibleAmmo query to Database.");
		query = new SQLiteQuery( instance.db,
			"SELECT b.* " + 
			"FROM Weapons a " + 
			"JOIN Compatible_Ammo b " +
			"ON a.Gun_ID = b.Gun_Model " +
			"WHERE a.Gun_ID = " + gunID.ToString()
			);
			
		int stepCount = 0;
		while(query.Step())
		{
			ammoArray[stepCount,0] = query.GetString("Name");
			ammoArray[stepCount,1] = query.GetDouble("Dmg_Modifier");
			ammoArray[stepCount,2] = query.GetDouble("Rec_Modifier");
			ammoArray[stepCount,3] = query.GetInteger("Mag_Size");
			ammoArray[stepCount,4] = query.GetString("Sprite_Name");
			ammoArray[stepCount,5] = query.GetInteger("Price");
			ammoArray[stepCount,6] = query.GetInteger("isPurchased");
			ammoArray[stepCount,7] = query.GetInteger("Unlock_Level");
			ammoArray[stepCount,8] = query.GetInteger("Amount_Owned");
			ammoArray[stepCount,9] = query.GetInteger("Ammo_ID");
			
			stepCount++;
		}
		DBAccess.Log("SQLiteKit: String of queries finished. Process time: " + ( Time.realtimeSinceStartup - startTime ) );
		query.Release();
		
		return ammoArray;
	}
	
	public List<System.Object[]> getWeapons( bool purchased )
	{
		float startTime = Time.realtimeSinceStartup;
		
		SQLiteQuery query;
		
		List<System.Object[]> purchasedList = new List<System.Object[]>();
		
		DBAccess.Log("SQLiteKit: Sending getWeapons(" + purchased + ") query to Database.");
		
		if( purchased )
			query = new SQLiteQuery( instance.db, "SELECT * FROM Weapons WHERE isPurchased = 1" );
		else
			query = new SQLiteQuery( instance.db, "SELECT * FROM Weapons" );
		
		int stepCount = 0;
		while(query.Step())
		{
			System.Object[] item = new System.Object[8];
			item[0] = query.GetInteger("Gun_ID");
			item[1] = query.GetString("Model");
			item[2] = query.GetDouble("Firing_Rate");
			item[3] = query.GetDouble("Recoil");
			item[4] = query.GetDouble("Power");
			item[5] = query.GetInteger("Price");
			item[6] = query.GetInteger("isPurchased");
			item[7] = query.GetInteger("Unlock_Level");
			purchasedList.Insert( stepCount, item );
			
			stepCount++;
		}
		DBAccess.Log("SQLiteKit: String of queries finished. Process time: " + ( Time.realtimeSinceStartup - startTime ) );
		query.Release();
		
		return purchasedList;
	}
	
	public List<System.Object[]> getPurchasedHealthpacks()
	{
		float startTime = Time.realtimeSinceStartup;
		
		SQLiteQuery query;
		
		List<System.Object[]> purchasedList = new List<System.Object[]>();
		
		DBAccess.Log("SQLiteKit: Sending getPurchasedHealthpacks query to Database.");
		query = new SQLiteQuery( instance.db, "SELECT * FROM HealthPacks" );
		
		int stepCount = 0;
		while(query.Step())
		{
			System.Object[] item = new System.Object[7];
			item[0] = query.GetString("Type");
			item[1] = query.GetDouble("HP_Restore");
			item[2] = query.GetInteger("Amount_Owned");
			item[3] = query.GetInteger("Price");
			item[4] = query.GetInteger("Pack_ID");
			item[5] = query.GetInteger("Unlock_Level");
			item[6] = query.GetInteger("isPurchased");
			
			purchasedList.Insert( stepCount, item );
			
			stepCount++;
		}
		DBAccess.Log("SQLiteKit: String of queries finished. Process time: " + ( Time.realtimeSinceStartup - startTime ) );
		query.Release();
		
		return purchasedList;
	}
	
	#endregion
	
	#endregion
	
	#region Write Methods
	
	#region Character
	
	public void setPlayerPrefs(System.Object[] insertValues, bool shouldBroadcast )
	{
		SQLiteQuery query;
		
		query = new SQLiteQuery( instance.db, 
			"UPDATE PlayerSavedPrefs " +
			"SET Level = ?, Experience = ?, First_Name = ?, Last_Name = ?, Sound = ?, Music = ?, Controls = ?, Play_Count = ?, Gold = ?, Preferred_Gun = ?, Wins = ?, Losses = ? " + 
			"WHERE Row_ID = 1" 
		);
		
		query.Bind((int)insertValues[0]);
		query.Bind((int)insertValues[1]);
		query.Bind((string)insertValues[2]);
		query.Bind((string)insertValues[3]);
		query.Bind((int)insertValues[4]);
		query.Bind((int)insertValues[5]);
		query.Bind((int)insertValues[6]);
		query.Bind((int)insertValues[7]);
		query.Bind((int)insertValues[8]);
		query.Bind( (string)( ( (Gun)insertValues[9] ).model ) );
		query.Bind((int)insertValues[10]);
		query.Bind((int)insertValues[11]);
		
		query.Step();
		
		query.Release();
		
		if( shouldBroadcast ) 
			instance.userPrefs.broadcastSupplyUpdate();
	}
	
	#endregion
	
	#region Items
	
	public void setGunInfo(System.Object[] insertValues)
	{
		SQLiteQuery query;
		
		query = new SQLiteQuery( instance.db,
			"UPDATE Weapons " + 
			"SET isPurchased = ? " +
			"WHERE Gun_ID = ?"
		);
		
		query.Bind((int)insertValues[0]);
		query.Bind((int)insertValues[1]);
		
		query.Step();
		
		query.Release();
	}
	
	public void setAmmoInfo(System.Object[] insertValues )
	{
		SQLiteQuery query;
		
		query = new SQLiteQuery( instance.db,
			"UPDATE Compatible_Ammo " +
			"SET isPurchased = ?, Amount_Owned = ? " +
			"WHERE Ammo_ID = ?"
		);
		
		query.Bind((int)insertValues[0]);
		query.Bind((int)insertValues[1]);
		query.Bind((int)insertValues[2]);
		
		query.Step();
		
		query.Release();
	}
	
	public void setHealthPackInfo(System.Object[] insertValues )
	{
		SQLiteQuery query;
		
		query = new SQLiteQuery( instance.db,
			"UPDATE HealthPacks " +
			"SET Amount_Owned = ? " +
			"WHERE Pack_ID = ?"
		);
		
		query.Bind((int)insertValues[0]);
		query.Bind((int)insertValues[1]);
		
		query.Step();
		
		query.Release();
	}
	
	public void setUnlockStatus( string tableName, string idColumn, int idRow, int isPurchased )
	{
		SQLiteQuery query;
		
		query = new SQLiteQuery( instance.db,
			"UPDATE " + tableName + " " +
			"SET isPurchased = ? " +
			"WHERE " + idColumn + " = ?"
		);
		
		query.Bind((int)isPurchased);
		query.Bind((int)idRow);
		
		query.Step();
		
		query.Release();
	}
	
	#endregion
	
	#endregion
	
	#region Init and Clean Up
	
	// db.Open() -- Open Stream, keep open until application is closed for faster reads.
    public void initDB()
    {
        instance.db = new SQLiteDB();

		string filename = Application.persistentDataPath + "/localCopy_v1_" + dbFileName;
		
		float startTime = Time.realtimeSinceStartup;
		
		// check if database already exists.
		if(!File.Exists(filename))
		{
			DBAccess.Log("DBAccess: No DB found. Creating new local instance.");
			// First application start
			// copy prebuilt DB from StreamingAssets and load / store to persistantPath	
			byte[] bytes = null;				
			
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
			string dbpath = "file://" + Application.streamingAssetsPath + "/" + dbFileName; 
			WWW www = new WWW(dbpath);
			instance.download(www);
			bytes = www.bytes;
#elif UNITY_WEBPLAYER
			string dbpath = "StreamingAssets/" + dbFileName;								
			WWW www = new WWW(dbpath);
			instance.download(www);
			bytes = www.bytes;
#elif UNITY_IPHONE
			string dbpath = Application.dataPath + "/Raw/" + dbFileName;										
			try{	
				using ( FileStream fs = new FileStream(dbpath, FileMode.Open, FileAccess.Read, FileShare.Read) ){
					bytes = new byte[fs.Length];
					fs.Read(bytes,0,(int)fs.Length);
				}			
			} catch (Exception e) { DBAccess.Log( e.ToString() ); }
#elif UNITY_ANDROID
			string dbpath = Application.streamingAssetsPath + "/" + dbFileName;	           
			WWW www = new WWW(dbpath);
			instance.download(www);
			bytes = www.bytes;
#endif
			if ( bytes != null )
			{
				try{	
					// copy database to real file into cache folder
					using( FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write ) )
					{
						fs.Write(bytes,0,bytes.Length);             
					}
					// initialize database
					instance.db.Open(filename); 
					DBAccess.Log("DBAccess: Local DB created and opened. Process Time: " + ( Time.realtimeSinceStartup - startTime ) );
					instance.isDbInit = true;
					
				} catch (Exception e) { DBAccess.Log( e.ToString() ); }
			}
		}
		else
		{
			DBAccess.Log("DBAccess: DB Found. Opening local instance.");
			// Means we already downloaded prebuilt data base and stored into persistantPath
			try
			{
				// initialize database
				instance.db.Open(filename); 
				DBAccess.Log("DBAccess: Local DB opened. Process Time: " + ( Time.realtimeSinceStartup - startTime ) );
				instance.isDbInit = true;
				
			} catch (Exception e) { DBAccess.Log( e.ToString() ); }
		}
		
		instance.userPrefs = new PlayerSavedPrefs( instance.getPlayerSavedPrefs() );
    }

    // Use Unity WWW class to read byte stream from file, used for cross compatibility
    IEnumerator download(WWW www) { yield return www; }
	
	// Close db connection upon Application Quit, just in case.
	public override void OnApplicationQuit()
	{
		if( instance.userPrefs != null )
			instance.userPrefs.CommitToDB(false);
		instance.db.Close();
		base.OnApplicationQuit();
	}
	
	#endregion
}
