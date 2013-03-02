using UnityEngine;
using System;
using System.Runtime.InteropServices;

#if UNITY_ANDROID
public class RevMobAndroidPopup : RevMobPopup {
    private AndroidJavaObject javaObject;

    public RevMobAndroidPopup(AndroidJavaObject javaObject) {
        this.javaObject = javaObject;
    }

    public override void Show() {
        javaObject.Call("show");
    }

    public override void Hide() {
        javaObject.Call("hide");
    }

    public override bool IsLoaded() {
        return javaObject.Call<bool>("isLoaded");
    }
}
#endif