using UnityEngine;
using System.Collections;

public class AmmoPackCount : MonoBehaviour {
	
	UILabel thisLabel;
	
	void Awake() { thisLabel = GetComponent<UILabel>(); }
	
	void OnEnable() 
	{ 
		Gun.onReloadStart += onReloadStart; 
		PlayerController.onUIUpdate += onUIUpdate;
	}
	
	void OnDisable() 
	{ 
		Gun.onReloadStart -= onReloadStart; 
		PlayerController.onUIUpdate -= onUIUpdate;
	}
	
	void onReloadStart( float reloadTime, Gun gun )
	{
		switch(name)
		{
		case "Count0":
			thisLabel.text = "INF";
			break;
		case "Count1":
			thisLabel.text = gun.compatibleAmmo[1].amountOwned.ToString();
			break;
		case "Count2":
			thisLabel.text = gun.compatibleAmmo[2].amountOwned.ToString();
			break;
		case "Count3":
			thisLabel.text = gun.compatibleAmmo[3].amountOwned.ToString();
			break;
		default:
			break;
		}
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState state, GameObject clicked, EnemyStats stats )
	{
		// Make sure to change only on proper event, clicked == null checks so we don't duplicate onReloadStart
		if( clicked == null )
		{
			switch(name)
			{
			case "Count0":
				thisLabel.text = "INF";
				break;
			case "Count1":
				thisLabel.text = gun.compatibleAmmo[1].amountOwned.ToString();
				break;
			case "Count2":
				thisLabel.text = gun.compatibleAmmo[2].amountOwned.ToString();
				break;
			case "Count3":
				thisLabel.text = gun.compatibleAmmo[3].amountOwned.ToString();
				break;
			default:
				break;
			}
		}
	}
}
