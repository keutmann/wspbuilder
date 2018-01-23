using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace WSPTools.BaseLibrary.Extensions
{
    public static class AssemblyNameExtensions
    {
        public static string GetPublicKey(AssemblyName assemblyName)
        {
            StringBuilder sb = new StringBuilder();

            byte[] token = assemblyName.GetPublicKey();

            foreach (byte b in token)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string GetPublicKeyToken(AssemblyName assemblyName)
        {
            StringBuilder sb = new StringBuilder();

            byte[] token = assemblyName.GetPublicKeyToken();

            foreach (byte b in token)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string GetCulture(AssemblyName assemblyName)
        {
            string result = "neutral";

            if (assemblyName.CultureInfo != null)
            {
                CultureInfo info = assemblyName.CultureInfo;
                if (!info.IsNeutralCulture && !String.IsNullOrEmpty(info.ToString()))
                {
                    result = info.ToString();
                }
            }
            return result;
        }

        public static string GetVersion(AssemblyName assemblyName)
        {
            string result = string.Empty;

            if (assemblyName.Version != null)
            {
                result = assemblyName.Version.ToString();
            }
            return result;
        }

    }
}
