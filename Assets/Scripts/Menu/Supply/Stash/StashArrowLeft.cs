using UnityEngine;
using System.Collections;

/** Stash Left Arrow Button
 * -- Attached to UI_Main > Anim_Root > StorePage > Text (Top Layer) > Arrow Left
 * -- Used for control of Stash Scroll view and to forward event to StashController
 */ 
public class StashArrowLeft : SupplyArrow {

    public delegate void StashArrowLeftHandler(MainMenuController.MenuState state);
    public static event StashArrowLeftHandler moveStashLeft;

    protected override void OnEnable() { MainMenuController.onStashClick += onSelection; }
    protected override void OnDisable() { MainMenuController.onStashClick -= onSelection; }

    protected override void onSelection(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit)
    {
        if (clicked != null)
        {
            if (clicked.gameObject == gameObject)
            {
                moveStashLeft(state);
                tweenArrow(-20f, state);
            }
        }
    }
}
