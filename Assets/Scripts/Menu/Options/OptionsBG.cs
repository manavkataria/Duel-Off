using UnityEngine;
using System.Collections;

public class OptionsBG : OptionsSprite {

    protected override void setSprite(PlayerSavedPrefs._OptionsType type, bool isOn, MainMenuController.MenuState state, PlayerSavedPrefs._ControlScheme scheme)
    {
        if (type == thisType)
        {
            // Not sure why this check has to be here for iTweens
            if (this != null && state != MainMenuController.MenuState.InTransition )
            {
                MainMenuController.instance.menuIsInTransition();
                iTween.ShakePosition(gameObject, iTween.Hash(
                    "amount", new Vector3(20f, 20f, 0),
                    "time", 0.4f,
                    "islocal", true,
                    "oncompletetarget", gameObject,
                    "oncomplete", "resetState",
                    "oncompleteparams", state
                    )
                );
            }
        }
    }
}
