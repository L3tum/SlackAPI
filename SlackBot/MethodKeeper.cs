using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using SlackAPI;
using WebSocketSharp;

namespace SlackBot
{
    public class MethodKeeper
    {

        //*addResponse Trigger|Reply
        public void addResponse(Dictionary<String, dynamic> myDic)
        {
            String tmp = myDic["text"];
            tmp = tmp.Replace("*addResponse", "");
            tmp = tmp.Trim();
            if (tmp.StartsWith("*"))
            {
                tmp = tmp.Remove(0, 1);
            }
            tmp = tmp.Trim();
            String[] tmps = tmp.Split('|');
            tmps[0] = tmps[0].Trim();
            tmps[1] = tmps[1].Trim();
            if (General.s.responses.ContainsKey(tmps[0]))
            {
                General.s.responses[tmps[0]].Add(tmps[1]);
            }
            else
            {
                General.s.responses.Add(tmps[0], new List<string>() {tmps[1]});
            }
            Console.WriteLine("Response: " + tmps[0] + "," + tmps[1] + " added!");
            General.sc.SendMessage(myDic["channel"],
                General.sc.getUserNameForPost(myDic["user"]) + ": Response: " + tmps[0] + "," + tmps[1] +
                " added!");
        }

        //*removeResponse:Response
        public void removeResponse(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission.ContainsKey(myDic["user"]))
            {
                if (General.s.permission[myDic["user"]] >= 2)
                {
                    String responseToRemove =
                        (((((String) myDic["text"])).Split(':')[1])).Trim();
                    if (General.s.responses.ContainsKey(responseToRemove))
                    {
                        General.s.responses.Remove(responseToRemove);
                        Console.WriteLine(responseToRemove + " removed!");
                        General.sc.SendMessage(myDic["channel"],
                            General.sc.getUserNameForPost(myDic["user"]) + ": Response removed!");
                    }
                    else
                    {
                        General.sc.SendMessage(myDic["channel"], "No valid Response!");
                        Console.WriteLine("User: " + General.sc.getUserName(myDic["user"]) +
                                          " tried to remove the non-valid response: " + responseToRemove);
                    }
                }
                else
                {
                    General.sc.SendMessage(myDic["channel"],
                        General.sc.getUserNameForPost(myDic["user"]) +
                        ": You don't have permission to use that command!")
                        ;
                }
            }
            else
            {
                General.s.permission.Add(myDic["user"], 0);
            }
        }

        //*switchResponse
        public void switchResponse(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission.ContainsKey(myDic["user"]))
            {
                if (General.s.permission[myDic["user"]] >= 2)
                {
                    try
                    {
                        General.s.sternchenResponse = !General.s.sternchenResponse;
                        Console.WriteLine("Switching sternchen to: " + General.s.sternchenResponse);
                        General.sc.SendMessage(myDic["channel"],
                            General.sc.getUserNameForPost(myDic["user"]) +
                            " has set the need to enter '*' for responses to: " +
                            General.s.sternchenResponse);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        General.sc.SendMessage(myDic["channel"],
                            General.sc.getUserNameForPost(myDic["user"]) +
                            ": Command not entered correctly!");
                    }
                }
                else
                {
                    Console.WriteLine(General.sc.getUserName(myDic["user"]) +
                                      " has insufficient permissions for '*switchResponse'");
                    General.sc.SendMessage(myDic["channel"],
                        General.sc.getUserName(myDic["user"]) +
                        " has insufficient permissions for '*switchResponse'");
                }
            }


            else
            {
                Console.WriteLine(General.sc.getUserName(myDic["user"]) +
                                  " is not registered for '*switchResponse'");
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserName(myDic["user"]) +
                    " is not registered for '*switchResponse'. Registering....Done");
                General.s.permission.Add(myDic["user"], 0);
            }
        }


