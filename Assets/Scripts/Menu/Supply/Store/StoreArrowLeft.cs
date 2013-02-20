using UnityEngine;
using System.Collections;

/** Stash Left Arrow Button
 * -- Attached to UI_Main > Anim_Root > StorePage > Text (Top Layer) > Arrow Left
 * -- Used for control of Stash Scroll view and to forward event to StashController
 */
public class StoreArrowLeft : SupplyArrow {

    public delegate void StoreArrowLeftHandler(MainMenuController.MenuState state);
    public static event StoreArrowLeftHandler moveStoreLeft;

    protected override void OnEnable() { MainMenuController.onStoreClick += onSelection; }
    protected override void OnDisable() { MainMenuController.onStoreClick -= onSelection; }

    protected override void onSelection(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit)
    {
        if (clicked != null)
        {
            if (clicked.gameObject == gameObject)
            {
                moveStoreLeft(state);
                tweenArrow(-20f, state);
            }
        }
    }
}
