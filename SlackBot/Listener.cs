using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading;
using SlackAPI;
using User = AIMLbot.User;

namespace SlackBot
{
    public class Listener
    {
        public bool should_listen = true;
        public WebSlack ws;
        public Dictionary<String, dynamic> myDic = new Dictionary<string, dynamic>();
        public bool finished;
        public bool activated = true;
        public Worker w;
        private readonly ParameterizedThreadStart pst;

        public Listener(WebSlack ws)
        {
            this.ws = ws;
            w = new Worker();
            pst = w.MessageWorker;
        }

        public void Listen()
        {
            while (should_listen)
            {
                finished = false;
                if (ws.changed)
                {
                    myDic = ws.Response.ToDictionary();
                    if (myDic != null)
                    {
                        if (activated)
                        {
                            Thread myThread = new Thread(pst);
                            myThread.Start(myDic);
                            ws.changed = false;
                            if (myDic.ContainsKey("text"))
                            {
                                foreach (KeyValuePair<string, User> keyValuePair in General.active_users)
                                {
                                    if (((String) myDic["text"]).StartsWith(keyValuePair.Key))
                                    {
                                        General.sc.SendMessage(myDic["channel"],
                                            keyValuePair.Value.bot.Chat(
                                                ((((String) myDic["text"]).Replace(keyValuePair.Key + ":", "")).Trim()),
                                                keyValuePair.Value.UserID).Output);
                                    }
                                }
                                if (((String) myDic["text"]).Equals(("*deactivate:" + General.sc.myself["name"])))
                                {
                                    activated = false;
                                    General.sc.SendMessage(myDic["channel"], "Listening deactivated!");
                                }
                            }
                        }
                        else if (myDic.ContainsKey("text") &&
                                 ((String) myDic["text"]).Equals(("*activate:" + General.sc.myself["name"])))
                        {
                            activated = true;
                            General.sc.SendMessage(myDic["channel"], "Listening activated!");
                        }
                    }
                    ws.changed = false;
                }
                finished = true;
            }
        }
    }
}