using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using SlackAPI;
using System.Web.Script.Serialization;

namespace SlackBot
{
    public class Storage
    {
        #region Storage
        //ID's as Users
        public Dictionary<object, string> lastMessager = new Dictionary<object, string>();
        public Dictionary<object, int> spamCounter = new Dictionary<object, int>();
        public Dictionary<string, int> permission = new Dictionary<string, int>();
        public List<string> afkUsers = new List<string>();
        public Dictionary<string, Dictionary<string, object>> privateChannels =
            new Dictionary<string, Dictionary<string, object>>();


        public bool sternchenResponse = true;
        public Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>();
        public Dictionary<string, List<Eval>> evals = new Dictionary<string, List<Eval>>();
        public Dictionary<string, dynamic> myDic = new Dictionary<string, dynamic>();
        public Dictionary<string, Poll> polls = new Dictionary<string, Poll>();

        [ScriptIgnore] 
        public bool another = false;

        [ScriptIgnore] 
        public String name;

        [ScriptIgnore]
        public Thread PollEndThread;

        #endregion

        #region setUp

        public void SetUp(bool another, String name)
        {

            Storage.Deserialize(name, another);
            if (another)
            {
                General.s.privateChannels = new Dictionary<String, Dictionary<String, object>>();
                General.s.another = true;
                General.s.name = name;
            }
            foreach (var VARIABLE in General.sc.Channels)
            {
                General.s.lastMessager[VARIABLE.Value["id"]] = "Hi";
                General.s.spamCounter[VARIABLE.Value["id"]] = 0;
            }
            foreach (KeyValuePair<string, dynamic> keyValuePair in General.sc.Users)
            {
                if (!General.s.permission.ContainsKey(keyValuePair.Value["id"]))
                {
                    General.s.permission.Add(keyValuePair.Value["id"], 1);
                }
            }
            General.s.afkUsers = new List<string>();
            if (!General.s.responses.ContainsKey("infoPLS*"))
            {
                General.s.responses.Add("infoPLS*",
                    new List<string>()
                    {
                        "Available commands: afk, back, addResponse, removeResponse, infoPLS*, save, load, setPermission, askPermission, addCodeResponse, google, switchResponse.\n" +
                        " Functions of this bot: SpamStopper, ResponseWriter(every Response/command you want to hear has to start with '*')"
                    });
            }
            if ((!General.s.permission.ContainsKey(General.sc.Users["letum"]["id"])) || (General.s.permission[General.sc.Users["letum"]["id"]] <= 9))
            {
                General.s.permission[General.sc.Users["letum"]["id"]] = 10;
            }

            if (General.commands.Count != General.methodCount)
            {
                General.commands = new Dictionary<string, General.delToCall>();
                MethodKeeper mk = new MethodKeeper();
                foreach (MethodInfo methodInfo in typeof(MethodKeeper).GetMethods())
                {
                    if (methodInfo.DeclaringType == typeof(MethodKeeper))
                    {
                        General.commands.Add(methodInfo.Name,
                            (General.delToCall) General.delToCall.CreateDelegate(typeof (General.delToCall), mk, methodInfo));
                    }
                }
            }

            setNextPoll();
        }

        #endregion

        public void setNextPoll()
        {
            try
            {
                KeyValuePair<String, Poll> earliest = General.s.polls.First(pair => pair.Value.isRunning == true);
                foreach (KeyValuePair<string, Poll> keyValuePair in General.s.polls)
                {
                    if (keyValuePair.Value.dt < earliest.Value.dt)
                    {
                        earliest = keyValuePair;
                        SomeOtherMethodsClass.endTime = earliest.Value.dt;
                        SomeOtherMethodsClass.PollName = earliest.Key;
                        General.s.PollEndThread = new Thread(SomeOtherMethodsClass.PollEndDeterminer);
                        General.s.PollEndThread.Start();
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #region Serialize
        public static void Serialize(Storage s)
        {
            if (s.PollEndThread != null)
            {
                s.PollEndThread.Abort();
            }
            String json = s.ToJSON();
            if (s.another)
            {
                TextWriter sw = new StreamWriter(Helper.GetApplicationPath() + "/Storage" + s.name + ".xml");
                sw.Write(json);
                sw.Close();
            }
            else
            {
                TextWriter sw = new StreamWriter(Helper.GetApplicationPath() + "/Storage.xml");
                sw.Write(json);
                sw.Close();
            }
        }
        #endregion

        #region Deserialize
        public static bool Deserialize(String name, bool another)
        {
            if (another)
            {
                if (File.Exists(Helper.GetApplicationPath() + "/Storage.xml"))
                {
                    try
                    {
                        TextReader sr =
                            new StreamReader(Helper.GetApplicationPath() + "/Storage.xml");
                        String json = sr.ReadLine();
                        sr.Close();
                        General.s = (Storage) json.ToObject<Storage>();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        File.Delete(Helper.GetApplicationPath() + "/Storage.xml");
                    }
                }
                return false;
            }
            else
            {
                if (File.Exists(Helper.GetApplicationPath() + "/Storage" + name + ".xml"))
                {
                    try
                    {
                        TextReader sr =
                            new StreamReader(Helper.GetApplicationPath() + "/Storage" + name + ".xml");
                        String json = sr.ReadLine();
                        sr.Close();
                        General.s = (Storage)json.ToObject<Storage>();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        File.Delete(Helper.GetApplicationPath() + "/Storage" + name + ".xml");
                    }
                }
                return false;
            }
        }
        #endregion
    }
}