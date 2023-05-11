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
    /// android client for adiscope core
    /// this class will call android native plugin's method
    /// </summary>
    internal class CoreClient : ICoreClient
    {
        private AndroidJavaObject activity;
             
        public CoreClient()
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(Values.PKG_UNITY_PLAYER))
            {
                if (unityPlayer == null)
                {
                    Debug.LogError("Android.CoreClient<Constructor> UnityPlayer: null");
                    return;
                }

                this.activity = unityPlayer.GetStatic<AndroidJavaObject>(Values.MTD_CURRENT_ACTIVITY);
            }
        }

        public void Initialize(Action<bool> callback)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, listener);
            }
        }

        public void Initialize(Action<bool> callback, string callbackTag)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, listener, callbackTag);
            }
        }

        public void Initialize(Action<bool> callback, string callbackTag, string childYN)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, listener, callbackTag, childYN);
            }
        }

        public void Initialize(string mediaId, string mediaSecret, string callbackTag, Action<bool> callback)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, mediaId, mediaSecret, callbackTag, listener);
            }
        }

        public void Initialize(string mediaId, string mediaSecret, string callbackTag, string childYN, Action<bool> callback)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (this.activity == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> UnityPlayerActivity: null");
                    return;
                }

                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<Initialize> " +
                        Values.PKG_ADISCOPE + ": null");
                    return;
                }


                AdiscopeInitializeListener listener = new AdiscopeInitializeListener(callback);
                jc.CallStatic(Values.MTD_INITIALIZE, this.activity, mediaId, mediaSecret, callbackTag, childYN, listener);
            }
        }

        public void SetUserId(string userId)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<SetUserId> " + Values.PKG_ADISCOPE + ": null");
                    return;
                }

                jc.CallStatic<bool>(Values.MTD_SET_USER_ID, userId);
            }
        }

        public void GetUnitStatus(string unitId, Action<AdiscopeError, UnitStatus> callback)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<GetUnitStatus> " + Values.PKG_ADISCOPE + ": null");
                }

                IUnitStatus status = new IUnitStatus(callback);
                jc.CallStatic(Values.MTD_GET_UNIT_STATUS, unitId, status);
            }
        }

        public bool IsInitialized()
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
            {
                if (jc == null)
                {
                    Debug.LogError("Android.CoreClient<GetUnitStatus> " + Values.PKG_ADISCOPE + ": null");
                }
                return jc.CallStatic<bool>(Values.MTD_ISINITIALIZE);
            }
            
        }
    }
}
#endif