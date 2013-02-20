using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class GameCenterLeaderboard
{
	public string leaderboardId;
	public string title;
	
	
	public static List<GameCenterLeaderboard> fromJSON( string json )
	{
		List<GameCenterLeaderboard> leaderboardList = new List<GameCenterLeaderboard>();
		
		// decode the json
		var ht = json.hashtableFromJson();
		
		// create DTO's from the Hashtable
		foreach( DictionaryEntry de in ht )
			leaderboardList.Add( new GameCenterLeaderboard( de.Value as string, de.Key as string ) );
		
		return leaderboardList;
	}
	
	
	public GameCenterLeaderboard( string leaderboardId, string title )
	{
		this.leaderboardId = leaderboardId;
		this.title = title;
	}
	
	
	public override string ToString()
	{
		 return string.Format( "<Leaderboard> leaderboardId: {0}, title: {1}", leaderboardId, title );
	}

}