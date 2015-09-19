using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackAPI;

namespace SlackBot
{
    class Listener
    {
        public bool should_listen = true;
        public WebSlack ws;

        public Listener(WebSlack ws)
        {
            this.ws = ws;
        }

        public void Listen()
        {
            while (should_listen)
            {
                if (ws.changed)
                {
                    Console.WriteLine(ws.Response);
                    Worker.MessageWorker(ws.Response);
                    ws.changed = false;
                }
            }
        }
    }
}
