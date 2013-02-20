using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


public enum GameCenterLeaderboardTimeScope
{
	Today = 0,
	Week,
	AllTime
};


// All Objective-C exposed methods should be bound here
public class GameCenterBinding
{
	[DllImport("__Internal")]
    private static extern bool _gameCenterIsGameCenterAvailable();

	// Checks to see if GameCenter is available on the current device and iOS version
    public static bool isGameCenterAvailable()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameCenterIsGameCenterAvailable();
		return false;
    }
	
	
	#region Player methods
	
	[DllImport("__Internal")]
    private static extern void _gameCenterAuthenticateLocalPlayer();

	// Authenticates the player.  This needs to be called before using anything in GameCenter and should
	// preferalbly be called shortly after application launch.
    public static void authenticateLocalPlayer()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterAuthenticateLocalPlayer();
    }
	
	
	[DllImport("__Internal")]
    private static extern bool _gameCenterIsPlayerAuthenticated();

	// Checks to see if the current player is authenticated.
	public static bool isPlayerAuthenticated()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameCenterIsPlayerAuthenticated();
		return false;
    }
	
	
	[DllImport("__Internal")]
    private static extern string _gameCenterPlayerAlias();

	// Gets the alias of the current player.
    public static string playerAlias()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameCenterPlayerAlias();
		return string.Empty;
    }
	

	[DllImport("__Internal")]
    private static extern string _gameCenterPlayerIdentifier();

	// Gets the playerIdentifier of the current player.
    public static string playerIdentifier()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameCenterPlayerIdentifier();
		return string.Empty;
    }
	
	
	[DllImport("__Internal")]
    private static extern bool _gameCenterIsUnderage();

	// Checks to see if the current player is underage.
    public static bool isUnderage()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _gameCenterIsUnderage();
		return false;
    }
	
	
	[DllImport("__Internal")]
    private static extern void _gameCenterRetrieveFriends( bool loadProfileImages );

	// Sends off a request to get the current users friend list and optionally loads profile images asynchronously
    public static void retrieveFriends( bool loadProfileImages )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterRetrieveFriends( loadProfileImages );
    }


	[DllImport("__Internal")]
    private static extern void _gameCenterLoadPlayerData( string playerIds, bool loadProfileImages );

	// Gets GameCenterPlayer objects for all the given playerIds and optionally loads the profile images asynchronously
    public static void loadPlayerData( string[] playerIdArray, bool loadProfileImages )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterLoadPlayerData( string.Join( ",", playerIdArray ), loadProfileImages );
    }
	

	[DllImport("__Internal")]
    private static extern void _gameCenterLoadProfilePhotoForLocalPlayer();

	// Starts the loading of the profile image for the currently logged in player
    public static void loadProfilePhotoForLocalPlayer()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterLoadProfilePhotoForLocalPlayer();
    }

	#endregion;
	
	
	#region Leaderboard methods
	
	[DllImport("__Internal")]
    private static extern void _gameCenterLoadLeaderboardLeaderboardTitles();

	// Sends off a request to get all the currently live leaderboards including leaderboardId and title.
    public static void loadLeaderboardTitles()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterLoadLeaderboardLeaderboardTitles();
    }


	[DllImport("__Internal")]
    private static extern void _gameCenterReportScore( System.Int64 score, System.Int64 context, string leaderboardId );

	// Reports a score for the given leaderboardId.
    public static void reportScore( System.Int64 score, string leaderboardId )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterReportScore( score, 0, leaderboardId );
    }
	
	public static void reportScore( System.Int64 score, System.Int64 context, string leaderboardId )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterReportScore( score, context, leaderboardId );
    }
	
	
	[DllImport("__Internal")]
	private static extern void _gameCenterIssueScoreChallenge( System.Int64 score, System.Int64 context, string leaderboardId, string playerIds, string message );
	
	// iOS 6 only! Issues a score challenge to the given players for the leaderboard
    public static void issueScoreChallenge( System.Int64 score, System.Int64 context, string leaderboardId, string[] playerIds, string message )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterIssueScoreChallenge( score, context, leaderboardId, string.Join( ",", playerIds ), message );
    }


	[DllImport("__Internal")]
    private static extern void _gameCenterShowLeaderboardWithTimeScope( int timeScope );

	// Shows the standard GameCenter leaderboard with the given time scope.
    public static void showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope timeScope )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterShowLeaderboardWithTimeScope( (int)timeScope );
    }


	[DllImport("__Internal")]
    private static extern void _gameCenterShowLeaderboardWithTimeScopeAndLeaderboardId( int timeScope, string leaderboardId );

	// Shows the standard GameCenter leaderboard for the given leaderboardId with the given time scope.
    public static void showLeaderboardWithTimeScopeAndLeaderboard( GameCenterLeaderboardTimeScope timeScope, string leaderboardId )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterShowLeaderboardWithTimeScopeAndLeaderboardId( (int)timeScope, leaderboardId );
    }


	[DllImport("__Internal")]
    private static extern void _gameCenterRetrieveScores( bool friendsOnly, int timeScope, int start, int end );

	// Sends a request to get the current scores with the given criteria.  Start and end MUST be between 1 and 100 inclusive.
    public static void retrieveScores( bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterRetrieveScores( friendsOnly, (int)timeScope, start, end );
    }
    
    
	[DllImport("__Internal")]
    private static extern void _gameCenterRetrieveScoresForLeaderboard( bool friendsOnly, int timeScope, int start, int end, string leaderboardId );

	// Sends a request to get the current scores with the given criteria.  Start and end MUST be between 1 and 100 inclusive.
    public static void retrieveScores( bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end, string leaderboardId )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterRetrieveScoresForLeaderboard( friendsOnly, (int)timeScope, start, end, leaderboardId );
    }
    

	[DllImport("__Internal")]
    private static extern void _gameCenterRetrieveScoresForPlayerId( string playerId );

	// Sends a request to get the current scores for the given playerId.
    public static void retrieveScoresForPlayerId( string playerId )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterRetrieveScoresForPlayerId( playerId );
    }
    
    
	[DllImport("__Internal")]
    private static extern void _gameCenterRetrieveScoresForPlayerIdAndLeaderboard( string playerId, string leaderboardId );

	// Sends a request to get the current scores for the given playerId and leaderboardId
    public static void retrieveScoresForPlayerId( string playerId, string leaderboardId )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterRetrieveScoresForPlayerIdAndLeaderboard( playerId, leaderboardId );
    }

	#endregion;


	#region Achievement methods

	[DllImport("__Internal")]
    private static extern void _gameCenterReportAchievement( string identifier, float percent );

	// Reports an achievement with the given identifier and percent complete
    public static void reportAchievement( string identifier, float percent )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterReportAchievement( identifier, percent );
    }

	
	[DllImport("__Internal")]
	private static extern void _gameCenterSelectChallengeablePlayerIDsForAchievement( string identifier, string playerIds );
	
	// iOS 6 only! Checks the given playerIds to see if any are elligible for the achievment challenge
    public static void selectChallengeablePlayerIDsForAchievement( string identifier, string[] playerIds)
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterSelectChallengeablePlayerIDsForAchievement( identifier, string.Join( ",", playerIds ) );
    }
	
	
	[DllImport("__Internal")]
	private static extern void _gameCenterIssueAchievementChallenge( string identifier, string playerIds, string message );
	
	// iOS 6 only! Issues an achievement challenge to the players for the given identifier
    public static void issueAchievementChallenge( string identifier, string[] playerIds, string message )
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterIssueAchievementChallenge( identifier, string.Join( ",", playerIds ), message );
    }


	[DllImport("__Internal")]
    private static extern void _gameCenterGetAchievements();

	// Sends a request to get a list of all the current achievements for the authenticated in player.
    public static void getAchievements()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterGetAchievements();
    }


	[DllImport("__Internal")]
    private static extern void _gameCenterResetAchievements();

	// Resets all the achievements for the authenticated player.
    public static void resetAchievements()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterResetAchievements();
    }


	[DllImport("__Internal")]
    private static extern void _gameCenterShowAchievements();

	// Shows the standard, GameCenter achievement list
    public static void showAchievements()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterShowAchievements();
	}


	[DllImport("__Internal")]
    private static extern void _gameCenterRetrieveAchievementMetadata();

	// Sends a request to get the achievements for the current game.
    public static void retrieveAchievementMetadata()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterRetrieveAchievementMetadata();
    }


	[DllImport("__Internal")]
	private static extern void _gameCenterShowCompletionBannerForAchievements();

	// Shows a completion banner for achievements if when reported they are at 100%.  Only has an effect on iOS 5+
    public static void showCompletionBannerForAchievements()
    {
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_gameCenterShowCompletionBannerForAchievements();
    }

	#endregion;

}
