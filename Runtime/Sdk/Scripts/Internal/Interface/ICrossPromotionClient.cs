/*
 * Created by nmj
 */
using Adiscope.Model.Cross;
using System;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for CrossPromotion client
    /// </summary>
    internal interface ICrossPromotionClient
    {
        event EventHandler<BannerResult> OnOpened;
        event EventHandler<BannerCloseResult> OnClosed;
        event EventHandler<BannerFailure> OnFailedToShow;
        event EventHandler<BannerResult> OnLeftApplication;
        event EventHandler<EventArgs> OnInitializationSucceeded;
        event EventHandler<InitializationFailure> OnInitializationFailed;

        void Initialize();
        void SetTrackingInfo(string userId);
        void ShowMoreGames();
        void ShowFullScreenPopup();
        void ShowEndingPopup();
    }
}
