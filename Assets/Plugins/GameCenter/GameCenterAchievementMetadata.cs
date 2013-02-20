using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


public class GameCenterAchievementMetadata
{
	public string identifier;
	public string description;
	public string unachievedDescription;
	public bool isHidden;
	public int maximumPoints;
	public string title;
	
	
	public static List<GameCenterAchievementMetadata> fromJSON( string json )
	{
		List<GameCenterAchievementMetadata> metadataList = new List<GameCenterAchievementMetadata>();
		
		// decode the json
		var list = json.arrayListFromJson();
		
		// create DTO's from the Hashtables
		foreach( Hashtable ht in list )
			metadataList.Add( new GameCenterAchievementMetadata( ht ) );
		
		return metadataList;
	}
	
	
	public GameCenterAchievementMetadata( Hashtable ht )
	{
		identifier = ht["identifier"] as string;
		description = ht["achievedDescription"] as string;
		unachievedDescription = ht["unachievedDescription"] as string;
		isHidden = (bool)ht["hidden"];
		maximumPoints = int.Parse( ht["maximumPoints"].ToString() );
		title = ht["title"] as string;
	}
	
	
	public override string ToString()
	{
		 return string.Format( "<AchievementMetaData> identifier: {0}, hidden: {1}, maxPoints: {2}, title: {3} desc: {4}, unachDesc: {5}",
		 	identifier, isHidden, maximumPoints, title, description, unachievedDescription );
	}

}
