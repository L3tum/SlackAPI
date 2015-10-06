using System;
using System.Collections.Generic;
using System.Globalization;

namespace SlackBot
{
    public class Poll
    {
        public String name;
        public String desc;
        public DateTime dt;
        public String creatorID;
        public Dictionary<String, int> votes = new Dictionary<string, int>();
        public List<String> usersAlreadyVotedID = new List<string>();
        public bool isRunning = true;

        public Poll(String name, String desc, String dt, String creatorID, String[] things)
        {
            this.name = name;
            this.desc = desc;
            this.creatorID = creatorID;
            foreach (string thing in things)
            {
                votes.Add(thing, 0);
            }
            try
            {
                this.dt = DateTimeOffset.Parse(dt, null).DateTime;
            }
            catch
            {
                
            }
        }

        public Poll()
        {
            
        }
    }
}
