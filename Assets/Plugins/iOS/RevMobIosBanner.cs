using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if UNITY_IPHONE
public class RevMobIosBanner : RevMobBanner {
	private float x;
	private float y;
	private float width;
	private float height;

	[DllImport("__Internal")]
    private static extern void RevMobUnityiOSBinding_showBanner(string placementId, ScreenOrientation[] orientations, float x, float y, float width, float height);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_hideBanner();

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_deactivateBannerAd();

	private ScreenOrientation[] orientations;
	private string placementId;

	public RevMobIosBanner(string placementId, ScreenOrientation[] orientations, float x, float y, float width, float height) {
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
		this.orientations = orientations;
		this.placementId = placementId;
	}

	public override void Show() {
		RevMobUnityiOSBinding_showBanner(placementId, orientations, x, y, width, height);
	}

	public override void Hide() {
		RevMobUnityiOSBinding_hideBanner();
	}

	public override bool IsLoaded() {
		return true;
	}

	public override void Release() {
		this.Hide();
		RevMobUnityiOSBinding_deactivateBannerAd();
	}

}
#endif
