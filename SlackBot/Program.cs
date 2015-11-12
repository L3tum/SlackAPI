using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using SlackAPI;

namespace SlackBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SlackClient sc;

            #region args

            if (args.Length > 0)
            {
                try
                {
                    if (args[0].Equals("timo"))
                    {
                        sc = new SlackClient("xoxb-5134150563-iZKW7CIodzRbffqVmFmz6m2S");
                    }
                    else if (args[0].Equals("lily"))
                    {
                        sc = new SlackClient("xoxb-7444634401-UTU2IHZE2kULUWu70hgKV0FA");
                    }
                    else
                    {
                        sc = new SlackClient(args[0]);
                    }
                }
                catch
                {
                    sc = new SlackClient("xoxp-5007212458-11027941589-11025314452-ac4fcf3c3b");
                }
            }
            else
            {
                sc = new SlackClient("xoxp-5007212458-11027941589-11025314452-ac4fcf3c3b");
            }

            #endregion

            General.sc = sc;

            #region WS
            var ws = new WebSlack();
            ws.CreateWebSocket(sc.URL);
            General.ws = ws;
            #endregion 

            #region Storage
            var s = new Storage();
            General.s = s;
            s.SetUp();
            #endregion 

            #region LS
            var ls = new Listener(General.ws);
            General.ls = ls;
            var question = new Thread(ls.Listen);
            General.question = question;
            question.Start();
            #endregion 

            #region sbr
            var sbr = new SlackBotRunner();
            General.sbr = sbr;
            #endregion 

            #region BackupStarter
            if (!File.Exists(Helper.GetApplicationPath() + "/SlackBotBackup.exe"))
            {
                String things = Helper.GetApplicationPath().Remove(Helper.GetApplicationPath().Length - 16, 16);
                File.Copy(things + "Backup/bin/Debug/SlackBotBackup.exe", Helper.GetApplicationPath() + "/SlackBotBackup.exe");
            }
            if (Process.GetProcessesByName("SlackBotBackup").Length == 0)
            {
                Process.Start(Helper.GetApplicationPath() + "/SlackBotBackup.exe");
            }
            #endregion 

            #region ProcessExit
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += OnProcessExit;
            #endregion 

            sc.SendMessage("bot", "HELLO GUYS! IT'S ME, ANAL MOLLY!\n Only Testin'!");
        }

        #region OnProcessExit
        public static void OnProcessExit(object sender, EventArgs e)
        {
            General.ls.should_listen = false;
            while (!General.ls.finished)
            {
                
            }
            General.question.Abort();
            Storage.Serialize(General.s);
            Console.WriteLine(@"I'm out of here");
            General.sc.SendMessage("bot", "Shutting down!");
            General.ls = null;
            General.sc = null;
            General.ws.ws.Close();
            General.ws.ws.Dispose();
            General.ws = null;
        }
        #endregion 
    }
}