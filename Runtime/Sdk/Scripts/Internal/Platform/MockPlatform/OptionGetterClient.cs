/*
 * Created by mjgu (mjgu@neowiz.com)
 */
#if (UNITY_EDITOR) || (!UNITY_ANDROID)

using Adiscope.Internal.Interface;

namespace Adiscope.Internal.Platform.MockPlatform
{
	internal class OptionGetterClient : IOptionGetterClient
	{
		public OptionGetterClient ()
		{
		}

		#region APIs 


		public string GetSDKVersion() { return ""; }

		public string GetNetworkVersions() { return ""; }

		#endregion
	}
}

#endif