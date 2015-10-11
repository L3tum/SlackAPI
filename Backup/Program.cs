using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackAPI;

namespace Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                if (Process.GetProcessesByName("SlackBot").Length == 0)
                {
                    Process.Start(Helper.GetApplicationPath() + "/SlackBot.exe");
                }
            }
        }
    }
}
