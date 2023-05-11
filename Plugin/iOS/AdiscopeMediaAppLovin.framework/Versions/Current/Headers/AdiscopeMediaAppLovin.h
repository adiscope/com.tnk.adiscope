//
//  AdiscopeMediaAppLovin.h
//  AdiscopeMediaAppLovin
//  SDK Version: 1.0.6
//
//  Created by mjgu on 2021. 5. 10.
//  Copyright © 2021년 구민재. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AppLovinSDK/AppLovinSDK.h>
#import "Adapter.h"

@interface AdiscopeMediaAppLovin : NSObject <Adapter, ALAdLoadDelegate, ALAdRewardDelegate, ALAdDisplayDelegate, ALAdVideoPlaybackDelegate>
@end
