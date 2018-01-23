using System;
using System.IO;

namespace WSPTools.BaseLibrary.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static void EnsureDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            EnsureDirectory(dir);
        }


        public static void EnsureDirectory(DirectoryInfo oDirInfo)
        {
            if (oDirInfo.Parent != null)
                EnsureDirectory(oDirInfo.Parent);

            if (!oDirInfo.Exists)
            {
                oDirInfo.Create();
            }
        }
    }
}