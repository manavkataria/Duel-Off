using UnityEngine;
using System.Collections;

/* Play Button Handler
 * -- Handles logic for the Play Button, attached to:
 * -- UI_Main > Anim_Root > CrateRoot > Crate03 in Hierarchy
 * -- Recieves SendMessages from MainMenuController
 */ 
public class PlayCrate : MonoBehaviour {
	
	public delegate void OnCharacterSelectStartHandler();
	public static event OnCharacterSelectStartHandler onCharacterSelectStart;
	
    void onMenuClick( MainMenuController mmc )
    {
        mmc.setOffset();

        if (mmc.menuState == MainMenuController.MenuState.Main)
        {
            mmc.menuIsInTransition();
            
			if( onCharacterSelectStart != null )
				onCharacterSelectStart();
			
            StartCoroutine
            (
                mmc.thisGO.animation.PlayWithOptions("MainPlay",
                () => { mmc.applyOffset(); },
                () =>
                {
                    StartCoroutine
                    (
                        mmc.thisGO.animation.PlayWithOptions("EnemySelStart",
                            () => { mmc.menuState = MainMenuController.MenuState.Character; }
                        )
                    );
                })
            );
        }
        else if (mmc.menuState == MainMenuController.MenuState.Store)
        {
            mmc.menuIsInTransition();

			if( onCharacterSelectStart != null )
				onCharacterSelectStart();;
			
            StartCoroutine
            (
                mmc.thisGO.animation.PlayWithOptions("SuppliesPlay",
                    () => { mmc.applyOffset(); },
                    () =>
                    {
                        StartCoroutine
                        (
                            mmc.thisGO.animation.PlayWithOptions("EnemySelStart",
                                () => { mmc.menuState = MainMenuController.MenuState.Character; }
                            )
                        );
                    }
                )
            );
        }
    }
}
