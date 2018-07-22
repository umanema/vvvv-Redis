#region usings
using System;
using System.Collections.Generic;
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
    [PluginInfo(Name = "Key", Category = "Redis")]
    #endregion PluginInfo
    public class RedisSendNodes : IPluginEvaluate
    {
        #region fields & pins
        [Input("Client")]
        public IDiffSpread<Client> FClient;

        [Input("Key", DefaultString = "")]
        public ISpread<string> FKey;

        [Input("String", DefaultString = "")]
        public ISpread<string> FStringIn;

        [Input("Set", IsBang = true, DefaultValue = 0)]
        public IDiffSpread<bool> FSet;

        [Input("Get", IsBang = true, DefaultValue = 0)]
        public IDiffSpread<bool> FGet;

        [Output("String")]
        public ISpread<string> FStringOut;

        [Import()]
        public ILogger FLogger;
        #endregion fields & pins
    	
        //called when data for any output pin is requested
        public void Evaluate(int SpreadMax)
        {
            FStringOut.SliceCount = FClient.SliceCount;
        	
            for (int i = 0; i < FStringOut.SliceCount; i++)
            {
                if (FSet[i])
                {
                	if (FClient[i] == null)
                	{
                		FLogger.Log(LogType.Debug, "Client is null");
                	} else
                	{
                		IDatabase db =  FClient[i].redis.GetDatabase();
                    	db.StringSet(FKey[i], FStringIn[i]);
                	}
                }

                if (FGet[i])
                {
                    IDatabase db = FClient[i].redis.GetDatabase(0);
                    FStringOut[i] = db.StringGet(FKey[i]);
                }
            }
        }
    }
}
