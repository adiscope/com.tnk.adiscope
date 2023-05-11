using Adiscope.Model;
using System;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for OptionSetter client
    /// </summary>
    internal interface IOptionSetterClient
    {
        void SetUseCloudFrontProxy(bool useCloudFrontProxy);

        // Only Android
        void SetChildYN(string childYN);

        // Only iOS
        void SetUseOfferwallWarningPopup(bool useOfferwallWarningPopup);
        void SetUseAppTrackingTransparencyPopup(bool useAppTrackingTransparencyPopup);
        void SetEnabledForcedOpenApplicationSetting(bool enabledForcedOpenApplicationSetting);
    }
}
