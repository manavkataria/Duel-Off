using UnityEngine;
using System;
using System.Collections;

/* Stash Window Controller
 * -- Central hub for Stash Window
 * -- Used to forward events and SupplyUIObject 
 * -- information to Stash Components / Listeners, as well as
 * -- direct control over the Stash Item Drag Panel
 */ 
public class StashController : SupplyWindow {

    public delegate void StashUpdateHandler(SuppliesUIObject selected);
    public static event StashUpdateHandler onSelection;
	
	// Primarily used to update Ammo related UI attached to the Stash GunUIObjects
	public delegate void UpdateStoreHandler(GunUIObject selected, int ammoIndex);
	public static event UpdateStoreHandler onStashSelection;
	
    protected override void OnEnable()
    {
        StashArrowRight.moveStashRight += moveGridRight;
        StashArrowLeft.moveStashLeft += moveGridLeft;
		MainMenuController.updateInfo += updateInfo;
        MainMenuController.onStashClick += setActiveGrid;
		MainMenuController.onStashClick += forwardAmmoToStore;
		PlayerSavedPrefs.onBroadcastUpdate += onBroadcastUpdate;
    }

    protected override void OnDisable()
    {
        StashArrowRight.moveStashRight -= moveGridRight;
        StashArrowLeft.moveStashLeft -= moveGridLeft;
		MainMenuController.updateInfo -= updateInfo;
        MainMenuController.onStashClick -= setActiveGrid;
		MainMenuController.onStashClick -= forwardAmmoToStore;
		PlayerSavedPrefs.onBroadcastUpdate -= onBroadcastUpdate;
		
		copyHPacksToUserPrefs();
    }
	
	public void Start()
	{
		grids[0].AddPurchasedItems(SuppliesUIObject._SupplyType.Gun, SuppliesUIObject._ItemLocale.StashGridItem );
		grids[1].AddPurchasedItems(SuppliesUIObject._SupplyType.Health, SuppliesUIObject._ItemLocale.StashGridItem);
	 	for (int i = 0; i < grids.Length; i++)
        	grids[i].isActive = (i == 0) ? true : false; 
		
		onGridMove = copyGunToUserPrefs;
	}
	
	void updateInfo() { onStashSelection( (GunUIObject)grids[0].itemArray[0], -1 ); }
	
	void onBroadcastUpdate()
	{
		grids[0].deletePurchasedItems();
		grids[0].AddPurchasedItems(SuppliesUIObject._SupplyType.Gun, SuppliesUIObject._ItemLocale.StashGridItem );
		grids[1].deletePurchasedItems();
		grids[1].AddPurchasedItems(SuppliesUIObject._SupplyType.Health, SuppliesUIObject._ItemLocale.StashGridItem);
	 	for (int i = 0; i < grids.Length; i++)
        	grids[i].isActive = (i == 0) ? true : false; 
	}

    #region Grid Tracking Methods

    protected override void updateCurPos(SupplyGrid grid)
    {
        grid.curPos += (int)grid.moveDirection;

        // Broadcast event of selected SupplyUIObject to listeners to update window info
        if (onSelection != null) 
            onSelection(grid.itemArray[grid.curPos]);
		
		/** Broadcast event of selected GunUIObject to StoreController to update Ammo SupplyGrid
		 *  ( StoreController.grids[1] ) */
		if (onStashSelection != null )
		{
			if( grids[0].isActive )
				onStashSelection((GunUIObject)grid.itemArray[grid.curPos], -1);
		}

        MainMenuController.instance.menuState = lastState;
		
		onGridMove();
    }
	
	/// <summary>
	/// Sets the active grid in instance of SupplyWindow
	/// </summary>
	/// <param name='clicked'>
	/// Clicked.
	/// </param>
	/// <param name='state'>
	/// State.
	/// </param>
	/// <param name='hit'>
	/// Hit.
	/// </param>
    protected override void setActiveGrid(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit)
    {
        if (clicked != null)
        {
            if (clicked.supplyType == SuppliesUIObject._SupplyType.Gun || clicked.supplyType == SuppliesUIObject._SupplyType.Health)
            {
                int j = 0;
                for (int i = 0; i < grids.Length; i++)
                {
                    if (grids[i].itemArray[0].supplyType == clicked.supplyType)
                    {
                        grids[i].isActive = true;
                        j = i;
                    }
                    else { grids[i].isActive = false; }
                }
                StartCoroutine(setCenterToCurPos(j));
            }
        }
    }

    protected override IEnumerator setCenterToCurPos(int activeIndex)
    {
        yield return new WaitForSeconds(0.2f);
		
        Vector3 stashReset = panelPosCache;
        stashReset.x -= grids[activeIndex].cellWidth * grids[activeIndex].curPos;
        windowPanel.transform.localPosition = stashReset;

        Vector4 stashCenterReset = windowPanel.clipRange;
        stashCenterReset.x = panelCenterCache.x + (grids[activeIndex].cellWidth * grids[activeIndex].curPos);
        windowPanel.clipRange = stashCenterReset;

        if (onSelection != null) 
			onSelection(grids[activeIndex].itemArray[grids[activeIndex].curPos]);	
    }
	
	// Announces selected GunUIObject's compatibleAmmo[] to Store for UI Updating
	void forwardAmmoToStore(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit)
	{
		if( clicked.itemLocale == SuppliesUIObject._ItemLocale.StashBottom )
		{
			if( grids[0].isActive )
			{
				int index;
				switch( clicked.name )
				{
				case "Ammo Type 1":
					index = 0;
					break;
				case "Ammo Type 2":
					index = 1;
					break;
				case "Ammo Type 3":
					index = 2;
					break;
				default:
					index = 0;
					break;
				}
				onStashSelection( (GunUIObject)grids[0].itemArray[grids[0].curPos], index );
			}
		}
	}
	
	void copyHPacksToUserPrefs()
	{
		for( int i = 0; i < grids[1].itemArray.Count; i++ )
		{
			HealthUIObject h = (HealthUIObject)grids[1].itemArray[i];
			DBAccess.instance.userPrefs.hPacks.Insert( i, h.hPack );
		}
	}
	
	void copyGunToUserPrefs()
	{
		GunUIObject g = (GunUIObject)grids[0].itemArray[grids[0].curPos];
		DBAccess.instance.userPrefs.userGun = g.gunObj;
	}
	
    #endregion
}
