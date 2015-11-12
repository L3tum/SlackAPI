using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using SlackAPI;

namespace SlackBot
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
        SlackClient GetSlackClient();
    }
    #endregion 

    #region SlackBot
    public class SlackBot : ISlackBot
    {
        public bool CallMethod(string method, Dictionary<string, dynamic> dic)
        {
            if (General.s.commands.ContainsKey(method))
            {
                General.s.commands[method].Invoke(dic);
                return true;
            }
            return false;
        }

        public List<String> GetMethods()
        {
            return General.s.commands.Keys.ToList();
        } 

        public Dictionary<string, dynamic> GetBot()
        {
            return General.sc.myself;
        }

        public bool ExitProcess()
        {
            Program.OnProcessExit(this, EventArgs.Empty);
            return true;
        }

        public SlackClient GetSlackClient()
        {
            return General.sc;
        }
    }
    #endregion 

    #region ServiceHost
    public class SlackBotRunner
    {
        public ServiceHost sh;
        private readonly Uri baseAdress = new Uri("net.pipe://localhost");

        public SlackBotRunner()
        {
            this.sh = new ServiceHost(typeof (SlackBot), baseAdress);
            sh.AddServiceEndpoint(typeof (ISlackBot), new NetNamedPipeBinding(), "SlackPipe" + General.sc.myself["name"]);
            sh.Open();
        }

        public void Terminate()
        {
            sh.Close();
        }
    }
    #endregion 
}
