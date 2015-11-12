using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public bool sternchenResponse = true;
        public Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>();
        public Dictionary<string, List<Eval>> evals = new Dictionary<string, List<Eval>>();
        public Dictionary<string, dynamic> myDic = new Dictionary<string, dynamic>();
        public Dictionary<string, Poll> polls = new Dictionary<string, Poll>();
        public Dictionary<String, DateTime> bdays = new Dictionary<string, DateTime>();
            
        [ScriptIgnore] public bool another = false;

        [ScriptIgnore] public String name;

        [ScriptIgnore] public Thread PollEndThread;

        [ScriptIgnore] public Dictionary<string, Dictionary<string, object>> privateChannels = new Dictionary<string, Dictionary<string, object>>();

        [ScriptIgnore]
        public readonly int methodCount = 46;

        public delegate void delToCall(System.Collections.Generic.Dictionary<String, object> myDic);

        [ScriptIgnore]
        public Dictionary<String, delToCall> commands = new Dictionary<string, delToCall>();

        [ScriptIgnore]
        public Dictionary<String, String> commandDesc = new Dictionary<string, string>(); 

        #endregion

        #region setUp

        public void SetUp()
        {
            Console.WriteLine(Storage.Deserialize());

            #region Spamcounter
            foreach (var VARIABLE in General.sc.Channels)
            {
                General.s.lastMessager[VARIABLE.Value["id"]] = "Hi";
                General.s.spamCounter[VARIABLE.Value["id"]] = 0;
            }
            #endregion

            #region Permissions
            foreach (KeyValuePair<string, dynamic> keyValuePair in General.sc.Users)
            {
                if (!General.s.permission.ContainsKey(keyValuePair.Value["id"]))
                {
                    General.s.permission.Add(keyValuePair.Value["id"], 1);
                }
            }
            #endregion

            General.s.afkUsers = new List<string>();

            #region myPermissionLevel
            if ((!General.s.permission.ContainsKey(General.sc.Users["letum"]["id"])) ||
                (General.s.permission[General.sc.Users["letum"]["id"]] <= 9))
            {
                General.s.permission[General.sc.Users["letum"]["id"]] = 10;
            }
            #endregion

            #region commands
            if (General.s.commands.Count != General.s.methodCount)
            {
                General.s.commands = new Dictionary<string, Storage.delToCall>();
                MethodKeeper mk = new MethodKeeper();
                foreach (MethodInfo methodInfo in typeof (MethodKeeper).GetMethods())
                {
                    if (methodInfo.DeclaringType == typeof (MethodKeeper))
                    {
                        General.s.commands.Add(methodInfo.Name,
                            (Storage.delToCall)
                                Delegate.CreateDelegate(typeof (Storage.delToCall), mk, methodInfo));

                        DescriptionAttribute myAttribute = (DescriptionAttribute)methodInfo.GetCustomAttributes().First(attribute => attribute is DescriptionAttribute);
                        General.s.commandDesc.Add(methodInfo.Name, myAttribute.Description);
                    }
                }
            }
            #endregion

            if (General.s.polls.Count > 0)
            {
                SomeOtherMethodsClass.setNextPoll();
            }
            if (General.s.bdays.Count > 0)
            {
                SomeOtherMethodsClass.setNextBday();
            }
        }

        #endregion

        #region Serialize

        public static void Serialize(Storage s)
        {
            String json = s.ToJSON();
            TextWriter sw = new StreamWriter(Helper.GetApplicationPath() + "/Storage.xml");
            sw.Write(json);
            sw.Close();
        }

        #endregion

        #region Deserialize

        public static bool Deserialize()
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

        #endregion
    }
}