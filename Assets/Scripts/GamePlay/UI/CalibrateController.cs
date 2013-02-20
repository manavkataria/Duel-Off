using UnityEngine;
using System.Collections;

public class CalibrateController : MonoBehaviour {
	
	public UIPanel windowPanel;
	
	public UIPanel clipPanel;
	
	public UIGrid grid;
	
	private Vector4 cRange;
	private Vector3 cPos;
	
	public UIDragPanelContents[] items;
	
	private int curPos = 0;
	
	private Vector3 panelPosCache;
	
	bool canPress = true;
	
	void Awake()
	{
		panelPosCache = clipPanel.transform.localPosition;
	}
	
	void onHitByRayCast(PlayerController c)
	{
		if( canPress )
		{
			canPress = false;
			StartCoroutine( readySetGo(c) );
		}
	}
	
	IEnumerator readySetGo( PlayerController c )
	{
		for( int i = 1; i < items.Length; i++ )
		{
			moveGridTo(i);
			yield return new WaitForSeconds(1);
		}
		
		iTween.MoveTo( windowPanel.gameObject, iTween.Hash(
			"x", -1500f,
			"islocal", true,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeOutExpo 
			)
		);
		
		c.isCalibrating = false;
		c.GameState = PlayerController._GameState.Active;
	}

	protected void moveGridTo( int index )
	{
		if( index < items.Length )
		{			
	        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
	            "from", clipPanel.clipRange.y,
	            "to", items[curPos+1].transform.localPosition.y,
	            "time", 0.4f,
	            "onupdatetarget", gameObject,
	            "onupdate", "moveCenter",
	            "easetype", iTween.EaseType.easeOutExpo
	            )
	        );
	        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
	            "from", clipPanel.transform.localPosition.y,
	            "to", panelPosCache.y + ( grid.cellHeight * (float)index ),
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
	}
	
    // Update callback for iTween ValueTo
    protected void moveCenter(float newValue)
    {
        cRange = clipPanel.clipRange;
        cRange.y = newValue;

        clipPanel.clipRange = cRange;
    }

    // Update callback for iTween ValueTo
    protected void movePosition(float newValue)
    {
        cPos = clipPanel.transform.localPosition;
        cPos.y = newValue;
        clipPanel.transform.localPosition = cPos;
    }
	
	void updateCurPosMoveTo( int index )
	{
		curPos = index;
	}
}