#if GOOGLE || ALL

        //*google WORDS
        public void google(Dictionary<String, dynamic> myDic)
        {
            String[] tmp = ((String) myDic["text"]).Split();
            String s = "<http://lmgtfy.com/?q=";
            for (int i = 1; i < tmp.Length; i++)
            {
                s += tmp[i] + "+";
            }
            s = s.Remove(s.Length - 1, 1);
            s += ">";
            Console.WriteLine("Sent link: " + s + " because " + General.sc.getUserName(myDic["user"]) +
                              " entered it!");
            Dictionary<String, dynamic> myDict = new Dictionary<string, dynamic>
            {
                {"unfurl_links", "true"},
                {"channel", myDic["channel"]},
                {"as_user", "true"},
                {"text", s},
                {"token", General.sc.Token}
            };
            Console.WriteLine(General.sc.caller.CallMethodPost("chat.postMessage", myDict).Result["ok"]);
        }
#endif

        //*writeTo:Name:Message
        public void writeTo(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission[myDic["user"]] >= 3)
            {
                String[] tmp = ((String) myDic["text"]).Split(':');
                tmp[0] = tmp[0].Trim();
                tmp[1] = tmp[1].Trim();
                for (int i = 3; i < tmp.Length; i++)
                {
                    tmp[2] += tmp[i];
                }
                if (!General.sc.Channels.ContainsKey(tmp[1]) &&
                    !General.s.privateChannels.ContainsKey(General.sc.Users[tmp[1]]["id"]))
                {
                    Dictionary<String, dynamic> paramse = new Dictionary<string, dynamic>();
                    paramse.Add("user", General.sc.Users[tmp[1]]["id"]);
                    Dictionary<String, dynamic> something =
                        General.sc.caller.CallMethodPost("im.open", paramse).Result;
                    General.s.privateChannels.Add(General.sc.Users[tmp[1]]["id"], something["channel"]);
                }
                if (General.sc.Channels.ContainsKey(tmp[1]))
                {
                    General.sc.SendMessage(
                        ((String) General.sc.Channels[tmp[1]]["id"]), tmp[2]);
                }
                else
                {
                    General.sc.SendMessage(((String) General.s.privateChannels[General.sc.Users[tmp[1]]["id"]]["id"]),
                        tmp[2]);
                }
            }
        }

        //*askPermission OR *askPermission:Name
        public void askPermission(Dictionary<String, dynamic> myDic)
        {
            String text = (String) myDic["text"];
            String user = (string) myDic["user"];
            String channel = (string) myDic["channel"];
            if (text.Contains(":"))
            {
                String name = text.Split(':')[1];
                if (General.sc.Users.ContainsKey(name))
                {
                    if (General.s.permission.ContainsKey(General.sc.Users[name]["id"]))
                    {
                        Console.WriteLine(General.sc.getUserName(user) +
                                          " asked for the permissionlevel of " + name + ", which is: " +
                                          General.s.permission[General.sc.Users[name]["id"]]);
                        General.sc.SendMessage(channel,
                            General.sc.getUserNameForPost(user) + ": the permissionlevel of " + name + " is: " +
                            General.s.permission[General.sc.Users[name]["id"]]);
                    }
                    else
                    {
                        Console.WriteLine(General.sc.getUserName(user) +
                                          " asked for the permissionlevel of " + name +
                                          ", but he isn't registered, yet!");
                        Console.WriteLine("Setting permissionlevel of " + name + " to 0!");
                        General.sc.SendMessage(channel,
                            General.sc.getUserNameForPost(user) +
                            ": this user hasn't been registered, yet! Setting the level to 0!");
                        General.s.permission[General.sc.Users[name]["id"]] = 0;
                    }
                }
                else
                {
                    Console.WriteLine(name + " doesn't exist!");
                    General.sc.SendMessage(myDic["channel"],
                        General.sc.getUserNameForPost(myDic["user"]) +
                        " the user you asked permission for doesn't exist!");
                }
            }
            else
            {
                if (General.s.permission.ContainsKey(user))
                {
                    Console.WriteLine(General.sc.getUserName(user +
                                      " asked for his permissionlevel, which is: " +
                                      General.s.permission[user]));
                    General.sc.SendMessage(channel,
                        General.sc.getUserNameForPost(user) + ": your permissionlevel is: " +
                        General.s.permission[user]);
                }
                else
                {
                    Console.WriteLine(General.sc.getUserName(user) +
                                      " asked for his permissionlevel, but he isn't registered, yet!");
                    Console.WriteLine("Setting permissionlevel of " + user + " to 0!");
                    General.sc.SendMessage(channel,
                        General.sc.getUserNameForPost(user) +
                        ": you haven't been registered, yet! Setting your level to 0!");
                    General.s.permission[user] = 0;
                }
            }
        }

        //*setPermission:Name:Permissions
        public void setPermission(Dictionary<String, dynamic> myDic)
        {
            var tmp = ((String) myDic["text"]).Replace("*setPermission", "");
            tmp = tmp.Trim();
            var Ntmp = tmp.Split(':');
            try
            {
                Ntmp[1] = Ntmp[1].Trim();
                Ntmp[2] = Ntmp[2].Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + ": Command not entered correctly!");
            }
            if (General.s.permission.ContainsKey(myDic["user"]))
            {
                if ((General.s.permission[myDic["user"]] >= 3) &&
                    ((General.s.permission[myDic["user"]] > General.s.permission[General.sc.Users[Ntmp[1]]["id"]]) ||
                     (myDic["user"] == General.sc.Users[Ntmp[1]]["id"])))
                {
                    if (Ntmp[2].Equals("OVERLORD"))
                    {
                        try
                        {
                            Console.WriteLine("Setting permissionlevel of: " + Ntmp[1] + " to: " + Ntmp[2]);
                            General.s.permission[General.sc.Users[Ntmp[1]]["id"]] = Int32.MaxValue;
                            General.sc.SendMessage(myDic["channel"],
                                General.sc.getUserNameForPost(myDic["user"]) +
                                " Set permissionlevel of: " + Ntmp[1] + " to: " +
                                Ntmp[2]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            General.sc.SendMessage(myDic["channel"],
                                General.sc.getUserNameForPost(myDic["user"]) +
                                ": Command not entered correctly!");
                        }
                    }
                    else
                    {
                        try
                        {
                            Console.WriteLine("Setting permissionlevel of: " + Ntmp[1] + " to: " + Ntmp[2]);
                            General.s.permission[General.sc.Users[Ntmp[1]]["id"]] = int.Parse(Ntmp[2]);
                            General.sc.SendMessage(myDic["channel"],
                                General.sc.getUserNameForPost(myDic["user"]) +
                                " Set permissionlevel of: " + Ntmp[1] + " to: " +
                                Ntmp[2]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            General.sc.SendMessage(myDic["channel"],
                                General.sc.getUserNameForPost(myDic["user"]) +
                                ": Command not entered correctly!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine(General.sc.getUserName(myDic["user"]) +
                                      " has insufficient permissions for '*setPermission'");
                    General.sc.SendMessage(myDic["channel"],
                        General.sc.getUserName(myDic["user"]) +
                        " has insufficient permissions for '*setPermission'");
                }
            }


            else
            {
                Console.WriteLine(General.sc.getUserName(myDic["user"]) +
                                  " is not registered for '*setPermission'");
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserName(myDic["user"]) +
                    " is not registered for '*setPermission'. Registering....Done");
                General.s.permission.Add(myDic["user"], 0);
            }
        }

#if AFK || ALL

        //*afk
        public void afk(Dictionary<String, dynamic> myDic)
        {
            if (General.s.afkUsers.Contains(myDic["user"]))
            {
                General.sc.SendMessage(myDic["channel"], "You are already afk!");
            }
            else
            {
                General.s.afkUsers.Add(myDic["user"]);
                Console.WriteLine(General.sc.getUserName(myDic["user"]) + " is now afk!");
            }
        }

        //*back
        public void back(Dictionary<String, dynamic> myDic)
        {
            if (General.s.afkUsers.Contains(myDic["user"]))
            {
                General.s.afkUsers.Remove(myDic["user"]);
                Console.WriteLine(General.sc.getUserName(myDic["user"]) + " is no longer afk!");
            }
            else
            {
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + ": You were never afk!");
            }
        }
#endif

        //*save
        public void save(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission.ContainsKey(myDic["user"]))
            {
                if (General.s.permission[myDic["user"]] >= 2)
                {
                    Storage.Serialize(General.s);
                    Console.WriteLine("Storage has been saved, because User: " +
                                      General.sc.getUserName(myDic["user"]) + " has entered '*save'");
                    General.sc.SendMessage(myDic["channel"],
                        General.sc.getUserNameForPost(myDic["user"]) + ": Storage has been saved!");
                }
                else
                {
                    General.sc.SendMessage(myDic["channel"],
                        General.sc.getUserNameForPost(myDic["user"]) +
                        ": You don't have permission to use that command!")
                        ;
                }
            }
            else
            {
                General.s.permission.Add(myDic["user"], 0);
            }
        }

        //*load
        public void load(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission.ContainsKey(myDic["user"]))
            {
                if (General.s.permission[myDic["user"]] >= 2)
                {
                    Console.WriteLine("Storage loaded: " + Storage.Deserialize());
                    General.sc.SendMessage(myDic["channel"],
                        General.sc.getUserNameForPost(myDic["user"]) + ": Storage has been loaded!");
                }
                else
                {
                    General.sc.SendMessage(myDic["channel"],
                        General.sc.getUserNameForPost(myDic["user"]) +
                        ": You don't have permission to use that command!")
                        ;
                }
            }
            else
            {
                General.s.permission.Add(myDic["user"], 0);
            }
        }

        #region pmRecieved
        /*
        public static void pmRecieved(Dictionary<String, dynamic> myDic)
        {
            if (((String) myDic["channel"]).StartsWith("D") &&
                !(General.sc.getUserName(myDic["user"]).Equals("letum")) &&
                !(General.sc.getUserName(myDic["user"]).Equals("someone")))
            {
                if (!General.s.privateChannels.ContainsKey(General.sc.Users["letum"]["id"]))
                {
                    Dictionary<String, dynamic> paramse = new Dictionary<string, dynamic>();
                    paramse.Add("user", General.sc.Users["letum"]["id"]);
                    Dictionary<String, dynamic> something = General.sc.caller.CallMethodPost("im.open", paramse).Result;
                    General.s.privateChannels.Add(General.sc.Users["letum"]["id"], something["channel"]);
                }
                General.sc.SendMessage(((String) General.s.privateChannels[General.sc.Users["letum"]["id"]]["id"]), (String) myDic["text"]);
                General.sc.SendMessage(((String) General.s.privateChannels[General.sc.Users["letum"]["id"]]["id"]), " by: " + General.sc.getUserName(myDic["user"]));
            }
        }
         * */
        #endregion 

