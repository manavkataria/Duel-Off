using UnityEngine;
using System.Collections;

public class AmmoUIObject : SuppliesUIObject {
	
	private Ammo _ammo;
	
    public Ammo ammo
	{
		get { return _ammo; }
		set { _ammo = value; }
	}

    public int storePrice;
	
	void OnEnable() { StashController.onStashSelection += updateAmmo; }
	void OnDisable() { StashController.onStashSelection -= updateAmmo; }
	
	public void updateAmmo(GunUIObject selected, int ammoIndex)
	{
		if( itemLocale == SuppliesUIObject._ItemLocale.StashBottom )
		{
			switch( gameObject.name )
			{
			case "Ammo Type 1": 
				ammo = selected.gunObj.compatibleAmmo[1];
				mWidget.spriteName = ammo.spriteName;
				mWidget.alpha = ( ammo.amountOwned > 0 && ammo.isPurchased) ? 1f : 0.5f;
				break;
			case "Ammo Type 2":
				ammo = selected.gunObj.compatibleAmmo[2];
				mWidget.spriteName = ammo.spriteName;
				mWidget.alpha = ( ammo.amountOwned > 0 && ammo.isPurchased) ? 1f : 0.5f;
				break;
			case "Ammo Type 3":
				ammo = selected.gunObj.compatibleAmmo[3];
				mWidget.spriteName = ammo.spriteName;
				mWidget.alpha = ( ammo.isPurchased ) ? 1f : 0.5f;
				break;
			default:
				break;
			}
		}
		if( itemLocale == SuppliesUIObject._ItemLocale.StoreGridItem )
		{
			SupplyGrid parentGrid = transform.parent.GetComponent<SupplyGrid>();
			switch( gameObject.name )
			{
			case "Ammo0":
				ammo = selected.gunObj.compatibleAmmo[1];
				mWidget.spriteName = ammo.spriteName;
				if( parentGrid.isActive )
					mWidget.alpha = ( ammo.amountOwned > 0 & ammo.isPurchased) ? 1f : 0.5f;
				break;
			case "Ammo1":
				ammo = selected.gunObj.compatibleAmmo[2];
				mWidget.spriteName = ammo.spriteName;
				if( parentGrid.isActive )
					mWidget.alpha = ( ammo.amountOwned > 0 && ammo.isPurchased) ? 1f : 0.5f;
				break;
			case "Ammo2":
				ammo = selected.gunObj.compatibleAmmo[3];
				mWidget.spriteName = ammo.spriteName;
				if( parentGrid.isActive )
					mWidget.alpha = ( ammo.isPurchased) ? 0.5f : 1f;
				break;
			default:
				break;
			}
		}
	}
}
