/*
 * Created by mjgu (mjgu@neowiz.com)
 */
#if UNITY_IOS

using Adiscope.Internal.Interface;
using Adiscope.Model;
using System;
using UnityEngine;

using System.Runtime.InteropServices;
using AOT;

namespace Adiscope.Internal.Platform.IOS {
	internal class OptionGetterClient : IOptionGetterClient {

		public OptionGetterClient () { }

#region APIs 

		[DllImport ("__Internal")]
		private static extern string getNetworkVersions();
		public string GetNetworkVersions() {
			return getNetworkVersions();
		}

		[DllImport ("__Internal")]
		private static extern string getSDKVersion();
		public string GetSDKVersion() {
			return getSDKVersion();
		}

#endregion

	}
}

#endif