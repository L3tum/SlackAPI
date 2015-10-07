using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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


                            /*
                            String text = "";
                            foreach (KeyValuePair<string, dynamic> keyValuePair in myDic)
                            {
                                text += keyValuePair.Key + ":";
                                if (keyValuePair.Value is Dictionary<String, object>)
                                {
                                    foreach (KeyValuePair<String, object> o in keyValuePair.Value)
                                    {
                                        text += o.Key + ":" + o.Value + "\n";
                                    }
                                }
                                else
                                {
                                    text += keyValuePair.Value + "\n";
                                }
                             * */
                            //}
                        /*
                            if (myDic.ContainsKey("file"))
                            {
                                TextWriter tw = new StreamWriter("C:/Users/Tom Niklas/Desktop/jadasjdbajh.txt");
                                tw.Write(text);
                                tw.Close();
                            }
                           Console.WriteLine(text);
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