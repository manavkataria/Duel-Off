using System;

public abstract class RevMobFullscreen {
	public abstract void Show();

    public abstract void Hide();

	public abstract bool IsLoaded();

    public virtual void Release() {
        this.Hide();
    }
}

