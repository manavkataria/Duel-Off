using UnityEngine;

public abstract class RevMobNotification {
    public abstract void Schedule();

    public abstract bool IsLoaded();

    public abstract void Cancel();
}
