/*
 * Created by Sunhak Lee (shlee@neowiz.com)
 */
#if UNITY_IOS

using Adiscope.Internal.Interface;
using Adiscope.Model;
using Adiscope.Model.Cross;
using System;
using UnityEngine;

using System.Runtime.InteropServices;
using AOT;

namespace Adiscope.Internal.Platform.IOS
{
	/// <summary>
	/// iOS client for cross promotion
	/// this class will call iOS native plugin's method
	/// </summary>		
	public class CrossPromotionClient : ICrossPromotionClient
	{
        public event EventHandler<BannerResult> OnOpened;
        public event EventHandler<BannerCloseResult> OnClosed;
        public event EventHandler<BannerFailure> OnFailedToShow;
        public event EventHandler<BannerResult> OnLeftApplication;
        public event EventHandler<EventArgs> OnInitializationSucceeded;
        public event EventHandler<InitializationFailure> OnInitializationFailed;

		public static CrossPromotionClient Instance;

		public CrossPromotionClient ()
		{
			Instance = this;
		}

		#region AD APIs 

		[DllImport ("__Internal")]
        private static extern void CPInitialize(onInitializationSucceededCallback succeededCallback, onInitializationFailedCallback failedCallback);

        public void Initialize()
        {
            Debug.Log("Initialize()");
            CPInitialize(onInitializationSucceeded, onInitializationFailed);
        }

        [DllImport ("__Internal")]
        private static extern void setTrackingInfo(string userId);

        public void SetTrackingInfo(string userId)
        {
            Debug.Log("SetTrackingInfo() userId = " + userId);
            setTrackingInfo(userId);
        }

        [DllImport ("__Internal")]
        private static extern void showMoreGames(onOpenedCallback openedCallback, onFailedToShowCallback failedToShowCallback, onLeftApplicationCallback leftApplicationCallback, onClosedCallback closedCallback);

        public void ShowMoreGames()
        {
            Debug.Log("ShowMoreGames()");
            showMoreGames(onOpened, onFailedToShow, onLeftApplication, onClosed);
        }    

        [DllImport ("__Internal")]
        private static extern void showFullScreenPopup(onOpenedCallback openedCallback, onFailedToShowCallback failedToShowCallback, onLeftApplicationCallback leftApplicationCallback, onClosedCallback closedCallback);

        public void ShowFullScreenPopup()
        {
            Debug.Log("ShowFullScreenPopup()");
            showFullScreenPopup(onOpened, onFailedToShow, onLeftApplication, onClosed);
        }   

        [DllImport ("__Internal")]
        private static extern void showEndingPopup(onOpenedCallback openedCallback, onFailedToShowCallback failedToShowCallback, onLeftApplicationCallback leftApplicationCallback, onClosedCallback closedCallback);

        public void ShowEndingPopup()
        {
            Debug.Log("ShowEndingPopup()");
            showEndingPopup(onOpened, onFailedToShow, onLeftApplication, onClosed);
        }

		#endregion

		#region Callbacks

        private delegate void onInitializationSucceededCallback();

        [MonoPInvokeCallback(typeof(onInitializationSucceededCallback))]
        public static void onInitializationSucceeded()
        {
			Debug.Log("onInitializationSucceeded()");
			Instance.InitializationSucceededProc();
		}

		private void InitializationSucceededProc()
		{
            if (Instance.OnInitializationSucceeded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    Instance.OnInitializationSucceeded(Instance, EventArgs.Empty);
                });
            }
		}

        private delegate void onInitializationFailedCallback(int code, string description);

        [MonoPInvokeCallback(typeof(onInitializationFailedCallback))]
        public static void onInitializationFailed(int code, string description)
        {
			Debug.Log("onInitializationFailed() Error code = " + code + " description = " + description);
			Instance.InitializationFailedProc(code, description);
		}

		private void InitializationFailedProc(int code, string description)
		{
            if (Instance.OnInitializationFailed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    Instance.OnInitializationFailed(Instance, new InitializationFailure(new AdiscopeError(code, description)));
                });
            }
		}

        private delegate void onOpenedCallback(int bannerType);

        [MonoPInvokeCallback(typeof(onOpenedCallback))] 
        public static void onOpened(int bannerType)
        {
			Debug.Log("onOpened() bannerType = " + bannerType);
			Instance.OpenedProc(bannerType);
		}

		private void OpenedProc(int bannerType)
		{
            if (Instance.OnOpened != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    Instance.OnOpened(Instance, new BannerResult(bannerType));
                });
            }
		}             

		private delegate void onFailedToShowCallback(int bannerType, int code, string description);

        [MonoPInvokeCallback(typeof(onFailedToShowCallback))] 
        public static void onFailedToShow(int bannerType, int code, string description)
        {
			Debug.Log("onFailedToShow() bannerType = " + bannerType + " Error code = " + code + " description = " + description);
			Instance.FailedToShowProc(bannerType, code, description);
		}

		private void FailedToShowProc(int bannerType, int code, string description)
		{
            if (Instance.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    Instance.OnFailedToShow(Instance, new BannerFailure(bannerType, new AdiscopeError(code, description)));
                });
            }
		}    

        private delegate void onLeftApplicationCallback(int bannerType);

        [MonoPInvokeCallback(typeof(onLeftApplicationCallback))] 
        public static void onLeftApplication(int bannerType)
        {
			Debug.Log("onLeftApplication() bannerType = " + bannerType);
			Instance.LeftApplicationProc(bannerType);
		}  

		private void LeftApplicationProc(int bannerType)
		{
            if (Instance.OnLeftApplication != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    Instance.OnLeftApplication(Instance, new BannerResult(bannerType));
                });
            }
		}    

        private delegate void onClosedCallback(int bannerType, int flag);

        [MonoPInvokeCallback(typeof(onClosedCallback))] 
        public static void onClosed(int bannerType, int flag)
        {
			Debug.Log("onClosed() bannerType = " + bannerType + " flag = " + flag);
			Instance.ClosedProc(bannerType, flag);
		}  

		private void ClosedProc(int bannerType, int flag)
		{
            if (Instance.OnClosed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    Instance.OnClosed(Instance, new BannerCloseResult(bannerType, flag));
                });             
            }
		}            

		#endregion
	}
}

#endif