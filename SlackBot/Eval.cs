using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SlackBot
{
    public class Eval
    {
        public bool printToChannel;
        public String path;
        public String desc;

        public Eval(bool print, String path, String desc)
        {
            printToChannel = print;
            this.path = path;
            this.desc = desc;
        }

        public Eval()
        {
            
        }
    }
}
