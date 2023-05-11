/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
#if UNITY_ANDROID
using Adiscope.Model;
using System;
using UnityEngine;

namespace Adiscope.Internal.Platform.Android
{
    internal class Utils
    {
        public static AdiscopeError ConvertToAdiscopeError(AndroidJavaObject error)
        {
            int code;
            string description;
            string xb3TraceId;

            if (error != null)
            {
                try
                {
                    code = error.Call<int>(Values.MTD_GET_CODE);
                }
                catch (Exception e)
                {
                    Debug.LogError("Android.Utils<ConvertToAdiscopeError> failed to get code: " + e);
                    code = -1;
                }

                try
                {
                    description = error.Call<string>(Values.MTD_GET_DESCRIPTION);
                }
                catch (Exception e)
                {
                    Debug.LogError("Android.Utils<ConvertToAdiscopeError> failed to get description: " + e);
                    description = "parsing error";
                }
                try
                {
                    xb3TraceId = error.Call<string>(Values.MTD_GET_XB3TRACEID);
                }
                catch (Exception e)
                {
                    Debug.LogError("Android.Utils<ConvertToAdiscopeError> failed to get xb3TraceId: " + e);
                    xb3TraceId = "parsing error";
                }
            }
            else
            {
                Debug.LogError("Android.Utils<ConvertToAdiscopeError> object from android null");
                code = -1;
                description = "no error info";
                xb3TraceId = "";
            }

            return new AdiscopeError(code, description, xb3TraceId);
        }

        public static RewardItem ConvertToRewardItem(string unitId, AndroidJavaObject rewardItem)
        {
            string type;
            long amount;

            if (rewardItem != null)
            {
                try
                {
                    type = rewardItem.Call<string>(Values.MTD_GET_TYPE);
                }
                catch (Exception e)
                {
                    Debug.LogError("Android.Utils<ConvertToRewardItem> failed to get type: " + e);
                    type = "unableToParse";
                }

                try
                {
                    amount = rewardItem.Call<long>(Values.MTD_GET_AMOUNT);
                }
                catch (Exception e)
                {
                    Debug.LogError("Android.Utils<ConvertToRewardItem> failed to get amount: " + e);
                    amount = -1;
                }
            }
            else
            {
                Debug.LogError("Android.Utils<ConvertToRewardItem> object from android null");
                type = "nullObject";
                amount = -1;
            }

            return new RewardItem(unitId, type, amount);
        }

        public static UnitStatus ConvertToUnitStatus(AndroidJavaObject unitStatus)
        {
            bool live;
            bool active;

            if (unitStatus != null)
            {
                try
                {
                    live = unitStatus.Call<bool>(Values.MTD_IS_LIVE);
                }
                catch (Exception e)
                {
                    Debug.LogError("Android.Utils<ConvertToUnitStatus> failed to get live: " + e);
                    live = false;
                }

                try
                {
                    active = unitStatus.Call<bool>(Values.MTD_IS_ACTIVE);
                }
                catch (Exception e)
                {
                    Debug.LogError("Android.Utils<ConvertToUnitStatus> failed to get active: " + e);
                    active = false;
                }
            }
            else
            {
                return null;
            }

            return new UnitStatus(live, active);
        }
    }
}
#endif