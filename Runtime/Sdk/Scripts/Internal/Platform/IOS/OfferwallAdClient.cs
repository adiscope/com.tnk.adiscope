/*
 * Created by Sunhak Lee (shlee@neowiz.com)
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
	/// <summary>
	/// iOS client for offerwall ad
	/// this class will call iOS native plugin's method
	/// </summary>		
	public class OfferwallAdClient : IOfferwallAdClient
	{
		public event EventHandler<ShowResult> OnOpened;
		public event EventHandler<ShowResult> OnClosed;
		public event EventHandler<ShowFailure> OnFailedToShow;

		public event EventHandler<ShowResult> OnOpenedBackground;
		public event EventHandler<ShowResult> OnClosedBackground;
		public event EventHandler<ShowFailure> OnFailedToShowBackground;

		public static OfferwallAdClient Instance;

		public OfferwallAdClient ()
		{
			Instance = this;
		}

		#region AD APIs 

		[DllImport ("__Internal")]
		private static extern bool showOfferwall(string unitId, string[] filters, onOfferwallAdOpenedCallback openedCallback, onOfferwallAdClosedCallback closedCallback, 
			onOfferwallAdFailedToShowCallback failedToShowCallback);

		public bool Show(string unitId, string[] filters)
		{
			return showOfferwall (unitId, filters, onOfferwallAdOpened, onOfferwallAdClosed, onOfferwallAdFailedToShow);
		}

		[DllImport("__Internal")]
		private static extern bool showOfferwallDetailFromUrl(string url, onOfferwallAdOpenedCallback openedCallback, onOfferwallAdClosedCallback closedCallback,
			onOfferwallAdFailedToShowCallback failedToShowCallback);

		public bool ShowOfferwallDetailFromUrl(string url)
		{
			return showOfferwallDetailFromUrl(url, onOfferwallAdOpened, onOfferwallAdClosed, onOfferwallAdFailedToShow);
		}

		[DllImport("__Internal")]
		private static extern bool showOfferwallDetail(string unitId, string itemId, string[] filters, onOfferwallAdOpenedCallback openedCallback, onOfferwallAdClosedCallback closedCallback,
			onOfferwallAdFailedToShowCallback failedToShowCallback);

		public bool ShowOfferwallDetail(string unitId, string[] filters, string itemId)
		{
			return showOfferwallDetail(unitId, itemId, filters, onOfferwallAdOpened, onOfferwallAdClosed, onOfferwallAdFailedToShow);
		}

		#endregion

		#region Callbacks

		private delegate void onOfferwallAdOpenedCallback(string unitId);

		[MonoPInvokeCallback(typeof(onOfferwallAdOpenedCallback))] 
		public static void onOfferwallAdOpened(string unitId)
		{
			Debug.Log("onOfferwallAdOpened() unitId = " + unitId);
			Instance.OfferwallAdOpenedProc (unitId);
		}

		private void OfferwallAdOpenedProc(string unitId)
		{
			if (Instance.OnOpened != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnOpened(Instance, new ShowResult(unitId));
				});
			}

			if (Instance.OnOpenedBackground != null)
			{
				Instance.OnOpenedBackground(Instance, new ShowResult(unitId));
			}
		}

		private delegate void onOfferwallAdClosedCallback(string unitId);

		[MonoPInvokeCallback(typeof(onOfferwallAdClosedCallback))] 
		public static void onOfferwallAdClosed(string unitId)
		{
			Debug.Log("onOfferwallAdClosed() unitId = " + unitId);
			Instance.OfferwallAdClosedClosedProc (unitId);
		}

		private void OfferwallAdClosedClosedProc(string unitId)
		{
			if (Instance.OnClosed != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnClosed(Instance, new ShowResult(unitId));
				});
			}

			if (Instance.OnClosedBackground != null)
			{
				Instance.OnClosedBackground(Instance, new ShowResult(unitId));
			}
		}
			
		private delegate void onOfferwallAdFailedToShowCallback(string unitId, int code, string description);

		[MonoPInvokeCallback(typeof(onOfferwallAdFailedToShowCallback))]
		public static void onOfferwallAdFailedToShow(string unitId, int code, string description)
		{
			Debug.Log("onOfferwallAdFailedToShow() unitId = " + unitId + " code = " + code + " description = " + description);
			Instance.OfferwallAdFailedToShowProc (unitId, code, description);
		}

		private void OfferwallAdFailedToShowProc(string unitId, int code, string description)
		{
			if (Instance.OnFailedToShow != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnFailedToShow(
						Instance, new ShowFailure(unitId, new AdiscopeError(code, description)));
				});
			}

			if (Instance.OnFailedToShowBackground != null)
			{
				Instance.OnFailedToShowBackground (
					Instance, new ShowFailure(unitId, new AdiscopeError (code, description)));
			}
		}
        #endregion
    }
}

#endif