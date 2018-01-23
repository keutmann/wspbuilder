using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using WSPTools.BaseLibrary.Win32;
using WSPTools.BaseLibrary.SharePoint;
using WSPTools.BaseLibrary.Xml.Serialization;
using WSPTools.BaseLibrary.Extensions;

namespace WSPTools.BaseLibrary.IO
{
    public static class ManifestExtensions
    {

        public static bool DoManifestExits(string projectPath)
        {
            string filename = null;
            if (SharePointRegistry.IsSharePoint12)
            {
                filename = Path.Combine(projectPath, ResourceReader.SOLUTIONID_TXT);
            }
            else
            {
                filename = Path.Combine(projectPath, ResourceReader.MANIFEST_CONFIG);
            }

            return File.Exists(filename);
        }


        public static string CreateManifestFile(string projectPath, string solutionId)
        {
            string filename = null;
            if (SharePointRegistry.IsSharePoint12)
            {
                // Create the old solutionid.txt for SharePoint 2007 projects
                filename = Path.Combine(projectPath, ResourceReader.SOLUTIONID_TXT);
                if (!File.Exists(filename))
                {
                    File.WriteAllText(filename, solutionId);
                }
            }
            else
            {
                filename = Path.Combine(projectPath, ResourceReader.MANIFEST_CONFIG);

                if (!File.Exists(filename))
                {
                    // Create the manifest.config for SharePoint 2010 projects
                    ResourceReader creator = new ResourceReader(projectPath);
                    creator.CreateFile(ResourceReader.MANIFEST_CONFIG);
                    ManifestExtensions.UpdateSolutionID(filename, solutionId);
                }
            }
            return filename;
        }


        public static void UpdateSolutionID(string path, string id)
        {
            //bool result = false;
            //string xml = FileExtensions.ReadAllTextSafely(path);
            //Solution manifest = Serializer.XmlToObject<Solution>(xml);

            //if (manifest.SolutionId == Guid.Empty.ToString())
            //{
            //    manifest.SolutionId = id;
            //    xml = Serializer.ObjectToXML(manifest, true);
            //    File.WriteAllText(path, xml);
            //    result = true;
            //}

            string data = File.ReadAllText(path);
            string result = data;
            string solutionIDAttributeName = "SolutionId";

            string oldString = String.Format("{0}=\"{1}\"", solutionIDAttributeName, Guid.Empty.ToString());
            string newString = String.Format("{0}=\"{1}\"", solutionIDAttributeName, id);

            // Replace the string
            int index = data.IndexOf(oldString, 0, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                result = data.Substring(0, index) + newString + data.Substring(index + newString.Length);
            }
            File.WriteAllText(path, result);
            //return result;
        }


        public static string GetSolutionID(string projectPath)
        {
            string result = string.Empty;

            string filename = null;
            if (SharePointRegistry.IsSharePoint12)
            {
                filename = Path.Combine(projectPath, ResourceReader.SOLUTIONID_TXT);
                result = FileExtensions.ReadAllTextSafely(filename);
            }
            else
            {
                filename = Path.Combine(projectPath, ResourceReader.MANIFEST_CONFIG);
                string xml = FileExtensions.ReadAllTextSafely(filename);
                Solution manifest = Serializer.XmlToObject<Solution>(xml);
                result = manifest.SolutionId;
            }
            return result;
        }
    }
}
