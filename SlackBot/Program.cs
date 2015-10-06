﻿using System;
using System.Diagnostics;
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
                    sc = new SlackClient(args[0]);
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
            s.SetUp(another);

            var ls = new Listener(General.ws);
            General.ls = ls;
            var question = new Thread(ls.Listen);
            General.question = question;
            question.Start();

            General.sc = sc;
            General.ws = ws;
            General.ls = ls;

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
            Console.WriteLine("I'm out of here");
            General.sc.SendMessage("bot", "Shutting down!");
        }
    }
}