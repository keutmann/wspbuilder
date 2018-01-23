using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WSPTools.BaseLibrary.Extensions
{
    public static  class FileExtensions
    {
        /// <summary>
        /// Reads all the text from a text file safely with minimum of locks on the file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadAllTextSafely(string path)
        {
            Encoding coding;
            return ReadAllTextSafely(path, out coding);
        }

        public static string ReadAllTextSafely(string path, out Encoding coding)
        {
            string result = null;
            coding = Encoding.UTF8;

            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (path.Length == 0)
            {
                throw new ArgumentException("Path cannot be empty!");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException("File do not exists at " + path);
            }

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader br = new StreamReader(fs))
                {
                    result = br.ReadToEnd();
                    coding = br.CurrentEncoding;
                }
            }
            return result;
        }
    }
}
