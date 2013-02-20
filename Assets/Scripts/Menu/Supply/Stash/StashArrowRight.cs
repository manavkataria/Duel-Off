using UnityEngine;
using System.Collections;

/** Stash Right Arrow Button
 * -- Attached to UI_Main > Anim_Root > StorePage > Text (Top Layer) > Arrow Right
 * -- Used for control of Stash Scroll view and to forward event to StashController
 */ 
public class StashArrowRight : SupplyArrow {

    public delegate void StashArrowRightHandler(MainMenuController.MenuState state);
    public static event StashArrowRightHandler moveStashRight;

    protected override void OnEnable() { MainMenuController.onStashClick += onSelection; }
    protected override void OnDisable() { MainMenuController.onStashClick -= onSelection; }

    protected override void onSelection(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit)
    {
        if (clicked != null)
        {
            if (clicked.gameObject == gameObject)
            {
                moveStashRight(state);
                tweenArrow(20f, state);
            }
        }
    }
}
