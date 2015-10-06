using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading;
using SlackAPI;

namespace SlackBot
{
    public class Listener
    {
        public bool should_listen = true;
        public WebSlack ws;
        public Dictionary<String, dynamic> myDic = new Dictionary<string, dynamic>();
        public bool finished;

        public Listener(WebSlack ws)
        {
            this.ws = ws;
        }

        public void Listen()
        {
            try
            {
                while (should_listen)
                {
                    finished = false;
                    if (ws.changed)
                    {
                        myDic = ws.Response.ToDictionary();
                        if (myDic != null)
                        {
                            /*
                        if (myDic.ContainsKey("text") && myDic.ContainsKey("user") && (General.sc.getUserName(myDic["user"]) != "someone"))
                        {
                            String text = myDic["text"];
                            String user = "Name: " + General.sc.getUserName(myDic["user"]) + ", ID: " + myDic["user"];
                            String channel = "Name: " + General.sc.getChannelName(myDic["channel"]) + ", ID: " +
                                             myDic["channel"];
                            Console.WriteLine("Text: " + text);
                            Console.WriteLine("User: " + user);
                            Console.WriteLine("Channel: " + channel);
                        }
                         * */
                            Worker w = new Worker();
                            ParameterizedThreadStart pst = w.MessageWorker;
                            Thread myThread = new Thread(pst);
                            myThread.Start(myDic);
                        }
                        ws.changed = false;
                    }
                    finished = true;
                }
            }
            catch
            {
                
            }
        }
    }
}