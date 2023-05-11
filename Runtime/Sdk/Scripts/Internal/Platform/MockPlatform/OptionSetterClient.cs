#if (UNITY_EDITOR) || (!UNITY_ANDROID)
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using System.Threading;

namespace Adiscope.Internal.Platform.MockPlatform
{
    /// <summary>
    /// mockup client for option setter
    /// this class will emulate callback very simply, limitedly
    /// </summary>
    internal class OptionSetterClient : IOptionSetterClient
    {

        public OptionSetterClient()
        {
        }

        #region APIs 
        public void SetUseCloudFrontProxy(bool useCloudFrontProxy)
        {
        }

        public void SetChildYN(string childYN)
        {
        }

        public void SetUseOfferwallWarningPopup(bool useOfferwallWarningPopup)
        {
        }

        public void SetUseAppTrackingTransparencyPopup(bool useAppTrackingTransparencyPopup)
        {
        }

        public void SetEnabledForcedOpenApplicationSetting(bool enabledForcedOpenApplicationSetting)
        {
        }
        #endregion
    }
}
#endif