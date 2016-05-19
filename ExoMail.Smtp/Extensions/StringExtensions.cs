using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Extensions
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string str)
        {
            return new MemoryStream(Encoding.ASCII.GetBytes(str));
        }

        public static string WordWrap(this string str, int cols)
        {
            return WordWrap(str, cols, string.Empty);
        }

        public static string WordWrap(this string str, int cols, string indent)
        {
            string[] words = str.Split(' ');
            var sb = new StringBuilder();
            int colIndex = 0;
            string space = string.Empty;

            for (int i = 0; i < words.Count(); i++)
            {
                space = i == words.Count() ? string.Empty : " ";
                colIndex += words[i].Length + space.Length;

                if (colIndex <= cols)
                {
                    sb.Append(string.Format("{0}{1}", words[i], space));
                }
                else
                {
                    colIndex = 0;
                    sb.Append(string.Format("\r\n{0}{1}{2}", indent, words[i], space));
                }
            }
            return sb.ToString();
        }
    }
}
