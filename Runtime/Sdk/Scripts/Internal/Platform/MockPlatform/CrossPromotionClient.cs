/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
#if (UNITY_EDITOR) || (!UNITY_ANDROID)
using Adiscope.Internal.Interface;
using Adiscope.Model;
using Adiscope.Model.Cross;
using System;
using System.Threading;

#pragma warning disable 0067
namespace Adiscope.Internal.Platform.MockPlatform
{
    /// <summary>
    /// mockup client for rewarded video ad
    /// this class will emulate callback very simply, limitedly
    /// </summary>
    internal class CrossPromotionClient : ICrossPromotionClient
    {
        public event EventHandler<BannerResult> OnOpened;
        public event EventHandler<BannerCloseResult> OnClosed;
        public event EventHandler<BannerFailure> OnFailedToShow;
        public event EventHandler<BannerResult> OnLeftApplication;
        public event EventHandler<EventArgs> OnInitializationSucceeded;
        public event EventHandler<InitializationFailure> OnInitializationFailed;

        private const int BannerTypeMoreGames   = 1;
        private const int BannerTypeFullScreen  = 2;
        private const int BannerTypeEnding      = 3;

        public CrossPromotionClient()
        {
        }

        #region AD APIs 
        public void Initialize()
        {
#if (UNITY_EDITOR)
            new Thread(() => DelayedCallback(onOpened, BannerTypeMoreGames, 100)).Start();
            new Thread(() =>
            {
                Thread.Sleep(100);
                if (this.OnInitializationSucceeded != null)
                {
                    UnityThread.executeInMainThread(() =>
                    {
                        this.OnInitializationSucceeded(this, EventArgs.Empty);
                    });
                }
            });
#else
            // nothing
#endif
        }

        public void SetTrackingInfo(string userId)
        {

        }

        public void ShowMoreGames()
        {

#if (UNITY_EDITOR)
            new Thread(() => DelayedCallback(onOpened, BannerTypeMoreGames, 100)).Start();
            new Thread(() => DelayedCallback(onClosed, BannerTypeMoreGames, 3000)).Start();
#else
            new Thread(() => DelayedCallback(onFailedToShowUnsupported, BannerTypeMoreGames, 5)).Start();
#endif
        }

        public void ShowFullScreenPopup()
        {

#if (UNITY_EDITOR)
            new Thread(() => DelayedCallback(onOpened, BannerTypeFullScreen, 100)).Start();
            new Thread(() => DelayedCallback(onClosed, BannerTypeFullScreen, 3000)).Start();
#else
            new Thread(() => DelayedCallback(onFailedToShowUnsupported, BannerTypeFullScreen, 5)).Start();
#endif
        }

        public void ShowEndingPopup()
        {

#if (UNITY_EDITOR)
            new Thread(() => DelayedCallback(onOpened, BannerTypeEnding, 100)).Start();
            new Thread(() => DelayedCallback(onClosed, BannerTypeEnding, 3000)).Start();
#else
            new Thread(() => DelayedCallback(onFailedToShowUnsupported, BannerTypeEnding, 5)).Start();
#endif
        }

        #endregion

        private void DelayedCallback(Action<int> action, int param, int delay)
        {
            Thread.Sleep(delay);
            action.Invoke(param);
        }

        #region Callbacks
        public void onOpened(int bannerType)
        {
            if (this.OnOpened != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnOpened(this, new BannerResult(bannerType));
                });
            }
        }

        public void onClosed(int bannerType)
        {
            if (this.OnClosed != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnClosed(this, new BannerCloseResult(bannerType, 0));
                });
            }
        }

        public void onFailedToShowUnsupported(int bannerType)
        {
            AdiscopeError error = new AdiscopeError(-1, "Adiscope only supports following platforms: Android");

            if (this.OnFailedToShow != null)
            {
                UnityThread.executeInMainThread(() =>
                {
                    this.OnFailedToShow(this, new BannerFailure(bannerType, error));
                });
            }
        }
        
#endregion
    }
}
#endif