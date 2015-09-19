using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using SlackAPI;

namespace SlackBot
{
    class Program
    {
        static void Main(string[] args)
        {
            SlackClient sc = new SlackClient("xoxb-5134150563-iZKW7CIodzRbffqVmFmz6m2S");
            Console.WriteLine(sc.SendMessage("bot", "HELLO"));
            General.sc = sc;

            WebSlack ws = new WebSlack();
            ws.CreateWebSocket(sc.URL);
            General.ws = ws;

            Listener ls = new Listener(General.ws);
            Thread question = new Thread(ls.Listen);
            question.Start();

            General.sc = sc;
            General.ws = ws;
            General.ls = ls;
        }
    }
}
