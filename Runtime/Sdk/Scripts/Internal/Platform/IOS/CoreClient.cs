/*
 * Created by Sunhak Lee (shlee@neowiz.com)
 */
#if UNITY_IOS

using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using AOT;

namespace Adiscope.Internal.Platform.IOS
{
    /// <summary>
    /// iOS client for adiscope core
    /// this class will call iOS native plugin's method
    /// </summary>

    using InitAction = Action<bool>;
    internal class CoreClient : ICoreClient
    {
		private static IDictionary<string, Action<AdiscopeError, UnitStatus>> handlerMap = new Dictionary<string, Action<AdiscopeError, UnitStatus>>();
		private static IDictionary<string, InitAction> initHandleMap = new Dictionary<string, InitAction>();
		private static string keyOfGetUnitStatus = null;
		private static string keyOfInitialize = null;

		public static CoreClient Instance;

		public CoreClient()
		{
			Instance = this;
		}

		private delegate void onInitializedCallback(bool isSuccess);

        [MonoPInvokeCallback(typeof(onInitializedCallback))]
        private static void OnInitializdCallback(bool isSuccess)
        {
            if (keyOfInitialize == null) { return; }

            InitAction callback;
            if (false == initHandleMap.TryGetValue(keyOfInitialize, out callback)) { return; }

            handlerMap.Remove(keyOfInitialize);
            callback.Invoke(isSuccess);
        }

		[DllImport("__Internal")]
		private static extern void unityInitialize(string media_id, string app_id, string callbackTag, onInitializedCallback callback);

		public void Initialize(string mediaId, string mediaSecret, string callbackTag, Action<bool> callback)
        {
            var key = Guid.NewGuid().ToString();
            if (callback != null) 
            { 
                keyOfInitialize = key;
                initHandleMap.Add(key, callback); 
            }

			unityInitialize(mediaId, mediaSecret, callbackTag, OnInitializdCallback);
		}

        // Not use childYN
        public void Initialize(string mediaId, string mediaSecret, string callbackTag, string childYN, Action<bool> callback) {
            this.Initialize(mediaId, mediaSecret, callbackTag, callback);
        }

        void ICoreClient.Initialize(InitAction callback)
        {
            
        }

        void ICoreClient.Initialize(InitAction callback, string callbackTag, string childYN)
        {
            
        }

        [DllImport ("__Internal")]
		private static extern bool isInitialized();
		public bool IsInitialized() { return isInitialized(); }


		[DllImport ("__Internal")]
		private static extern bool setUserId(string user_id);        

        public void SetUserId(string user_id) 
        {
            if (!setUserId(user_id))
                throw new System.ArgumentException();
        }

    	[DllImport("__Internal")] 
        private static extern void getUnitStatus(string unitId, onGetUnitStatusCallback callback);
        private delegate void onGetUnitStatusCallback(int code, string description, bool live, bool active);

        [MonoPInvokeCallback(typeof(onGetUnitStatusCallback))] 
	    private static void delegateGetUnitStatusCallback(int code, string description, bool live, bool active) {

            if (keyOfGetUnitStatus == null)
            {
                Debug.Log("keyOfGetUnitStatus = null");
                return;
            }
            
            Action<AdiscopeError, UnitStatus> action = GetCallback(keyOfGetUnitStatus);

            if (description != null)
                action.Invoke(new AdiscopeError(code, description), null);
            else
                action.Invoke(null, new UnitStatus(live, active));

            Debug.Log("delegateGetUnitStatusCallback: keyOfGetUnitStatus = " + keyOfGetUnitStatus);
        }


		public void GetUnitStatus(string unitId, Action<AdiscopeError, UnitStatus> callback)
		{
			keyOfGetUnitStatus = RegisterCallback(callback);
			Debug.Log ("GetUnitStatus = " + keyOfGetUnitStatus);

			getUnitStatus(unitId, delegateGetUnitStatusCallback);
		}

		private static Action<AdiscopeError, UnitStatus> GetCallback(string key)
        {
			Action<AdiscopeError, UnitStatus> callback;
            if (handlerMap.TryGetValue(key, out callback))
            {
                handlerMap.Remove(key);
				return callback;
            }
            else
            {
				Debug.Log("Unregistered callback handler: " + key);
            }
			return null;
        }

		private string RegisterCallback(Action<AdiscopeError, UnitStatus> callback)
        {
            if (callback == null)
                return "";

            var key = Guid.NewGuid().ToString();
            handlerMap.Add(key, callback);
            return key;
        }

        public void Initialize(InitAction callback, string callbackTag)
        {
            throw new NotImplementedException();
        }
    }
}

#endif