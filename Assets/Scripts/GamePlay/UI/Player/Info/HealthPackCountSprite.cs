using UnityEngine;
using System.Collections;

public class HealthPackCountSprite : MonoBehaviour {

	UILabel thisLabel;
	
	void Awake() { thisLabel = GetComponent<UILabel>(); }
	
	void OnEnable() { PlayerSavedPrefs.onHealthPackUse += onHealthPackUse; }
	void OnDisable() { PlayerSavedPrefs.onHealthPackUse -= onHealthPackUse; }
	
	void onHealthPackUse()
	{
		switch( name )
		{
		case "Count0":
			thisLabel.text = DBAccess.instance.userPrefs.hPacks[0].quantity.ToString();
			break;
		case "Count1":
			thisLabel.text = DBAccess.instance.userPrefs.hPacks[1].quantity.ToString();
			break;
		case "Count2":
			thisLabel.text = DBAccess.instance.userPrefs.hPacks[2].quantity.ToString();
			break;
		case "Count3":
			thisLabel.text = DBAccess.instance.userPrefs.hPacks[3].quantity.ToString();
			break;
		case "Count4":
			thisLabel.text = DBAccess.instance.userPrefs.hPacks[4].quantity.ToString();
			break;
		default:
			break;
		}
	}
}
