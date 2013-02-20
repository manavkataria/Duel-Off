using UnityEngine;
using System.Collections;

/** Purchaseable
 *  -- Base class for all items that will be used in UI and in-game,
 *  -- but are planned to be purchasable through AppStore or Play Store.
 *  -- Include all purchasing information inside of here for transactional
 *  -- information.
 */ 
public class Purchasable {
	
	#region Properties
	
	private int _isPurchased;
	private int _price;
	private int _unlockLevel;
	
	#endregion
	
	#region Accessors
	
	public bool isPurchased
	{
		get { return ( _isPurchased == 1 ) ? true : false; }
		set { _isPurchased = ( value == true ) ? 1 : 0; }
	}
	
	public int price
	{
		get { return _price; }
		set { _price = value; }
	}
	
	public int unlockLevel
	{
		get { return _unlockLevel; }
		set { _unlockLevel = value; }
	}
	
	#endregion
	
	#region Methods
	
	// Use this method to automatically handle logic for SuppliesUIObjects that contain
	// an instance of a derived Purchasable
	public virtual void setIsPurchased(UISprite sprite, bool setTo)
	{
		if( sprite != null )
		{
			if( setTo )
			{
				sprite.alpha = 1f;
				_isPurchased = 1;
			} else {
				sprite.alpha = 0.5f;
				_isPurchased = 0;
			}
		}
	}
	
	// Used to forego conversion from Accessor when need be for DB writes
	public int IsPurchased() { return _isPurchased; }
	
	public void commitUnlockStatus( string tableName, string idColumn, int idRow, int isPurchased )
	{
		DBAccess.instance.setUnlockStatus( tableName, idColumn, idRow, isPurchased );
	}
	
	#endregion
}
