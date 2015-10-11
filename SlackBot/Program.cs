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
            bool another = false;
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
                    another = true;
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
            General.sc = sc;

            var ws = new WebSlack();
            ws.CreateWebSocket(sc.URL);
            General.ws = ws;

            var s = new Storage();
            General.s = s;
            s.SetUp();

            var ls = new Listener(General.ws);
            General.ls = ls;
            var question = new Thread(ls.Listen);
            General.question = question;
            question.Start();

            General.sc = sc;
            General.ws = ws;
            General.ls = ls;

            if (!File.Exists(Helper.GetApplicationPath() + "/SlackBotBackup.exe"))
            {
                String things = Helper.GetApplicationPath().Remove(Helper.GetApplicationPath().Length - 16, 16);
                File.Copy(things + "Backup/bin/Debug/SlackBotBackup.exe", Helper.GetApplicationPath() + "/SlackBotBackup.exe");
            }
            if (Process.GetProcessesByName("SlackBotBackup").Length == 0)
            {
                Process.Start(Helper.GetApplicationPath() + "/SlackBotBackup.exe");
            }

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += OnProcessExit;
            sc.SendMessage("bot", "HELLO GUYS! IT'S ME, ANAL MOLLY!\n Only Testin'!");
        }

        public static void OnProcessExit(object sender, EventArgs e)
        {
            General.ls.should_listen = false;
            Storage.Serialize(General.s);
            while (!General.ls.finished)
            {
                
            }
            General.question.Abort();
            Console.WriteLine("I'm out of here");
            General.sc.SendMessage("bot", "Shutting down!");
        }
    }
}