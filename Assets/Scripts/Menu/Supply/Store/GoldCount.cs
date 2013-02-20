using UnityEngine;
using System.Collections;

public class GoldCount : MonoBehaviour {
	
	UILabel thisLabel;
	
	void Awake()
	{
		thisLabel = GetComponent<UILabel>();
	}
	
	void OnEnable()
	{
		MainMenuController.updateInfo += updateInfo;
		BuyButtonController.onPurchased += onPurchased;
	}
	
	void OnDisable()
	{
		MainMenuController.updateInfo -= updateInfo;
		BuyButtonController.onPurchased -= onPurchased;
	}
	
	void updateInfo()
	{
		thisLabel.text = DBAccess.instance.userPrefs.Gold.ToString() + " Gold";
	}
	
	void onPurchased(SuppliesUIObject bought)
	{
		thisLabel.text = DBAccess.instance.userPrefs.Gold.ToString() + " Gold";
	}
}
