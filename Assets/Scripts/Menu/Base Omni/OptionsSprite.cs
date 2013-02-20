using UnityEngine;
using System.Collections;

public abstract class OptionsSprite : MonoBehaviour {

    public PlayerSavedPrefs._OptionsType thisType;
    public UISprite thisSprite;

    void Awake() { thisSprite = GetComponent<UISprite>(); }

    void OnEnable() 
	{ 
		PlayerSavedPrefs.onOptionsUpdate += setSprite; 
	}
	
    void OnDisable() 
	{ 
		PlayerSavedPrefs.onOptionsUpdate  += setSprite; 
	}

    protected abstract void setSprite(PlayerSavedPrefs._OptionsType type, bool isOn, MainMenuController.MenuState state, PlayerSavedPrefs._ControlScheme scheme);
	
    protected virtual void hitByRayCast() { }

    void resetState(MainMenuController.MenuState state)
    {
        transform.localPosition = Vector3.zero;
        MainMenuController.instance.menuState = state;
    }
}
