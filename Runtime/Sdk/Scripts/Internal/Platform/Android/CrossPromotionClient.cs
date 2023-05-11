/*
 * Created by nmj
 */
#if UNITY_ANDROID
using Adiscope.Internal.Interface;
using Adiscope.Model.Cross;
using System;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    /// <summary>
    /// android client for cross promotion
    /// this class will call android native plugin's method
    /// </summary>
    internal class CrossPromotionClient : AndroidJavaProxy, ICrossPromotionClient
    {
        public event EventHandler<BannerResult> OnOpened;
        public event EventHandler<BannerCloseResult> OnClosed;
        public event EventHandler<BannerFailure> OnFailedToShow;
        public event EventHandler<BannerResult> OnLeftApplication;
        public event EventHandler<EventArgs> OnInitializationSucceeded;
        public event EventHandler<InitializationFailure> OnInitializationFailed;

        private AndroidJavaObject crossPromotion;

        public CrossPromotionClient() : base(Values.PKG_CROSS_PROMOTION_LISTENER)
        {
            this.crossPromotion = GetCrossPromotionInstance();

            if (this.crossPromotion == null)
            {
                Debug.LogError("Android.CrossPromotionClient<Constructor> CrossPromotion: null");
                return;
            }

            this.crossPromotion.Call(Values.MTD_SET_CROSS_PROMOTION_LISTENER, this);
        }

        private AndroidJavaObject GetCrossPromotionInstance()
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE_CROSS))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CrossPromotionClient<GetCrossPromotionClient> " +
                        Values.PKG_ADISCOPE_CROSS + ": null");
                    return null;
                }

                AndroidJavaObject crossPromotion = jc.CallStatic<AndroidJavaObject>(
                    Values.MTD_GET_CROSS_PROMOTION_INSTANCE);

                return crossPromotion;
            }
        }

        #region AD APIs 
        public void Initialize()
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.CrossPromotionClient<Constructor> UnityPlayer: null");
                    return;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            if (crossPromotion == null)
            {
                Debug.LogError("Android.CrossPromotionClient<Initialize> CrossPromotion: null");
                return;
            }

            crossPromotion.Call(Values.MTD_CROSS_INITIALIZE, activity);
        }

        public void SetTrackingInfo(string userId)
        {
            if (crossPromotion == null)
            {
                Debug.LogError("Android.CrossPromotionClient<SetTrackingInfo> CrossPromotion: null");
                return;
            }

            crossPromotion.Call(Values.MTD_CROSS_SET_TRACKING_INFO, userId);
        }

        public void ShowMoreGames()
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.CrossPromotionClient<Constructor> UnityPlayer: null");
                    return;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            if (crossPromotion == null)
            {
                Debug.LogError("Android.CrossPromotionClient<ShowMoreGames> CrossPromotion: null");
                return;
            }

            crossPromotion.Call(Values.MTD_CROSS_SHOW_MORE_GAMES, activity);
        }

        public void ShowFullScreenPopup()
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.CrossPromotionClient<Constructor> UnityPlayer: null");
                    return;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            if (crossPromotion == null)
            {
                Debug.LogError("Android.CrossPromotionClient<showFullScreenPopup> CrossPromotion: null");
                return;
            }

            crossPromotion.Call(Values.MTD_CROSS_SHOW_FULL_SCREEN_POPUP, activity);
        }

        public void ShowEndingPopup()
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.CrossPromotionClient<Constructor> UnityPlayer: null");
                    return;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            if (crossPromotion == null)
            {
                Debug.LogError("Android.CrossPromotionClient<showEndingPopup> CrossPromotion: null");
                return;
            }

            crossPromotion.Call(Values.MTD_CROSS_SHOW_ENDING_POPUP, activity);
        }
        #endregion

        #region Callbacks
        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onOpened(int bannerType)
        {
            if (this.OnOpened != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnOpened(this, new BannerResult(bannerType));
                });
            }
        }

        public void onClosed(int bannerType, int flag)
        {
            if (this.OnClosed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnClosed(this, new BannerCloseResult(bannerType, flag));
                });
            }
        }

        public void onLeftApplication(int bannerType)
        {
            if (this.OnLeftApplication != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnLeftApplication(this, new BannerResult(bannerType));
                });
            }
        }

        public void onFailedToShow(int bannerType, AndroidJavaObject error)
        {
            if (this.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToShow(this, new BannerFailure(bannerType, Utils.ConvertToAdiscopeError(error)));
                });
            }
        }

        public void onInitializationSucceeded()
        {
            if (this.OnInitializationSucceeded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnInitializationSucceeded(this, EventArgs.Empty);
                });
            }
        }

        public void onInitializationFailed(AndroidJavaObject error)
        {
            if (this.OnInitializationFailed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnInitializationFailed(this, new InitializationFailure(Utils.ConvertToAdiscopeError(error)));
                });
            }
        }


        #endregion

        // ToString implementation to be used in Android native
        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public override string ToString()
        {
            return "Adiscope.Internal.Platform.Android.CrossPromotionClient as " + 
                Values.PKG_CROSS_PROMOTION_LISTENER;
        }
    }
}
#endif