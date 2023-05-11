/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
#if (UNITY_EDITOR) || (!UNITY_ANDROID)
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using System.Threading;
using Adiscope.Feature;

namespace Adiscope.Internal.Platform.MockPlatform
{
    /// <summary>
    /// mockup client for offerwall ad
    /// this class will emulate callback very simply, limitedly
    /// </summary>
    internal class OfferwallAdClient : IOfferwallAdClient
    {
        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowFailure> OnFailedToShow;

        public event EventHandler<ShowResult> OnOpenedBackground;
        public event EventHandler<ShowResult> OnClosedBackground;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        private string unitId;
        private string itemId;
        private string url;
        private bool showing;

        public OfferwallAdClient()
        {
        }

        #region AD APIs 
        public bool Show(string unitId, string[] filters)
        {
            if (this.showing)
            {
                return false;
            }

            this.showing = true;

            this.unitId = unitId;
#if (UNITY_EDITOR)
            new Thread(() => DelayedCallback(onOfferwallAdOpened, 100)).Start();
            new Thread(() => DelayedCallback(onOfferwallAdClosed, 5000)).Start();
#else
            new Thread(() => DelayedCallback(onOfferwallAdFailedToShow, 5)).Start();
#endif
            return true;
        }
        #endregion

        static void DelayedCallback(Action action, int delay)
        {
            Thread.Sleep(delay);
            action.Invoke();
        }

        #region Callbacks
        public void onOfferwallAdOpened()
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

        public void onOfferwallAdClosed()
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

            this.showing = false;
        }

        public void onOfferwallAdFailedToShow()
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

        public bool ShowOfferwallDetail(string unitId, string[] filters, string itemId)
        {
            if (this.showing)
            {
                return false;
            }

            this.showing = true;

            this.unitId = unitId;
            this.itemId = itemId;
#if (UNITY_EDITOR)
            new Thread(() => DelayedCallback(onOfferwallAdOpened, 100)).Start();
            new Thread(() => DelayedCallback(onOfferwallAdClosed, 5000)).Start();
#else
            new Thread(() => DelayedCallback(onOfferwallAdFailedToShow, 5)).Start();
#endif
            return true;
        }

        public bool ShowOfferwallDetailFromUrl(string url)
        {
            if (this.showing)
            {
                return false;
            }

            this.showing = true;

            this.url = url;
#if (UNITY_EDITOR)
            new Thread(() => DelayedCallback(onOfferwallAdOpened, 100)).Start();
            new Thread(() => DelayedCallback(onOfferwallAdClosed, 5000)).Start();
#else
            new Thread(() => DelayedCallback(onOfferwallAdFailedToShow, 5)).Start();
#endif
            return true;
        }
        #endregion
    }
}
#endif