using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TestWSPBuilder.Library
{
    public class ManifestHandler
    {
        private string _manifestPath = string.Empty;
        private Solution _manifest = null;


        public string ManifestPath
        {
            get { return _manifestPath; }
            set { _manifestPath = value; }
        }

        public Solution Manifest
        {
            get 
            {
                if (_manifest == null)
                {
                    if (File.Exists(ManifestPath))
                    {
                        string xml = string.Empty;
                        using (StreamReader sr = new StreamReader(ManifestPath))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Solution));
                            _manifest = (Solution)serializer.Deserialize(sr);
                            sr.Close();
                        }
                        if (_manifest == null)
                        {
                            throw new ApplicationException("Unable to Deserialize Manifest xml to object!");
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Can not find file : " + ManifestPath);
                    }
                }
                return _manifest; 
            }
            set { _manifest = value; }
        }

        public ManifestHandler(string manifestpath)
        {
            this.ManifestPath = manifestpath;
        }
    }
}
