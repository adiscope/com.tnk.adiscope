/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using Adiscope.Internal.Interface;

namespace Adiscope.Internal.Platform
{
    /// <summary>
    /// client builder based on which platform to use
    /// </summary>
    internal class ClientBuilder
    {
        /// <summary>
        /// build adscope core client based on the runtime env.
        /// </summary>
        /// <returns>adiscope core client based on current runtime platform</returns>
        public static ICoreClient BuildCoreClient()
        {
#if UNITY_EDITOR
            return new MockPlatform.CoreClient();
#elif UNITY_ANDROID
            return new Android.CoreClient();
#elif UNITY_IOS
            return new IOS.CoreClient();
#else
            return new MockPlatform.CoreClient();
#endif
        }

        /// <summary>
        /// build rewarded video ad client based on the runtime env.
        /// </summary>
        /// <returns>rewarded video ad client based on current runtime platform</returns>
        public static IRewardedVideoAdClient BuildRewardedVideoAdClient()
        {
#if UNITY_EDITOR
            return new MockPlatform.RewardedVideoAdClient();
#elif UNITY_ANDROID
            return new Android.RewardedVideoAdClient();
#elif UNITY_IOS
			return new IOS.RewardedVideoAdClient();
#else
            return new MockPlatform.RewardedVideoAdClient();
#endif
        }

        /// <summary>
        /// build offerwall ad client based on the runtime env.
        /// </summary>
        /// <returns>offerwall ad client based on current runtime platform</returns>
        public static IOfferwallAdClient BuildOfferwallAdClient()
        {
#if UNITY_EDITOR
            return new MockPlatform.OfferwallAdClient();
#elif UNITY_ANDROID
            return new Android.OfferwallAdClient();
#elif UNITY_IOS
			return new IOS.OfferwallAdClient();
#else
            return new MockPlatform.OfferwallAdClient();
#endif
        }

        /// <summary>
        /// build interstitial ad client based on the runtime env.
        /// </summary>
        /// <returns>interstitial ad client based on current runtime platform</returns>
        public static IInterstitialAdClient BuildInterstitialAdClient()
        {
#if UNITY_EDITOR
            return new MockPlatform.InterstitialAdClient();
#elif UNITY_ANDROID
            return new Android.InterstitialAdClient();
#elif UNITY_IOS
            return new IOS.InterstitialAdClient();
#else
            return new MockPlatform.InterstitialAdClient();
#endif
        }

        /// <summary>
        /// build cross promotion client based on the runtime env.
        /// </summary>
        /// <returns>cross promotion client based on current runtime platform</returns>
        public static ICrossPromotionClient BuildCrossPromotionClient()
        {
#if UNITY_EDITOR
            return new MockPlatform.CrossPromotionClient();
#elif UNITY_ANDROID
            return new Android.CrossPromotionClient();
#elif UNITY_IOS
                return new IOS.CrossPromotionClient();            
#else
            return new MockPlatform.CrossPromotionClient();
#endif
        }

        /// <summary>
        /// build option setter client based on the runtime env.
        /// </summary>
        /// <returns>option setter client based on current runtime platform</returns>
        public static IOptionSetterClient BuildOptionSetterClient()
        {
#if UNITY_EDITOR
            return new MockPlatform.OptionSetterClient();
#elif UNITY_ANDROID
            return new Android.OptionSetterClient();
#elif UNITY_IOS
			return new IOS.OptionSetterClient();
#else
            return new MockPlatform.OptionSetterClient();
#endif
        }

        /// <summary>
        /// build option setter client based on the runtime env.
        /// </summary>
        /// <returns>option setter client based on current runtime platform</returns>
        public static IOptionGetterClient BuildOptionGetterClient()
        {
#if UNITY_EDITOR
            return new MockPlatform.OptionGetterClient();
#elif UNITY_ANDROID
            return new Android.OptionGetterClient();
#elif UNITY_IOS
			return new IOS.OptionGetterClient();
#else
            return new MockPlatform.OptionGetterClient();
#endif
        }
    }
}
