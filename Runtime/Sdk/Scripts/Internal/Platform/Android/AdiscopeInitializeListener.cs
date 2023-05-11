#if UNITY_ANDROID
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    public class AdiscopeInitializeListener : AndroidJavaProxy
    {
        Action<bool> callback;

        public AdiscopeInitializeListener(Action<Boolean> callback) : base("com.nps.adiscope.listener.AdiscopeInitializeListener")
        {
            this.callback = callback;
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        void onInitialized(bool isSuccess)
        {
            Debug.Log("onInitialized : " + isSuccess);
            callback.Invoke(isSuccess);
        }
    }
}
#endif