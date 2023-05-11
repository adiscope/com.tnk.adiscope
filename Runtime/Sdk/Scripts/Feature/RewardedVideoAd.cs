/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Feature
{
    public class RewardedVideoAd
    {
        public event EventHandler<LoadResult> OnLoaded;
        public event EventHandler<LoadResult> OnLoadedBackground;

        public event EventHandler<LoadFailure> OnFailedToLoad;
        public event EventHandler<LoadFailure> OnFailedToLoadBackground;

        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnOpenedBackground;

        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowResult> OnClosedBackground;

        public event EventHandler<ShowFailure> OnFailedToShow;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        public event EventHandler<RewardItem> OnRewarded;
        public event EventHandler<RewardItem> OnRewardedBackground;
        

        private IRewardedVideoAdClient client;

        private static class ClassWrapper { public static readonly RewardedVideoAd instance = new RewardedVideoAd(); }

        public static RewardedVideoAd Instance { get
            {
                RewardedVideoAd abcd = ClassWrapper.instance;
                return abcd;
            }
        }

        private RewardedVideoAd()
        {
            this.client = ClientBuilder.BuildRewardedVideoAdClient();
            this.client.OnLoaded += (sender, args) => { OnLoaded?.Invoke(sender, args); };

            this.client.OnLoadedBackground += (sender, args) => { OnLoadedBackground?.Invoke(sender, args); };

            this.client.OnFailedToLoad += (sender, args) => { OnFailedToLoad?.Invoke(sender, args); };
            this.client.OnFailedToLoadBackground += (sender, args) => { OnFailedToLoadBackground?.Invoke(sender, args); };

            this.client.OnOpened += (sender, args) => { OnOpened?.Invoke(sender, args); };
            this.client.OnOpenedBackground += (sender, args) => { OnOpenedBackground?.Invoke(sender, args); };

            this.client.OnClosed += (sender, args) => { OnClosed?.Invoke(sender, args); };
            this.client.OnClosedBackground += (sender, args) => { OnClosedBackground?.Invoke(sender, args); };

            this.client.OnFailedToShow += (sender, args) => { OnFailedToShow?.Invoke(sender, args); };
            this.client.OnFailedToShowBackground += (sender, args) => { OnFailedToShowBackground?.Invoke(sender, args); };

            this.client.OnRewarded += (sender, args) => { OnRewarded?.Invoke(sender, args); };
            this.client.OnRewardedBackground += (sender, args) => { OnRewardedBackground?.Invoke(sender, args); };
        }

        public void Load(string unitId) { client.Load(unitId); }

        public bool IsLoaded(string unitId) { return client.IsLoaded(unitId); }

        public bool Show() { return client.Show();}
    }
}
