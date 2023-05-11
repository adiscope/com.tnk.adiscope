using Adiscope.Model;
using System;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for InterstitialAd client
    /// </summary>
    internal interface IInterstitialAdClient
    {
        event EventHandler<LoadResult> OnLoaded;
        event EventHandler<LoadFailure> OnFailedToLoad;
        event EventHandler<ShowResult> OnOpened;
        event EventHandler<ShowResult> OnClosed;
        event EventHandler<ShowFailure> OnFailedToShow;

        event EventHandler<LoadResult> OnLoadedBackground;
        event EventHandler<LoadFailure> OnFailedToLoadBackground;
        event EventHandler<ShowResult> OnOpenedBackground;
        event EventHandler<ShowResult> OnClosedBackground;
        event EventHandler<ShowFailure> OnFailedToShowBackground;

        void Load(string unitId);
        bool IsLoaded(string unitId);
        bool Show();
    }
}
