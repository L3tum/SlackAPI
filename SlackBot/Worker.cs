using System;
using System.Collections.Generic;
using System.Reflection;

namespace SlackBot
{
    public class Worker
    {
        public bool listenToItself;
        public bool multiple;

        public void MessageWorker(object Message)
        {
            Dictionary<String, dynamic> myDic = (Dictionary<String, dynamic>)Message;
            bool answered = false;
            if (myDic.ContainsKey("text"))
            {
                String ttxt = myDic["text"].ToString();

                #region commands

                if (!listenToItself)
                {
                    if (ttxt.StartsWith("*") && !((String)myDic["user"]).Equals(General.sc.myself["id"]))
                    {
                        foreach (KeyValuePair<string, Storage.delToCall> command in General.s.commands)
                        {
                            if (!multiple)
                            {
                                if (ttxt.Contains(("*" + command.Key)))
                                {
                                    try
                                    {
                                        ((Storage.delToCall) command.Value).Invoke(myDic);
                                        answered = true;
                                        //Console.WriteLine("Called!");
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                        General.sc.SendMessage(myDic["channel"],
                                            "Oops, something went wrong! Check if you entered the command correctly with '*help:command'.\nIf you've entered '*mirror' your word could have been too long.");
                                    }
                                }
                            }
                            else
                            {
                                if (ttxt.Contains((General.sc.myself["name"] + "*" + command.Key)))
                                {
                                    try
                                    {
                                        myDic["text"] =((String) ((String) myDic["text"]).Replace(General.sc.myself["name"], "")).Trim();
                                        ((Storage.delToCall)command.Value).Invoke(myDic);
                                        answered = true;
                                        //Console.WriteLine("Called!");
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                        General.sc.SendMessage(myDic["channel"],
                                            "Oops, something went wrong! Check if you entered the command correctly with '*help:command'.\nIf you've entered '*mirror' your word could have been too long.");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (ttxt.StartsWith("*"))
                    {
                        foreach (KeyValuePair<string, Storage.delToCall> command in General.s.commands)
                        {
                            if (ttxt.Contains(("*" + command.Key)))
                            {
                                try
                                {
                                    ((Storage.delToCall)command.Value).Invoke(myDic);
                                    answered = true;
                                    //Console.WriteLine("Called!");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                    General.sc.SendMessage(myDic["channel"],
                                        "Oops, something went wrong! Check if you entered the command correctly with '*help:command'.\nIf you've entered '*mirror' your word could have been too long.");
                                }
                            }
                        }
                    }
                }

                #endregion

                #region afkNotifier

#if AFK || ALL
                if (ttxt.Contains("@"))
                {
                    foreach (var afkUser in General.s.afkUsers)
                    {
                        if (ttxt.Contains(afkUser) && (((String)General.sc.getUserName(myDic["user"])).Equals(General.sc.myself["name"])))
                        {
                            if (General.sc.getChannelName(myDic["channel"]) != "general")
                            {
                                General.sc.SendMessage(myDic["channel"],
                                    General.sc.getUserNameForPost(myDic["user"]) + ": The user you just tagged is afk!");
                            }

                            if (!General.s.privateChannels.ContainsKey(afkUser))
                            {
                                Dictionary<String, dynamic> paramse = new Dictionary<string, dynamic>();
                                paramse.Add("user", afkUser);
                                Dictionary<String, dynamic> something =
                                    General.sc.caller.CallMethodPost("im.open", paramse).Result;
                                General.s.privateChannels.Add(afkUser, something["channel"]);
                            }
                            if (!General.s.privateChannels.ContainsKey(myDic["user"]))
                            {
                                Dictionary<String, dynamic> paramse = new Dictionary<string, dynamic>();
                                paramse.Add("user", myDic["user"]);
                                Dictionary<String, dynamic> something =
                                    General.sc.caller.CallMethodPost("im.open", paramse).Result;
                                General.s.privateChannels.Add(myDic["user"], something["channel"]);
                            }
                            General.sc.SendMessage(
                                (String) General.s.privateChannels[afkUser]["id"],
                                "You have been tagged by: " + General.sc.getUserNameForPost(myDic["user"]) + " .");
                            General.sc.SendMessage(
                                (String) General.s.privateChannels[afkUser]["id"],
                                "Message: " + ttxt);
                            General.sc.SendMessage((String) General.s.privateChannels[myDic["user"]]["id"],
                                "The user you just tagged is afk!");
                            answered = true;
                        }
                    }
                }
#endif

                #endregion

                #region spamPreventer

#if SPAM || ALL
                if (myDic.ContainsKey("user"))
                {
                    if (General.s.lastMessager.ContainsKey(myDic["channel"]))
                    {
                        if (myDic["user"].Equals(General.s.lastMessager[myDic["channel"]]))
                        {
                            if (!(ttxt.ToLower().Equals("sorry")) &&
                                !(ttxt.ToLower().Equals("sry")) &&
                                !(ttxt.ToLower().Equals("ok")) && (ttxt.Length <= 5) &&
                                !(((String) myDic["channel"]).StartsWith("D")))
                            {
                                General.s.spamCounter[myDic["channel"]]++;
                                if (General.s.spamCounter[myDic["channel"]] >= 5)
                                {
                                    if (!General.s.privateChannels.ContainsKey(General.sc.getUserName(myDic["user"])))
                                    {
                                        Dictionary<String, dynamic> paramse = new Dictionary<string, dynamic>();
                                        paramse.Add("user", myDic["user"]);
                                        Dictionary<String, dynamic> something =
                                            General.sc.caller.CallMethodPost("im.open", paramse).Result;
                                        General.s.privateChannels.Add(General.sc.getUserName(myDic["user"]),
                                            something["channel"]);
                                    }

                                    General.sc.SendMessage(
                                        General.s.privateChannels[General.sc.getUserName(myDic["user"])]["id"],
                                        "PLEASE STOP SPAMMING IN " + General.sc.getChannelName(myDic["channel"]));
                                    Console.WriteLine(General.sc.getUserName(myDic["user"]) + " has spammed!");
                                }
                                if (General.s.spamCounter[myDic["channel"]] == 5)
                                {
                                    General.sc.SendMessage(myDic["channel"],
                                        General.sc.getUserNameForPost(myDic["user"]) + ": Please stop spamming!");
                                }
                            }
                            else
                            {
                                General.s.spamCounter[myDic["channel"]] = 0;
                            }
                        }
                        else
                        {
                            General.s.lastMessager[myDic["channel"]] = myDic["user"];
                            General.s.spamCounter[myDic["channel"]] = 1;
                        }
                    }
                    else
                    {
                        General.s.lastMessager.Add(myDic["channel"], myDic["user"]);
                        General.s.spamCounter.Add(myDic["channel"], 1);
                    }
                }
#endif

                #endregion

                #region Responses

                try
                {
                    if (!listenToItself)
                    {
                        if (((String)myDic["user"]).Equals(General.sc.myself["id"]) && !answered)
                        {
                            if (((String) myDic["channel"]).StartsWith("D"))
                            {
                                if (!General.s.privateChannels.ContainsKey(General.sc.Users["letum"]["id"]))
                                {
                                    Dictionary<String, dynamic> paramse = new Dictionary<string, dynamic>();
                                    paramse.Add("user", General.sc.Users["letum"]["id"]);
                                    Dictionary<String, dynamic> something =
                                        General.sc.caller.CallMethodPost("im.open", paramse).Result;
                                    General.s.privateChannels.Add(General.sc.Users["letum"]["id"], something["channel"]);
                                }
                                General.sc.SendMessage(
                                    ((String) General.s.privateChannels[General.sc.Users["letum"]["id"]]["id"]),
                                    myDic["text"] + " by " + General.sc.getUserName(myDic["user"]));
                            }
                            if (General.s.sternchenResponse)
                            {
                                if (ttxt.StartsWith("*"))
                                {
                                    bool found = false;
                                    foreach (KeyValuePair<string, List<string>> keyValuePair in General.s.responses)
                                    {
                                        if ((ttxt.ToLower()).StartsWith(("*" + keyValuePair.Key.ToLower())))
                                        {
                                            Random rand = new Random();
                                            int dis = rand.Next(0, keyValuePair.Value.Count);
                                            General.sc.SendMessage(myDic["channel"], keyValuePair.Value[dis]);
                                            Console.WriteLine("Message: " + keyValuePair.Value[dis] +
                                                              " sent, because User: " +
                                                              General.sc.getUserName(myDic["user"]) + " entered: " +
                                                              keyValuePair.Key + " with random value: " + dis);
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        foreach (KeyValuePair<string, List<Eval>> keyValuePair in General.s.evals)
                                        {
                                            if ((ttxt.ToLower()).StartsWith(("*" + keyValuePair.Key.ToLower())))
                                            {
                                                String[] tmp = ttxt.Split(':');
                                                Random rand = new Random();
                                                int dis = rand.Next(0, keyValuePair.Value.Count);
                                                MethodInfo mi =
                                                    Assembly.LoadFrom(keyValuePair.Value[dis].path)
                                                        .GetType("MyNamespace.MyProgram")
                                                        .GetMethod("MyMethod");
                                                String result =
                                                    (String) mi.Invoke(null, new object[] {((String) myDic["text"])});
                                                if (keyValuePair.Value[dis].printToChannel)
                                                {
                                                    General.sc.SendMessage(myDic["channel"], result);
                                                }
                                                found = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                bool found = false;
                                foreach (KeyValuePair<string, List<string>> keyValuePair in General.s.responses)
                                {
                                    if ((ttxt.ToLower()).StartsWith((keyValuePair.Key.ToLower())))
                                    {
                                        Random rand = new Random();
                                        int dis = rand.Next(0, keyValuePair.Value.Count);
                                        General.sc.SendMessage(myDic["channel"], keyValuePair.Value[dis]);
                                        Console.WriteLine("Message: " + keyValuePair.Value[dis] +
                                                          " sent, because User: " +
                                                          General.sc.getUserName(myDic["user"]) + " entered: " +
                                                          keyValuePair.Key + " with random value: " + dis);
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!answered)
                        {
                            if (((String)myDic["channel"]).StartsWith("D"))
                            {
                                if (!General.s.privateChannels.ContainsKey(General.sc.Users["letum"]["id"]))
                                {
                                    Dictionary<String, dynamic> paramse = new Dictionary<string, dynamic>();
                                    paramse.Add("user", General.sc.Users["letum"]["id"]);
                                    Dictionary<String, dynamic> something =
                                        General.sc.caller.CallMethodPost("im.open", paramse).Result;
                                    General.s.privateChannels.Add(General.sc.Users["letum"]["id"], something["channel"]);
                                }
                                General.sc.SendMessage(
                                    ((String)General.s.privateChannels[General.sc.Users["letum"]["id"]]["id"]),
                                    myDic["text"] + " by " + General.sc.getUserName(myDic["user"]));
                            }
                            if (General.s.sternchenResponse)
                            {
                                if (ttxt.StartsWith("*"))
                                {
                                    bool found = false;
                                    foreach (KeyValuePair<string, List<string>> keyValuePair in General.s.responses)
                                    {
                                        if ((ttxt.ToLower()).StartsWith(("*" + keyValuePair.Key.ToLower())))
                                        {
                                            Random rand = new Random();
                                            int dis = rand.Next(0, keyValuePair.Value.Count);
                                            General.sc.SendMessage(myDic["channel"], keyValuePair.Value[dis]);
                                            Console.WriteLine("Message: " + keyValuePair.Value[dis] +
                                                              " sent, because User: " +
                                                              General.sc.getUserName(myDic["user"]) + " entered: " +
                                                              keyValuePair.Key + " with random value: " + dis);
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        foreach (KeyValuePair<string, List<Eval>> keyValuePair in General.s.evals)
                                        {
                                            if ((ttxt.ToLower()).StartsWith(("*" + keyValuePair.Key.ToLower())))
                                            {
                                                String[] tmp = ttxt.Split(':');
                                                Random rand = new Random();
                                                int dis = rand.Next(0, keyValuePair.Value.Count);
                                                MethodInfo mi =
                                                    Assembly.LoadFrom(keyValuePair.Value[dis].path)
                                                        .GetType("MyNamespace.MyProgram")
                                                        .GetMethod("MyMethod");
                                                String result =
                                                    (String)mi.Invoke(null, new object[] { ((String)myDic["text"]) });
                                                if (keyValuePair.Value[dis].printToChannel)
                                                {
                                                    General.sc.SendMessage(myDic["channel"], result);
                                                }
                                                found = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                bool found = false;
                                foreach (KeyValuePair<string, List<string>> keyValuePair in General.s.responses)
                                {
                                    if ((ttxt.ToLower()).StartsWith((keyValuePair.Key.ToLower())))
                                    {
                                        Random rand = new Random();
                                        int dis = rand.Next(0, keyValuePair.Value.Count);
                                        General.sc.SendMessage(myDic["channel"], keyValuePair.Value[dis]);
                                        Console.WriteLine("Message: " + keyValuePair.Value[dis] + " sent, because User: " +
                                                          General.sc.getUserName(myDic["user"]) + " entered: " +
                                                          keyValuePair.Key + " with random value: " + dis);
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    
                }

                #endregion
            }
        } 
    }
}