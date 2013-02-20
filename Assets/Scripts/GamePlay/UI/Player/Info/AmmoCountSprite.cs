using UnityEngine;
using System.Collections;

public class AmmoCountSprite : MonoBehaviour {
	
	UISprite thisSprite;
	
	void Awake() { thisSprite = GetComponent<UISprite>(); }
	
	void OnEnable() { Gun.onReloadStart += onReloadStart; }
	void OnDisable() { Gun.onReloadStart -= onReloadStart; }
	
	void onReloadStart( float reloadTime, Gun gun ) { thisSprite.spriteName = gun.loadedAmmo.spriteName; }
}
