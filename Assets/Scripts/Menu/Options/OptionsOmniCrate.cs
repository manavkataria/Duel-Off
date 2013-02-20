using UnityEngine;
using System.Collections;

/* Options Button Handler
 * -- Handles logic for the Options Button, attached to:
 * -- UI_Main > Anim_Root > CrateRoot > Crate01 in Hierarchy
 * -- Recieves SendMessages from MainMenuController
 * -- Special Case: Recycled use for back button
 */
public class OptionsOmniCrate : MonoBehaviour {
	
	public delegate void OnOptionsBackHandler();
	public static event OnOptionsBackHandler onOptionsBack;
	
    void onMenuClick( MainMenuController mmc )
    {
        mmc.setOffset();

        if (mmc.menuState == MainMenuController.MenuState.Main)
        {
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				ChartBoostBinding.trackEvent( "On Options Click" );
			
            mmc.menuIsInTransition();

            StartCoroutine
            (
                mmc.thisGO.animation.PlayWithOptions("MainOptions",
                () => { mmc.applyOffset(); },
                () =>
                {
                    StartCoroutine
                    (
                        mmc.thisGO.animation.PlayWithOptions("OptionsStart",
                            () => 
							{
								mmc.menuState = MainMenuController.MenuState.Options; 
								mmc.thisGO.animation.Play("OptionsIdle");
							}
                        )
                    );
                }
            ));
        }
        else if (mmc.menuState == MainMenuController.MenuState.Options)
        {
            mmc.menuIsInTransition();

            StartCoroutine
            (
                mmc.thisGO.animation.PlayWithOptions("OptionsBack",
                    () => { mmc.applyOffset(); },
                    () => { mmc.playMenuStart(); } 
                )
            );
        }
        else if (mmc.menuState == MainMenuController.MenuState.Character )
        {
            mmc.menuIsInTransition();

            StartCoroutine
            (
                mmc.thisGO.animation.PlayWithOptions("EnemySelBack",
                    () => { mmc.applyOffset(); },
                    () => { mmc.playMenuStart(); }
                )
            );
			
			if( onOptionsBack != null )
				onOptionsBack();
        }
        else if (mmc.menuState == MainMenuController.MenuState.Store)
        {
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				ChartBoostBinding.trackEvent( "On Store Enter" );
				
            mmc.menuIsInTransition();

            StartCoroutine
            (
                mmc.thisGO.animation.PlayWithOptions("SuppliesBack",
                    () => { mmc.applyOffset(); },
                    () => { mmc.playMenuStart(); }
                )
            );
        }
    }
}
