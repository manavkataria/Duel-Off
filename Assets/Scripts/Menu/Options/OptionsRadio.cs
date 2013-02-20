using UnityEngine;
using System.Collections;

/** Options Controller
 *  -- Used across all options buttons to determine action to take
 *  -- from inherited Event Listeners.
 * 
 *  -- Attach to Options UI Object, and designate type of Option as well
 *  -- if it's the on switch of the radio based selection
 */ 
public class OptionsRadio : OptionsSprite {
	
    public bool isOnSwitch;

    public PlayerSavedPrefs._ControlScheme thisControlScheme;

    protected override void setSprite(PlayerSavedPrefs._OptionsType type, bool isOn, MainMenuController.MenuState state, PlayerSavedPrefs._ControlScheme scheme)
    {
        if( thisType == PlayerSavedPrefs._OptionsType.Sounds && type == PlayerSavedPrefs._OptionsType.Sounds )
        {
            if (isOnSwitch) 
				thisSprite.spriteName = (isOn) ? "On_Checked" : "On_Unchecked";
            else 
				thisSprite.spriteName = (isOn) ? "Off_UnChecked" : "Off_Checked";

            return;
        }
        if (thisType == PlayerSavedPrefs._OptionsType.Music && type == PlayerSavedPrefs._OptionsType.Music )
		{
			if (isOnSwitch) 
				thisSprite.spriteName = (isOn) ? "On_Checked" : "On_Unchecked";
            else 
				thisSprite.spriteName = (isOn) ? "Off_UnChecked" : "Off_Checked";

            return;
        }
        if ( thisType == PlayerSavedPrefs._OptionsType.Controls && type == PlayerSavedPrefs._OptionsType.Controls)
        {
            if( isOnSwitch )
                thisSprite.spriteName = ( thisControlScheme == scheme ) ? "A_Checked" : "A_Unchecked";
            else
                thisSprite.spriteName = ( thisControlScheme == scheme ) ? "B_Checked" : "B_Unchecked";
        }
    }

    protected override void hitByRayCast()
    {
        if (thisType == PlayerSavedPrefs._OptionsType.Sounds)
        {
            DBAccess.instance.userPrefs.isSoundOn = (isOnSwitch) ? true : false;
            return;
        }
        if (thisType == PlayerSavedPrefs._OptionsType.Music)
        {
			MainMenuController m = GameObject.Find("UI_Main").GetComponent<MainMenuController>();
						
			if( name == "Music On" )
				m.mainMenuMusic.Play();
			else if( name == "Music Off" )
				m.mainMenuMusic.Stop();
			
            DBAccess.instance.userPrefs.isMusicOn = (isOnSwitch) ? true : false;
            return;
        }
        if (thisType == PlayerSavedPrefs._OptionsType.Controls)
            DBAccess.instance.userPrefs.ControlScheme = (isOnSwitch) ? PlayerSavedPrefs._ControlScheme.Accelerometer : PlayerSavedPrefs._ControlScheme.Gyroscope;
    }
}
