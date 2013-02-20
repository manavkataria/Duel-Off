using UnityEngine;
using System.Collections;

public class ReloadBarTween : MonoBehaviour {
		
	private Vector3 originalScale;
	private Vector3 resetState;
	
	private Vector3 scaleToBulletCapacity;
	
	void Awake() 
	{ 
		originalScale = transform.localScale;
		
		resetState = transform.localScale;
		resetState.x = 0.1f;
		
		transform.localScale = resetState;
	}
	
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
	
	void onReloadStart( float timeToReload, Gun gun )
	{
		iTween.ScaleTo( gameObject, iTween.Hash(
			"x", originalScale.x,
			"islocal", true,
			"time", timeToReload,
			"oncomplete", "revertState",
			"oncompletetarget", gameObject,
			"oncompleteparams", gun,
			"easetype", iTween.EaseType.linear
			)
		);
	}
	
	void revertState( Gun gun ) 
	{ 
		gun.capacity = gun.loadedAmmo.magSize;
		gun.isReloading = false; 
		
		gun.reloadFinished();
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState state, GameObject clicked, EnemyStats stats )
	{
		scaleToBulletCapacity = originalScale;
		scaleToBulletCapacity.x = originalScale.x * ( (float)gun.capacity / (float)gun.loadedAmmo.magSize );
				
		if( scaleToBulletCapacity.x == 0 ) scaleToBulletCapacity.x = 0.1f;
		
		transform.localScale = scaleToBulletCapacity;
	}
}
