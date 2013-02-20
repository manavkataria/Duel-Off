using UnityEngine;
using System.Collections;

/** Store Right Arrow Button
 * -- Attached to UI_Main > Anim_Root > StorePage > Text (Top Layer) > Arrow Right
 * -- Used for control of Store Scroll view and to forward event to StoreController
 */
public class StoreArrowRight : SupplyArrow {

    public delegate void StoreArrowRightHandler(MainMenuController.MenuState state);
    public static event StoreArrowRightHandler moveStoreRight;

    protected override void OnEnable() { MainMenuController.onStoreClick += onSelection; }
    protected override void OnDisable() { MainMenuController.onStoreClick -= onSelection; }

    protected override void onSelection(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit)
    {
        if (clicked != null)
        {
            if (clicked.gameObject == gameObject)
            {
                moveStoreRight(state);
                tweenArrow(20f, state);
            }
        }
    }
}
