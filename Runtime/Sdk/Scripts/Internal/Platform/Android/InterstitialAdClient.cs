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
    /// android client for interstitial ad
    /// this class will call android native plugin's method
    /// </summary>
    internal class InterstitialAdClient : AndroidJavaProxy, IInterstitialAdClient
    {
        public event EventHandler<LoadResult> OnLoaded;
        public event EventHandler<LoadFailure> OnFailedToLoad;
        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowFailure> OnFailedToShow;

        public event EventHandler<LoadResult> OnLoadedBackground;
        public event EventHandler<LoadFailure> OnFailedToLoadBackground;
        public event EventHandler<ShowResult> OnOpenedBackground;
        public event EventHandler<ShowResult> OnClosedBackground;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        private AndroidJavaObject interstitialAd;

        public InterstitialAdClient() : base(Values.PKG_INTERSTITIAL_AD_LISTENER)
        {
            this.interstitialAd = GetInterstitialAdInstance();

            if (this.interstitialAd == null)
            {
                Debug.LogError("Android.InterstitialAdClient<Constructor> InterstitialAd: null");
                return;
            }

            this.interstitialAd.Call(Values.MTD_SET_INTERSTITIAL_AD_LISTENER, this);
        }

        private AndroidJavaObject GetInterstitialAdInstance()
        {
            AndroidJavaObject activity = null;
            Debug.Log("nmj nmj GetInterstitialAdInstance");
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.InterstitialAdClient<Constructor> UnityPlayer: null");
                    return null;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.InterstitialAdClient<GetInterstitialAdClient> " +
                        Values.PKG_ADISCOPE + ": null");
                    return null;
                }

                AndroidJavaObject InterstitialAd = jc.CallStatic<AndroidJavaObject>(
                    Values.MTD_GET_INTERSTITIAL_AD_INSTANCE, activity);

                return InterstitialAd;
            }
        }

        #region AD APIs 
        public void Load(string unitId)
        {
            if (interstitialAd == null)
            {
                Debug.LogError("Android.InterstitialAdClient<Load> InterstitialAd: null");
                return;
            }

            interstitialAd.Call(Values.MTD_LOAD, unitId);
        }

        public bool IsLoaded(string unitId)
        {
            if (interstitialAd == null)
            {
                Debug.LogError("Android.InterstitialAdClient<IsLoaded> InterstitialAd: null");
                return false;
            }

            return interstitialAd.Call<bool>(Values.MTD_IS_LOADED, unitId);
        }

        public bool Show()
        {
            if (interstitialAd == null)
            {
                Debug.LogError("Android.InterstitialAdClient<Show> InterstitialAd: null");
                return false;
            }

            return interstitialAd.Call<bool>(Values.MTD_SHOW);
        }
        #endregion

        #region Callbacks
        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onInterstitialAdLoaded()
        {
            if (this.OnLoaded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnLoaded(this, new LoadResult(null));
                });
            }

            if (this.OnLoadedBackground != null)
            {
                this.OnLoadedBackground(this, new LoadResult(null));
            }

        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onInterstitialAdFailedToLoad(AndroidJavaObject error)
        {
            if (this.OnFailedToLoad != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToLoad(
                        this, new LoadFailure("",  Utils.ConvertToAdiscopeError(error)));
                });
            }

            if (this.OnFailedToLoadBackground != null)
            {
                this.OnFailedToLoadBackground(
                    this, new LoadFailure("", Utils.ConvertToAdiscopeError(error)));
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onInterstitialAdOpened(string unitId)
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
        public void onInterstitialAdClosed(string unitId)
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
        public void onInterstitialAdFailedToShow(string unitId, AndroidJavaObject error)
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
            return "Adiscope.Internal.Platform.Android.InterstitialAdClient as " +
                Values.PKG_INTERSTITIAL_AD_LISTENER;
        }
    }
}
#endif