#if EVAL || ALL
        //USAGE: 
        //"*addCodeResponse§NameOfEval§Coooode§printToChannel(true/false?)§desc
        public void addCodeResponse(Dictionary<String, dynamic> myDic)
        {
            try
            {
                String[] things = ((String) myDic["text"]).Split('§');
                Eval somesEval = SomeOtherMethodsClass.CreateEval(things[2], bool.Parse(things[3]), things[4]);
                if (General.s.evals.ContainsKey(things[1]))
                {
                    General.s.evals[things[1]].Add(somesEval);
                }
                else
                {
                    General.s.evals.Add(things[1], new List<Eval>(){ somesEval});
                }
                General.sc.SendMessage(myDic["channel"], "Eval added!");
            }
            catch (Exception e)
            {
                Console.WriteLine(General.sc.getUserName(myDic["user"]) +
                                  " has entered the command *addCodeResponse wrong!");
                Console.WriteLine(e.ToString());
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + ": you entered the command incorrectly!");
            }
        }

        //*removeCodeResponse:NameOfEval
        public void removeCodeResponse(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (General.s.evals.ContainsKey(things[1]) && General.s.permission[myDic["user"]] >= 20)
            {
                foreach (Eval eval in General.s.evals[things[1]])
                {
                    File.Delete(eval.path);
                }
                General.s.evals.Remove(things[1]);
                General.sc.SendMessage(myDic["channel"], "Response removed!");
            }
        }

        //*listResponses
        public void listCodeResponses(Dictionary<String, dynamic> myDic)
        {
            String post = General.s.evals.Aggregate("", (current1, keyValuePair) => keyValuePair.Value.Aggregate(current1, (current, eval) => current + (keyValuePair.Key + ":" + eval.desc + "\n")));
            if (post.IsNullOrEmpty())
            {
                post = "No responses!";
            }
            General.sc.SendMessage(myDic["channel"], post);
        }
