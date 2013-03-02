using System;
using System.Runtime.InteropServices;

using UnityEngine;

#if UNITY_IPHONE
public class RevMobIosLink : RevMobLink {

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_loadAdLink(string placementId);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_openLoadedAdLink();

	[DllImport("__Internal")]
	private static extern bool RevMobUnityiOSBinding_isLoadedAdLink();

	public RevMobIosLink(string placementId) {
		RevMobUnityiOSBinding_loadAdLink(placementId);
	}

	public override void Open()	{
		RevMobUnityiOSBinding_openLoadedAdLink();
	}

	public override void Cancel() {
	}

	public override bool IsLoaded() {
		return RevMobUnityiOSBinding_isLoadedAdLink();
	}

}
#endif