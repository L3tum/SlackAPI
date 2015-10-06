using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using Microsoft.CSharp;

namespace SlackAPI
{
    public static class Helper
    {
        /// <summary>
        ///     Serializes an Object to a Json String
        /// </summary>
        /// <param name="obj = your object you want to serialize"></param>
        /// <returns>Json String</returns>
        public static string ToJSON(this object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
        /// <summary>
        /// Deserializes a JSON String to an Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object ToObject<T>(this string json)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Deserializes a Json String to a Dictionary
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Dictionary<string, dynamic> ToDictionary(this string json)
        {
            var deserializer = new JavaScriptSerializer();
            return deserializer.Deserialize<Dictionary<string, dynamic>>(json);
        }


        /// <summary>
        /// Deserializes a Json String to a Dictionary with the given Type as value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Dictionary<string, T> ToDictionary<T>(this string json)
        {
            var deserializer = new JavaScriptSerializer();
            return deserializer.Deserialize<Dictionary<string, T>>(json);
        }

        public static byte[] ToBytes(this string str)
        {
            var bytes = new byte[str.Length*sizeof (char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string ToString(this byte[] bytes)
        {
            var chars = new char[bytes.Length/sizeof (char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string ToString(this object thing)
        {
            return (string)thing.ToString();
        }

        public static string ReplaceForSlack(this string s)
        {
            s = s.Replace("&", "&amp;");
            s = s.Replace(">", "&gt;");
            s = s.Replace("<", "&lt;");
            return s;
        }

        public static string GetApplicationPath()
        {
            String tmp = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            tmp = tmp.Replace("file:\\", "");
            tmp = tmp.Replace("\\", "/");
            tmp = tmp.Trim();
            return tmp;
        }

        public static Dictionary<String, dynamic> Eval(string nameSpace, string classname, string sCSCode, string[] parameters, string[] imports, string[] usings)
        {

            CSharpCodeProvider c = new CSharpCodeProvider();
            ICodeCompiler icc = c.CreateCompiler();
            

            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("SlackAPI.dll");
            cp.ReferencedAssemblies.Add("SlackBot.exe");
            foreach (string VARIABLE in imports)
            {
                try
                {
                    if ((VARIABLE != "null") && !String.IsNullOrEmpty(VARIABLE))
                    {
                        cp.ReferencedAssemblies.Add(VARIABLE);
                    }
                }
                catch
                {
                    
                }
            }

            cp.CompilerOptions = "/t:library";
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            cp.OutputAssembly = nameSpace + classname + ".dll";

            StringBuilder sb = new StringBuilder("");
            sb.Append("using System;\n");
            sb.Append("using System.Xml;\n");
            sb.Append("using System.Data;\n");
            sb.Append("using SlackAPI;\n");
            sb.Append("using SlackBot;\n");
            sb.Append("using System.Collections.Generic;\n");

            foreach (string VARIABLE in usings)
            {
                try
                {
                    if ((VARIABLE != "null") && !String.IsNullOrEmpty(VARIABLE))
                    {
                        sb.Append("using " + VARIABLE + ";\n");
                    }
                }
                catch
                {
                    
                }
            }

            sb.Append("namespace " + nameSpace + "{ \n");
            sb.Append("public class " + classname + "{ \n");
            sb.Append("public object EvalCode(");
            foreach (var VARIABLE in parameters)
            {
                if ((VARIABLE != "null") && !String.IsNullOrEmpty(VARIABLE))
                {
                    sb.Append(VARIABLE + ", ");
                }
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("){\n");
            sb.Append(sCSCode + "\n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");

            TextWriter sw = new StreamWriter("C:/Users/Tom Niklas/Desktop/hahdbashdbd.txt");
            sw.Write(sb);
            sw.Close();

            CompilerResults cr = icc.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                Console.WriteLine("False eval!");
                foreach (CompilerError VARIABLE in cr.Errors)
                {
                    Console.WriteLine(VARIABLE.ToString());
                }
                return null;
            }

            try
            {
                System.Reflection.Assembly a = cr.CompiledAssembly;
                object o = a.CreateInstance(nameSpace + "." + classname);
                Type t = a.GetType(classname);
                Dictionary<String, dynamic> rer = new Dictionary<string, dynamic>
                {
                    {"namespace", nameSpace},
                    {"classname", classname},
                    {"Assembly", a.GetName()}
                };
                return rer;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public static String GetXMLElement(this String xml, String element)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                reader.ReadToFollowing(element);
                try
                {
                    reader.ReadToFollowing(element);
                    String s = reader.ReadElementContentAsString();
                    return s;
                }
                catch (XmlException e)
                {
                    //reader.ReadToFollowing(element);
                    //return reader.ReadElementContentAsString();
                    TextWriter sw = new StreamWriter("C:/Users/Tom Niklas/Desktop/tsdfsfds.xml");
                    sw.Write(xml);
                    sw.Close();
                    return e.ToString();
                }
            }
        }
    }
}