/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 * Modified by mjgu (mjgu@neowiz.com)
 */
#if (UNITY_EDITOR) || (!UNITY_ANDROID)
using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using System.Threading;

namespace Adiscope.Internal.Platform.MockPlatform
{
    /// <summary>
    /// mockup client for adiscope core
    /// this class will do nothing
    /// </summary>
    internal class CoreClient : ICoreClient
    {
        public event EventHandler<InitResult> OnInitialized;
        public event EventHandler<InitResult> OnInitializedBackground;

        public static CoreClient Instance;

        public CoreClient()
        {
            Instance = this;
        }

        private void DelayedCallback(Action action, int delay)
        {
            Thread.Sleep(delay);
            action.Invoke();
        }

        public void Initialize(string media_id, string app_id, string callbackTag, Action<bool> callback)
        {
            new Thread(() => DelayedCallback(
                () => {

                    if (OnInitialized != null)
                    {
                        UnityThread.executeInMainThread(() =>
                        {
                            OnInitialized(this, new InitResult(true));
                        });
                    }

                    if (OnInitializedBackground != null)
                    {
                        OnInitializedBackground(this, new InitResult(true));
                    }

                }, 10)).Start();
        }


        public void Initialize(string mediaId, string mediaSecret, string callbackTag, string childYN, Action<bool> callback)
        {
            new Thread(() => DelayedCallback(
               () => {

                   if (OnInitialized != null)
                   {
                       UnityThread.executeInMainThread(() =>
                       {
                           OnInitialized(this, new InitResult(true));
                       });
                   }

                   if (OnInitializedBackground != null)
                   {
                       OnInitializedBackground(this, new InitResult(true));
                   }

               }, 10)).Start();
        }

        public bool IsInitialized()
        {
            return true;
        }

        public void SetUserId(string userId)
        {
        }

        public void GetUnitStatus(string unitId, Action<AdiscopeError, UnitStatus> callback)
        {
            Thread thd = new Thread(() =>
            {
                Thread.Sleep(100);
                callback.Invoke(null, new UnitStatus(true, true));
            });
            thd.Start();
        }

        public void Initialize(Action<bool> callback)
        {
            new Thread(() => DelayedCallback(() =>
            {

                if (OnInitialized != null)
                {
                    UnityThread.executeInMainThread(() =>
                    {
                        OnInitialized(this, new InitResult(true));
                    });
                }

                if (OnInitializedBackground != null)
                {
                    OnInitializedBackground(this, new InitResult(true));
                }

            }, 10)).Start();
        }

        public void Initialize(Action<bool> callback, string callbackTag)
        {
            new Thread(() => DelayedCallback(() => {

                if (OnInitialized != null)
                {
                    UnityThread.executeInMainThread(() =>
                    {
                        OnInitialized(this, new InitResult(true));
                    });
                }

                if (OnInitializedBackground != null)
                {
                    OnInitializedBackground(this, new InitResult(true));
                }

            }, 10)).Start();
        }

        public void Initialize(Action<bool> callback, string callbackTag, string childYN)
        {
            new Thread(() => DelayedCallback(() =>
            {

                if (OnInitialized != null)
                {
                    UnityThread.executeInMainThread(() =>
                    {
                        OnInitialized(this, new InitResult(true));
                    });
                }

                if (OnInitializedBackground != null)
                {
                    OnInitializedBackground(this, new InitResult(true));
                }

            }, 10)).Start();
        }
    }
}
#endif