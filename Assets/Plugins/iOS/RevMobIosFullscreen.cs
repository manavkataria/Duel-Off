using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if UNITY_IPHONE
public class RevMobIosFullscreen : RevMobFullscreen {

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_loadFullscreen(string placementId);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_loadFullscreenWithSpecificOrientations(ScreenOrientation[] orientations);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_showLoadedFullscreen();

	[DllImport("__Internal")]
	private static extern bool RevMobUnityiOSBinding_isLoadedFullscreen();

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_releaseLoadedFullscreen();


	public RevMobIosFullscreen(ScreenOrientation[] orientations) {
		RevMobUnityiOSBinding_loadFullscreenWithSpecificOrientations(orientations);
	}

	public RevMobIosFullscreen() {
		RevMobUnityiOSBinding_loadFullscreen(null);
	}

	public RevMobIosFullscreen(string placementId) {
		RevMobUnityiOSBinding_loadFullscreen(placementId);
	}

	public override void Show() {
		if (IsLoaded()) {
			RevMobUnityiOSBinding_showLoadedFullscreen();
		}
	}

	public override void Hide() {
	}

	public override bool IsLoaded() {
		return RevMobUnityiOSBinding_isLoadedFullscreen();
	}

	public override void Release() {
		this.Hide();
		RevMobUnityiOSBinding_releaseLoadedFullscreen();
	}
}
#endif