using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Prime31;


public class GameCenterAchievement
{
	public string identifier;
	public bool isHidden;
	public bool completed;
	public DateTime lastReportedDate;
	public float percentComplete;
	
	
	public static List<GameCenterAchievement> fromJSON( string json )
	{
		var achievementList = new List<GameCenterAchievement>();
		
		// decode the json
		var list = json.arrayListFromJson();
		
		// create DTO's from the Hashtables
		foreach( Hashtable ht in list )
			achievementList.Add( new GameCenterAchievement( ht ) );
		
		return achievementList;
	}
	
	
	public GameCenterAchievement( Hashtable ht )
	{
		if( ht.Contains( "identifier" ) )
			identifier = ht["identifier"] as string;
		
		if( ht.Contains( "hidden" ) )
			isHidden = (bool)ht["hidden"];
		
		if( ht.Contains( "completed" ) )
			completed = (bool)ht["completed"];
		
		if( ht.Contains( "percentComplete" ) )
			percentComplete = float.Parse( ht["percentComplete"].ToString() );
		
		// grab and convert the date
		if( ht.Contains( "lastReportedDate" ) )
		{
			double timeSinceEpoch = double.Parse( ht["lastReportedDate"].ToString() );
			DateTime intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			lastReportedDate = intermediate.AddSeconds( timeSinceEpoch );
		}
	}
	
	
	public override string ToString()
	{
		 return string.Format( "<Achievement> identifier: {0}, hidden: {1}, completed: {2}, percentComplete: {3}, lastReported: {4}",
			identifier, isHidden, completed, percentComplete, lastReportedDate );
	}

}
