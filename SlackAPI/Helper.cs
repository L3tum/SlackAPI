using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

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
            return deserializer.Deserialize<Dictionary<string, T>> (json);
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
    }
}