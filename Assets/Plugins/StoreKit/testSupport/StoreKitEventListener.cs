using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StoreKitEventListener : MonoBehaviour
{
#if UNITY_IPHONE
	void OnEnable()
	{
		// Listens to all the StoreKit events.  All event listeners MUST be removed before this object is disposed!
		StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessful;
		StoreKitManager.purchaseCancelledEvent += purchaseCancelled;
		StoreKitManager.purchaseFailedEvent += purchaseFailed;
		StoreKitManager.receiptValidationFailedEvent += receiptValidationFailed;
		StoreKitManager.receiptValidationRawResponseReceivedEvent += receiptValidationRawResponseReceived;
		StoreKitManager.receiptValidationSuccessfulEvent += receiptValidationSuccessful;
		StoreKitManager.productListReceivedEvent += productListReceived;
		StoreKitManager.productListRequestFailedEvent += productListRequestFailed;
		StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailed;
		StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinished;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;
	}
	
	
	void OnDisable()
	{
		// Remove all the event handlers
		StoreKitManager.productPurchaseAwaitingConfirmationEvent -= productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessful;
		StoreKitManager.purchaseCancelledEvent -= purchaseCancelled;
		StoreKitManager.purchaseFailedEvent -= purchaseFailed;
		StoreKitManager.receiptValidationFailedEvent -= receiptValidationFailed;
		StoreKitManager.receiptValidationRawResponseReceivedEvent -= receiptValidationRawResponseReceived;
		StoreKitManager.receiptValidationSuccessfulEvent -= receiptValidationSuccessful;
		StoreKitManager.productListReceivedEvent -= productListReceived;
		StoreKitManager.productListRequestFailedEvent -= productListRequestFailed;
		StoreKitManager.restoreTransactionsFailedEvent -= restoreTransactionsFailed;
		StoreKitManager.restoreTransactionsFinishedEvent -= restoreTransactionsFinished;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent -= paymentQueueUpdatedDownloadsEvent;
	}
	
	
	void productListReceived( List<StoreKitProduct> productList )
	{
		Debug.Log( "total productsReceived: " + productList.Count );
		
		// Do something more useful with the products than printing them to the console
		foreach( StoreKitProduct product in productList )
			Debug.Log( product.ToString() + "\n" );
	}
	
	
	void productListRequestFailed( string error )
	{
		Debug.Log( "productListRequestFailed: " + error );
	}
	
	
	void receiptValidationSuccessful()
	{
		Debug.Log( "receipt validation successful" );
	}
	
	
	void receiptValidationFailed( string error )
	{
		Debug.Log( "receipt validation failed with error: " + error );
	}
	
	
	void receiptValidationRawResponseReceived( string response )
	{
		Debug.Log( "receipt validation raw response: " + response );
	}
	

	void purchaseFailed( string error )
	{
		Debug.Log( "purchase failed with error: " + error );
	}
	

	void purchaseCancelled( string error )
	{
		Debug.Log( "purchase cancelled with error: " + error );
	}
	
	
	void productPurchaseAwaitingConfirmationEvent( StoreKitTransaction transaction )
	{
		Debug.Log( "productPurchaseAwaitingConfirmationEvent: " + transaction );
	}
	
	
	void purchaseSuccessful( StoreKitTransaction transaction )
	{
		Debug.Log( "purchased product: " + transaction );
	}
	
	
	void restoreTransactionsFailed( string error )
	{
		Debug.Log( "restoreTransactionsFailed: " + error );
	}
	
	
	void restoreTransactionsFinished()
	{
		Debug.Log( "restoreTransactionsFinished" );
	}
	
	
	void paymentQueueUpdatedDownloadsEvent( List<StoreKitDownload> downloads )
	{
		Debug.Log( "paymentQueueUpdatedDownloadsEvent: " );
		foreach( var dl in downloads )
			Debug.Log( dl );
	}
	
#endif
}

