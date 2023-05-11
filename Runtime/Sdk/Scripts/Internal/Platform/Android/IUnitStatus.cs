#if UNITY_ANDROID
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    public class IUnitStatus : AndroidJavaProxy
    {
        Action<AdiscopeError, UnitStatus> callback;

        public IUnitStatus(Action<AdiscopeError, UnitStatus> callback) : base("com.nps.adiscope.model.IUnitStatus")
        {
            this.callback = callback;
        }

        [System.Reflection.Obfuscation(Exclude = true, Feature = "renaming")]
        void onResult(AndroidJavaObject error, AndroidJavaObject unitStatus)
        {
            if (callback == null)
                return;

            AdiscopeError errorResult = null;
            if (error != null)
            {
                errorResult = Utils.ConvertToAdiscopeError(error);
            }

            UnitStatus unitStatusResult = null;
            if (unitStatus != null)
            {
                unitStatusResult = Utils.ConvertToUnitStatus(unitStatus);
            }
            callback.Invoke(errorResult, unitStatusResult);
        }
    }
}
#endif