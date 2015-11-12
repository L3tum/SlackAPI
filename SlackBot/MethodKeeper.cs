using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AIMLbot;
using IronPython.Hosting;
using LoreSoft.MathExpressions;
using Microsoft.Scripting.Hosting;
using SlackAPI;
using WebSocketSharp;

namespace SlackBot
{
    public class MethodKeeper
    {
        #region Responses

        #region addResponse
        [Description("*addResponse Trigger|Reply \nAdds a response to the list of responses.")]
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
        #endregion 

        #region removeResponse
        [Description("*removeResponse:Response \nRemoves a response from the list of responses.")]
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
        #endregion 

        #region switchResponse
        [Description("*switchResponse \nSwitches if '*' is needed to trigger a response.")]
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
        #endregion

        #region switchListening
        [Description("*switchListening \nSwitches if the bot should answer itself.")]
        public void switchListening(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission[myDic["user"]] >= 10)
            {
                General.ls.w.listenToItself = !General.ls.w.listenToItself;
                General.sc.SendMessage(myDic["channel"], "Bot listens to itself: " + General.ls.w.listenToItself);
            }
        }
        #endregion

        #region switchMultiple
        [Description("*switchMultiple \nSwitches the need to enter *Name*Command.")]
        public void switchMultiple(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission[myDic["user"]] >= 50)
            {
                General.ls.w.multiple = !General.ls.w.multiple;
            }
        }
        #endregion 

        #endregion

        #region permissions

        #region askPermission
        [Description("*askPermission OR *askPermission:Name \nReturns your or the 'Name's permissionlevel.")]
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
        #endregion 

        #region setPermission
        [Description("*setPermission:Name:Permissionlevel \nSets permissionlevel of 'Name' to 'Permissionlevel.")]
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
                    else if (Ntmp[2].Equals("NOOB"))
                    {
                        try
                        {
                            Console.WriteLine("Setting permissionlevel of: " + Ntmp[1] + " to: " + Ntmp[2]);
                            General.s.permission[General.sc.Users[Ntmp[1]]["id"]] = Int32.MinValue;
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
        #endregion 

        #endregion

        #region afk

#if AFK || ALL

        #region affk
        [Description("*afk \nSets you to be afk.")]
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
        #endregion 

        #region back
        [Description("*back \nSets you to be active again.")]
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
        #endregion 
#endif

        #endregion

        #region save/load

        #region save
        [Description("*save \nSaves the Storage.")]
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
        #endregion 
        
        #region load
        [Description("*load \nLoads the Storage.")]
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
        #endregion 

        #endregion

        #region Evals

#if EVAL || ALL
        #region addCodeResponse 
        [Description("*addCodeResponse§NameOfEval§Coooode§printToChannel(true/false?)§desc \nCreates an Assembly with 'Coooode' inside it and runs it when 'NameOfEval' is entered in a channel.")]
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
                    General.s.evals.Add(things[1], new List<Eval>() {somesEval});
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
        #endregion 

        #region removeCodeResponse
        [Description("*removeCodeResponse:NameOfEval \nRemoves Assemblies with the trigger 'NameOfEval'.")]
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
        #endregion 

        #region listCodeResponses
        [Description("*listCodeResponses \nLists all Assemblies.")]
        public void listCodeResponses(Dictionary<String, dynamic> myDic)
        {
            String post = General.s.evals.Aggregate("",
                (current1, keyValuePair) =>
                    keyValuePair.Value.Aggregate(current1,
                        (current, eval) => current + (keyValuePair.Key + ":" + eval.desc + "\n")));
            if (post.IsNullOrEmpty())
            {
                post = "No responses!";
            }
            General.sc.SendMessage(myDic["channel"], post);
        }
        #endregion 

        #region runPython
        [Description("*runPython§Something")]
        public void runPython(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split('§');
            ScriptEngine engine = Python.CreateEngine();
            General.sc.SendMessage(myDic["channel"], engine.Execute(things[1]));
        }
        #endregion 
#endif

        #endregion

        #region restart/stop

        #region restart
        [Description("*restart OR *restart:Token \nRestarts the bot and logs in with 'Token' if it's given.")]
        public void restart(Dictionary<String, dynamic> myDic)
        {
            String ttxt = (String) myDic["text"];
            if (General.s.permission.ContainsKey(myDic["user"]))
            {
                if (General.s.permission[myDic["user"]] >= 10)
                {
                    Console.WriteLine(General.sc.getUserName(myDic["user"]) + " wants to restart!");
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
        #endregion 

        #region stop
        [Description("*stopItPl0x \nStops the bot.")]
        public void stopItPl0x(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission.ContainsKey(myDic["user"])
                ? General.s.permission[myDic["user"]] >= 10
                : null)
            {
                Console.WriteLine("User: " + General.sc.getUserName(myDic["user"]) +
                                  " wants to stop the process!");
                Program.OnProcessExit(General.sc, EventArgs.Empty);
                Environment.Exit(-1);
            }
            else
            {
                Console.WriteLine(General.sc.getUserName(myDic["user"]) + " tried to stop the process!");
            }
        }
        #endregion 

        #region start
        [Description("*start OR *start:Token \nStarts another instance of the currently running bot or with 'Token'.")]
        public void start(Dictionary<String, dynamic> myDic)
        {
            if (((String) myDic["text"]).Contains(":"))
            {
                String[] things = ((String) myDic["text"]).Split(':');
                Process.Start(Helper.GetApplicationPath() + "/SlackBot.exe", things[1]);
            }
            else
            {
                Process.Start(Helper.GetApplicationPath() + "/SlackBot.exe");
            }
        }
        #endregion 

        #endregion

        #region random

        #region randomInsult
        [Description("*randomInsult OR *randomInsult:NameToInsult \nReturns a random insult with the 'NameToInsult' if it's given.")]
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
        #endregion 

        #region randomWord
        [Description("*randomWord \nRetuns a random word.")]
        public void randomWord(Dictionary<String, dynamic> myDic)
        {
            String newDic = General.sc.caller.CallAPIString("http://randomword.setgetgo.com/get.php",
                new Dictionary<string, dynamic>()).Result;
            Console.WriteLine(General.sc.getUserName(myDic["user"]) + " requested a word and got " +
                              newDic);
            General.sc.SendMessage(myDic["channel"], newDic);
        }
        #endregion

        #region define
        [Description("*define WORD \nReturns the definition of 'WORD'.")]
        public void define(Dictionary<String, dynamic> myDic)
        {
            try
            {
                String xml =
                    General.sc.caller.CallAPIString(
                        "http://services.aonaware.com/DictService/DictService.asmx/Define?word=" +
                        (((String) myDic["text"]).Split()[1]), new Dictionary<string, dynamic>()).Result;
                xml = xml.GetXMLElement("WordDefinition");
                xml = xml.Normalize();
                xml = xml.Replace("&", "");
                xml = System.Text.RegularExpressions.Regex.Replace(xml, @"\s+", " ");
                //Debugger.Break();
                Console.WriteLine("Got definition of the word " + (((String) myDic["text"]).Split()[1]));
                General.sc.SendMessage(myDic["channel"], xml);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion

        #region google
        [Description("*google WORD/S \nReturns an lmgtfy link with 'WORD/S'.")]
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
        #endregion 

        #region writeTo
        [Description("*writeTo:Name/Channel:Message \nSends a 'Message' to 'Name/Channel'.")]
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
        #endregion 

        #region mirror
        [Description("*mirror:WORD/S \nReturns an image with 'WORD/S' mirrored in it.")]
        public void mirror(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (!File.Exists(Helper.GetApplicationPath() + "/images/" + things[1] + ".jpg"))
            {
                Bitmap bmp = new Bitmap(1920, 780);
                RectangleF rectf = new RectangleF(0, 0, bmp.Width, bmp.Height);

                // Create graphic object that will draw onto the bitmap
                Graphics g = Graphics.FromImage(bmp);

                g.FillRectangle(
                    Brushes.White, 0, 0, bmp.Width, bmp.Height);

                // Ensure the best possible quality rendering
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // The smoothing mode specifies whether lines, curves, and the edges of filled areas use smoothing (also called antialiasing). One exception is that path gradient brushes do not obey the smoothing mode. Areas filled using a PathGradientBrush are rendered the same way (aliased) regardless of the SmoothingMode property.
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // The interpolation mode determines how intermediate values between two endpoints are calculated.
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Use this property to specify either higher quality, slower rendering, or lower quality, faster rendering of the contents of this Graphics object.
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit; // This one is important

                // Create string formatting options (used for alignment)
                StringFormat format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                // Draw the text onto the image
                g.DrawString(things[1], new Font("Tahoma", 80), Brushes.Black, rectf, format);

                // Flush all graphics changes to the bitmap
                g.Flush();

                bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                SizeF size =g.MeasureString(things[1], new Font("Tahoma", 80));

                bmp = bmp.Clone(new Rectangle((int)(bmp.Width / 2 - size.Width), (int)(bmp.Height / 2 - size.Height), (int)size.Width * 2 + 10, (int)size.Height * 2 + 5), bmp.PixelFormat);

                bmp.Save(Helper.GetApplicationPath() + "/images/" + things[1] + ".jpg", ImageFormat.Jpeg);
            }
            String path = Helper.GetApplicationPath() + "/images/" + things[1] + ".jpg";

            Task<String> s = General.sc.caller.SlackSendFile(path, myDic["channel"], things[1] + ".jpg");

            //File.Delete((Helper.GetApplicationPath() + "/" + newDic + ".bmp"));
        }
        #endregion 

        #region search
        [Description("*search:WORD/S \nReturns the first google search result.")]
        public void search(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            String result = General.sc.caller.CallAPIString("http://ajax.googleapis.com/ajax/services/search/web?v=1.0&q=" + things[1],
                new Dictionary<string, dynamic>()).Result;
            Rootobject dd = (Rootobject)result.ToObject<Rootobject>();
            General.sc.SendMessage(myDic["channel"], dd.responseData.results[0].url + "\n" + dd.responseData.results[0].content);
        }
        #endregion 

        #region iSearch
        [Description("*iSearch:WORD/S \nReturns the first google image search result.")]
        public void iSearch(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String)myDic["text"]).Split(':');
            String result = General.sc.caller.CallAPIString("http://ajax.googleapis.com/ajax/services/search/images?v=1.0&q=" + things[1],
                new Dictionary<string, dynamic>()).Result;
            ImagesGoogle.Rootobject dd = (ImagesGoogle.Rootobject)result.ToObject<ImagesGoogle.Rootobject>();
            General.sc.SendMessage(myDic["channel"], dd.responseData.results[0].url);
        }
        #endregion

        #region vsearch
        [Description("*vSearch:WORD/S \nReturns the first google video search result.")]
        public void vSearch(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String)myDic["text"]).Split(':');
            String result = General.sc.caller.CallAPIString("http://ajax.googleapis.com/ajax/services/search/video?v=1.0&q=" + things[1],
                new Dictionary<string, dynamic>()).Result;
            VideoGoogle.Rootobject dd = (VideoGoogle.Rootobject)result.ToObject<VideoGoogle.Rootobject>();
            General.sc.SendMessage(myDic["channel"], dd.responseData.results[0].title + "\n" + dd.responseData.results[0].url);
        }
        #endregion

        #region math
        [Description("*math:Expression \nReturns Math.")]
        public void math(Dictionary<String, dynamic> myDic)
        {
            MathEvaluator eval = new MathEvaluator();
            double e = eval.Evaluate((((String)myDic["text"]).Replace("*math:", "")).Trim());
            General.sc.SendMessage(myDic["channel"], e.ToString() + ", https://www.reddit.com/r/theydidthemath");
        }
        #endregion

        #region setBday
        [Description("*setBday:username:Birthday:Age")]
        public void setBday(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (!things[1].Equals(General.sc.getUserName(myDic["user"])) && General.s.permission[myDic["user"]] >= 100)
            {
                DateTime tmp = DateTime.Parse(things[2]);
                tmp = tmp.AddYears(Int32.Parse(things[3]) + 1);
                General.s.bdays.Add(things[1], tmp);
                SomeOtherMethodsClass.setNextBday();
                General.sc.SendMessage(myDic["channel"], "Birthday set!");
            }
            else if (things[1].Equals(General.sc.getUserName(myDic["user"])))
            {
                DateTime tmp = DateTime.Parse(things[2]);
                tmp = tmp.AddYears(Int32.Parse(things[3]) + 1);
                General.s.bdays.Add(things[1], tmp);
                SomeOtherMethodsClass.setNextBday();
                General.sc.SendMessage(myDic["channel"], "Birthday set!");
            }
            else
            {
                General.sc.SendMessage(myDic["channel"],
                    "You don't have enough permission to set the birthday of another person!");
            }
        }
        #endregion 

        #endregion

        #region Polls

        #region addPoll
        [Description("*addPoll|MyName|MyDescription|06/07/2008 5:12:12 PM|Choice1,Choice2,Choice3 \nAdds a Poll with 'MyName' and 'MyDescription', which expires on 06.07.2008 at 5:12:12 PM and has the choices at the end.")]
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
                    General.sc.getUserNameForPost(myDic["user"]) + " poll started! Expire date: " +
                    General.s.polls[things[1]].dt);
                SomeOtherMethodsClass.setNextPoll();
            }
            else
            {
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " poll with this name already exists!");
            }
        }
        #endregion 

        #region removePoll
        [Description("*removePoll:Name \nRemoves Poll 'Name'.")]
        public void removePoll(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if ((General.s.polls[things[1]].creatorID == myDic["user"]) || (General.s.permission[myDic["user"]] >= 10))
            {
                General.s.polls.Remove(things[1]);
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " poll removed!");
                SomeOtherMethodsClass.setNextPoll();
            }
            else
            {
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " Insufficient Permissions!");
            }
        }
        #endregion 

        #region vote
        [Description("*vote:NameOfPoll:NameOfChoice \nAdds your vote to 'NameOfChoice' in the Poll 'NameOfPoll'.")]
        public void vote(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]) && General.s.polls[things[1]].isRunning)
            {
                if (General.s.polls[things[1]].votes.ContainsKey((things[2].Trim())) &&
                    !General.s.polls[things[1]].usersAlreadyVotedID.Contains(myDic["user"]))
                {
                    General.s.polls[things[1]].votes[(things[2].Trim())]++;
                    General.s.polls[things[1]].usersAlreadyVotedID.Add(myDic["user"]);
                }
                else
                {
                    General.sc.SendMessage(myDic["channel"],
                        General.sc.getUserNameForPost(myDic["user"]) +
                        " no such choice exists or you have already voted!");
                }
            }
            else
            {
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + " no such poll exists!");
            }
        }
        #endregion

        #region listPolls
        [Description("*listPolls OR *listPolls:true \nList all polls and polls that have ended if 'true' is given.")]
        public void listPolls(Dictionary<String, dynamic> myDic)
        {
            if (((String) myDic["text"]).Contains(":"))
            {
                String[] things = ((String) myDic["text"]).Split(':');
                if (things[1].Equals("true"))
                {
                    String result = General.s.polls.Keys.Aggregate("", (current, key) => current + (key + "\n"));
                    if (result.IsNullOrEmpty())
                    {
                        result = "No polls!";
                    }
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
        #endregion 

        #region listChoices
        [Description("*listChoices:NameOfPoll \nLists all choices in the Poll 'NameOfPoll'.")]
        public void listChoices(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            String result = General.s.polls[things[1]].votes.Aggregate("", ((s, pair) => s + (pair.Key + "\n")));
            General.sc.SendMessage(myDic["channel"], result);
        }
        #endregion 

        #region getPollResult
        [Description("*getPollResult:NameOfPoll \nGets the result of the Poll 'NameOfPoll'.")]
        public void getPollResult(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            String result = General.s.polls[things[1]].votes.Aggregate("",
                (s, pair) => s + (pair.Key + ":" + pair.Value + "\n"));
            General.sc.SendMessage(myDic["channel"], result);
        }
        #endregion 

        #region getPollDescription
        [Description("*getPollDescription:NameOfPoll \nGets the poll-description of Poll 'NameOfPoll'.")]
        public void getPollDescription(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]))
            {
                General.sc.SendMessage(myDic["channel"], General.s.polls[things[1]].desc);
            }
        }
        #endregion

        #region getPollEndDate
        [Description("*getPollEndDate:NameOfPoll \nGets the end-date of Poll 'NameOfPoll'.")]
        public void getPollEndDate(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]))
            {
                General.sc.SendMessage(myDic["channel"], General.s.polls[things[1]].dt.ToUniversalTime() + " UTC");
            }
        }
        #endregion

        #region getVoterCount
        [Description("*getVoterCount:NameOfPoll \nGets count of Users already voted in Poll 'NameOfPoll'.")]
        public void getVoterCount(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]))
            {
                General.sc.SendMessage(myDic["channel"], General.s.polls[things[1]].usersAlreadyVotedID.Count.ToString());
            }
        }
        #endregion

        #region getPollTime
        [Description("*getPollTime:NameOfPoll \nGets time until Poll 'NameOfPoll' ends.")]
        public void getPollTime(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if (General.s.polls.ContainsKey(things[1]))
            {
                TimeSpan ts = (General.s.polls[things[1]].dt) - DateTime.Now;
                ts = ts.Add(new TimeSpan(0, 2, 0, 0));
                General.sc.SendMessage(myDic["channel"],
                    ts.Days + " days, " + ts.Hours + " hours, " + ts.Minutes + " minutes!");
            }
        }
        #endregion 

        #region stopPoll
        [Description("*stopPoll:NameOfPoll \nStops the Poll 'NameOfPoll'.")]
        public void stopPoll(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            if ((General.s.polls[things[1]].creatorID == myDic["user"]) || (General.s.permission[myDic["user"]] >= 10))
            {
                General.s.polls[things[1]].isRunning = false;
                General.s.polls[things[1]].dt = DateTime.Now;
                SomeOtherMethodsClass.setNextPoll();
                General.sc.SendMessage(myDic["channel"],
                    General.sc.getUserNameForPost(myDic["user"]) + ": Poll stopped!");
            }
        }
        #endregion 

        #endregion

        #region CHAT

        #region beginChat
        [Description("*beginChat:Name \nBegins a chat with bots of these names.")]
        public void beginChat(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission.ContainsKey(myDic["user"]) && (General.s.permission[myDic["user"]] >= 2))
            {
                String tmp = (((String) myDic["text"]).Replace("*beginChat:", "")).Trim();
                AIMLbot.Bot bot = new Bot();
                General.sc.SendMessage(myDic["channel"],
                    "Loading...." + Helper.GetApplicationPath() + "/" + tmp + "_config/Settings.xml");
                bot.loadSettings(Helper.GetApplicationPath() + "/" + tmp + "_config/Settings.xml");
                bot.loadAIMLFromFiles();
                AIMLbot.User user = new AIMLbot.User(myDic["user"], bot);
                General.active_users.Add(tmp, user);
                General.sc.SendMessage(myDic["channel"], "It begun!");
            }
            else
            {
                General.sc.SendMessage(myDic["channel"], "Insufficient permission!");
            }
        }
        #endregion 

        #region addBot
        [Description("*addBot:Name:Location:true(male)/false(female):birthday:friend,friend,friend:favoriteMovie:religion:favoriteFood:favoriteColor:favoriteActor:nationality:forFun:favoriteSong:favoriteBook:kindOfMusic:favoriteBand:starSign:girlfriend:boyfriend:favoriteSport:favoriteAuthor:orientation")]
        public void addBot(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String)myDic["text"]).Split(':');
            if (!Directory.Exists(Helper.GetApplicationPath() + "/" + things[1] + "_config"))
            {
                Directory.CreateDirectory(Helper.GetApplicationPath() + "/" + things[1] + "_config");
                DirectoryInfo di = new DirectoryInfo(Helper.GetApplicationPath() + "/default_config");
                FileInfo[] fis = di.GetFiles();
                foreach (FileInfo fileInfo in fis)
                {
                    File.Copy(fileInfo.DirectoryName + "/" + fileInfo.Name,
                        Helper.GetApplicationPath() + "/" + things[1] + "_config/" + fileInfo.Name);
                }
            }
            XmlSerializer xml = new XmlSerializer(typeof(root));
            FileStream fs = new FileStream(Helper.GetApplicationPath() + "/" + things[1] + "_config/Settings.xml", FileMode.Open);
            root r = (root)xml.Deserialize(fs);
            fs.Close();
            File.Delete(Helper.GetApplicationPath() + "/" + things[1] + "_config/Settings.xml");

            r.item.First(item => item.name == "name").value = things[1];
            r.item.First(item => item.name == "location").value = things[2];
            r.item.First(item => item.name == "gender").value = (bool.Parse(things[3])
                ? AIMLbot.Utils.Gender.Male.ToString()
                : AIMLbot.Utils.Gender.Female.ToString());
            r.item.First(item => item.name == "birthday").value = things[4];
            r.item.First(item => item.name == "friends").value = things[5];
            r.item.First(item => item.name == "favoritemovie").value = things[6];
            r.item.First(item => item.name == "religion").value = things[7];
            r.item.First(item => item.name == "favoritefood").value = things[8];
            r.item.First(item => item.name == "favoritecolor").value = things[9];
            r.item.First(item => item.name == "favoriteactor").value = things[10];
            r.item.First(item => item.name == "nationality").value = things[11];
            r.item.First(item => item.name == "forfun").value = things[12];
            r.item.First(item => item.name == "favoritesong").value = things[13];
            r.item.First(item => item.name == "favoritebook").value = things[14];
            r.item.First(item => item.name == "kindmusic").value = things[15];
            r.item.First(item => item.name == "favoriteband").value = things[16];
            r.item.First(item => item.name == "sign").value = things[17];
            r.item.First(item => item.name == "girlfriend").value = things[18];
            r.item.First(item => item.name == "boyfriend").value = things[19];
            r.item.First(item => item.name == "favoritesport").value = things[20];
            r.item.First(item => item.name == "favoriteauthor").value = things[21];
            r.item.First(item => item.name == "orientation").value = things[22];
            r.item.First(item => item.name == "configdirectory").value = things[1] + "_config";
            fs = new FileStream(Helper.GetApplicationPath() + "/" + things[1] + "_config/Settings.xml", FileMode.Create);
            xml.Serialize(fs, r);
            DirectoryInfo d = new DirectoryInfo(Helper.GetApplicationPath() + "/aiml");
            FileInfo[] ffs = d.GetFiles();
            Directory.CreateDirectory(Helper.GetApplicationPath() + "/" + things[1] + "_config/aiml");
            foreach (FileInfo fileInfo in ffs)
            {
                fileInfo.CopyTo(Helper.GetApplicationPath() + "/" + things[1] + "_config/aiml/" + fileInfo.Name);
            }
            General.sc.SendMessage(myDic["channel"], "Bot added!");
        }
        #endregion 

        #region terminateChat
        [Description("*terminateChat:NameOfBot\nUseful if bot broke.")]
        public void terminateChat(Dictionary<String, dynamic> myDic)
        {
            String[] things = ((String) myDic["text"]).Split(':');
            General.active_users.Remove(things[1]);
            General.sc.SendMessage(myDic["channel"], "Terminated " + things[1]);
        }
        #endregion

        #region listBots
        [Description("*listBots \nList all available bots.")]
        public void listBots(Dictionary<String, dynamic> myDic)
        {
            String result = Directory.GetDirectories(Helper.GetApplicationPath()).Where(directory => directory.Contains("_config")).Aggregate("", (current, directory) => current + ((((directory.Replace(Helper.GetApplicationPath(), "")).Replace("_config", "")).Replace("\\", "")).Trim() + ", "));
            result = result.Remove(result.Length - 2, 2);
            General.sc.SendMessage(myDic["channel"], result);
        }
        #endregion 

        #endregion

        #region HELP
        [Description("*help OR *help:NameOfMethod \nReturns a help message.")]
        public void help(Dictionary<String, dynamic> myDic)
        {
            String ttxt = (String) myDic["text"];
            if (ttxt.Contains(":"))
            {
                String[] things = ttxt.Split(':');
                General.sc.SendMessage(myDic["channel"], General.s.commandDesc[things[1]]);
            }
            else
            {
                IOrderedEnumerable<KeyValuePair<string, Storage.delToCall>> something = General.s.commands.OrderBy(i => i.Key);
                String result = something.Aggregate("", (current, keyValuePair) => current + (keyValuePair.Key + ", "));
                General.sc.SendMessage(myDic["channel"], result);
            }
        }
        #endregion

        #region backup
        [Description("*noBackUp \nStops the backup process.")]
        public void noBackUp(Dictionary<String, dynamic> myDic)
        {
            if (General.s.permission[myDic["user"]] >= 100)
            {
                Process[] ps = Process.GetProcessesByName("SlackBotBackup");
                foreach (Process process in ps)
                {
                    try
                    {
                        process.Close();
                        process.Kill();
                    }
                    catch
                    {
                        
                    }
                }
            }
        }
        #endregion 
    }
}