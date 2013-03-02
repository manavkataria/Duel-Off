using UnityEngine;
using System;
using System.Runtime.InteropServices;

#if UNITY_ANDROID
public class RevMobAndroidBanner : RevMobBanner {
    private AndroidJavaObject javaObject;

    public RevMobAndroidBanner(AndroidJavaObject javaObject) {
        this.javaObject = javaObject;
    }

    public override void Show() {}

    public override void Hide() {}

    public override bool IsLoaded() {
        return javaObject.Call<bool>("isLoaded");
    }
}
#endif