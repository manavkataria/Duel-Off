using System;

public abstract class RevMobLink {
	public abstract void Open();

    public abstract void Cancel();

	public abstract bool IsLoaded();

    public virtual void Release() {
        this.Cancel();
    }
}
