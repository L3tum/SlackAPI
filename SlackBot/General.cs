using System;
using System.Collections.Generic;
using System.Threading;
using SlackAPI;

namespace SlackBot
{
    public static class General
    {
        public static WebSlack ws;
        public static SlackClient sc;
        public static Listener ls;
        public static Storage s;
        public static int methodCount = 30;
        public delegate void delToCall(System.Collections.Generic.Dictionary<String, dynamic> myDic);
        public static Dictionary<String, delToCall> commands = new Dictionary<string, delToCall>();
        public static Thread question;
    }
}