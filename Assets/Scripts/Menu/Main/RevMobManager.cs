using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RevMobManager : MonoBehaviour {
	
	public RevMob revmob;
 	
	public static readonly Dictionary<string, string> appIds = new Dictionary<string, string>() {
		{ "Android", "DUMMY-ID"},
		{ "IOS", "5130f2e2424648d21e000025" }
    };
	
    public void Awake() {
		Debug.Log("RevMob Start");
		revmob = RevMob.Start(appIds);
		DontDestroyOnLoad (this);
    }
	
	//Display RevMob Ad
	public void DisplayAd() {
#if UNITY_IPHONE
		if (revmob != null) {
			Debug.Log("Displaying REVMOB FullScreen Ad");
			revmob.ShowFullscreen();
		} else {
			Debug.Log("REVMOB is NULL!");
		}
#endif
	}
}
