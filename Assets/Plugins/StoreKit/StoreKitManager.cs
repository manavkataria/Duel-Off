using UnityEngine;
using System;
using System.Collections.Generic;



public class StoreKitManager : MonoBehaviour
{
#if UNITY_IPHONE
	public static bool autoConfirmTransactions = true;
	
	
	// Fired when the product list your required returns.  Automatically serializes the productString into StoreKitProduct's.
	public static event Action<List<StoreKitProduct>> productListReceivedEvent;
	
	// Fired when requesting product data fails
	public static event Action<string> productListRequestFailedEvent;
	
	// Fired when a product purchase has returned from Apple's servers and is awaiting completion. By default the plugin will finish transactions for you.
	// You can change that behaviour by setting autoConfirmTransactions to false which then requires that you call StoreKitBinding.finishPendingTransaction
	// to complete a purchase.
	public static event Action<StoreKitTransaction> productPurchaseAwaitingConfirmationEvent;
	
	// Fired when a product is successfully paid for.  returnValue will hold the productIdentifer and receipt of the purchased product.
	public static event Action<StoreKitTransaction> purchaseSuccessfulEvent;
	
	// Fired when a product purchase fails
	public static event Action<string> purchaseFailedEvent;
	
	// Fired when a product purchase is cancelled by the user or system
	public static event Action<string> purchaseCancelledEvent;
	
	// Fired when the validateReceipt call fails
	public static event Action<string> receiptValidationFailedEvent;
	
	// Fired when receive validation completes and returns the raw receipt data
	public static event Action<string> receiptValidationRawResponseReceivedEvent;
	
	// Fired when the validateReceipt method finishes.  It does not automatically mean success.
	public static event Action receiptValidationSuccessfulEvent;
	
	// Fired when an error is encountered while adding transactions from the user's purchase history back to the queue
	public static event Action<string> restoreTransactionsFailedEvent;
	
	// Fired when all transactions from the user's purchase history have successfully been added back to the queue
	public static event Action restoreTransactionsFinishedEvent;
	
	// Fired when any SKDownload objects are updated by iOS. If using hosted content you should not be confirming the transaction until all downloads are complete.
	public static event Action<List<StoreKitDownload>> paymentQueueUpdatedDownloadsEvent;
	
    void Awake()
    {
		// Set the GameObject name to the class name for easy access from Obj-C
		gameObject.name = this.GetType().ToString();
		DontDestroyOnLoad( this );
    }
	
	
	public void productPurchaseAwaitingConfirmation( string json )
	{
		if( productPurchaseAwaitingConfirmationEvent != null )
			productPurchaseAwaitingConfirmationEvent( StoreKitTransaction.transactionFromJson( json ) );
		
		if( autoConfirmTransactions )
			StoreKitBinding.finishPendingTransactions();
	}

	
	public void productPurchased( string json )
	{
		if( purchaseSuccessfulEvent != null )
			purchaseSuccessfulEvent( StoreKitTransaction.transactionFromJson( json ) );
	}
	
	
	public void productPurchaseFailed( string error )
	{
		if( purchaseFailedEvent != null )
			purchaseFailedEvent( error );
	}
	
		
	public void productPurchaseCancelled( string error )
	{
		if( purchaseCancelledEvent != null )
			purchaseCancelledEvent( error );
	}
	
	
	public void productsReceived( string json )
	{
		if( productListReceivedEvent != null )
			productListReceivedEvent( StoreKitProduct.productsFromJson( json ) );
	}
	
	
	public void productsRequestDidFail( string error )
	{
		if( productListRequestFailedEvent != null )
			productListRequestFailedEvent( error );
	}
	
	
	public void validateReceiptFailed( string error )
	{
		if( receiptValidationFailedEvent != null )
			receiptValidationFailedEvent( error );
	}
	
	
	public void validateReceiptRawResponse( string response )
	{
		if( receiptValidationRawResponseReceivedEvent != null )
			receiptValidationRawResponseReceivedEvent( response );
	}
	
	
	public void validateReceiptFinished( string statusCode )
	{
		if( statusCode == "0" )
		{
			if( receiptValidationSuccessfulEvent != null )
				receiptValidationSuccessfulEvent();
		}
		else
		{
			if( receiptValidationFailedEvent != null )
				receiptValidationFailedEvent( "Receipt validation failed with statusCode: " + statusCode );
		}
	}

	
	public void restoreCompletedTransactionsFailed( string error )
	{
		if( restoreTransactionsFailedEvent != null )
			restoreTransactionsFailedEvent( error );
	}
	
	
	public void restoreCompletedTransactionsFinished( string empty )
	{
		if( restoreTransactionsFinishedEvent != null )
			restoreTransactionsFinishedEvent();
	}
	
	
	public void paymentQueueUpdatedDownloads( string json )
	{
		if( paymentQueueUpdatedDownloadsEvent != null )
			paymentQueueUpdatedDownloadsEvent( StoreKitDownload.downloadsFromJson( json ) );
		
	}

#endif
}

