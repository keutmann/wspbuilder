/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;

using Keutmann.SharePoint.WSPBuilder.Library;
using System.IO;
using System.Xml;
using System.Collections;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public partial class SolutionHandler
    {
        #region Memebers

        #endregion

        #region Properties


        #endregion


        #region Methods

        private bool DoLocationExist(List<SiteDefinitionManifestFileReference> siteDefinitionManifestFileReferenceList, string location)
        {
            if (!Config.Current.BuildSolution)
            {
                // Do not check for existing location files, because its not possible in a project only build.
                return false;
            }

            bool found = false;
            foreach (SiteDefinitionManifestFileReference siteDefinitionManifest in siteDefinitionManifestFileReferenceList)
            {
                if (siteDefinitionManifest.Location.Equals(location, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    Log.Warning("Multiple site definitions found with the same name '" + siteDefinitionManifest.Location + "', therefore only the first one found is used.");
                    break;
                }
            }
            return found;
        }


        /// <summary>
        /// Find and load all webtemplates files in the solution
        /// </summary>
        /// <param name="parentDir"></param>
        /// <param name="webTemps"></param>
        private void LoadWebTemplates(DirectoryInfo parentDir, Dictionary<string, XmlDocument> webTemps)
        {
            if (parentDir.Name.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (FileInfo file in parentDir.GetFiles("*.xml"))
                {
                    // Is the file a webtemp*.xml file
                    if (file.Name.StartsWith("webtemp", StringComparison.InvariantCultureIgnoreCase))
                    {
                        XmlDocument webTempDoc = new XmlDocument();
                        webTempDoc.Load(file.FullName);
                        webTemps.Add(file.FullName, webTempDoc);
                    }
                }
            }

            foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                LoadWebTemplates(childDir, webTemps);
            }
        }


        private WebTempFileDefinition[] BuildWebTempFileDefinition(DirectoryInfo templateDir, string siteDefinitionName, int pathIndex)
        {
            List<WebTempFileDefinition> webTempFileDefinitionList = new List<WebTempFileDefinition>();

            foreach (KeyValuePair<string, XmlDocument> entry in GetAllWebTemplateDocuments(templateDir))
            {
                XmlDocument webTempDoc = entry.Value;

                XmlNode templateNode = webTempDoc.SelectSingleNode("Templates/Template[@Name='" + siteDefinitionName + "']");
                if (templateNode != null)
                {
                    string fileFullName = entry.Key;
                    string filePath = fileFullName.Substring(pathIndex);

                    FileInfo file = new FileInfo(fileFullName);
                    if (FileProvider.IncludeFile(file))
                    {
                        WebTempFileDefinition webTempFileDefinition = new WebTempFileDefinition();
                        webTempFileDefinition.Location = filePath;
                        webTempFileDefinitionList.Add(webTempFileDefinition);

                        if (!ReservedFiles.ContainsKey(fileFullName))
                        {
                            // Add the file to the list of reserved files so it do not get included in the TemplateFiles section
                            this.ReservedFiles.Add(fileFullName, file);

                            // Add the file to list of cab files
                            //this.CabFiles.Add(new string[] { fileFullName, filePath });
                            this.AddToCab(fileFullName, filePath);
                        }
                    }
                }
            }

            if (webTempFileDefinitionList.Count == 0)
            {
                return null;
            }

            return webTempFileDefinitionList.ToArray();
        }



        private void FindWebTempFiles(DirectoryInfo parentDir, List<WebTempFileDefinition> webTempFileDefinitionList, string siteDefinitionName, int pathIndex)
        {
            string webTempFileName = "webtemp" + siteDefinitionName + ".xml";

            foreach (FileInfo file in parentDir.GetFiles())
            {

                // parentDir.Name.Equals(siteDefinitionName, StringComparison.InvariantCultureIgnoreCase))
                // Do the file match the SiteDefinition name and directory
                if (file.Name.Equals(webTempFileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Create a new WebTempFileDefinition for the file
                    WebTempFileDefinition webTempFileDefinition = new WebTempFileDefinition();
                    webTempFileDefinition.Location = file.FullName.Substring(pathIndex);

                    // Add the file to the list of reserved files so it do not get included in the TemplateFiles section
                    this.ReservedFiles.Add(file.FullName, file);
                    
                    // Add the file to list of cab files
                    string filePath = file.FullName.Substring(pathIndex);
                    //this.CabFiles.Add(new string[] { file.FullName, filePath });
                    this.AddToCab(file.FullName, filePath);


                    webTempFileDefinitionList.Add(webTempFileDefinition);
                }
            }

            foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                FindWebTempFiles(childDir, webTempFileDefinitionList, siteDefinitionName, pathIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentDir"></param>
        /// <param name="pathIndex"></param>
        private void AddSiteDefinitionManifestFiles(DirectoryInfo parentDir, int pathIndex)
        {
            foreach (FileInfo file in parentDir.GetFiles())
            {
                if (FileProvider.IncludeFile(file))
                {
                    string filePath = file.FullName.Substring(pathIndex);

                    //this.CabFiles.Add(new string[] { file.FullName, filePath });
                    this.AddToCab(file.FullName, filePath);

                }
            }
            foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                AddSiteDefinitionManifestFiles(childDir, pathIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteDefinitionsDir"></param>
        /// <returns></returns>
        public SiteDefinitionManifestFileReference[] BuildSiteDefinitionManifestFileReference(DirectoryInfo siteDefinitionsDir, SiteDefinitionManifestFileReference[] siteDefinitionManifests)
        {
            List<SiteDefinitionManifestFileReference> siteDefinitionManifestFileReferenceList =
                (siteDefinitionManifests != null) ? new List<SiteDefinitionManifestFileReference>(siteDefinitionManifests) : new List<SiteDefinitionManifestFileReference>();

            int webTemplatePathIndex = siteDefinitionsDir.FullName.LastIndexOf(@"\") + 1;

            foreach (DirectoryInfo childDir in FileProvider.GetDirectories(siteDefinitionsDir))
            {
                if (!DoLocationExist(siteDefinitionManifestFileReferenceList, childDir.Name))
                {
                    // Create a new SiteDefinitionManifestFileReference
                    SiteDefinitionManifestFileReference siteDefinitionManifestFileReference = new SiteDefinitionManifestFileReference();
                    siteDefinitionManifestFileReference.Location = childDir.Name;

                    Log.Verbose("SiteDefinition added: " + siteDefinitionManifestFileReference.Location);

                    // find the webtemp configuration for the site definition
                    WebTempFileDefinition[] arrWebTempFileDefinitions = BuildWebTempFileDefinition(siteDefinitionsDir.Parent, childDir.Name, webTemplatePathIndex);

                    // Test that there is a webtemp configuration for this site definition
                    if (arrWebTempFileDefinitions != null)
                    {
                        siteDefinitionManifestFileReference.WebTempFile = AppendArray<WebTempFileDefinition>(siteDefinitionManifestFileReference.WebTempFile, arrWebTempFileDefinitions);

                        int siteDefinitionPathIndex = childDir.FullName.LastIndexOf(@"\") + 1;

                        // Add files to the cab list
                        AddSiteDefinitionManifestFiles(childDir, siteDefinitionPathIndex);

                        siteDefinitionManifestFileReferenceList.Add(siteDefinitionManifestFileReference);

                    }
                    else
                    {
                        // did not find a webtemp configuration for the site definition,
                        // therefore add to IncludeTemplateDirectory list, so the files
                        // gets included by the TemplateFiles section.
                        this.IncludeTemplateDirectory.Add(childDir);

                        Log.Warning("No webtemp configuration for sitedefinition '" + siteDefinitionManifestFileReference.Location + "' was found.");

                    }
                }
            }

            if (siteDefinitionManifestFileReferenceList.Count == 0)
            {
                return null;
            }
            return siteDefinitionManifestFileReferenceList.ToArray();
        }

        private Dictionary<string, XmlDocument> GetAllWebTemplateDocuments(DirectoryInfo templateDir)
        {
            Dictionary<string, XmlDocument> webTemps = new Dictionary<string, XmlDocument>(StringComparer.InvariantCultureIgnoreCase);

            LoadWebTemplates(templateDir, webTemps);

            return webTemps;
        }


        #endregion
    }
}
