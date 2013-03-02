using UnityEngine;
using System.Collections;

/* Store Window Controller
 * -- Central hub for Store Window
 * -- Used to forward events and SupplyUIObject 
 * -- information to Stash Components / Listeners, as well as
 * -- direct control over the Store Item Drag Panel
 */
public class StoreController : SupplyWindow {

    public delegate void StoreUpdateHandler(SuppliesUIObject selected);
    public static event StoreUpdateHandler onSelection;
	
	public delegate void StorePurchasingHandler(SuppliesUIObject selected, GameObject tag);
	public static event StorePurchasingHandler onPurchasing;

    protected override void OnEnable()
    {
        StoreArrowRight.moveStoreRight += moveGridRight;
        StoreArrowLeft.moveStoreLeft += moveGridLeft;
        MainMenuController.onStoreClick += setActiveGrid;
		StashController.onStashSelection += updateAmmoGrid;
		MainMenuController.onPurchasableClick += onPurchasableClick;
		PlayerSavedPrefs.onBroadcastUpdate += onBroadcastUpdate;
    }

    protected override void OnDisable()
    {
        StoreArrowRight.moveStoreRight -= moveGridRight;
        StoreArrowLeft.moveStoreLeft -= moveGridLeft;
        MainMenuController.onStoreClick -= setActiveGrid;
		StashController.onStashSelection -= updateAmmoGrid;
		MainMenuController.onPurchasableClick -= onPurchasableClick;
		PlayerSavedPrefs.onBroadcastUpdate -= onBroadcastUpdate;
    }
	
	public void Start()
	{
		// Populate Store items with info from Database
		grids[0].AddPurchasedItems( SuppliesUIObject._SupplyType.Gun, SuppliesUIObject._ItemLocale.StoreGridItem );
		// QUESTION: Why _SupplyType.None, why not _SupplyType.Ammo?
		grids[1].AddPurchasedItems( SuppliesUIObject._SupplyType.None, SuppliesUIObject._ItemLocale.StoreGridItem );
		grids[2].AddPurchasedItems( SuppliesUIObject._SupplyType.Health, SuppliesUIObject._ItemLocale.StoreGridItem );
		
		// Set first grid to active, 0 is default which are the Guns
	 	for (int i = 0; i < grids.Length; i++)
        	grids[i].isActive = (i == 0) ? true : false; 
	}
	
	void onBroadcastUpdate()
	{
		// QUESTION: Why delete Purchased Items?
		grids[0].deletePurchasedItems();
		grids[0].AddPurchasedItems( SuppliesUIObject._SupplyType.Gun, SuppliesUIObject._ItemLocale.StoreGridItem );
		grids[1].deletePurchasedItems();
		grids[1].AddPurchasedItems( SuppliesUIObject._SupplyType.None, SuppliesUIObject._ItemLocale.StoreGridItem );
		grids[2].deletePurchasedItems();
		grids[2].AddPurchasedItems( SuppliesUIObject._SupplyType.Health, SuppliesUIObject._ItemLocale.StoreGridItem );
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

        MainMenuController.instance.menuState = lastState;
    }
	
	protected void updateCurPosMoveTo(int index)
	{
		grids[1].curPos = index;
		
		// Broadcast event of selected SupplyUIObject to listeners to update window info
        if (onSelection != null) 
			onSelection(grids[1].itemArray[grids[1].curPos]);

        MainMenuController.instance.menuState = lastState;
	}

    protected override void setActiveGrid(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit)
    {
			
        if (clicked != null)
        {
			// Temporary bypass to prevent user from purchasing bullets 
			if (clicked.supplyType == SuppliesUIObject._SupplyType.Ammo) {
				//clicked.mWidget.alpha = 0.5f;
				return;
			}

            if ( clicked.supplyType == SuppliesUIObject._SupplyType.Gun || 
                 clicked.supplyType == SuppliesUIObject._SupplyType.Health || 
                 clicked.supplyType == SuppliesUIObject._SupplyType.Ammo ) {   
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
	
	/** Update Ammo Grid ( grids[1] ) depending on the selection sent from the event
	 *  from StashController. 
	 */
	void updateAmmoGrid( GunUIObject selection, int index )
	{
		if( index == -1 )
		{
			for( int i = 0; i < 3 ; i++ )
			{
				AmmoUIObject ammo = (AmmoUIObject)grids[1].itemArray[i];
				ammo.ammo = selection.gunObj.compatibleAmmo[i];
				ammo.mWidget.spriteName = selection.gunObj.compatibleAmmo[i].spriteName;
				
				// If ammo grid is already active, and the current ammo is displayed, update Description
				if( grids[1].isActive && grids[1].curPos == i )
					onSelection(ammo);
			}
		} else {
			if( grids[1].isActive )
				moveGridTo(index, MainMenuController.instance.menuState);
		}
	}
	
	void onPurchasableClick(GameObject clicked)
	{
		if( clicked.tag == "BuyButton" )
		{
			onPurchasing(getActiveGrid().itemArray[getActiveGrid().curPos], clicked);
		}
		else if( clicked.tag == "GoldButton" )
		{
			onPurchasing(null, clicked);
		}
	}

    #endregion 
}
