using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class GameCenterPlayer
{
	public string playerId;
	public string alias;
	public string displayName;
	public bool isFriend;
	
	
	public static List<GameCenterPlayer> fromJSON( string json )
	{
		List<GameCenterPlayer> scoreList = new List<GameCenterPlayer>();
		
		// decode the json
		var list = json.arrayListFromJson();
		
		// create DTO's from the Hashtables
		foreach( Hashtable ht in list )
			scoreList.Add( new GameCenterPlayer( ht ) );
		
		return scoreList;
	}
	
	
	public GameCenterPlayer( Hashtable ht )
	{
		if( ht.Contains( "playerId" ) )
			playerId = ht["playerId"] as string;
		
		if( ht.Contains( "alias" ) )
			alias = ht["alias"] as string;
		
		if( ht.Contains( "displayName" ) )
			displayName = ht["displayName"] as string;
		
		if( ht.Contains( "isFriend" ) )
			isFriend = (bool)ht["isFriend"];
	}
	
	
	public override string ToString()
	{
		 return string.Format( "<Player> playerId: {0}, alias: {1}, displayName: {2}, isFriend: {3}", playerId, alias, displayName, isFriend );
	}

}