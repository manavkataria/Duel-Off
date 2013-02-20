using UnityEngine;
using System;
using System.Collections;

public class HealthPack : Purchasable {
	
	#region Properties
	
	private int _quantity;
	private float _power;
	private string _type;
	private int _hpackID;
	
	#endregion
	
	#region Accessors
	
    public int quantity
	{
		get { return _quantity; }
		set { _quantity = ( value > 0 ) ? value : 0; }
	}
	
    public float power
	{
		get { return _power; }
		set { _power = value; }
	}
	
	public string model
	{
		get { return _type; }
		set { _type = value; }
	}
	
	public int hpackID
	{
		get { return _hpackID; }
		set { _hpackID = value; }
	}
	
	#endregion
	
	#region Constructors
	
	public HealthPack( System.Object[] hpInfo )
	{
		_type = Convert.ToString(hpInfo[0]);
		_power = Convert.ToSingle(hpInfo[1]);
		_quantity = Convert.ToInt32(hpInfo[2]);
		price = Convert.ToInt32(hpInfo[3]);
		_hpackID = Convert.ToInt32(hpInfo[4]);
		unlockLevel = Convert.ToInt32(hpInfo[5]);
		isPurchased = ( Convert.ToInt32(hpInfo[6]) == 1 ) ? true : false;
	}
	
	#endregion
	
	#region Main Methods
	
	public void CommitToDB()
	{
		System.Object[] packValues = new System.Object[2];
			
		packValues[0] = _quantity;
		packValues[1] = _hpackID;
		
		DBAccess.instance.setHealthPackInfo( packValues );
	}
	
	public override void setIsPurchased(UISprite sprite, bool setTo)
	{
		if( sprite != null )
		{
			if( setTo && DBAccess.instance.userPrefs.Level >= unlockLevel )
			{
				sprite.alpha = 1f;
				isPurchased = true;
			} else {
				sprite.alpha = 0.5f;
				isPurchased = false;
			}
		}
	}
	
	#endregion
}
