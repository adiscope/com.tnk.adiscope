/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using Adiscope.Model;
using System;
using Adiscope.Feature;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for OfferwallAd client
    /// </summary>
    internal interface IOfferwallAdClient
    {
        event EventHandler<ShowResult> OnOpened;
        event EventHandler<ShowResult> OnClosed;
        event EventHandler<ShowFailure> OnFailedToShow;

        event EventHandler<ShowResult> OnOpenedBackground;
        event EventHandler<ShowResult> OnClosedBackground;
        event EventHandler<ShowFailure> OnFailedToShowBackground;

        bool Show(string unitId, string[] filters);

        bool ShowOfferwallDetail(string unitId, string[] filters, string itemId);

        bool ShowOfferwallDetailFromUrl(string url);

        
    }
}
