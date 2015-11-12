using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SlackGUI
{
    #region ISlackBot
    [ServiceContract]
    public interface ISlackBot
    {
        [OperationContract]
        bool CallMethod(String method, Dictionary<String, dynamic> dic);

        [OperationContract]
        List<String> GetMethods();
        
        [OperationContract]
        Dictionary<String, dynamic> GetBot();

        [OperationContract]
        bool ExitProcess();

        [OperationContract]
        SlackAPI.SlackClient GetSlackClient();
    }
    #endregion

    #region SBC

    public class SlackBotConnector
    {
        public EndpointAddress adress;
        public ISlackBot client;

        public SlackBotConnector(String name)
        {
            adress = new EndpointAddress("net.pipe://localhost/SlackPipe" + name);
            ChannelFactory<ISlackBot> factory = new ChannelFactory<ISlackBot>(new NetNamedPipeBinding(), adress);
            client = factory.CreateChannel();
        }
    }

    #endregion
}
