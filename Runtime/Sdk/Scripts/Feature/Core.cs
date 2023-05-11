/*
 * Created by Hyunchang Kim (mjgu@neowiz.com)
 */
using Adiscope.Internal.Interface;
using Adiscope.Internal.Platform;
using Adiscope.Internal;
using Adiscope.Model;
using System;

namespace Adiscope.Feature
{
    public class Core
    {
        private readonly ICoreClient client;

        private static class ClassWrapper { public static readonly Core instance = new Core(); }

        public static Core Instance { get { return ClassWrapper.instance; } }

        private Core() { this.client = ClientBuilder.BuildCoreClient(); }


        public void Initialize(Action<bool> callback = null)
        {
            this.client.Initialize(callback);
            UnityThread.initUnityThread(true);
        }

        public void Initialize(Action<bool> callback = null, string callbackTag = "")
        {
            this.client.Initialize(callback, callbackTag);
            UnityThread.initUnityThread(true);
        }

        public void Initialize(Action<bool> callback = null, string callbackTag = "", string childYN = "")
        {
            this.client.Initialize(callback, callbackTag, childYN);
            UnityThread.initUnityThread(true);
        }

        public void Initialize(string mediaId, string mediaSecret, string callbackTag = "", Action<bool> callback = null)
        {
            Initialize(mediaId, mediaSecret, callbackTag, "", callback);
        }

        public void Initialize(string mediaId, string mediaSecret, string callbackTag = "", string childYN = "", Action<bool> callback = null)
        {
            this.client.Initialize(mediaId, mediaSecret, callbackTag, childYN, callback);
            UnityThread.initUnityThread(true);
        }

        public bool IsInitialized() { return this.client.IsInitialized(); }

        public void SetUserId(string userId) { this.client.SetUserId(userId); }

        public void GetUnitStatus(string unitId, Action<AdiscopeError, UnitStatus> callback) { this.client.GetUnitStatus(unitId, callback); }
    }
}
