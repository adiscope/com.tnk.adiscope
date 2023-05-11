/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
#if UNITY_ANDROID
using Adiscope.Feature;
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    /// <summary>
    /// android client for offerwall ad
    /// this class will call android native plugin's method
    /// </summary>
    internal class OfferwallAdClient : AndroidJavaProxy, IOfferwallAdClient
    {
        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowFailure> OnFailedToShow;

        public event EventHandler<ShowResult> OnOpenedBackground;
        public event EventHandler<ShowResult> OnClosedBackground;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        private AndroidJavaObject offerwallAd;

        public OfferwallAdClient() : base(Values.PKG_OFFERWALL_AD_LISTENER)
        {
            this.offerwallAd = GetOfferwallAdInstance();

            if (this.offerwallAd == null)
            {
                Debug.LogError("Android.OfferwallAdClient<Constructor> OfferwallAd: null");
                return;
            }

            this.offerwallAd.Call(Values.MTD_SET_OFFERWALL_AD_LISTENER, this);
        }

        private AndroidJavaObject GetOfferwallAdInstance()
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.OfferwallAdClient<Constructor> UnityPlayer: null");
                    return null;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.OfferwallAdClient<GetRewardedVideoAdClient> " +
                        Values.PKG_ADISCOPE + ": null");
                    return null;
                }

                AndroidJavaObject offerwallAd = jc.CallStatic<AndroidJavaObject>(
                    Values.MTD_GET_OFFERWALL_AD_INSTANCE, activity);

                return offerwallAd;
            }
        }

        #region AD APIs 
        public bool Show(string unitId, string[] filters)
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.OfferwallAdClient<Show> UnityPlayer: null");
                    return false;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            if (offerwallAd == null)
            {
                Debug.LogError("Android.OfferwallAdClient<Show> OfferwallAd: null");
                return false;
            }
            return offerwallAd.Call<bool>(Values.MTD_SHOW, activity, unitId, filters);

        }
 
        public bool ShowOfferwallDetail(string unitId, string[] filters, string itemId)
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.OfferwallAdClient<Show> UnityPlayer: null");
                    return false;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            if (offerwallAd == null)
            {
                Debug.LogError("Android.OfferwallAdClient<ShowOfferwallDetail> OfferwallAd: null");
                return false;
            }
            
            int itemIdInt = Int16.Parse(itemId);
            return offerwallAd.Call<bool>(Values.MTD_SHOW_DETAIL, activity, unitId, filters, itemIdInt);
        }

        public bool ShowOfferwallDetailFromUrl(string url)
        {
            AndroidJavaObject activity = null;

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.OfferwallAdClient<ShowOfferwallDetailFromUrl> UnityPlayer: null");
                    return false;
                }
                activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }

            if (offerwallAd == null)
            {
                Debug.LogError("Android.OfferwallAdClient<ShowDetail> OfferwallAd: null");
                return false;
            }
            return offerwallAd.Call<bool>(Values.MTD_SHOW_DETAIL, activity, url);
        }
        
        #endregion

        #region Callbacks
        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        public void onOfferwallAdOpened(string unitId)
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
        public void onOfferwallAdClosed(string unitId)
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
        public void onOfferwallAdFailedToShow(string unitId, AndroidJavaObject error)
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
            return "Adiscope.Internal.Platform.Android.OfferwallAdClient as " +
                Values.PKG_OFFERWALL_AD_LISTENER;
        }
    }
}
#endif