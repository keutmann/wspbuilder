using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Framework.Serialization;

namespace Keutmann.SharePoint.WSPBuilder.Framework.IO
{
    public class FileSystem
    {
        public static string Combine(string path, string name)
        {
            return path + name;
        }

        public static string Combine(DirectoryInfo dir, string name)
        {
            return dir.FullName + @"\" + name;
        }

        public static void Copy(string source, string target, bool overwrite)
        {
            DirectorySystem.EnsureFileDirectory(target);
            File.Copy(source, target, overwrite);
        }

        public static void Move(string source, string target, bool overwrite)
        {
            DirectorySystem.EnsureFileDirectory(target);
            if (File.Exists(target))
            {
                if (overwrite)
                {
                    File.Delete(target);
                }
                else
                {
                    return;
                }
            }
            File.Move(source, target);
        }


        public static DirectoryInfo EnsureDirectory(string fullPath)
        {
            DirectoryInfo resultDir = null;
            if (!Directory.Exists(fullPath))
            {
                resultDir = Directory.CreateDirectory(fullPath);
            }
            else
            {
                resultDir = new DirectoryInfo(fullPath);
            }
            return resultDir;
        }

        public static DirectoryInfo EnsureDirectory(DirectoryInfo currentDir, string name)
        {
            DirectoryInfo resultDir = null;
            string fullPath = FileSystem.Combine(currentDir, name);
            if (!Directory.Exists(fullPath))
            {
                resultDir = currentDir.CreateSubdirectory(name);
            }
            else
            {
                resultDir = new DirectoryInfo(fullPath);
            }
            return resultDir;
        }

        public static DirectoryInfo EnsureDirectory(string currentDir, string name)
        {
            DirectoryInfo resultDir = null;
            string fullPath = Path.Combine(currentDir, name);
            if (!Directory.Exists(fullPath))
            {
                resultDir = Directory.CreateDirectory(fullPath);
            }
            else
            {
                resultDir = new DirectoryInfo(fullPath);
            }
            return resultDir;
        }

        public static void Create(DirectoryInfo dir, string filename, object obj)
        {
            Create(dir.FullName, filename, obj);
        }

        public static void Create(string directoryPath, string filename, object obj)
        {
            string fullPath = Path.Combine(directoryPath, filename);
            string xml = Serializer.FormatXml(Serializer.ObjectToXML(obj, false));
            xml = xml.Insert(xml.IndexOf('>') + 1, String.Format("\r\n<!-- Created by WSPDev - {0} -->", DateTime.Now.ToString()));
            File.WriteAllText(fullPath, xml, Encoding.UTF8);
        }

        public static T Load<T>(string path)
        {
            T result = default(T);
            if (File.Exists(path))
            {
                result = Serializer.XmlToObject<T>(File.ReadAllText(path));
            }
            return result;
        }

        public static T LoadOrCreate<T>(string path)
        {
            T result = default(T);
            if (File.Exists(path))
            {
                result = Serializer.XmlToObject<T>(File.ReadAllText(path));
            }
            if (result == null)
            {
                result = Activator.CreateInstance<T>();
            }
            return result;
        }



    }
}
