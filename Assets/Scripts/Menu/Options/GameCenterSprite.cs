using UnityEngine;
using System.Collections;

public class GameCenterSprite : MonoBehaviour {

	void hitByRayCast()
	{
		if( GameCenterBinding.isPlayerAuthenticated() )
			GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.AllTime);
		else
			EtceteraBinding.showAlertWithTitleMessageAndButtons( "Oops!", "You have to be logged into Game Center to do that!", new string[] { "OK" } );
	}
}
