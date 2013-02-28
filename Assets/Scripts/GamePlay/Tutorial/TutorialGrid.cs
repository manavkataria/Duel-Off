using UnityEngine;
using System.Collections;

public class TutorialGrid : UIGrid {
	
	public UIPanel windowPanel;
	
	public UIDragPanelContents[] itemArray;
	
	public int curPos = 0;
	
	private Vector4 cRange;
	private Vector3 cPos;
	
	// Cached Tutorial state for handling return of state after animation / step-throughs
	private Tutorial._TutorialState nextState = Tutorial._TutorialState.None;
	private Tutorial tut;
	
	// Used to announce Tutorial step to listeners for handling logic locally
	public delegate void TutorialUpdateHandler( int curPos );
	public static event TutorialUpdateHandler onTutorialUpdate;
	
	void Awake()
	{
		itemArray = GetComponentsInChildren<UIDragPanelContents>();
		tut = GameObject.Find("Player").GetComponent<Tutorial>();
		Reposition();
	}
	
	// Initial automatic step through of tutorial
	new IEnumerator Start()
	{
		base.Start();
		yield return new WaitForSeconds(1.2f);
		moveGrid(Tutorial._TutorialState.InTransition);
		yield return new WaitForSeconds(7.5f);
		moveGrid(Tutorial._TutorialState.None);
	}
	