#endif
        
        //*restart OR *restart:Token
        public void restart(Dictionary<String, dynamic> myDic)
        {
            String ttxt = (String) myDic["text"];
            if (General.s.permission.ContainsKey(myDic["user"]))
            {
                if (General.s.permission[myDic["user"]] >= 10)
                {
                    Console.WriteLine(General.sc.getUserName(myDic["user"]) + " wants to restart!");
                    Console.ReadLine();
                    Process p = Process.GetCurrentProcess();
                    foreach (Thread thread1 in p.Threads.OfType<Thread>())
                    {
                        thread1.Abort();
                    }
                    Program.OnProcessExit(General.sc, EventArgs.Empty);
                    if (ttxt.Contains(":"))
                    {
                        Process.Start(Helper.GetApplicationPath() + "/SlackBot.exe", (ttxt.Split(':'))[1]);
                    }
                    else
                    {
                        Process.Start(Helper.GetApplicationPath() + "/SlackBot.exe");
                    }
                    p.Kill();

                }
                else
                {
                    Console.WriteLine("User: " + General.sc.getUserName(myDic["user"]) +
                                      " wanted to restart! Insufficient permission!");
                }
            }
        }

        //*stopItPl0x
        public void stopItPl0x(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission.ContainsKey(myDic["user"])
                            ? General.s.permission[myDic["user"]] >= 10
                            : null)
            {
                Console.WriteLine("User: " + General.sc.getUserName(myDic["user"]) +
                                  " wants to stop the process!");
                Console.ReadLine();
                Program.OnProcessExit(General.sc, EventArgs.Empty);
                Environment.Exit(-1);
            }
            else
            {
                Console.WriteLine(General.sc.getUserName(myDic["user"]) + " tried to stop the process!");
            }
        }

        //*randomInsult OR *randomInsult:NameToInsult
        public void randomInsult(Dictionary<String, dynamic> myDic)
        {
            Dictionary<String, dynamic> newDic = General.sc.caller.CallAPI("http://quandyfactory.com/insult/json",
                            new Dictionary<string, dynamic>()).Result;
            Console.WriteLine(General.sc.getUserName(myDic["user"]) + " requested a word and got " +
                              newDic["insult"]);
            if (((String) myDic["text"]).Contains(":"))
            {
                newDic["insult"] = ((String) newDic["insult"]).Replace("Thou art",
                    (((String) myDic["text"]).Split(':')[1] + " is"));
            }
            General.sc.SendMessage(myDic["channel"], newDic["insult"]);
        }

        //*randomWord
        public void randomWord(Dictionary<String, dynamic> myDic)
        {
            String newDic = General.sc.caller.CallAPIXML("http://randomword.setgetgo.com/get.php",
                            new Dictionary<string, dynamic>()).Result;
            Console.WriteLine(General.sc.getUserName(myDic["user"]) + " requested a word and got " +
                              newDic);
            General.sc.SendMessage(myDic["channel"], newDic);
        }

        //*define Word
        public void define(Dictionary<String, dynamic> myDic)
        {
            try
            {
                String xml =
                    General.sc.caller.CallAPIXML(
                        "http://services.aonaware.com/DictService/DictService.asmx/Define?word=" +
                        (((String) myDic["text"]).Split()[1]), new Dictionary<string, dynamic>()).Result;
                xml = xml.GetXMLElement("WordDefinition");
                xml = xml.Normalize();
                xml = xml.Replace("&", "");
                xml = System.Text.RegularExpressions.Regex.Replace(xml,@"\s+"," ");
                //Debugger.Break();
                Console.WriteLine("Got definition of the word " + (((String) myDic["text"]).Split()[1]));
                General.sc.SendMessage(myDic["channel"], xml);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #region Polls
        //*addPoll|MyName|MyDescription|06/07/2008 5:12:12 PM|Choice1,Choice2,Choice3
        public void addPoll(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split('|');
            String[] choices = things[4].Split(',');
            Console.WriteLine(choices[0]);
            if (!General.s.polls.ContainsKey(things[1]))
            {
                General.s.polls.Add(things[1].Trim(),
                    new Poll(things[1].Trim(), things[2], things[3], myDic["user"], choices));
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " poll started! Expire date: " + General.s.polls[things[1]].dt);
                General.s.setNextPoll();
            }
            else
            {
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " poll with this name already exists!");
            }
        }

        //*removePoll:NameOfPoll
        public void removePoll(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if ((General.s.polls[things[1]].creatorID == myDic["user"]) || (General.s.permission[myDic["user"]] >= 10))
            {
                General.s.polls.Remove(things[1]);
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " poll removed!");
                General.s.setNextPoll();
            }
            else
            {
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " Insufficient Permissions!");
            }
        }

        //*vote:NameOfPoll:MyChoice
        public void vote(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]) && General.s.polls[things[1]].isRunning)
            {
                if (General.s.polls[things[1]].votes.ContainsKey((things[2].Trim())) && !General.s.polls[things[1]].usersAlreadyVotedID.Contains(myDic["user"]))
                {
                    General.s.polls[things[1]].votes[(things[2].Trim())]++;
                    General.s.polls[things[1]].usersAlreadyVotedID.Add(myDic["user"]);
                }
                else
                {
                    General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " no such choice exists or you have already voted!");
                }
            }
            else
            {
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " no such poll exists!");
            }
        }

        //*listPolls OR *listPolls:true
        public void listPolls(Dictionary<String, dynamic> myDic)
        {
            if (((String) myDic["text"]).Contains(":"))
            {
                String[] things = ((String) myDic["text"]).Split(':');
                if (things[1].Equals("true"))
                {
                    String result = General.s.polls.Keys.Aggregate("", (current, key) => current + (key + "\n"));
                    General.sc.SendMessage(myDic["channel"], result);
                }
                else
                {
                    String result = General.s.polls.Where(poll => poll.Value.isRunning)
                        .Aggregate("", (current, poll) => current + (poll.Key + "\n"));
                    if (result.IsNullOrEmpty())
                    {
                        result = "No running polls!";
                    }
                    General.sc.SendMessage(myDic["channel"], result);
                }
            }
            else
            {
                String result = General.s.polls.Where(poll => poll.Value.isRunning)
                        .Aggregate("", (current, poll) => current + (poll.Key + "\n"));
                if (result.IsNullOrEmpty())
                {
                    result = "No running polls!";
                }
                General.sc.SendMessage(myDic["channel"], result);
            }
        }

        //*listChoices:NameOfPoll
        public void listChoices(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            String result = General.s.polls[things[1]].votes.Aggregate("", ((s, pair) => s + (pair.Key + "\n")));
            General.sc.SendMessage(myDic["channel"], result);
        }

        //*getPollResult:NameOfPoll
        public void getPollResult(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            String result = General.s.polls[things[1]].votes.Aggregate("",
                (s, pair) => s + (pair.Key + ":" + pair.Value + "\n"));
            General.sc.SendMessage(myDic["channel"], result);
        }

        //*getPollDescription:NameOfPoll
        public void getPollDescription(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]))
            {
                General.sc.SendMessage(myDic["channel"], General.s.polls[things[1]].desc);
            }
        }

        //*getPollEndDate:NameOfPoll
        public void getPollEndDate(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]))
            {
                General.sc.SendMessage(myDic["channel"], General.s.polls[things[1]].dt.ToUniversalTime() + " UTC");
            }
        }

        //*getVoterCount:NameOfPoll
        public void getVoterCount(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String)myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]))
            {
                General.sc.SendMessage(myDic["channel"], General.s.polls[things[1]].usersAlreadyVotedID.Count.ToString());
            }
        }

        //*getPollTime:NameOfPoll
        public void getPollTime(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String)myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]))
            {
                TimeSpan ts = (General.s.polls[things[1]].dt) - DateTime.Now;
                ts = ts.Add(new TimeSpan(0, 2, 0, 0));
                General.sc.SendMessage(myDic["channel"], ts.Days + " days, " + ts.Hours + " hours, " + ts.Minutes + " minutes!");
            }
        }

        //*stopPoll:NameOfPoll
        public void stopPoll(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if ((General.s.polls[things[1]].creatorID == myDic["user"]) || (General.s.permission[myDic["user"]] >= 10))
            {
                General.s.polls[things[1]].isRunning = false;
                General.s.setNextPoll();
                General.sc.SendMessage(myDic["channel"], General.sc.getUserNameForPost(myDic["user"]) + ": Poll stopped!");
            }
        }

        #endregion 
    }
}