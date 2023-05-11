/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using Adiscope.Model;
using System;

namespace Adiscope.Internal.Interface
{
    /// <summary>
    /// interface for core client
    /// </summary>
    internal interface ICoreClient
    {
        void Initialize(Action<bool> callback);
        void Initialize(Action<bool> callback, string callbackTag);
        void Initialize(Action<bool> callback, string callbackTag, string childYN);

        void Initialize(string mediaId, string mediaSecret, string callbackTag, Action<bool> callback);
        void Initialize(string mediaId, string mediaSecret, string callbackTag, string childYN, Action<bool> callback);

        bool IsInitialized();
        void SetUserId(string userId);
        void GetUnitStatus(string unitId, Action<AdiscopeError, UnitStatus> callback);
    }
}