	/** Tutorial Step through Logic.
	 * 
	 *  Consider placing this logic into switch statement when time allowed.
	 *  Also, control of Tutorial movement would probably make more sense in
	 *  Tutorial script rather than here.  Possibly change design later on.
	 * 
	 *  This logic is used before a step through, where as updateCurPos() is used
	 *  to finalize a step through.
	 * 
	 */ 
    public void moveGrid(Tutorial._TutorialState nextState)
    {
		if( curPos < 17 )
		{
		this.nextState = nextState;
		
		float delay = 0f;
		bool increasePos = true;
		
		if( curPos == 2 || curPos == 3 && tut.calibratePanel.transform.localPosition.x >= 0)
		{
			Vector3 tutPos = ( curPos == 2 ) ? tut.leftOffScreenPos : Vector3.zero;
			Vector3 calPos = ( curPos == 2 ) ? Vector3.zero : tut.leftOffScreenPos;
			increasePos = ( curPos == 2 ) ? true : false;
			
			moveTutorialPanel( tutPos );
			
			iTween.MoveTo( tut.calibratePanel.gameObject, iTween.Hash(
				"position", calPos,
				"islocal", true,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeOutExpo
				)
			);
			delay = 0.4f;	
		}
		if( curPos == 4 )
			onTutorialUpdate(4);  // Used to stop animations on TiltPhones
		if( curPos == 6 )
			onTutorialUpdate(curPos);	// Used to snap in player gun from hiding
		if( curPos == 6 && tut.tutorialPanel.transform.localPosition.x >= 0)
		{
			moveTutorialPanel( tut.leftOffScreenPos );
			delay = 0.4f;
		}
		else if( curPos == 7 && tut.tutorialPanel.transform.localPosition.x < 0)
		{
			tut.tutorialPanel.transform.localPosition = tut.rightOffScreenPos;
			moveTutorialPanel( Vector3.zero );

			delay = 0.4f;
			increasePos = false;
		}
		if( curPos == 8 && tut.tutorialPanel.transform.localPosition.x >= 0 )
		{
			moveTutorialPanel( tut.leftOffScreenPos );

			delay = 0.4f;
		}
		if( curPos == 9 && tut.tutorialPanel.transform.localPosition.x < 0 )
		{
			tut.tutorialPanel.transform.localPosition = tut.rightOffScreenPos;
			moveTutorialPanel( Vector3.zero );

			increasePos = false;
		}
		if( curPos == 9 && tut.tutorialPanel.transform.localPosition.x <= 0 )
		{
			moveTutorialPanel( tut.leftOffScreenPos );

			iTween.RotateTo( tut.gameObject, iTween.Hash(
				"rotation", Vector3.zero,
				"islocal", true,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeOutExpo
				)
			);
				
			delay = 0.4f;
		}
		if( curPos == 10 && tut.tutorialPanel.transform.localPosition.x < 0 )
		{
			tut.tutorialPanel.transform.localPosition = tut.rightOffScreenPos;
			moveTutorialPanel( Vector3.zero );
				
			increasePos = false;
		}
		if( curPos == 10 && tut.tutorialPanel.transform.localPosition.x == 0 )
		{
			moveTutorialPanel( tut.leftOffScreenPos );

			delay = 0.4f;
		}
		if( curPos == 11 && tut.tutorialPanel.transform.localPosition.x < 0 )
		{
			tut.tutorialPanel.transform.localPosition = tut.rightOffScreenPos;
			moveTutorialPanel( Vector3.zero );

			increasePos = false;
		}
		if( curPos == 13 )
		{
			moveTutorialPanel(tut.leftOffScreenPos);
			
			delay = 0.4f;
		}
		if( curPos == 14 && tut.tutorialPanel.transform.localPosition.x < 0 )
		{
			tut.TutorialState = Tutorial._TutorialState.None;
			tut.axes = Tutorial.RotationAxes.None;
			tut.tutorialPanel.transform.localPosition = tut.rightOffScreenPos;
			moveTutorialPanel( Vector3.zero );
			
			increasePos = false;
		}
		if( curPos == 16 )
			{
				tut.changeLevelInTutorial();
				return;
			}
		
		/** Update of Panel Center and Position. All calls above usually need to step
		 *  through this.  If not, simply flag increasePos to false. The delay variable
		 *  is used below in the iTween's for proper ordering of execution.
		 */ 
		if( increasePos )
		{
	        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
	            "from", windowPanel.clipRange.y,
	            "to", itemArray[curPos + 1].transform.localPosition.y,
	            "time", 0.4f,
				"delay", delay,
	            "onupdatetarget", gameObject,
	            "onupdate", "moveCenter",
	            "easetype", iTween.EaseType.easeOutExpo
	            )
	        );
	        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
	            "from", windowPanel.transform.localPosition.y,
	            "to", windowPanel.transform.localPosition.y + cellHeight,
	            "time", 0.4f,
				"delay", delay,
	            "onupdatetarget", gameObject,
	            "onupdate", "movePosition",
	            "oncompletetarget", gameObject,
	            "oncomplete", "updateCurPos",
	            "easetype", iTween.EaseType.easeOutExpo
	            )
	        );
		}
		}
	}

    // Update callback for iTween ValueTo
    void moveCenter(float newValue)
    {
        cRange = windowPanel.clipRange;
        cRange.y = newValue;

        windowPanel.clipRange = cRange;
    }

    // Update callback for iTween ValueTo
    void movePosition(float newValue)
    {
        cPos = windowPanel.transform.localPosition;
        cPos.y = newValue;
        windowPanel.transform.localPosition = cPos;
    }
	
	// Used for iTween oncomplete Hash method. Used to finalize a step.
	void updateCurPos()
	{
		tut.TutorialState = nextState;
		
		curPos++;		
		if( curPos == 5 )
		{
			tut.axes = Tutorial.RotationAxes.MouseX;
			tut.TutorialState = Tutorial._TutorialState.MissionOne;
			onTutorialUpdate(curPos);
		}
		if( curPos == 9 )
			onTutorialUpdate(curPos);
		if( curPos == 7 )
		{
			tut.axes = Tutorial.RotationAxes.MouseXAndY;
			tut.TutorialState = Tutorial._TutorialState.MissionTwo;
		}
		if( curPos == 9 )
		{
			tut.axes = Tutorial.RotationAxes.MouseXAndY;
			tut.TutorialState = Tutorial._TutorialState.MissionTwoSecond;
		}
		if( curPos == 10 )
		{
			tut.axes = Tutorial.RotationAxes.MouseXAndY;
			tut.TutorialState = Tutorial._TutorialState.MissionThree;
			tut.rotationX = 0;
			tut.rotationY = 0;
			tut.localRotation = Vector3.zero;
			onTutorialUpdate(curPos);
		}
		if( curPos == 11 )
		{
			tut.axes = Tutorial.RotationAxes.MouseXAndY;
			tut.TutorialState = Tutorial._TutorialState.MissionThree;
			onTutorialUpdate(curPos);
		}
		if( curPos == 12 )
			onTutorialUpdate(curPos);
		if( curPos == 13 )
			onTutorialUpdate(curPos);
		if( curPos == 14 )
		{
			tut.axes = Tutorial.RotationAxes.MouseXAndY;
			tut.TutorialState = Tutorial._TutorialState.MissionFour;
			onTutorialUpdate(curPos);
		}
	}
	
	// Recycled method in moveGrid()
	void moveTutorialPanel( Vector3 positionTo )
	{
		iTween.MoveTo( tut.tutorialPanel.gameObject, iTween.Hash(
				"position", positionTo,
				"islocal", true,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeOutExpo,
				"oncompletetarget", gameObject,
				"oncomplete", "resetTutorialPanelPos"
				)
			);
	}
	
	void resetTutorialPanelPos()
	{
		if( curPos == 2 )
			tut.tutorialPanel.transform.localPosition = tut.rightOffScreenPos;
		else if( curPos == 3)
		{
			tut.axes = Tutorial.RotationAxes.MouseX;
			tut.TutorialState = Tutorial._TutorialState.MissionOne;
			onTutorialUpdate(curPos);
		}
		else
			tut.TutorialState = nextState;
	}
}
