using Adiscope.Model;
using System;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for OptionSetter client
    /// </summary>
    internal interface IOptionGetterClient
    {
        string GetNetworkVersions();
        string GetSDKVersion();
    }
}
