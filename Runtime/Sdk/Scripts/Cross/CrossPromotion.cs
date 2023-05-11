/*
 * Created by nmj
 */
using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using Adiscope.Model.Cross;
using System;

namespace Adiscope.Cross
{
    /// <summary>
    /// CrossPromotion singleton instance class
    /// </summary>
    public class CrossPromotion
    {
        /// <summary>
        /// callback when CrossPromotion dialog is opened
        /// </summary>
        public event EventHandler<BannerResult> OnOpened;

        /// <summary>
        /// callback when CrossPromotion dialog is closed
        /// </summary>
        public event EventHandler<BannerCloseResult> OnClosed;

        /// <summary>
        /// callback when CrossPromotion is failed to be shown
        /// </summary>
        public event EventHandler<BannerFailure> OnFailedToShow;

        /// <summary>
        /// as the user clicks the banner, the foreground process changes to another application
        /// </summary>
        public event EventHandler<BannerResult> OnLeftApplication;

        /// <summary>
        /// callback when CrossPromotion initialization is successful
        /// </summary>
        public event EventHandler<EventArgs> OnInitializationSucceeded;

        /// <summary>
        /// callback when CrossPromotion initialization fails
        /// </summary>
        public event EventHandler<InitializationFailure> OnInitializationFailed;

        private ICrossPromotionClient client;

        private static class InitializationOnDemandHolderIdiom
        {
            public static readonly CrossPromotion SingletonInstance = new CrossPromotion();
        }

        public static CrossPromotion Instance
        {
            get
            {
                return InitializationOnDemandHolderIdiom.SingletonInstance;
            }
        }

        private CrossPromotion()
        {
            this.client = ClientBuilder.BuildCrossPromotionClient();

            this.client.OnOpened += (sender, args) =>
            {
                if (this.OnOpened != null)
                {
                    this.OnOpened(sender, args);
                }
            };

            this.client.OnClosed += (sender, args) =>
            {
                if (this.OnClosed != null)
                {
                    this.OnClosed(sender, args);
                }
            };

            this.client.OnFailedToShow += (sender, args) =>
            {
                if (this.OnFailedToShow != null)
                {
                    this.OnFailedToShow(sender, args);
                }
            };

            this.client.OnLeftApplication += (sender, args) =>
            {
                if (this.OnLeftApplication != null)
                {
                    this.OnLeftApplication(sender, args);
                }
            };

            this.client.OnInitializationSucceeded += (sender, args) =>
            {
                if (this.OnInitializationSucceeded != null)
                {
                    this.OnInitializationSucceeded(sender, args);
                }
            };

            this.client.OnInitializationFailed += (sender, args) =>
            {
                if (this.OnInitializationFailed != null)
                {
                    this.OnInitializationFailed(sender, args);
                }
            };

        }

        /// <summary>
        /// initialize for cross promotion
        /// </summary>
        public void Initialize()
        {
            client.Initialize();
        }

        /// <summary>
        /// set tracking information.It should be call before show api is called
        /// </summary>
        /// <param name="userId">userId</param>
        public void SetTrackingInfo(string userId)
        {
            client.SetTrackingInfo(userId);
        }

        /// <summary>
        /// pop up More Games dialog
        /// </summary>
        public void ShowMoreGames()
        {
            client.ShowMoreGames();
        }

        /// <summary>
        /// pop up FullScreen Banner dialog
        /// </summary>
        public void ShowFullScreenPopup()
        {
            client.ShowFullScreenPopup();
        }

        /// <summary>
        /// pop up Ending Banner dialog
        /// </summary>
        public void ShowEndingPopup()
        {
            client.ShowEndingPopup();
        }

    }
}
