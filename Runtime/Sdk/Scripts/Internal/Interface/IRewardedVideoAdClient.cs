/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using Adiscope.Model;
using System;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for RewardedVideoAd client
    /// </summary>
    internal interface IRewardedVideoAdClient
    {
        event EventHandler<LoadResult> OnLoaded;
        event EventHandler<LoadFailure> OnFailedToLoad;
        event EventHandler<ShowResult> OnOpened;
        event EventHandler<ShowResult> OnClosed;
        event EventHandler<RewardItem> OnRewarded;
        event EventHandler<ShowFailure> OnFailedToShow;

        event EventHandler<LoadResult> OnLoadedBackground;
        event EventHandler<LoadFailure> OnFailedToLoadBackground;
        event EventHandler<ShowResult> OnOpenedBackground;
        event EventHandler<ShowResult> OnClosedBackground;
        event EventHandler<RewardItem> OnRewardedBackground;
        event EventHandler<ShowFailure> OnFailedToShowBackground;

        void Load(string unitId);
        bool IsLoaded(string unitId);
        bool Show();
    }
}
