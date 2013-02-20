using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SupplyGrid : UIGrid {

    public enum _moveDirection { Left = -1, None, Right };
    public _moveDirection moveDirection;

    public int curPos = 0;
	public int length;
	
	/** Hide itemArray from Inspector because itemArray Count needs
	 *  to be determined by the database query step count. When properties
	 *  are public and viewable in the Inspector, Unity serializes the data
	 *  causing the List.Count to be set to serialized values.
	 */ 
	[System.NonSerializedAttribute]
    public List<SuppliesUIObject> itemArray = new List<SuppliesUIObject>();

    private bool _isActive = false;

    public bool isActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            StartCoroutine(tweenAlpha(_isActive));
        }
    }
	
	/// <summary>
	/// Adds purchased items from DB to instance of SupplyWindow.
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	/// <param name='locale'>
	/// Locale.
	/// </param>
	public void AddPurchasedItems( SuppliesUIObject._SupplyType type, SuppliesUIObject._ItemLocale locale )
	{	
		if( type == SuppliesUIObject._SupplyType.Gun && locale == SuppliesUIObject._ItemLocale.StashGridItem)
		{
			List<System.Object[]> read = DBAccess.instance.getWeapons(true);
			
			for( int i = 0; i < read.Count; i++ )
			{
				GameObject temp = (GameObject)Instantiate( Resources.Load("Prefabs/UI/Supply/StashGunUIObject") );
				temp.transform.parent = transform;
				temp.name = "StashGunUIObject" + i.ToString();

				GunUIObject g = temp.GetComponent<GunUIObject>();
				g.gunObj = new Gun( read[i] );
				g.transform.localScale = g.cachedLocalScale;
				g.transform.localPosition = g.cachedLocalPosition;
				g.transform.localEulerAngles = Vector3.zero;
				g.model = g.gunObj.model;				
				
				itemArray.Insert( i, g );
				itemArray[i].mWidget.spriteName = g.model + "_Icon";
			}
		}
		else if( type == SuppliesUIObject._SupplyType.Gun && locale == SuppliesUIObject._ItemLocale.StoreGridItem )
		{
			List<System.Object[]> read = DBAccess.instance.getWeapons(false);
			
			for( int i = 0; i < read.Count; i++ )
			{
				GameObject temp = (GameObject)Instantiate( Resources.Load("Prefabs/UI/Supply/StoreGunUIObject") );
				temp.transform.parent = transform;
				temp.name = "StoreGunUIObject" + i.ToString();
				
				GunUIObject g = temp.GetComponent<GunUIObject>();
				g.gunObj = new Gun( read[i] );
				g.transform.localScale = g.cachedLocalScale;
				g.transform.localPosition = g.cachedLocalPosition;
				g.transform.localEulerAngles = Vector3.zero;
				g.model = g.gunObj.model;
				
				itemArray.Insert( i, g );
				itemArray[i].mWidget.spriteName = g.model + "_Icon";
			}
		}
		else if( type == SuppliesUIObject._SupplyType.Health )
		{
			List<System.Object[]> read = DBAccess.instance.getPurchasedHealthpacks();
			
			HealthUIObject[] buffer = GetComponentsInChildren<HealthUIObject>();
			
			for( int i = 0; i < read.Count; i++ )
			{
				if( buffer[i].name.Contains(i.ToString()) )
				{
					buffer[i].hPack = new HealthPack( read[i] );
					buffer[i].hPack.setIsPurchased( buffer[i].mWidget, buffer[i].hPack.isPurchased );
					itemArray.Insert( i, buffer[i] );
				}
			}
		}
		else
		{
			AmmoUIObject[] buffer = GetComponentsInChildren<AmmoUIObject>();
			
			for( int i = 0; i < buffer.Length; i++ )
			{
				if( buffer[i].name.Contains(i.ToString()) )
				{
					itemArray.Insert( i, buffer[i] );
				}
			}
		}
		
		length = itemArray.Count;
		repositionNow = true;
	}
	/// <summary>
	/// Used only when purchasing an item, deletes Arrays to be re-read and populated again
	/// by renewed DB information.
	/// </summary>
	public void deletePurchasedItems()
	{
		if( itemArray[0].GetType() == typeof(GunUIObject) )
		{
			for( int i = 0; i < itemArray.Count; i++ )
				DestroyImmediate( itemArray[i].gameObject );
		}
		
		itemArray.Clear();
		length = itemArray.Count;
	}

    IEnumerator tweenAlpha(bool isActive)
    {
        if (isActive)
        {
            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < itemArray.Count; i++)
                if (i != curPos) 
					itemArray[i].mWidget.alpha = getAlphaValue(itemArray[i]);
            
            iTween.ValueTo( gameObject, iTween.Hash( 
                "from", itemArray[curPos].mWidget.alpha,
                "to", getAlphaValue(itemArray[curPos]),
                "time", 0.2f,
                "onupdate", "tweenUpdate",
                "easetype", iTween.EaseType.linear
                )
            );
        }
        else
        {
            yield return null;

            for (int i = 0; i < itemArray.Count; i++)
                if (i != curPos) 
					itemArray[i].mWidget.alpha = 0;

            iTween.ValueTo(gameObject, iTween.Hash(
                "from", itemArray[curPos].mWidget.alpha,
                "to", 0f,
                "time", 0.2f,
                "onupdate", "tweenUpdate",
                "easetype", iTween.EaseType.linear
                )
            );
        }
    }

    void tweenUpdate(float newValue) { itemArray[curPos].mWidget.alpha = newValue; }
	
	/// <summary>
	/// Gets the alpha value for DB items.
	/// </summary>
	/// <returns>
	/// The alpha value.
	/// </returns>
	/// <param name='item'>
	/// Item.
	/// </param>
	float getAlphaValue(SuppliesUIObject item)
	{
		Type g = typeof(GunUIObject);
		Type h = typeof(HealthUIObject);
		Type a = typeof(AmmoUIObject);
		
		Type t = item.GetType();
		
		if( t == g )
		{
			GunUIObject temp = (GunUIObject)item;
			if( temp.itemLocale == SuppliesUIObject._ItemLocale.StoreGridItem )
				return ( temp.gunObj.isPurchased ) ? 0.5f : 1f;
		}
		if( t == h )
		{
			HealthUIObject temp = (HealthUIObject)item;
			if( temp.itemLocale == SuppliesUIObject._ItemLocale.StashGridItem )
				return ( temp.hPack.quantity > 0 ) ? 1f : 0.5f;
		}
		if( t == a )
		{
			AmmoUIObject temp = (AmmoUIObject)item;
			int index = itemArray.IndexOf( temp );
				
			if( temp.itemLocale == SuppliesUIObject._ItemLocale.StoreGridItem )
			{
				if( index < 2 )
					return 1f;
				else
					return ( temp.ammo.isPurchased ) ? 0.5f : 1f;
			}
			return ( temp.ammo.isPurchased ) ? 1f : 0.5f;
		}
		
		return 1f;
	}
}
