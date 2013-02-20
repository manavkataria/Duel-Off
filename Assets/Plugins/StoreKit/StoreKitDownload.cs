using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;



#if UNITY_IPHONE
public enum StoreKitDownloadState
{
	Waiting,
	Active,
	Paused,
	Finished,
	Failed,
	Cancelled
}


public class StoreKitDownload
{
    public StoreKitDownloadState downloadState;
    public double contentLength;
    public string contentIdentifier;
    public string contentURL;
	public string contentVersion;
	public string error;
	public float progress;
	public double timeRemaining;
	public StoreKitTransaction transaction;
	
	
	public static List<StoreKitDownload> downloadsFromJson( string json )
	{
		var downloadList = new List<StoreKitDownload>();
		
		var products = json.arrayListFromJson();
		foreach( Hashtable ht in products )
			downloadList.Add( downloadFromHashtable( ht ) );
		
		return downloadList;
	}
	

    public static StoreKitDownload downloadFromHashtable( Hashtable ht )
    {
        var download = new StoreKitDownload();
		
		if( ht.ContainsKey( "downloadState" ) )
        	download.downloadState = (StoreKitDownloadState)int.Parse( ht["downloadState"].ToString() );
		
		if( ht.ContainsKey( "contentLength" ) )
        	download.contentLength = double.Parse( ht["contentLength"].ToString() );
		
		if( ht.ContainsKey( "contentIdentifier" ) )
        	download.contentIdentifier = ht["contentIdentifier"].ToString();
		
		if( ht.ContainsKey( "contentURL" ) )
        	download.contentURL = ht["contentURL"].ToString();
		
		if( ht.ContainsKey( "contentVersion" ) )
			download.contentVersion = ht["contentVersion"].ToString();
		
		if( ht.ContainsKey( "error" ) )
			download.error = ht["error"].ToString();
		
		if( ht.ContainsKey( "progress" ) )
			download.progress = float.Parse( ht["progress"].ToString() );
		
		if( ht.ContainsKey( "timeRemaining" ) )
        	download.timeRemaining = double.Parse( ht["timeRemaining"].ToString() );
		
		if( ht.ContainsKey( "transaction" ) )
        	download.transaction = StoreKitTransaction.transactionFromHashtable( ht["transaction"] as Hashtable );

        return download;
    }
	
	
	public override string ToString()
	{
		return String.Format( "<StoreKitDownload> downloadState: {0}\n contentLength: {1}\n contentIdentifier: {2}\n contentURL: {3}\n contentVersion: {4}\n error: {5}\n progress: {6}\n transaction: {7}",
			downloadState, contentLength, contentIdentifier, contentURL, contentVersion, error, progress, transaction );
	}
}
#endif
