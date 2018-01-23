#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library
{
    public class Utility
    {

        #region public static methods

        public static string RemoveTailingBackSlash(string path)
        {
            if (path.EndsWith(@"\"))
            {
                return path.Substring(0, path.Length - 1);
            }
            return path;
        }

        public static object GetValue(Project proj, string key)
        {
            object value = null;
            foreach (Property itemprop in proj.Properties)
            {
                try
                {

                    if (itemprop.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        value = itemprop.Value;
                        break;
                    }
                }
                catch
                {
                    // Do nothing
                }
            }
            return value;
        }

        public static string CombinePaths(string path1, string path2)
        {
            string result = string.Empty;
            if (path1.EndsWith(@"\") && path2.StartsWith(@"\"))
            {
                result = path1 + path2.Substring(1);
            }
            else
                if (!path1.EndsWith(@"\") && !path2.StartsWith(@"\"))
                {
                    result = path1 + @"\" + path2;
                }
                else
                {
                    result = path1 + path2;
                }
            return result;
        }

        #endregion
    }
}