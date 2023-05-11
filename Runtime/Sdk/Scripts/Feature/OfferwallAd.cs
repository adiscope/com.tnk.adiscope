/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using System.Collections.Generic;
using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using Adiscope.Model;
using System;

namespace Adiscope 
{
        public enum OfferwallFilterType {
        CPS,
        CPI,
        CPA
    }
}

namespace Adiscope.Feature
{
    public class OfferwallAd
    {
        public event EventHandler<ShowResult> OnOpened;
        public event EventHandler<ShowResult> OnOpenedBackground;

        public event EventHandler<ShowResult> OnClosed;
        public event EventHandler<ShowResult> OnClosedBackground;

        public event EventHandler<ShowFailure> OnFailedToShow;
        public event EventHandler<ShowFailure> OnFailedToShowBackground;

        private IOfferwallAdClient client;

        private static class ClassWrapper { public static readonly OfferwallAd instance = new OfferwallAd(); }
        public static OfferwallAd Instance { get { return ClassWrapper.instance; } }

        private OfferwallAd()
        {
            this.client = ClientBuilder.BuildOfferwallAdClient();

            this.client.OnOpened += (sender, args) => { OnOpened?.Invoke(sender, args); };
            this.client.OnOpenedBackground += (sender, args) => { OnOpenedBackground?.Invoke(sender, args); };

            this.client.OnClosed += (sender, args) => { OnClosed?.Invoke(sender, args); };
            this.client.OnClosedBackground += (sender, args) => { OnClosedBackground?.Invoke(sender, args); };

            this.client.OnFailedToShowBackground += (sender, args) => { OnFailedToShowBackground?.Invoke(sender, args); };
            this.client.OnFailedToShow += (sender, args) => { OnFailedToShow?.Invoke(sender, args); };
        }

        public bool Show(string unitId) { return client.Show(unitId, new string[] {}); }
        public bool Show(string unitId, OfferwallFilterType[] filters)
        {
            List<string> array = new List<string>();

            foreach (OfferwallFilterType filter in filters)
            {
                string filterString = null;

                if (filter == OfferwallFilterType.CPS) { filterString = "CPS"; }
                else if (filter == OfferwallFilterType.CPI) { filterString = "CPI"; }
                else if (filter == OfferwallFilterType.CPA) { filterString = "CPA"; }
                array.Add(filterString);
            }
            return client.Show(unitId, array.ToArray());
        }

        public bool ShowOfferwallDetail(string unitId, OfferwallFilterType[] filters, string itemId) { 
            List<string> array = new List<string>();
            
            foreach (OfferwallFilterType filter in filters) {
                string filterString = null;

                if (filter == OfferwallFilterType.CPS)      { filterString = "CPS"; }
                else if (filter == OfferwallFilterType.CPI) { filterString = "CPI"; }
                else if (filter == OfferwallFilterType.CPA) { filterString = "CPA"; }
                array.Add(filterString);
            }           
            return client.ShowOfferwallDetail(unitId, array.ToArray(), itemId); 
        }

        public bool ShowOfferwallDetailFromUrl(string url)
        {
            return client.ShowOfferwallDetailFromUrl(url);
        }
    }
}
