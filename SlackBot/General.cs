using System;
using System.Collections.Generic;
using System.Threading;
using SlackAPI;
using User = AIMLbot.User;

namespace SlackBot
{
    public static class General
    {
        public static WebSlack ws;
        public static SlackClient sc;
        public static Listener ls;
        public static Storage s;
        public static Thread question;
        public static Dictionary<String,AIMLbot.User> active_users = new Dictionary<string, User>();
        public static SlackBotRunner sbr;
    }
}