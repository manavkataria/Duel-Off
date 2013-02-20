using UnityEngine;
using System.Collections;

/** Base Supply Arrow functionality
 * -- Base class for movement arrows in the Store / Stash Windows
 */ 
public abstract class SupplyArrow : MonoBehaviour {
	
	// Simple flag for prevention of over clicking on items
    public bool isAnimating = false;
	
	/** Used for registration and unregistration of required event listener events from MainMenuController
	 * and other sources. */ 
    protected abstract void OnEnable();
    protected abstract void OnDisable();
	
	/* Used to call abstractly declared delegates to signal StashController
	 * to move the grid and update information accordingly. */
    protected abstract void onSelection(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit);
	
	// Move the arrow left and right for UI feedback to the user
    protected void tweenArrow(float amount, MainMenuController.MenuState state)
    {
        if (state == MainMenuController.MenuState.Store && !isAnimating)
        {
            isAnimating = true;

            iTween.MoveTo(gameObject, iTween.Hash(
                "x", transform.localPosition.x + amount,
                "time", 0.2f,
                "islocal", true,
                "easetype", iTween.EaseType.easeOutExpo
                )
            );
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", transform.localPosition.x,
                "time", 0.2f,
                "islocal", true,
                "delay", 0.2f,
                "oncompletetarget", gameObject,
                "oncomplete", "resetState",
                "easetype", iTween.EaseType.easeInExpo
                )
            );
        }
    }
	
	// Used to turn off flag for over clicking on items
    protected void resetState() { isAnimating = false; }
}
