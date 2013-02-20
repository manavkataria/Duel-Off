using UnityEngine;
using System.Collections;

/** Store Top Highlight Control
 * -- Used to select and highlight pressed items
 * -- at the top of the Store Window, as well as to forward
 * -- this event to StashController
 */ 
public class StoreTopController : MonoBehaviour {

    void OnEnable() { MainMenuController.onStoreClick += onSelection; }

    void OnDisable() { MainMenuController.onStoreClick -= onSelection; }

    void onSelection(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit )
    {
        if (clicked != null)
        {
            if (state != MainMenuController.MenuState.InTransition && clicked.itemLocale == SuppliesUIObject._ItemLocale.StoreTop)
            {
                MainMenuController.instance.menuState = MainMenuController.MenuState.InTransition;

                iTween.MoveTo(gameObject, iTween.Hash(
                        "x", clicked.transform.localPosition.x,
                        "time", 0.4f,
                        "islocal", true,
                        "easetype", iTween.EaseType.easeOutExpo
                    )
                );
                iTween.ScaleTo(clicked.gameObject, iTween.Hash(
                        "x", clicked.transform.localScale.x * 1.5f,
                        "y", clicked.transform.localScale.y * 1.5f,
                        "time", 0.2f,
                        "easetype", iTween.EaseType.easeOutExpo
                    )
                );
                iTween.ScaleTo(clicked.gameObject, iTween.Hash(
                        "x", clicked.transform.localScale.x,
                        "y", clicked.transform.localScale.y,
                        "time", 0.2f,
                        "delay", 0.2f,
                        "oncompletetarget", gameObject,
                        "oncomplete", "stopAnimating",
                        "oncompleteparams", state,
                        "easetype", iTween.EaseType.easeInExpo
                    )
                );
            }
        }
    }

    void stopAnimating( MainMenuController.MenuState lastState ) { MainMenuController.instance.menuState = lastState; }
}
