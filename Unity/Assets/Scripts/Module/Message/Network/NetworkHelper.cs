using System.Net;

namespace ETModel
{
	public static class NetworkHelper
	{
		public static IPEndPoint ToIPEndPoint(string host, int port)
		{
			return new IPEndPoint(IPAddress.Parse(host), port);
		}

	    public static IPEndPoint ToIPEndPoint(IPAddress host, int port)
		{
			return new IPEndPoint(host, port);
		}

		public static IPEndPoint ToIPEndPoint(string address)
		{
			int index = address.LastIndexOf(':');
			string host = address.Substring(0, index);
			string p = address.Substring(index + 1);
			int port = int.Parse(p);
			return ToIPEndPoint(host, port);
		}

	    public static IPEndPoint ToIPEndPointWithYuMing()
	    {
	        string serverUrl = NetConfig.getInstance().getServerUrl();
	        int serverPort = NetConfig.getInstance().getServerPort();

	        Log.Debug("serverUrl:" + serverUrl);
	        Log.Debug("serverPort:" + serverPort);
	        IPAddress ip;   
	        IPHostEntry IPinfo = Dns.GetHostEntry(serverUrl);
	        if (IPinfo.AddressList.Length <= 0)
	        {
	            ToastScript.createToast("域名解析出错");
	            return null;
	        }
	        ip = IPinfo.AddressList[0];

	        return ToIPEndPoint(ip,serverPort);
	    }
    }
}
