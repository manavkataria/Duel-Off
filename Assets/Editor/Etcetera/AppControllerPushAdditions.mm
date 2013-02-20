//
//  AppControllerPushAdditions.m
//  EtceteraTest
//
//  Created by Mike on 10/5/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "AppControllerPushAdditions.h"
#import "EtceteraManager.h"
#import "JSONKit.h"


void UnitySendMessage( const char * className, const char * methodName, const char * param );

#if USING_UNITY_35
void UnitySendDeviceToken( NSData* deviceToken );
void UnitySendRemoteNotification( NSDictionary* notification );
void UnitySendRemoteNotificationError( NSError* error );
#endif

@implementation AppController(PushAdditions)


+ (void)load
{
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationDidFinishLaunchingNotification:) name:UIApplicationDidFinishLaunchingNotification object:nil];
}


+ (void)applicationDidFinishLaunchingNotification:(NSNotification*)note
{
	if( note.userInfo )
	{
		NSDictionary *remoteNotificationDictionary = [note.userInfo objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey];
		if( remoteNotificationDictionary )
		{
			NSLog( @"opened with remote notification: %@", remoteNotificationDictionary );
			AppController *appCon = (AppController*)[UIApplication sharedApplication].delegate;
			[appCon performSelector:@selector(handleNotification:) withObject:remoteNotificationDictionary afterDelay:5];
		}
		
		NSDictionary *localNotificationDict = [note.userInfo objectForKey:UIApplicationLaunchOptionsLocalNotificationKey];
		if( localNotificationDict )
		{
			NSLog( @"opened with local notification: %@", localNotificationDict );
			//AppController *appCon = (AppController*)[UIApplication sharedApplication].delegate;
			//[appCon performSelector:@selector(handleNotification:) withObject:remoteNotificationDictionary afterDelay:5];
		}
	}
}


// From: http://www.cocoadev.com/index.pl?BaseSixtyFour
- (NSString*)base64forData:(NSData*)theData
{
    const uint8_t *input = (const uint8_t*)[theData bytes];
    NSInteger length = [theData length];
    
    static char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    
    NSMutableData *data = [NSMutableData dataWithLength:((length + 2) / 3) * 4];
    uint8_t *output = (uint8_t*)data.mutableBytes;
    
    NSInteger i;
    for( i = 0; i < length; i += 3 )
	{
        NSInteger value = 0;
        NSInteger j;
        for( j = i; j < (i + 3); j++ )
		{
            value <<= 8;
            
            if( j < length )
                value |= (0xFF & input[j]);
        }
        
        NSInteger theIndex = (i / 3) * 4;
        output[theIndex + 0] =                    table[(value >> 18) & 0x3F];
        output[theIndex + 1] =                    table[(value >> 12) & 0x3F];
        output[theIndex + 2] = (i + 1) < length ? table[(value >> 6)  & 0x3F] : '=';
        output[theIndex + 3] = (i + 2) < length ? table[(value >> 0)  & 0x3F] : '=';
    }
    
    return [[[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding] autorelease];
}


- (void)handleNotification:(NSDictionary*)dict
{
	NSDictionary *aps = [dict objectForKey:@"aps"];
	if( !aps )
		return;
	
	NSString *json = [aps JSONString];
	
	if( json )
		UnitySendMessage( "EtceteraManager", "remoteNotificationWasReceived", json.UTF8String );
	else
		UnitySendMessage( "EtceteraManager", "remoteNotificationWasReceived", "" );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIApplicationDelegate

- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
#if USING_UNITY_35
	UnitySendDeviceToken( deviceToken );
#endif
	
	NSString *deviceTokenString = [[[[deviceToken description]
									 stringByReplacingOccurrencesOfString:@"<" withString:@""] 
									stringByReplacingOccurrencesOfString:@">" withString:@""] 
								   stringByReplacingOccurrencesOfString:@" " withString:@""];
	
	UnitySendMessage( "EtceteraManager", "remoteRegistrationDidSucceed", [deviceTokenString UTF8String] );
	
	// If this is a user deregistering for notifications, dont proceed past this point
	if( [[UIApplication sharedApplication] enabledRemoteNotificationTypes] == UIRemoteNotificationTypeNone )
	{
		NSLog( @"Notifications are disabled for this application. Not registering with Urban Airship" );
		return;
	}
	
	// Grab the Urban Airship info from the info.plist file
	NSString *appKey = [EtceteraManager sharedManager].urbanAirshipAppKey;
	NSString *appSecret = [EtceteraManager sharedManager].urbanAirshipAppSecret;
	NSString *alias = [EtceteraManager sharedManager].urbanAirshipAlias;
	
	if( !appKey || !appSecret )
	{
		NSLog( @"Urban Airship appKey and/or appSecret not in set so not registering with UA" );
		return;
	}
	
    // Register the deviceToken with Urban Airship    
    NSString *UAServer = @"https://go.urbanairship.com";
    NSString *urlString = [NSString stringWithFormat:@"%@%@%@/", UAServer, @"/api/device_tokens/", deviceTokenString];
    NSURL *url = [NSURL URLWithString:urlString];
	
    NSMutableURLRequest *request = [[NSMutableURLRequest alloc] initWithURL:url];
    [request setHTTPMethod:@"PUT"];
	
	// handle teh alias if we are sending one
	if( alias )
	{
		[request setValue:@"application/json" forHTTPHeaderField:@"Content-Type"];
		NSDictionary *dict = [NSDictionary dictionaryWithObject:alias forKey:@"alias"];
		[request setHTTPBody:[dict JSONData]];
	}
	
    // Authenticate to the server
    [request addValue:[NSString stringWithFormat:@"Basic %@",
                       [self base64forData:[[NSString stringWithFormat:@"%@:%@",
											 appKey,
											 appSecret] dataUsingEncoding: NSUTF8StringEncoding]]] forHTTPHeaderField:@"Authorization"];
    
    [[NSURLConnection connectionWithRequest:request delegate:self] start];
}


- (void)application:(UIApplication*)application didFailToRegisterForRemoteNotificationsWithError:(NSError*)error
{
#if USING_UNITY_35
	UnitySendRemoteNotificationError( error );
#endif
	
	UnitySendMessage( "EtceteraManager", "remoteRegistrationDidFail", [[error localizedDescription] UTF8String] );
	
	NSLog( @"remoteRegistrationDidFail: %@", error );
}


- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo
{
#if USING_UNITY_35
	UnitySendRemoteNotification( userInfo );
#endif
	
	[self handleNotification:userInfo];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSURLConnection

- (void)connection:(NSURLConnection*)theConnection didReceiveResponse:(NSURLResponse*)response
{
	UnitySendMessage( "EtceteraManager", "urbanAirshipRegistrationDidSucceed", "" );
	
    NSLog( @"registered with UA: %@, %d",
		  [(NSHTTPURLResponse*)response allHeaderFields],
          [(NSHTTPURLResponse*)response statusCode] );
}


- (void)connection:(NSURLConnection*)theConnection didFailWithError:(NSError*)error
{
	UnitySendMessage( "EtceteraManager", "urbanAirshipRegistrationDidFail", [[error localizedDescription] UTF8String] );
	NSLog( @"Failed to register with UA: %@", error );
}


@end
