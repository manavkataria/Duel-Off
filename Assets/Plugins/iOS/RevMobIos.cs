using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;


#if UNITY_IPHONE
public class RevMobIos : RevMob {
	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_startSession(string appId, string version);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_setTestingMode(int testingMode);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_showFullscreen(string placementId);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_showFullscreenWithSpecificOrientations(ScreenOrientation[] orientations);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_openAdLink(string placementId);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_showPopup(string placementId);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_printEnvironmentInformation();

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_setGameObjectDelegateCallback(string gameObjectName);

	[DllImport("__Internal")]
	private static extern void RevMobUnityiOSBinding_setTimeoutInSeconds(int timeout);

	public RevMobIos(string appId, string gameObjectName) {
		this.gameObjectName = gameObjectName;
		RevMobUnityiOSBinding_startSession(appId, REVMOB_VERSION);
		RevMobUnityiOSBinding_setGameObjectDelegateCallback(gameObjectName);
	}


	public override bool IsDevice() {
		return (Application.platform == RuntimePlatform.IPhonePlayer);
	}



	public override void PrintEnvironmentInformation() {
		RevMobUnityiOSBinding_printEnvironmentInformation();
	}

	public override void SetTestingMode(RevMob.Test test) {
		RevMobUnityiOSBinding_setTestingMode((int)test);
	}

	public override void SetTimeoutInSeconds(int timeout) {
		RevMobUnityiOSBinding_setTimeoutInSeconds(timeout);
	}



	public override RevMobFullscreen ShowFullscreen(string placementId) {
		RevMobUnityiOSBinding_showFullscreen(placementId);
		return null;
	}

	public override RevMobFullscreen CreateFullscreen(string placementId) {
		return (IsDevice()) ? new RevMobIosFullscreen(placementId) : null;
	}

	public RevMobFullscreen ShowFullscreen(ScreenOrientation[] orientations) {
		RevMobUnityiOSBinding_showFullscreenWithSpecificOrientations(orientations);
		return null;
	}


	public override RevMobBanner CreateBanner(float x, float y, float width, float height, string placementId, ScreenOrientation[] orientations) {
		return (IsDevice()) ? new RevMobIosBanner(placementId, orientations, x, y, width, height) : null;
	}


	public override RevMobLink OpenAdLink(string placementId) {
		RevMobUnityiOSBinding_openAdLink(placementId);
		return null;
	}

	public override RevMobLink CreateAdLink(string placementId)	{
		return (IsDevice()) ? new RevMobIosLink(placementId) : null;
	}



	public override RevMobPopup ShowPopup(string placementId) {
		RevMobUnityiOSBinding_showPopup(placementId);
		return null;
	}

	public override RevMobPopup CreatePopup(string placementId) {
		return this.ShowPopup(placementId); // TODO: iOS does not have Popup pre-load
	}


	public override RevMobNotification ScheduleNotification(string placementId, DateTime? fireDate) {
		return null;
	}

	public override void OpenNotification() {
	}

}
#endif