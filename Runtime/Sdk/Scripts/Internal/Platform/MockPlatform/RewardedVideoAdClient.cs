/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
#if (UNITY_EDITOR) || (!UNITY_ANDROID)
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using System.Threading;

namespace Adiscope.Internal.Platform.MockPlatform
{
    /// <summary>
    /// mockup client for rewarded video ad
    /// this class will emulate callback very simply, limitedly
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

        private bool loaded;
        private string unitId;

        public RewardedVideoAdClient()
        {
        }

        #region AD APIs 

        public void Load(string unitId)
        {
            this.unitId = unitId;

#if UNITY_EDITOR
            new Thread(() => DelayedCallback(onRewardedVideoAdLoaded, 1000)).Start();
#else
            new Thread(() => DelayedCallback(onRewardedVideoAdFailedToLoad, 5)).Start();
#endif
        }

        public bool IsLoaded(string unitId)
        {
            return this.loaded;
        }

        public bool Show()
        {
#if UNITY_EDITOR
            if (!this.loaded)
            {
                new Thread(() => DelayedCallback(onRewardedVideoAdFailedToShow, 10)).Start();
            }
            else
            {
                new Thread(() => DelayedCallback(onRewardedVideoAdOpened, 100)).Start();
                new Thread(() => DelayedCallback(onRewardedVideoAdClosed, 5000)).Start();
                new Thread(() => DelayedCallback(onRewarded, 5500)).Start();
                this.loaded = false;
            }
#else
            new Thread(() => DelayedCallback(onRewardedVideoAdFailedToShowUnsupported, 5)).Start();
#endif
            return true;
        }
        #endregion

        private void DelayedCallback(Action action, int delay)
        {
            Thread.Sleep(delay);
            action.Invoke();
        }

        #region Callbacks

        public void onRewardedVideoAdLoaded()
        {
            this.loaded = true;

            if (this.OnLoaded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnLoaded(this, new LoadResult(this.unitId));
                });
            }

            if (this.OnLoadedBackground != null)
            {
                this.OnLoadedBackground(this, new LoadResult(this.unitId));
            }
        }

        public void onRewardedVideoAdFailedToLoad()
        {
            AdiscopeError error = new AdiscopeError(-1, "Adiscope only supports following platforms: Android");

            if (this.OnFailedToLoad != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToLoad(this, new LoadFailure(this.unitId, error));
                });
            }

            if (this.OnFailedToLoadBackground != null)
            {
                this.OnFailedToLoadBackground(this, new LoadFailure(this.unitId, error));
            }
        }

        public void onRewardedVideoAdOpened()
        {
            if (this.OnOpened != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnOpened(this, new ShowResult(this.unitId));
                });
            }

            if (this.OnOpenedBackground != null)
            {
                this.OnOpenedBackground(this, new ShowResult(this.unitId));
            }
        }

        public void onRewardedVideoAdClosed()
        {
            if (this.OnClosed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnClosed(this, new ShowResult(this.unitId));
                });
            }

            if (this.OnClosedBackground != null)
            {
                this.OnClosedBackground(this, new ShowResult(this.unitId));
            }
        }

        public void onRewarded()
        {
            RewardItem param = new RewardItem(this.unitId, "sample", 0);

            if (this.OnRewarded != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnRewarded(this, param);
                });
            }

            if (this.OnRewardedBackground != null)
            {
                this.OnRewardedBackground(this, param);
            }
        }

        public void onRewardedVideoAdFailedToShow()
        {
            AdiscopeError error = new AdiscopeError(6, "No more ads to show");

            if (this.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToShow(this, new ShowFailure(this.unitId, error));
                });
            }

            if (this.OnFailedToShowBackground != null)
            {
                this.OnFailedToShowBackground(this, new ShowFailure(this.unitId, error));
            }
        }

        public void onRewardedVideoAdFailedToShowUnsupported()
        {
            AdiscopeError error = new AdiscopeError(-1, "Adiscope only supports following platforms: Android");

            if (this.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToShow(this, new ShowFailure(this.unitId, error));
                });
            }

            if (this.OnFailedToShowBackground != null)
            {
                this.OnFailedToShowBackground(this, new ShowFailure(this.unitId, error));
            }
        }
#endregion
    }
}
#endif