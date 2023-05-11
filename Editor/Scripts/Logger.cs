/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using System;
using UnityEngine;

namespace Adiscope.Editor
{
    public class Logger
    {
        public static bool verbose = false;

        public static void d(string format, params object[] args)
        {
            if (verbose)
            {
                try
                {
                    Debug.Log(string.Format(format, args));
                }
                catch (Exception e)
                {
                    Debug.LogError("Logger.d failed");
                    Debug.LogError(e);
                }
            }
        }

        public static void i(string format, params object[] args)
        {
            try
            {
                Debug.Log(string.Format(format, args));
            }
            catch (Exception e)
            {
                Debug.LogError("Logger.w failed");
                Debug.LogError(e);
            }
        }

        public static void w(string format, params object[] args)
        {
            if (verbose)
            {
                try
                {
                    Debug.LogWarning(string.Format(format, args));
                }
                catch (Exception e)
                {
                    Debug.LogError("Logger.w failed");
                    Debug.LogError(e);
                }
            }
        }

        public static void w(Exception e)
        {
            if (verbose)
            {
                Debug.LogWarning(e);
            }
        }

        public static void e(string format, params object[] args)
        {
            try
            {
                Debug.LogError(string.Format(format, args));
            }
            catch (Exception e)
            {
                Debug.LogError("Logger.e failed");
                Debug.LogError(e);
            }
        }

        public static void e(Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
