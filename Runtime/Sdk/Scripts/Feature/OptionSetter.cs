using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using System;

namespace Adiscope.Feature
{
    /// <summary>
    /// OptionSetter singleton instance class
    /// </summary>
    public class OptionSetter
    {
        private IOptionSetterClient client;

        private static class InitializationOnDemandHolderIdiom
        {
            public static readonly OptionSetter SingletonInstance = new OptionSetter();
        }

        public static OptionSetter Instance
        {
            get
            {
                return InitializationOnDemandHolderIdiom.SingletonInstance;
            }
        }

        private OptionSetter()
        {
            this.client = ClientBuilder.BuildOptionSetterClient();

        }

        /// <summary>
        /// Set whether to use Cloud Front Proxy or not
        /// </summary>
        /// <param name="useCloudFrontProxy">true if you want to use Cloud Front Proxy</param>
        public void SetUseCloudFrontProxy(bool useCloudFrontProxy)
        {
            client.SetUseCloudFrontProxy(useCloudFrontProxy);
        }

        /// <summary>
        /// Set whether user is child, Only Using for Android.
        /// </summary>
        /// <param name="childYN">value whether user is child (This value need for Google Family Policy)</param>
        public void SetChildYN(string childYN)
        {
            client.SetChildYN(childYN);
        }

        /// <summary>
        /// Setup the warning popup Flag in Offerwall. Only Using for iOS.
        /// </summary>
        /// <param name="useOfferwallWarningPopup">if the turn on this flag, Using popup on startup for Offerwall. default flag is true</param>
        public void SetUseOfferwallWarningPopup(bool useOfferwallWarningPopup)
        {
            client.SetUseAppTrackingTransparencyPopup(useOfferwallWarningPopup);
        }

        /// <summary>
        /// Setup the ATT popup Flag in Adiscope. Only Using for iOS.
        /// </summary>
        /// <param name="useAppTrackingTransparencyPopup">if the turn on this flag, Using popup on will start an Initialize. default flag is true</param>
        public void SetUseAppTrackingTransparencyPopup(bool useAppTrackingTransparencyPopup)
        {
            client.SetUseAppTrackingTransparencyPopup(useAppTrackingTransparencyPopup);
        }

        /// <summary>
        /// Setup the Flag: use Jump to Adiscope in Setting App. Only Using for iOS.
        /// </summary>
        /// <param name="enabledForcedOpenApplicationSetting">if the turn on this flag, Showing "OK" Button Message is change to "Move up".. default flag is true</param>
        public void SetEnabledForcedOpenApplicationSetting(bool enabledForcedOpenApplicationSetting)
        {
            client.SetUseAppTrackingTransparencyPopup(enabledForcedOpenApplicationSetting);
        }
    }
}