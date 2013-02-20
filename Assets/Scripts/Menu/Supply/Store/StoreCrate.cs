using UnityEngine;
using System.Collections;

/* Store Button Handler
 * -- Handles logic for the Supplies Button, attached to:
 * -- UI_Main > Anim_Root > CrateRoot > Crate02 in Hiearchy
 * -- Recieves SendMessages from MainMenuController
 */
public class StoreCrate : MonoBehaviour {

    void onMenuClick(MainMenuController mmc)
    {
        mmc.setOffset();

        if (mmc.menuState == MainMenuController.MenuState.Main)
        {
            mmc.menuIsInTransition();

            StartCoroutine
            (
                mmc.thisGO.animation.PlayWithOptions("MainSupplies",
                () => { mmc.applyOffset(); },
                () =>
                {
                    StartCoroutine
                    (
                        mmc.thisGO.animation.PlayWithOptions("SuppliesStart",
                            () => 
							{ 
								mmc.menuState = MainMenuController.MenuState.Store; 
								mmc.animation.Play("SuppliesIdle");
							}
                        )
                    );
                }
            ));
        }
    }
}
