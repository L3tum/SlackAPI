using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace SlackAPI
{
    public class SlackClient
    {
        public Dictionary<String, dynamic> Users = new Dictionary<string, dynamic>();
        public Dictionary<String, Dictionary<string, object>> Channels = new Dictionary<string, Dictionary<string, object>>();
        public String Token;
        public Dictionary<String, dynamic> myself;
        public APICaller caller;
        public String URL;

        #region Constructor
        public SlackClient(String Token)
        {
            caller = new APICaller(Token);
            this.Token = Token;
            Dictionary<String, dynamic> General = new Dictionary<string, dynamic>();
            try
            {
                General = caller.CallMethod("rtm.start", new Dictionary<string, dynamic>()).Result;
                this.myself = General["self"];
                foreach (Dictionary<String, object> VARIABLE in General["channels"])
                {
                    if (VARIABLE != null)
                    {
                        Channels.Add(VARIABLE["name"].ToString(), VARIABLE);
                    }
                }
                foreach (dynamic VARIABLE in General["users"])
                {
                    Users.Add(VARIABLE["name"], VARIABLE);
                }
                URL = General["url"];
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
        #endregion 

        #region SendMessage
        /// <summary>
        /// "Shortcut" for sending a message.
        /// You can either type in the id or the name of the channel.
        /// To get each other from each other, you can use a "Helper" method.
        /// </summary>
        /// <param name="Channel"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public bool SendMessage(String Channel, String Message)
        {
            
            bool returned = false;
            Dictionary<String, dynamic> Params = new Dictionary<string, dynamic>();
            
            if (Channel.StartsWith("C") || Channel.StartsWith("D"))
            {
                Params.Add("channel", Channel);
                Params.Add("text", Message);
                Params.Add("as_user", "true");
                returned = caller.CallMethodPost("chat.postMessage", Params).Result["ok"];
            }
            else
            {
                String ID = Channels[Channel]["id"].ToString();
                Params.Add("channel", ID);
                Params.Add("text", Message);
                Params.Add("as_user", "true");
                returned = caller.CallMethodPost("chat.postMessage", Params).Result["ok"];
            }
            Console.WriteLine("Posted message: " + Message + " to Channel: " + Channel + " with return code: " + returned);
            return returned;
        }
        #endregion

        #region SendMessageCustomToken
        public static bool SendMessageCustomToken(String Channel, String Message, String Token)
        {
            APICaller apic = new APICaller(Token);
            bool returned = false;
            Dictionary<String, dynamic> Params = new Dictionary<string, dynamic>();

            if (Token.Equals("someone"))
            {
                Token = "xoxp-5007212458-11027941589-11025314452-ac4fcf3c3b";
            }
            else if (Token.Equals("timo"))
            {
                Token = "xoxb-5134150563-iZKW7CIodzRbffqVmFmz6m2S";
            }
            else if (Token.Equals("lily"))
            {
                Token = "xoxb-7444634401-UTU2IHZE2kULUWu70hgKV0FA";
            }

            if (Channel.StartsWith("C") || Channel.StartsWith("D"))
            {
                Params.Add("channel", Channel);
                Params.Add("text", Message);
                Params.Add("as_user", "true");
                Params.Add("token", Token);
                returned = apic.CallMethodCustomToken("chat.postMessage", Params).Result["ok"];
            }
            Console.WriteLine("Posted message: " + Message + " to Channel: " + Channel + " with return code: " + returned);
            return returned;
        }
        #endregion 

        #region gets
        public String getUserName(String id)
        {
            foreach (KeyValuePair<string, dynamic> VARIABLE in Users)
            {
                if (VARIABLE.Value["id"] == id)
                {
                    return VARIABLE.Key;
                }
            }
            return "This user is not alive!";
        }

        public String getChannelName(String id)
        {
            foreach (KeyValuePair<string, Dictionary<String, dynamic>> VARIABLE in Channels)
            {
                if (VARIABLE.Value["id"] == id)
                {
                    return VARIABLE.Key;
                }
            }
            return "This channel has no name!";
        }

        public String getUserNameForPost(String id)
        {
            return ("<@" + id + "|" + getUserName(id) + ">");
        }
        #endregion 
    }
}