#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
using StackExchange.Redis;
#endregion usings

namespace vvvv_Redis
{
	#region PluginInfo
	[PluginInfo(Name = "Client", Category = "Redis")]
	#endregion PluginInfo
	public class RedisClientNodes : IPluginEvaluate
	{
		#region fields & pins
		[Input("Host", DefaultValue = 1.0)]
		public ISpread<string> FHost;

        [Input("Connect", IsBang = true, DefaultValue = 0)]
        public IDiffSpread<bool> FConnect;

        [Input("Disconnect", IsBang = true, DefaultValue = 0)]
        public IDiffSpread<bool> FDisconnect;

        [Output("Client")]
		public ISpread<Client> FClient;

        [Output("Status")]
        public ISpread<string> FStatus;

        [Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FClient.SliceCount = FHost.SliceCount;

            for (int i = 0; i < FHost.SliceCount; i++)
            {
                if (FConnect[i])
                {
                    FClient[i] = new Client(FHost[i]);
                    FClient[i].Connect();
                }

                if (FDisconnect[i])
                {
                    FClient[i].Disconnect();
                }
            }


			for (int i = 0; i < FClient.SliceCount; i++)
            {
                if (FClient[i] == null)
                {
                    FStatus[i] = "";
                } else
                {
                    FStatus[i] = FClient[i].GetStatus();
                }
                
            }
				
		}
	}

    public class Client
    {
        public string host;
        public ConnectionMultiplexer redis;
        public Client(string h)
        {
            host = h;
        }

        public void Connect()
        {
            redis = ConnectionMultiplexer.Connect(host);
        }

        public void Disconnect()
        {
            redis.Close();
        }

        public string GetStatus()
        {
            string status = null;

            if (redis == null)
            {
                status = "null";
            } else if (redis.IsConnected)
            {
                status = "CONNECTED";
            } else
            {
                status = "DISCONNECTED";
            }
            return status;
        }
    }
}
