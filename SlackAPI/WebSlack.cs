using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using WebSocket4Net;

namespace SlackAPI
{
    public class WebSlack
    {
        public WebSocket ws;
        public bool changed = false;
        public String Response = "";

        public void CreateWebSocket(String url)
        {
            ws = new WebSocket(url);
            ws.Opened += (sender, args) => Console.WriteLine("Opened..." + sender.ToString() + "," + args.ToString());
            ws.Closed += (sender, args) => Console.WriteLine("Closed..." + sender.ToString() + "," + args.ToString());
            ws.MessageReceived += WsOnMessageReceived;
            ws.Open();
        }

        private void WsOnMessageReceived(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            changed = true;
            Response = messageReceivedEventArgs.Message;
        }
    }
}