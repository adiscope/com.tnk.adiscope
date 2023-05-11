/*
 * Created by Minjae Gu (mjgu@neowiz.com)
 */
#if UNITY_IOS

using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

using System.Runtime.InteropServices;
using AOT;

namespace Adiscope.Internal.Platform.IOS
{

    internal class InterstitialAdClient : IInterstitialAdClient
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

        private static InterstitialAdClient interstitialAd;

        public InterstitialAdClient ()
        {
            interstitialAd = this;
        }

#region AD APIs

        [DllImport("__Internal")]
        private static extern void loadInterstitial(
            string unitId,
            onInterstitialAdLoadedCallback loadedCallback,
            onInterstitialAdFailedToLoadCallback failLoadedCallback);

        public void Load(string unitId)
        {
            loadInterstitial(unitId, onInterstitialAdLoaded, onInterstitialAdFailedToLoad);
        }

        [DllImport("__Internal")]
        private static extern bool isLoadedInterstitial(string unitId);
        public bool IsLoaded(string unitId)
        {
            return isLoadedInterstitial(unitId);
        }

        [DllImport("__Internal")]
        private static extern bool showInterstitial(
            onInterstitialWillPresentScreenCallback openedCallback,
            onInterstitialWillDismissScreenCallback closedCallback,
            onInterstitialDidFailToPresentScreenCallback failedToShowCallback);
        public bool Show()
        {
            return showInterstitial(
                onInterstitialWillPresentScreen,
                onInterstitialWillDismissScreen,
                onInterstitialDidFailToPresentScreen);
        }
#endregion

#region CallBacks
        private delegate void onInterstitialAdLoadedCallback(string unitId);

        [MonoPInvokeCallback(typeof(onInterstitialAdLoadedCallback))]
        public static void onInterstitialAdLoaded(string unitId)
        {
            Debug.Log("onInterstitialAdLoaded()");
            interstitialAd.InterstitialAdLoadedProc(unitId);
        }

        public void InterstitialAdLoadedProc(string unitId)
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


        private delegate void onInterstitialAdFailedToLoadCallback(string unitId, int code, string description, string xb3TraceID);
        [MonoPInvokeCallback(typeof(onInterstitialAdFailedToLoadCallback))]
        public static void onInterstitialAdFailedToLoad(string unitId, int code, string description, string xb3TraceID)
        {
            Debug.Log("onInterstitialAdFailedToLoad()");
            interstitialAd.InterstitialAdFailedToLoadProc(unitId, code, description, xb3TraceID);
        }

       public void InterstitialAdFailedToLoadProc(string unitId, int code, string description, string xb3TraceID)
        {
            if (this.OnFailedToLoad != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToLoad(
                        this,
                        new LoadFailure(unitId, new AdiscopeError(code, description, xb3TraceID)));
                });
            }

            if (this.OnFailedToLoadBackground != null)
            {
                this.OnFailedToLoadBackground(
                    interstitialAd,
                    new LoadFailure(unitId, new AdiscopeError(code, description, xb3TraceID)));
            }
        }

        private delegate void onInterstitialWillPresentScreenCallback(string unitId);
        [MonoPInvokeCallback(typeof(onInterstitialWillPresentScreenCallback))]
        public static void onInterstitialWillPresentScreen(string unitId)
        {
            Debug.Log("onInterstitialWillPresentScreen()");
            interstitialAd.InterstitialWillPresentScreen(unitId);
        }

        public void InterstitialWillPresentScreen(string unitId)
        {
            if (interstitialAd.OnOpened != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    interstitialAd.OnOpened(interstitialAd, new ShowResult(unitId));
                });
            }

            if (interstitialAd.OnOpenedBackground != null)
            {
                interstitialAd.OnOpenedBackground(interstitialAd, new ShowResult(unitId));
            }
        }


        private delegate void onInterstitialWillDismissScreenCallback(string unitId);
        [MonoPInvokeCallback(typeof(onInterstitialWillDismissScreenCallback))]
        public static void onInterstitialWillDismissScreen(string unitId)
        {
            Debug.Log("onInterstitialWillDismissScreen()");
            interstitialAd.InterstitialWillDismissScreen(unitId);
        }

        public void InterstitialWillDismissScreen(string unitId)
        {
            if (interstitialAd.OnClosed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    interstitialAd.OnClosed(interstitialAd, new ShowResult(unitId));
                });
            }

            if (interstitialAd.OnClosedBackground != null)
            {
                interstitialAd.OnClosedBackground(interstitialAd, new ShowResult(unitId));
            }
        }


        private delegate void onInterstitialDidFailToPresentScreenCallback(string unitId, int code, string description, string xb3TraceID);
        [MonoPInvokeCallback(typeof(onInterstitialDidFailToPresentScreenCallback))]
        public static void onInterstitialDidFailToPresentScreen(string unitId, int code, string description, string xb3TraceID)
        {
            Debug.Log("onInterstitialDidFailToPresentScreen()");
            interstitialAd.InterstitialDidFailToPresentScreen(unitId, code, description, xb3TraceID);
        }

        public void InterstitialDidFailToPresentScreen(string unitId, int code, string description, string xb3TraceID)
        {
            if (interstitialAd.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    interstitialAd.OnFailedToShow(
                        interstitialAd, new ShowFailure(unitId, new AdiscopeError(code, description, xb3TraceID)));
                });
            }

            if (interstitialAd.OnFailedToShowBackground != null)
            {
                interstitialAd.OnFailedToShowBackground(
                    interstitialAd, new ShowFailure(unitId, new AdiscopeError(code, description, xb3TraceID)));
            }
        }

#endregion
    }
}
#endif