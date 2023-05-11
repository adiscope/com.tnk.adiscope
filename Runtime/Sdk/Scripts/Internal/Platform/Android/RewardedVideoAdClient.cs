/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
#if UNITY_ANDROID
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    /// <summary>
    /// android client for rewarded video ad
    /// this class will call android native plugin's method
    /// </summary>
    internal class RewardedVideoAdClient : AndroidJavaProxy, IRewardedVideoAdClient
    {
        public event EventHandler<LoadResult> OnLoaded;
        public event EventHandler<LoadFailure> OnFailedToLoad;
        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<RewardItem> OnRewarded;
        public event EventHandler<ShowFailure> OnFailedToShow;

        public event EventHandler<LoadResult> OnLoadedBackground;
        public event EventHandler<LoadFailure> OnFailedToLoadBackground;
        public event EventHandler<ShowResult> OnOpenedBackground;
        public event EventHandler<ShowResult> OnClosedBackground;
        public event EventHandler<RewardItem> OnRewardedBackground;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        private AndroidJavaObject rewardedVideoAd;

        public RewardedVideoAdClient() : base(Values.PKG_REWARDED_VIDEO_AD_LISTENER)
        {

            this.rewardedVideoAd = GetRewardedVideoAdInstance();

            if (this.rewardedVideoAd == null)
            {
                Debug.LogError("Android.RewardedVideoAdClient<Constructor> RewardedVideoAd: null");
                return;
            }
            
            this.rewardedVideoAd.Call(Values.MTD_SET_REWARDED_VIDEO_AD_LISTENER, this);
        }

        private AndroidJavaObject GetRewardedVideoAdInstance()
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.RewardedVideoAdClient<Constructor> UnityPlayer: null");
                    return null;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.RewardedVideoAdClient<GetRewardedVideoAdClient> " +
                        Values.PKG_ADISCOPE + ": null");
                    return null;
                }

                AndroidJavaObject rewardedVideoAd = jc.CallStatic<AndroidJavaObject>(
                    Values.MTD_GET_REWARDED_VIDEO_AD_INSTANCE, activity);
                return rewardedVideoAd;
            }
        }

        public void Load(string unitId)
        {

            
            if (rewardedVideoAd == null)
            {
                Debug.LogError("Android.RewardedVideoAdClient<Load> RewardedVideoAd: null");
                return;
            }
            
            rewardedVideoAd.Call(Values.MTD_LOAD, unitId);
        }

        public bool IsLoaded(string unitId)
        {
            if (rewardedVideoAd == null)
            {
                Debug.LogError("Android.RewardedVideoAdClient<IsLoaded> RewardedVideoAd: null");
                return false;
            }

            return rewardedVideoAd.Call<bool>(Values.MTD_IS_LOADED, unitId);
        }

        public bool Show()
        {
            if (rewardedVideoAd == null)
            {
                Debug.LogError("Android.RewardedVideoAdClient<Show> RewardedVideoAd: null");
                return false;
            }

            return rewardedVideoAd.Call<bool>(Values.MTD_SHOW);
        }

        #region Callbacks
        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onRewardedVideoAdLoaded(string unitId)
        {
            if (this.OnLoaded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnLoaded(this, new LoadResult(unitId));
                });
            }

            if (this.OnLoadedBackground != null)
            {
                this.OnLoadedBackground(this, new LoadResult(unitId));
            }

        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onRewardedVideoAdFailedToLoad(string unitId, AndroidJavaObject error)
        {
            if (this.OnFailedToLoad != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToLoad(
                        this, new LoadFailure(unitId, Utils.ConvertToAdiscopeError(error)));
                });
            }

            if (this.OnFailedToLoadBackground != null)
            {
                this.OnFailedToLoadBackground(
                    this, new LoadFailure(unitId, Utils.ConvertToAdiscopeError(error)));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onRewardedVideoAdOpened(string unitId)
        {
            if (this.OnOpened != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnOpened(this, new ShowResult(unitId));
                });
            }

            if (this.OnOpenedBackground != null)
            {
                this.OnOpenedBackground(this, new ShowResult(unitId));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onRewardedVideoAdClosed(string unitId)
        {
            if (this.OnClosed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnClosed(this, new ShowResult(unitId));
                });
            }

            if (this.OnClosedBackground != null)
            {
                this.OnClosedBackground(this, new ShowResult(unitId));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onRewarded(string unitId, AndroidJavaObject rewardItem)
        {
            if (this.OnRewarded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnRewarded(this, Utils.ConvertToRewardItem(unitId, rewardItem));
                });
            }

            if (this.OnRewardedBackground != null)
            {
                this.OnRewardedBackground(this, Utils.ConvertToRewardItem(unitId, rewardItem));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onRewardedVideoAdFailedToShow(string unitId, AndroidJavaObject error)
        {
            if (this.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToShow(
                        this, new ShowFailure(unitId, Utils.ConvertToAdiscopeError(error)));
                });
            }

            if (this.OnFailedToShowBackground != null)
            {
                this.OnFailedToShowBackground(
                    this, new ShowFailure(unitId, Utils.ConvertToAdiscopeError(error)));
            }
        }
        #endregion

        // ToString implementation to be used in Android native
        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public override string ToString()
        {
            return "Adiscope.Internal.Platform.Android.RewardedVideoAdClient as " +
                Values.PKG_REWARDED_VIDEO_AD_LISTENER;
        }
    }
}
#endif