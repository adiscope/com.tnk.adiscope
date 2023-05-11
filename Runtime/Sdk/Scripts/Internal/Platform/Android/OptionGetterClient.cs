/*
 * Created by mjgu (mjgu@neowiz.com)
 */
#if UNITY_ANDROID

using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

using System.Runtime.InteropServices;
using AOT;

namespace Adiscope.Internal.Platform.Android
{
	internal class OptionGetterClient : IOptionGetterClient
	{
		public OptionGetterClient() { }

		#region APIs 

		public string GetNetworkVersions()
		{
			using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
			{
				if (jc == null)
				{
					Debug.LogError("Android.CoreClient<Initialize> " +
						Values.PKG_ADISCOPE + ": null");
					return "";
				}
				return jc.CallStatic<string>(Values.MTD_GET_NETWORK_VERSIONS);
			}
		}

		private static string getSDKVersion() 
		{
			using (AndroidJavaClass jc = new AndroidJavaClass(Values.PKG_ADISCOPE))
			{
				if (jc == null)
				{
					Debug.LogError("Android.CoreClient<Initialize> " +
						Values.PKG_ADISCOPE + ": null");
					return "";
				}
				return jc.CallStatic<string>(Values.MTD_GET_SDK_VERSION);
			}
		}

		public string GetSDKVersion() 
		{
			return getSDKVersion();
		}

		#endregion
	}
}

#endif