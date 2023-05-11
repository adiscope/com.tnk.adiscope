using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using System;

namespace Adiscope.Feature
{
    /// <summary>
    /// OptionSetter singleton instance class
    /// </summary>
    public class OptionGetter
    {
        private IOptionGetterClient client;

        private static class WrapperClass { public static readonly OptionGetter instance = new OptionGetter();}

        public static OptionGetter Instance { get { return WrapperClass.instance; } }

        private OptionGetter()
        {
            this.client = ClientBuilder.BuildOptionGetterClient();
        }

        public string GetSDKVersion() { return client.GetSDKVersion(); }

        public string GetNetworkVersions() { return client.GetNetworkVersions(); }
    }
}