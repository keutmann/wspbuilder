using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Keutmann.Framework
{
    public static class StringExtensions
    {
        /// <summary>
        /// Fast incase sensitive replacement.
        /// http://www.codeproject.com/KB/string/fastestcscaseinsstringrep.aspx
        /// </summary>
        /// <param name="original"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceFast(string original, string pattern, string replacement, bool caseSensitive)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;

            string upperString = (caseSensitive) ? original : original.ToUpper();
            string upperPattern = (caseSensitive) ? pattern : pattern.ToUpper();

            int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];

            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                {
                    chars[count++] = original[i];
                }
                
                for (int i = 0; i < replacement.Length; ++i)
                {
                    chars[count++] = replacement[i];
                }

                position0 = position1 + pattern.Length;
            }

            if (position0 == 0)
            {
                return original;
            }

            for (int i = position0; i < original.Length; ++i)
            {
                chars[count++] = original[i];
            }

            return new string(chars, 0, count);
        }

        /// <summary>
        /// Replaces multiple strings in a single string. 
        /// CaseInsentive
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string MultipleReplace(string text, NameValueCollection replacements)
        {
            return MultipleReplace(text, replacements, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Replaces multiple strings in a single string.
        /// Use the code like this.            
        /// string temp = "Jonathan Smith is a developer";            
        /// adict.Add("Jonathan", "David");            
        /// adict.Add("Smith", "Seruyange");            
        /// string rep = MultipleReplace(temp, adict);
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string MultipleReplace(string text, NameValueCollection replacements, RegexOptions options) 
        { 
            if (replacements != null && replacements.Keys.Count > 0)
            {
                return Regex.Replace(text,
                        "(" + String.Join("|", replacements.AllKeys) + ")", 
                        delegate(Match m) 
                        {
                            return replacements[m.Value]; 
                        },
                        options);
            }
            return text;
        }

        public static bool MultipleReplace(string text, Dictionary<string, string> replacements, out string result, RegexOptions options)
        {
            result = text;
            bool replaced = false;
            if (replacements != null && replacements.Keys.Count > 0)
            {
                string[] keys = new List<string>(replacements.Keys).ToArray();
                string search = "(" + String.Join("|", keys) + ")";
                MultipleReplace(text, search, replacements, out result, options);
            }
            return replaced;
        }

        /// <summary>
        /// For speed the search parameter has to be defined from start!
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replacements"></param>
        /// <param name="result"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool MultipleReplace(string text, string search, Dictionary<string, string> replacements, out string result, RegexOptions options)
        {
            result = text;
            bool replaced = false;
            if (!String.IsNullOrEmpty(search))
            {
                result = Regex.Replace(text,
                        search,
                        delegate(Match m)
                        {
                            replaced = true;
                            return replacements[m.Value];
                        },
                        options);
            }
            return replaced;
        }

        public static string NamedFormat(string text, NameValueCollection replacements)
        {
            return NamedFormat(text, replacements, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public static string NamedFormat(string text, NameValueCollection replacements, RegexOptions options)
        {
            if (replacements != null && replacements.Keys.Count > 0)
            {
                return Regex.Replace(text,
                        @"(\{" + String.Join(@"\}|\{", replacements.AllKeys) + @"\})",
                        delegate(Match m)
                        {
                            string key = m.Value.Substring(1, m.Value.Length - 2);
                            return replacements[key];
                        },
                        options);
            }
            return text;
        }

        public static string CommentCode(string text)
        {
            StringBuilder sb = new StringBuilder();
            string[] textLines = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.None);

            foreach (string line in textLines)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    sb.Append("// " + line.TrimStart(new char[] { ' ', '\t' }) + Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        public static string EscapeXml(string s )
        {
          string xml = s;
          if ( !string.IsNullOrEmpty( xml ) )
          {
            // replace literal values with entities
            xml = xml.Replace( "&", "&amp;" );
            xml = xml.Replace( "<", "&lt;" );
            xml = xml.Replace( ">", "&gt;" );
            xml = xml.Replace( "\"", "&quot;" );
            xml = xml.Replace( "'", "&apos;" );
          }
          return xml;
        }

        public static string UnescapeXml(string s )
        {
          string unxml = s;
          if ( !string.IsNullOrEmpty( unxml ) )
          {
            // replace entities with literal values
            unxml = unxml.Replace( "&apos;", "'" );
            unxml = unxml.Replace( "&quot;", "\"" );
            unxml = unxml.Replace( "&gt;", ">" );
            unxml = unxml.Replace( "&lt;", "<" );
            unxml = unxml.Replace( "&amp;", "&" );
          }
          return unxml;
        }
 
    }
}
