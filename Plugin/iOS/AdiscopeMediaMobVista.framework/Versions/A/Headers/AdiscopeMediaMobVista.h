//
//  AdiscopeMediaMobVista.h
//  AdiscopeMediaMobVista
//  SDK Version: 1.0.7
//
//  Created by 이선학 on 2018. 9. 27..
//  Copyright © 2018년 이선학. All rights reserved.
//

#import <Foundation/Foundation.h>

#import <MTGSDK/MTGSDK.h>
#import <MTGSDKReward/MTGRewardAdManager.h>
#import "Adapter.h"

@interface AdiscopeMediaMobVista : NSObject <Adapter, MTGRewardAdLoadDelegate, MTGRewardAdShowDelegate>

@end
