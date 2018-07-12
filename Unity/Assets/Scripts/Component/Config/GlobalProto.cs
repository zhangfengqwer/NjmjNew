namespace ETModel
{
	public class GlobalProto
	{
		public string AssetBundleServerUrl;
		public string Address;

		public string GetUrl()
		{
		    string url = NetConfig.getInstance().getWebUrl() + "/AssetBundle/" + PlatformHelper.GetVersionName() + "/";
#if UNITY_ANDROID
            url += "Android/";
#elif UNITY_IOS
			url += "IOS/";
#elif UNITY_WEBGL
			url += "WebGL/";
#else
			url += "PC/";
#endif

			return url;
		}
	}
}
