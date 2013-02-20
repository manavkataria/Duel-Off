using UnityEngine;
using System.Collections;

public class Ammo : Purchasable {
	
	#region Properties
	
	private string _ammoName;
	private float _dmgModifier;
	private float _recoilModifier;
	private int _magSize;
	private string _spriteName;
	private int _unlockLevel;
	private int _amountOwned;
	private int _ammoID;
	
	#endregion
	
	#region Accessors
	
	public string ammoName
	{
		get { return _ammoName; }
		set { _ammoName = value; }
	}
	
	public float dmgModifier
	{
		get { return _dmgModifier; }
		set { _dmgModifier = value; }
	}
	
	public float recoilModifier
	{
		get { return _recoilModifier; }
		set { _recoilModifier = value; }
	}
	
	public int magSize
	{
		get { return _magSize; }
		set { _magSize = value; }
	}
	
	public string spriteName
	{
		get { return _spriteName; }
		set { _spriteName = value; }
	}
	
	public int amountOwned
	{
		get { return _amountOwned; }
		set { _amountOwned = value; }
	}
	
	public int ammoID
	{
		get { return _ammoID; }
		set { _ammoID = value; }
	}
	
	#endregion
	
	#region Constructors
	
	public Ammo()
	{
		_ammoName = "";
		_dmgModifier = -1f;
		_recoilModifier = -1f;
		_magSize = -1;
		_spriteName = "";
	}
	
	#endregion
	
	#region Main Methods
	
	public void CommitToDB()
	{
		System.Object[] ammoValues = new System.Object[3];
		
		ammoValues[0] = IsPurchased();
		ammoValues[1] = _amountOwned;
		ammoValues[2] = _ammoID;
		
		DBAccess.instance.setAmmoInfo( ammoValues );
	}
	
	#endregion
}
