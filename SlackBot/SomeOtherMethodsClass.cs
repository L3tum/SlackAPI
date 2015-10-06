using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using SlackAPI;

namespace SlackBot
{
    public static class SomeOtherMethodsClass
    {
        public static DateTime endTime;
        public static String PollName;

        public static void PollEndDeterminer()
        {
            Timer t = new Timer();
            while (((endTime - DateTime.Now) > TimeSpan.Zero))
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
            if (PollName != "null")
            {
                General.s.polls[PollName].isRunning = false;
                General.s.setNextPoll();
            }
        }

        //Method has to be static
        //Method name has to be "MyMethod" and Type name has to be "MyProgram" and Namespace name has to be "MyNamespace"
        public static Eval CreateEval(string sCSCode, bool print, string desc)
        {
            CSharpCodeProvider ccc = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("SlackBot.exe");
            cp.ReferencedAssemblies.Add("SlackAPI.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            TextWriter tw = new StreamWriter("C:/Users/Tom Niklas/Desktop/namee.txt");
            String name = ((String) General.sc.caller.CallAPIXML("http://randomword.setgetgo.com/get.php", new Dictionary<string, dynamic>()).Result).Trim();
            tw.WriteLine(Helper.GetApplicationPath() + "/" + name + ".dll");
            tw.Close();
            cp.OutputAssembly = Helper.GetApplicationPath() + "/" + name + ".dll";
            String finalcode =
                "using System; \nusing SlackBot; \nusing SlackAPI; \nusing System.Data;\nnamespace MyNamespace { \npublic class MyProgram { \npublic static String MyMethod(String text){\n " +
                sCSCode + "\n } \n } \n }";
            CompilerResults cr = ccc.CompileAssemblyFromSource(cp, finalcode);

            if (cr.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                TextWriter sw = new StreamWriter("C:/Users/Tom Niklas/Desktop/sjdbshadbahjd.cs");
                sw.Write(finalcode);
                sw.WriteLine();
                sw.WriteLine(cr.CompiledAssembly.Location + "/" + cr.CompiledAssembly.FullName);
                sw.Close();

                foreach (CompilerError error in cr.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}): {1} : {2} : {3}", error.ErrorNumber, error.ErrorText, error.Line, error.Column));
                }

                throw new InvalidOperationException(sb.ToString());
            }
            String path = (((cr.CompiledAssembly.Location.Replace("file:///", "")).Replace("\\", "/")).Trim());
            return new Eval(print, path, desc);
        }
    }
}
