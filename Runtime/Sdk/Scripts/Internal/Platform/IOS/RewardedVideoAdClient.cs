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
	/// iOS client for rewarded video ad
	/// this class will call iOS native plugin's method
	/// </summary>	
	internal class RewardedVideoAdClient : IRewardedVideoAdClient
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

		public static RewardedVideoAdClient Instance;

		public RewardedVideoAdClient ()
		{
			Instance = this;
		}

		#region AD APIs 
		[DllImport ("__Internal")]
		private static extern void unityLoad(string unitId, onRewardedVideoAdLoadedCallback loadedCallback, onRewardedVideoAdFailedToLoadCallback failLoadedCallback);

		public void Load(string unitId)
		{
			unityLoad(unitId, onRewardedVideoAdLoaded, onRewardedVideoAdFailedToLoad);
		}

		[DllImport ("__Internal")]
		private static extern bool isLoaded(string unitId);

		public bool IsLoaded(string unitId)
		{
			return isLoaded (unitId);
		}

		[DllImport ("__Internal")]
		private static extern bool show(onRewardedVideoAdOpenedCallback openedCallback, onRewardedVideoAdClosedCallback closedCallback, 
			onRewardedCallback rewardedCallback, onRewardedVideoAdFailedToShowCallback failedToShowCallback);

		public bool Show()
		{
			return show (onRewardedVideoAdOpened, onRewardedVideoAdClosed, onRewarded, onRewardedVideoAdFailedToShow);
		}

		#endregion

		#region Callbacks

		private delegate void onRewardedVideoAdLoadedCallback(string unitId);

		[MonoPInvokeCallback(typeof(onRewardedVideoAdLoadedCallback))] 
		public static void onRewardedVideoAdLoaded(string unitId)
		{
			Debug.Log("onRewardedVideoAdLoaded()");
			Instance.RewardedVideoAdLoadedProc(unitId);
		}

		private void RewardedVideoAdLoadedProc(string unitId) 
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
				this.OnLoadedBackground (this, new LoadResult(unitId));
			}
		}

		private delegate void onRewardedVideoAdFailedToLoadCallback(string unitId, int code, string description, string xb3TraceID);

		[MonoPInvokeCallback(typeof(onRewardedVideoAdFailedToLoadCallback))] 
		public static void onRewardedVideoAdFailedToLoad(string unitId, int code, string description, string xb3TraceID)
		{
			Instance.RewardedVideoAdFailedToLoadProc(unitId, code, description, xb3TraceID);
		}

		private void RewardedVideoAdFailedToLoadProc(string unitId, int code, string description, string xb3TraceID)
		{
			if (this.OnFailedToLoad != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					this.OnFailedToLoad(
						this, new LoadFailure(unitId, new AdiscopeError (code, description, xb3TraceID)));
				});
			}

			if (this.OnFailedToLoadBackground != null)
			{
				this.OnFailedToLoadBackground(
					Instance, new LoadFailure(unitId, new AdiscopeError (code, description, xb3TraceID)));
			}
		}

		private delegate void onRewardedVideoAdOpenedCallback(string unitId);

		[MonoPInvokeCallback(typeof(onRewardedVideoAdOpenedCallback))] 
		public static void onRewardedVideoAdOpened(string unitId)
		{
			Debug.Log("onRewardedVideoAdOpened() unitId = " + unitId);
			Instance.RewardedVideoAdOpenedProc (unitId);
		}

		private void RewardedVideoAdOpenedProc(string unitId)
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

		private delegate void onRewardedVideoAdClosedCallback(string unitId);

		[MonoPInvokeCallback(typeof(onRewardedVideoAdClosedCallback))] 
		public static void onRewardedVideoAdClosed(string unitId)
		{
			Debug.Log("onRewardedVideoAdClosed() unitId = " + unitId);
			string nullSafetyUnitID = unitId;
			if (nullSafetyUnitID == null) { nullSafetyUnitID = ""; }
			Instance.RewardedVideoAdClosedProc (unitId);
		}

		private void RewardedVideoAdClosedProc(string unitId)
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

		private delegate void onRewardedCallback(string unitId, string type, long amount);

		[MonoPInvokeCallback(typeof(onRewardedCallback))]
		public static void onRewarded(string unitId, string type, long amount)
		{
			Debug.Log("onRewarded() unitId = " + unitId + " type = " + type + " amount = " + amount);
			Instance.RewardedProc (unitId, type, amount);
		}

		private void RewardedProc(string unitId, string type, long amount)
		{
			if (Instance.OnRewarded != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnRewarded(Instance, new RewardItem(unitId, type, amount));
				});
			}

			if (Instance.OnRewardedBackground != null)
			{
				Instance.OnRewardedBackground(Instance, new RewardItem(unitId, type, amount));
			}		
		}

		private delegate void onRewardedVideoAdFailedToShowCallback(string unitId, int code, string description, string xb3TraceID);

		[MonoPInvokeCallback(typeof(onRewardedVideoAdFailedToShowCallback))]
		public static void onRewardedVideoAdFailedToShow(string unitId, int code, string description, string xb3TraceID)
		{
			Instance.RewardedVideoAdFailedToShowProc (unitId, code, description, xb3TraceID);
		}

		private void RewardedVideoAdFailedToShowProc(string unitId, int code, string description, string xb3TraceID)
		{
			if (Instance.OnFailedToShow != null)
			{
				UnityThread.executeInMainThread(() =>
				{
					Instance.OnFailedToShow(
						Instance, new ShowFailure(unitId, new AdiscopeError(code, description, xb3TraceID)));
				});
			}

			if (Instance.OnFailedToShowBackground != null)
			{
				Instance.OnFailedToShowBackground (
					Instance, new ShowFailure(unitId, new AdiscopeError (code, description, xb3TraceID)));
			}
		}

		#endregion
	}
}

#endif