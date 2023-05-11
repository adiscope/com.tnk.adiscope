/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
#if UNITY_ANDROID
namespace Adiscope.Internal.Platform.Android
{
    /// <summary>
    /// class for constant values
    /// </summary>
    internal class Values
    {
        // adiscope
        public const string PKG_ADISCOPE = "com.nps.adiscope.core.Adiscope";
        public const string PKG_REWARDED_VIDEO_AD_LISTENER = "com.nps.adiscope.reward.RewardedVideoAdListener";
        public const string PKG_OFFERWALL_AD_LISTENER = "com.nps.adiscope.offerwall.OfferwallAdListener";
        public const string PKG_INTERSTITIAL_AD_LISTENER = "com.nps.adiscope.interstitial.InterstitialAdListener";

        public const string PKG_ADISCOPE_CROSS = "com.nps.adiscope.cp.Cross";
        public const string PKG_CROSS_PROMOTION_LISTENER = "com.nps.adiscope.cp.CrossPromotionListener";

        public const string MTD_GET_REWARDED_VIDEO_AD_INSTANCE = "getRewardedVideoAdInstance";
        public const string MTD_SET_REWARDED_VIDEO_AD_LISTENER = "setRewardedVideoAdListener";
        public const string MTD_GET_OFFERWALL_AD_INSTANCE = "getOfferwallAdInstance";
        public const string MTD_SET_OFFERWALL_AD_LISTENER = "setOfferwallAdListener";
        public const string MTD_GET_INTERSTITIAL_AD_INSTANCE = "getInterstitialAdInstance";
        public const string MTD_SET_INTERSTITIAL_AD_LISTENER = "setInterstitialAdListener";
        public const string MTD_GET_CROSS_PROMOTION_INSTANCE = "getCrossPromotionInstance";
        public const string MTD_SET_CROSS_PROMOTION_LISTENER = "setCrossPromotionListener";
        public const string MTD_GET_OPTION_SETTER_INSTANCE = "getOptionSetterInstance";
        public const string MTD_GET_NETWORK_VERSIONS = "getNetworkVersions";
        public const string MTD_GET_SDK_VERSION = "getSDKVersion";
        public const string MTD_INITIALIZE = "initialize";
        public const string MTD_ISINITIALIZE = "isInitialized";
        public const string MTD_SET_USER_ID = "setUserId";
        public const string MTD_GET_UNIT_STATUS = "getUnitStatus";
        public const string MTD_LOAD_ALL = "loadAll";
        public const string MTD_LOAD = "load";
        public const string MTD_IS_LOADED = "isLoaded";
        public const string MTD_SHOW = "show";
        public const string MTD_SHOW_DETAIL = "showDetail";
        public const string MTD_GET_CODE = "getCode";
        public const string MTD_GET_DESCRIPTION = "getDescription";
        public const string MTD_GET_XB3TRACEID = "getXb3TraceId";
        public const string MTD_GET_TYPE = "getType";
        public const string MTD_GET_AMOUNT = "getAmount";
        public const string MTD_IS_LIVE = "isLive";
        public const string MTD_IS_ACTIVE = "isActive";
        public const string MTD_SET_USE_CLOUD_FRONT_PROXY = "setUseCloudFrontProxy";
        public const string MTD_SET_CHILD_YN = "setChildYN";

        // cross
        public const string MTD_CROSS_INITIALIZE = "initialize";
        public const string MTD_CROSS_SET_TRACKING_INFO = "setTrackingInfo";
        public const string MTD_CROSS_SHOW_MORE_GAMES = "showMoreGames";
        public const string MTD_CROSS_SHOW_FULL_SCREEN_POPUP = "showFullScreenPopup";
        public const string MTD_CROSS_SHOW_ENDING_POPUP = "showEndingPopup";


        // unity
        public const string PKG_UNITY_PLAYER = "com.unity3d.player.UnityPlayer";

        public const string MTD_CURRENT_ACTIVITY = "currentActivity";
    }
}
#endif