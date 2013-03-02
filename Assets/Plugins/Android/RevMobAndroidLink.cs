using UnityEngine;
using System;
using System.Runtime.InteropServices;

#if UNITY_ANDROID
public class RevMobAndroidLink : RevMobLink {
	private AndroidJavaObject javaObject;

	public RevMobAndroidLink(AndroidJavaObject javaObject) {
		this.javaObject = javaObject;
	}

	public override void Open()	{
		javaObject.Call("open");
	}

	public override void Cancel()	{
		javaObject.Call("cancel");
	}

	public override bool IsLoaded() {
		return javaObject.Call<bool>("isLoaded");
	}

}
#endif
