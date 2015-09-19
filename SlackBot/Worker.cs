using System;
using System.Collections.Generic;
using System.IO;
using SlackAPI;

namespace SlackBot
{
    static class Worker
    {
        public static void MessageWorker(String Message)
        {
            Dictionary<String, dynamic> myDic = Message.ToDictionary();
            TextWriter sw = new StreamWriter("C:/Users/Tom Niklas/Desktop/hahahahahahahax.txt");
            sw.Write(myDic);
            sw.Close();
            if (myDic.ContainsKey("text"))
            {
                if (((String)myDic["text"]).Contains("Hello") || ((String)myDic["text"]).Contains("Hi"))
                {
                    General.sc.SendMessage(myDic["channel"],
                        "Hello @" + General.sc.getUserName(myDic["user"]));
                }
                if (myDic.ContainsKey("user"))
                {
                    if (myDic["user"].Equals(General.sc.Users["theguysyoudespise"]))
                    {
                        General.sc.SendMessage(myDic["channel"],
                            "What do you think about me @" + General.sc.getUserName(myDic["user"]) + " ?");
                    }
                }
            }
        }
    }
}
