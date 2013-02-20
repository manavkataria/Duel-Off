using UnityEngine;
using System;
using System.Collections;

/* Supply Window Control Base
 * -- Base class for Supply windows ( Stash, Store )
 * -- Creation of abstract base in case of extended windows added later
 */
public abstract class SupplyWindow : MonoBehaviour {

    public UIPanel windowPanel;

    public SupplyGrid[] grids;

    protected Vector3 cPos;
    protected Vector4 cRange;

    protected Vector3 panelPosCache;
    protected Vector4 panelCenterCache;

    protected MainMenuController.MenuState lastState;
	
	protected Action onGridMove;

    protected abstract void OnEnable();
    protected abstract void OnDisable();

    protected void Awake()
    {
        panelPosCache = windowPanel.transform.localPosition;
        panelCenterCache = windowPanel.clipRange;
    }

    #region Animation Methods

    protected void moveGridLeft(MainMenuController.MenuState state)
    {
        SupplyGrid active = getActiveGrid();
        active.moveDirection = SupplyGrid._moveDirection.Left;

        if (active.curPos - 1 >= 0 && MainMenuController.instance.menuState == MainMenuController.MenuState.Store)
        {
            MainMenuController.instance.menuIsInTransition();
            lastState = state;

            //Debug.Log(MainMenuController.instance.menuState);
            iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
                "from", windowPanel.clipRange.x,
                "to", active.itemArray[active.curPos - 1].transform.localPosition.x,
                "time", 0.4f,
                "onupdatetarget", gameObject,
                "onupdate", "moveCenter",
                "easetype", iTween.EaseType.easeOutExpo
                )
            );
            iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
                "from", windowPanel.transform.localPosition.x,
                "to", windowPanel.transform.localPosition.x + active.cellWidth,
                "time", 0.4f,
                "onupdatetarget", gameObject,
                "onupdate", "movePosition",
                "oncompletetarget", gameObject,
                "oncomplete", "updateCurPos",
                "oncompleteparams", active,
                "easetype", iTween.EaseType.easeOutExpo
                )
            );
        }
        else if (MainMenuController.instance.menuState == MainMenuController.MenuState.Store)
            gridCantMove(-20f, state);
    }

    protected void moveGridRight(MainMenuController.MenuState state)
    {
        SupplyGrid active = getActiveGrid();
        active.moveDirection = SupplyGrid._moveDirection.Right;

        if (active.curPos + 1 <= active.itemArray.Count - 1 && MainMenuController.instance.menuState == MainMenuController.MenuState.Store)
        {
            MainMenuController.instance.menuIsInTransition();
            lastState = state;

            //Debug.Log(MainMenuController.instance.menuState);
            iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
                "from", windowPanel.clipRange.x,
                "to", active.itemArray[active.curPos + 1].transform.localPosition.x,
                "time", 0.4f,
                "onupdatetarget", gameObject,
                "onupdate", "moveCenter",
                "easetype", iTween.EaseType.easeOutExpo
                )
            );
            iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
                "from", windowPanel.transform.localPosition.x,
                "to", windowPanel.transform.localPosition.x - active.cellWidth,
                "time", 0.4f,
                "onupdatetarget", gameObject,
                "onupdate", "movePosition",
                "oncompletetarget", gameObject,
                "oncomplete", "updateCurPos",
                "oncompleteparams", active,
                "easetype", iTween.EaseType.easeOutExpo
                )
            );
        }
        else if (MainMenuController.instance.menuState == MainMenuController.MenuState.Store)
            gridCantMove(20f, state);
    }
	
	protected void moveGridTo( int index, MainMenuController.MenuState state )
	{
		SupplyGrid active = getActiveGrid();
        active.moveDirection = SupplyGrid._moveDirection.Left;

        if (MainMenuController.instance.menuState == MainMenuController.MenuState.Store)
        {
            MainMenuController.instance.menuIsInTransition();
            lastState = state;

            //Debug.Log(MainMenuController.instance.menuState);
            iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
                "from", windowPanel.clipRange.x,
                "to", active.itemArray[index].transform.localPosition.x,
                "time", 0.4f,
                "onupdatetarget", gameObject,
                "onupdate", "moveCenter",
                "easetype", iTween.EaseType.easeOutExpo
                )
            );
            iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
                "from", windowPanel.transform.localPosition.x,
                "to", panelPosCache.x - ( active.cellWidth * (float)index ),
                "time", 0.4f,
                "onupdatetarget", gameObject,
                "onupdate", "movePosition",
                "oncompletetarget", gameObject,
                "oncomplete", "updateCurPosMoveTo",
                "oncompleteparams", index,
                "easetype", iTween.EaseType.easeOutExpo
                )
            );
        }
        else if (MainMenuController.instance.menuState == MainMenuController.MenuState.Store)
            gridCantMove(-20f, state);
	}

    protected void gridCantMove(float tweenX, MainMenuController.MenuState state)
    {
        MainMenuController.instance.menuIsInTransition();
        lastState = state;

        SupplyGrid active = getActiveGrid();
        active.moveDirection = SupplyGrid._moveDirection.None;

        //Debug.Log(MainMenuController.instance.menuState);
        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
            "from", windowPanel.clipRange.x,
            "to", windowPanel.clipRange.x + tweenX,
            "time", 0.2f,
            "onupdatetarget", gameObject,
            "onupdate", "moveCenter",
            "easetype", iTween.EaseType.easeOutExpo
            )
        );
        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
            "from", windowPanel.transform.localPosition.x,
            "to", windowPanel.transform.localPosition.x - tweenX,
            "time", 0.2f,
            "onupdatetarget", gameObject,
            "onupdate", "movePosition",
            "easetype", iTween.EaseType.easeOutExpo
            )
        );
        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
            "from", windowPanel.clipRange.x + tweenX,
            "to", windowPanel.clipRange.x,
            "time", 0.2f,
            "delay", 0.2f,
            "onupdatetarget", gameObject,
            "onupdate", "moveCenter",
            "easetype", iTween.EaseType.easeInExpo
            )
        );
        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
            "from", windowPanel.transform.localPosition.x - tweenX,
            "to", windowPanel.transform.localPosition.x,
            "time", 0.2f,
            "delay", 0.2f,
            "onupdatetarget", gameObject,
            "onupdate", "movePosition",
            "oncompletetarget", gameObject,
            "oncomplete", "updateCurPos",
            "oncompleteparams", active,
            "easetype", iTween.EaseType.easeInExpo
            )
        );
    }

    // Update callback for iTween ValueTo
    protected void moveCenter(float newValue)
    {
        cRange = windowPanel.clipRange;
        cRange.x = newValue;

        windowPanel.clipRange = cRange;
    }

    // Update callback for iTween ValueTo
    protected void movePosition(float newValue)
    {
        cPos = windowPanel.transform.localPosition;
        cPos.x = newValue;
        windowPanel.transform.localPosition = cPos;
    }

    #endregion

    #region Grid Tracking Methods

    protected abstract void updateCurPos(SupplyGrid grid);

    protected abstract void setActiveGrid(SuppliesUIObject clicked, MainMenuController.MenuState state, GameObject hit);

    protected abstract IEnumerator setCenterToCurPos(int activeIndex);
	
	protected SupplyGrid getActiveGrid()
    {
        int j = -1;
        for (int i = 0; i < grids.Length; i++)
        {
            if (j == -1 && grids[i].isActive) j = i;
            else continue;
        }

        return grids[j];
    }

    #endregion
}
