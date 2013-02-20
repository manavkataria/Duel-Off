using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


#if UNITY_IPHONE
public class StoreKitBinding
{
    [DllImport("__Internal")]
    private static extern bool _storeKitCanMakePayments();
 
    public static bool canMakePayments()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _storeKitCanMakePayments();
		return false;
    }


    [DllImport("__Internal")]
    private static extern void _storeKitRequestProductData( string productIdentifier );
 
	// Accepts an array of product identifiers. All of the products you have for sale should be requested in one call.
    public static void requestProductData( string[] productIdentifiers )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_storeKitRequestProductData( string.Join( ",", productIdentifiers ) );
    }


    [DllImport("__Internal")]
    private static extern void _storeKitPurchaseProduct( string productIdentifier, int quantity );

	// Purchases the given product and quantity
    public static void purchaseProduct( string productIdentifier, int quantity )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_storeKitPurchaseProduct( productIdentifier, quantity );
    }
	
	
    [DllImport("__Internal")]
    private static extern void _storeKitFinishPendingTransactions();

	// Finishes any and all pending transactions
    public static void finishPendingTransactions()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_storeKitFinishPendingTransactions();
    }
	
	
    [DllImport("__Internal")]
    private static extern void _storeKitFinishPendingTransaction( string transactionIdentifier );

	// Finishes the pending transaction identified by the transactionIdentifier
    public static void finishPendingTransaction( string transactionIdentifier )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_storeKitFinishPendingTransaction( transactionIdentifier );
    }


    [DllImport("__Internal")]
    private static extern void _storeKitRestoreCompletedTransactions();

	// Restores all previous transactions.  This is used when a user gets a new device and they need to restore their old purchases.
	// DO NOT call this on every launch.  It will prompt the user for their password.
    public static void restoreCompletedTransactions()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_storeKitRestoreCompletedTransactions();
    }


    [DllImport("__Internal")]
    private static extern void _storeKitValidateReceipt( string base64EncodedTransactionReceipt, bool isTest );

	// Validates the given receipt for non-consumable products.  If you are using the sandbox server (not a live sale) set isTest to true.
    public static void validateReceipt( string base64EncodedTransactionReceipt, bool isTest )
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
			_storeKitValidateReceipt( base64EncodedTransactionReceipt, isTest );
    }


	[DllImport("__Internal")]
	private static extern void _storeKitValidateAutoRenewableReceipt( string base64EncodedTransactionReceipt, string secret, bool isTest );

	// Validates a receipt from an auto-renewable product
	public static void validateAutoRenewableReceipt( string base64EncodedTransactionReceipt, string secret, bool isTest )
	{
	    if( Application.platform == RuntimePlatform.IPhonePlayer )
			_storeKitValidateAutoRenewableReceipt( base64EncodedTransactionReceipt, secret, isTest );
	}

	
    [DllImport("__Internal")]
    private static extern string _storeKitGetAllSavedTransactions();
 
	// Returns a list of all the transactions that occured on this device.  They are stored in the Document directory.
    public static List<StoreKitTransaction> getAllSavedTransactions()
    {
        if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
			// Grab the transactions and parse them out
			var json = _storeKitGetAllSavedTransactions();
			return StoreKitTransaction.transactionsFromJson( json );
		}
		
		return new List<StoreKitTransaction>();
    }
	
}
#endif
