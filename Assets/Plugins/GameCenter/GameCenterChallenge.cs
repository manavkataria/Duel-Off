using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Prime31;




public enum GameCenterChallengeState
{
    Invalid = 0,
    Pending = 1, // The challenge has been issued, but neither completed nor declined
    Completed = 2, // The challenge has been completed by the receiving player
    Declined = 3, // The challenge has been declined by the receiving player
}


public class GameCenterChallenge
{
	public string issuingPlayerID;
	public string receivingPlayerID;
	public GameCenterChallengeState state;
	public DateTime issueDate;
	public DateTime completionDate;
	public string message;
	
	// either a score or an achievement will be present but not both
	public GameCenterScore score;
	public GameCenterAchievement achievement;
	
	
	public GameCenterChallenge( Hashtable ht )
	{
		if( ht.Contains( "issuingPlayerID" ) )
			issuingPlayerID = ht["issuingPlayerID"] as string;
		
		if( ht.Contains( "receivingPlayerID" ) )
			receivingPlayerID = ht["receivingPlayerID"] as string;
		
		if( ht.Contains( "state" ) )
		{
			var intState = int.Parse( ht["state"].ToString() );
			state = (GameCenterChallengeState)intState;
		}
		
		// grab and convert the dates
		if( ht.Contains( "issueDate" ) )
		{
			var timeSinceEpoch = double.Parse( ht["issueDate"].ToString() );
			var intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			issueDate = intermediate.AddSeconds( timeSinceEpoch );
		}
		
		if( ht.Contains( "completionDate" ) )
		{
			var timeSinceEpoch = double.Parse( ht["completionDate"].ToString() );
			var intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			completionDate = intermediate.AddSeconds( timeSinceEpoch );
		}
		
		if( ht.Contains( "message" ) )
			message = ht["message"] as string;
		
		// do we have a score or an achievement?
		if( ht.Contains( "score" ) )
			score = new GameCenterScore( ht["score"] as Hashtable );
		
		if( ht.Contains( "achievement" ) )
			achievement = new GameCenterAchievement( ht["achievement"] as Hashtable );
	}
	
	
	public override string ToString()
	{
		 return string.Format( "<Challenge> issuingPlayerID: {0}, receivingPlayerID: {1}, message: {2}, state: {3}, score: {4}, achievement: {5}",
			issuingPlayerID, receivingPlayerID, message, state, score, achievement );
	}

}