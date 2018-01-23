using System;
using System.IO;
using System.Reflection;

namespace WSPTools.BaseLibrary.IO
{
    public class ResourceReader
    {
        public const string RESOURCE_PATH = "Resources";

        public const string MANIFEST_CONFIG = "Manifest.Config";
        public const string SOLUTIONID_TXT = "SolutionID.txt";

        public string Namespace { get; set; }
        public string OutputPath { get; set; }

        public Assembly CurrentAssembly { get; set; }

        public ResourceReader(string outputPath) : this(Assembly.GetAssembly(typeof(ResourceReader)), outputPath)
        {
        }

        public ResourceReader(Assembly assembly, string outputPath)
        {
            this.CurrentAssembly = assembly;
            this.Namespace = String.Format("{0}.{1}", "WSPTools.BaseLibrary", RESOURCE_PATH);
            this.OutputPath = outputPath;
        }

        public static string ReadString(Assembly assembly, string assemblyNamespace, string name)
        {
            string result = string.Empty;

            string key = String.Format("{0}.{1}.{2}", assemblyNamespace, RESOURCE_PATH, name);

            using (Stream source = assembly.GetManifestResourceStream(key))
            {
                if (source != null)
                {
                    using (StreamReader sr = new StreamReader(source))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            return result;
        }

        public void CreateFile(string filename)
        {
            string fullFilename = Path.Combine(this.OutputPath, filename);
            if (!File.Exists(fullFilename))
            {
                string resourceName = String.Format("{0}.{1}", this.Namespace, filename);
                using (Stream source = this.CurrentAssembly.GetManifestResourceStream(resourceName))
                {
                    long length = source.Length;
                    byte[] data = new byte[length];
                    source.Read(data, 0, (int)length);

                    using (FileStream fs = File.OpenWrite(fullFilename))
                    {
                        fs.Write(data, 0, (int)length);
                    }
                }
            }
        }

    }
}
