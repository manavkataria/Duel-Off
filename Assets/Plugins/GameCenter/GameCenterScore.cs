using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Prime31;


public class GameCenterScore
{
	public string category;
	public string formattedValue;
	public Int64 value;
	public Int64 context;
	public DateTime date;
	public string playerId;
	public int rank;
	public bool isFriend;
	public string alias;
	public int maxRange; // this is only properly set when retrieving all scores without limiting by playerId
	
	
	public static List<GameCenterScore> fromJSON( string json )
	{
		var scoreList = new List<GameCenterScore>();
		
		// decode the json
		var list = json.arrayListFromJson();
		
		// create DTO's from the Hashtables
		foreach( Hashtable ht in list )
			scoreList.Add( new GameCenterScore( ht ) );
		
		return scoreList;
	}
	
	
	public GameCenterScore( Hashtable ht )
	{
		if( ht.Contains( "category" ) )
			category = ht["category"] as string;
		
		if( ht.Contains( "formattedValue" ) )
			formattedValue = ht["formattedValue"] as string;
		
		if( ht.Contains( "value" ) )
			value = Int64.Parse( ht["value"].ToString() );
		
		if( ht.Contains( "context" ) )
			context = Int64.Parse( ht["context"].ToString() );
		
		if( ht.Contains( "playerId" ) )
			playerId = ht["playerId"] as string;
		
		if( ht.Contains( "rank" ) )
			rank = int.Parse( ht["rank"].ToString() );
		
		if( ht.Contains( "isFriend" ) )
			isFriend = (bool)ht["isFriend"];
		
		if( ht.Contains( "alias" ) )
			alias = ht["alias"] as string;
		else
			alias = "Anonymous";
		
		if( ht.Contains( "maxRange" ) )
			maxRange = int.Parse( ht["maxRange"].ToString() );
		
		// grab and convert the date
		if( ht.Contains( "date" ) )
		{
			double timeSinceEpoch = double.Parse( ht["date"].ToString() );
			DateTime intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			date = intermediate.AddSeconds( timeSinceEpoch );
		}
	}
	
	
	public override string ToString()
	{
		 return string.Format( "<Score> category: {0}, formattedValue: {1}, date: {2}, rank: {3}, alias: {4}, maxRange: {5}",
			category, formattedValue, date, rank, alias, maxRange );
	}

}