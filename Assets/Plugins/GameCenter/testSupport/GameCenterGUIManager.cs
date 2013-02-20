using UnityEngine;
using System.Collections.Generic;


public class GameCenterGUIManager : MonoBehaviour
{
	// some useful ivars to hold information retrieved from GameCenter. These will make it easier
	// to test this code with your GameCenter enabled application because they allow the sample
	// to not hardcode any values for leaderboards and achievements.
	private List<GameCenterLeaderboard> leaderboards;
	private List<GameCenterAchievementMetadata> achievementMetadata;
	
	
	
	void Start()
	{
		// use anonymous delegates for this simple example for gathering data from GameCenter. In production you would want to
		// add and remove your event listeners in OnEnable/OnDisable
		GameCenterManager.categoriesLoaded += delegate( List<GameCenterLeaderboard> leaderboards )
		{
			this.leaderboards = leaderboards;
		};
		
		GameCenterManager.achievementMetadataLoaded += delegate( List<GameCenterAchievementMetadata> achievementMetadata )
		{
			this.achievementMetadata = achievementMetadata;
		};
		
		// after authenticating grab the players profile image
		GameCenterManager.playerAuthenticated += () =>
		{
			GameCenterBinding.loadProfilePhotoForLocalPlayer();
		};
	}
	
	
	void OnGUI()
	{
		float yPos = 5.0f;
		float xPos = 5.0f;
		float width = ( Screen.width >= 960 || Screen.height >= 960 ) ? 320 : 160;
		float height = ( Screen.width >= 960 || Screen.height >= 960 ) ? 80 : 40;
		float heightPlus = height + 5.0f;
		
		
		if( GUI.Button( new Rect( xPos, yPos, width, height ), "Authenticate" ) )
		{
			GameCenterBinding.authenticateLocalPlayer();
		}		
		

		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Load Achievement Metadata" ) )
		{
			GameCenterBinding.retrieveAchievementMetadata();
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Get Raw Achievements" ) )
		{
			GameCenterBinding.getAchievements();
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Post Achievement" ) )
		{
			if( achievementMetadata != null && achievementMetadata.Count > 0 )
			{
				int percentComplete = (int)Random.Range( 2, 60 );
				Debug.Log( "sending percentComplete: " + percentComplete );
				GameCenterBinding.reportAchievement( achievementMetadata[0].identifier, percentComplete );
			}
			else
			{
				Debug.Log( "you must load achievement metadata before you can post an achievement" );
			}
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Issue Achievement Challenge" ) )
		{
			if( achievementMetadata != null && achievementMetadata.Count > 0 )
			{
				var playerIds = new string[] { "player1", "player2" };
				GameCenterBinding.issueAchievementChallenge( achievementMetadata[0].identifier, playerIds, "I challenge you" );
			}
			else
			{
				Debug.Log( "you must load achievement metadata before you can issue an achievement challenge" );
			}
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Show Achievements" ) )
		{
			GameCenterBinding.showAchievements();
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Reset Achievements" ) )
		{
			GameCenterBinding.resetAchievements();
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Multiplayer Scene" ) )
		{
			Application.LoadLevel( "GameCenterMultiplayerTestScene" );
		}
	
	
		// Second Column
		xPos = Screen.width - width - 5.0f;
		yPos = 5.0f;
		
		
		if( GUI.Button( new Rect( xPos, yPos, width, height ), "Get Player Alias" ) )
		{
			string alias = GameCenterBinding.playerAlias();
			Debug.Log( "Player alias: " + alias );
		}
		
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Load Leaderboard Data" ) )
		{
			GameCenterBinding.loadLeaderboardTitles();
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Post Score" ) )
		{
			// We must have a leaderboard to post the score to
			if( leaderboards != null && leaderboards.Count > 0 )
			{
				Debug.Log( "about to report a random score for leaderboard: " + leaderboards[0].leaderboardId );
				GameCenterBinding.reportScore( Random.Range( 1, 99999 ), leaderboards[0].leaderboardId );
			}
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Issue Score Challenge" ) )
		{
			// We must have a leaderboard to post the score to
			if( leaderboards != null && leaderboards.Count > 0 )
			{
				var playerIds = new string[] { "player1", "player2" };
				var score = Random.Range( 1, 9999 );
				GameCenterBinding.issueScoreChallenge( score, 0, leaderboards[0].leaderboardId, playerIds, "Beat this score!" );
			}
			else
			{
				Debug.Log( "you must load your leaderboards before you can issue a score challenge" );
			}
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Show Leaderboards" ) )
		{
			GameCenterBinding.showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope.AllTime );
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Get Raw Score Data" ) )
		{
			// We must have a leaderboard to retrieve scores from
			if( leaderboards != null && leaderboards.Count > 0 )
				GameCenterBinding.retrieveScores( false, GameCenterLeaderboardTimeScope.AllTime, 1, 25, leaderboards[0].leaderboardId );
			else
				Debug.Log( "Load leaderboard data before attempting to retrieve scores" );
		}

		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Get Scores for Me" ) )
		{
			// We must have a leaderboard to retrieve scores from
			if( leaderboards != null && leaderboards.Count > 0 )
				GameCenterBinding.retrieveScoresForPlayerId( GameCenterBinding.playerIdentifier(), leaderboards[0].leaderboardId );
			else
				Debug.Log( "Load leaderboard data before attempting to retrieve scores" );
		}
	
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Retrieve Friends" ) )
		{
			GameCenterBinding.retrieveFriends( true );
		}


	}

}
