using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/** Gun Object Base
 * Properties and Methods for Gun to be passed into main Player Script
 * -- Property values are populated by sqlite DB from stored values
 */
public class Gun : Purchasable {

	#region Gun Properties

	private float _firingRate;
	private float _power;
	private float _modifiedPower;
	private float _dmgRange;
	private float _recoil;
	
	private List<Ammo> _compatibleAmmo = new List<Ammo>();
	private int _capacity;
	private int _gunID;
	
	private string _model;
	
	private bool _isLob;
	private bool _isReloading;
	
	private Ammo _loadedAmmo;
	
	#endregion
	
	#region Gun Accessors
	
	public float firingRate
	{
		get{ return _firingRate; }
		set{ _firingRate = value; }
	}
	
	public float power
	{
		get{ return _power; }
		set{ _power = value; }
	}
	
	public float modifiedPower
	{
		get { return _power + _loadedAmmo.dmgModifier; }
	}
	
    public float dmgRange
	{
		get{ return _dmgRange; }
		set{ _dmgRange = value; }
	}

    public float recoil
	{
		get{ return _recoil; }
		set{ _recoil = value; }
	}

    public List<Ammo> compatibleAmmo
	{
		get{ return _compatibleAmmo; }
		set{ _compatibleAmmo = value; }
	}

    public int capacity
	{
		get{ return _capacity; }
		set{ _capacity = value; }
	}
	
	public int gunID
	{
		get{ return _gunID; }
		set{ _gunID = value; }
	}
	
	public string model
	{
		get{ return _model; }
		set{ _model = value; }
	}

    public bool isLob
	{
		get{ return _isLob; }
		set{ _isLob = value; }
	}
	
	public bool isReloading
	{
		get{ return _isReloading; }
		set{ _isReloading = value; }
	}
	
	public Ammo loadedAmmo
	{
		get{ return _loadedAmmo; }
		set{ _loadedAmmo = value; }
	}
	
	#endregion

    #region Constructors

    public Gun( System.Object[] gunInfo )
    {
    	_gunID = Convert.ToInt32(gunInfo[0]);
		_model = Convert.ToString(gunInfo[1]);
		_firingRate = Convert.ToSingle(gunInfo[2]);
		_recoil = Convert.ToSingle(gunInfo[3]);
		_power = Convert.ToSingle(gunInfo[4]);
		price = Convert.ToInt32(gunInfo[5]);
		isPurchased = ( Convert.ToInt32(gunInfo[6]) == 1 ) ? true : false;
		unlockLevel = Convert.ToInt32(gunInfo[7]);
		
		isReloading = false;
		
		populateCompatibleAmmo();
    }
	
	public Gun() { }

    #endregion
	
	#region Delegates / Events
	
	public delegate void OnPlayerHitEnemyHandler( GameObject hit );
	public static event OnPlayerHitEnemyHandler onHitByRayCast;
	
	public delegate void OnReloadingHandler( float timeToReload, Gun gun );
	public static event OnReloadingHandler onReloadStart;
	
	public delegate void OnFinishedReloadingHandler(GameObject clicked, Gun gun);
	public static event OnFinishedReloadingHandler onReloadFinish;
	
	public delegate void OnEmptiedHandler(Gun gun);
	public static event OnEmptiedHandler onGunEmptied;
	
	#endregion
	
    #region Main Methods
	
	public void populateCompatibleAmmo()
	{
		/** This below population of compatibleAmmo is a workaroud.  Tried creating new Ammo[3]
		 *  inside of DBAccess.getCompatibleAmmo to return directly, but was constantly getting
		 *  null reference exceptions when setting the values through query.Step().  For some
		 *  reason, creating a new Ammo() instance during Awake() / Start() cycles does not 
		 *  allocate the Ammo() correctly, so System.Object[,] was used instead
		 */ 
		System.Object[,] buffer = DBAccess.instance.getCompatibleAmmo(_gunID);
		
		for( int i = 0; i < 4; i++ )
		{
			Ammo a = new Ammo();
			a.ammoName = Convert.ToString(buffer[i,0]);
			a.dmgModifier = Convert.ToSingle(buffer[i,1]);
			a.recoilModifier = Convert.ToSingle(buffer[i,2]);
			a.magSize = Convert.ToInt32(buffer[i,3]);
			a.spriteName = Convert.ToString(buffer[i,4]);
			a.price = Convert.ToInt32(buffer[i,5]);
			a.isPurchased = ( Convert.ToInt32(buffer[i,6]) == 1 ) ? true : false;
			a.unlockLevel = Convert.ToInt32(buffer[i,7]);
			a.amountOwned = Convert.ToInt32(buffer[i,8]);
			a.ammoID = Convert.ToInt32(buffer[i,9]);
			
			compatibleAmmo.Insert(i,a);
		}
		
		loadedAmmo = compatibleAmmo[0];
	}

    public virtual void Shoot( Transform source, LayerMask rayCastmask, GameObject gunModel ) 
	{ 
		if( capacity > 0 )
		{
			gunModel.animation.Stop();
			gunModel.animation.Play("Shoot");
			
			RaycastHit hit;
			if( Physics.Raycast( source.position, Camera.mainCamera.transform.forward, out hit, 100f, rayCastmask ) )
			{
				if( onHitByRayCast != null )
					onHitByRayCast(hit.collider.gameObject);
			}
			
			capacity--;
			
			if( capacity == 0 ) 
				onGunEmptied(this);
		} else {
			onGunEmptied(this);
		}
	}
	
	public virtual void ShootShotgun( Transform source, LayerMask rayCastmask, GameObject gunModel )
	{
		if( capacity > 0 )
		{
			gunModel.animation.Stop();
			gunModel.animation.Play("Shoot");
			
			Vector3 pointTwo = source.position;
			pointTwo.z += 1;
			
			RaycastHit hit;
			if( Physics.CapsuleCast( source.position, pointTwo, 0.009f, Camera.mainCamera.transform.forward, out hit, 100f, rayCastmask ) )
				if( onHitByRayCast != null )
					onHitByRayCast(hit.collider.gameObject);
			
			capacity--;
			
			if( capacity == 0 )
				onGunEmptied(this);
		} else {
			onGunEmptied(this);
		}
	}
	
	public virtual void Reload( Ammo ammo ) 
	{ 	
		/** Prevent loading of an inferior ammo when a greater ammo is loaded.
		 *  
		 *  The or signifies if it is a lesser ammo and it's magsize is greater than current capacity
		 *  then it is ok to reload
		 * 
		 */ 
		if( compatibleAmmo.IndexOf( ammo ) >= compatibleAmmo.IndexOf( loadedAmmo ) || ammo.magSize > capacity )
		{
			float reloadTime = ( ammo.magSize * 0.05f ) - ( capacity * 0.05f );	
			
			isReloading = true;
			
			loadedAmmo = ammo;	
			
			// If not infinite lead ammo, decrement
			if( compatibleAmmo.IndexOf( ammo ) > 0 )
			{
				loadedAmmo.amountOwned--;
				//Debug.Log("amountOwned--");
			}
			
			onReloadStart( reloadTime, this );
		}
	}
	
	public virtual void CommitToDB()
	{
		System.Object[] gunValues = new System.Object[2];
		
		gunValues[0] = IsPurchased();
		gunValues[1] = _gunID;
		
		for( int i = 1; i < compatibleAmmo.Count; i++ )
			compatibleAmmo[i].CommitToDB();
			
		DBAccess.instance.setGunInfo( gunValues );
	}
	
	public void reloadFinished()
	{
		if( onReloadFinish != null )
			onReloadFinish( null, this );
	}
	
    #endregion

}
