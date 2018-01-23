/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Xml.Serialization;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public partial class SolutionHandler
    {
        #region Members

        private Dictionary<string, FileInfo> _reservedFiles = new Dictionary<string, FileInfo>(StringComparer.InvariantCultureIgnoreCase);

        private List<DirectoryInfo> _includeTemplateDirectory = new List<DirectoryInfo>();

        #endregion

        #region Properties 

        [XmlIgnore]
        public Dictionary<string, FileInfo> ReservedFiles
        {
            get { return _reservedFiles; }
            set { _reservedFiles = value; }
        }

        [XmlIgnore]
        public List<DirectoryInfo> IncludeTemplateDirectory
        {
            get { return _includeTemplateDirectory; }
            set { _includeTemplateDirectory = value; }
        }

        #endregion

        #region Methods

        private bool DoLocationExist(List<TemplateFileReference> templateFileReferenceList, string location)
        {
            if (!Config.Current.BuildSolution)
            {
                // Do not check for Existing location files, because its not possible in a project only build.
                return false;
            }

            bool found = false;
            foreach (TemplateFileReference templateFileReference in templateFileReferenceList)
            {
                if (templateFileReference.Location.Equals(location, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    Log.Warning("Multiple template files found with the same name '" + templateFileReference.Location + "', therefore only the first one found is used.");
                    break;
                }
            }
            return found;
        }

        private void AddTemplateFileReferences(DirectoryInfo parentDir, List<TemplateFileReference> templateFileReferenceList, int pathIndex)
        {
            foreach(FileInfo file in parentDir.GetFiles())
            {
                if (FileProvider.IncludeFile(file))
                {
                    string locationPath = file.FullName.Substring(pathIndex);

                    if (!DoLocationExist(templateFileReferenceList, locationPath))
                    {
                        // Do not include webtemp*.xml file, because they are handle by the SiteDefinitionManifest
                        if (!this.ReservedFiles.ContainsKey(file.FullName))
                        {
                            TemplateFileReference templateFileReference = new TemplateFileReference();
                            templateFileReference.Location = locationPath;

                            templateFileReferenceList.Add(templateFileReference);

                            Log.Verbose("TemplateFile added: " + locationPath);

                            //this.CabFiles.Add(new string[] { file.FullName, locationPath });
                            this.AddToCab(file.FullName, locationPath);
                        }
                    }
                }
            }

            foreach(DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                AddTemplateFileReferences(childDir, templateFileReferenceList, pathIndex);
            }
        }


        public TemplateFileReference[] BuildTemplateFileReference(DirectoryInfo parentDir, TemplateFileReference[] templateFileReferences)
        {
            List<TemplateFileReference> templateFileReferenceList = 
                (templateFileReferences != null) ? new List<TemplateFileReference>(templateFileReferences) : new List<TemplateFileReference>();

            int parentPathIndex = parentDir.FullName.Length + 1; // Plus one for the slash

            // Add features
            string pathFeatures = parentDir.FullName + @"\features";
            if (Directory.Exists(pathFeatures))
            {
                DirectoryInfo featuresDir = new DirectoryInfo(pathFeatures);

                this.Solution.FeatureManifests = BuildFeatureManifestReference(featuresDir, this.Solution.FeatureManifests);

                // Add Features resources files.
                this.Solution.Resources = AppendArray<ResourceDefinition>(this.Solution.Resources, BuildResourceDefinition(featuresDir));

            }   

            // Add sitedefinitions
            if(Directory.Exists(parentDir.FullName+@"\sitetemplates"))
            {
                this.Solution.SiteDefinitionManifests = BuildSiteDefinitionManifestFileReference(new DirectoryInfo(parentDir.FullName + @"\sitetemplates"), this.Solution.SiteDefinitionManifests);
            }

            // Every other file gets added to the TemplateFileReference
            foreach(DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                switch (childDir.Name.ToLower())
                {
                    case "features":  break; // Do nothing here
                    case "sitetemplates": break; // Do nothing here
                    default: AddTemplateFileReferences(childDir, templateFileReferenceList, parentPathIndex); break; 
                }
            }

            // Find and add files in the TEMPLATE folder
            foreach (FileInfo file in parentDir.GetFiles())
            {
                if (FileProvider.IncludeFile(file))
                {
                    string locationPath = file.FullName.Substring(parentPathIndex);

                    if (!DoLocationExist(templateFileReferenceList, locationPath))
                    {
                        TemplateFileReference templateFileReference = new TemplateFileReference();
                        templateFileReference.Location = locationPath;

                        templateFileReferenceList.Add(templateFileReference);

                        //this.CabFiles.Add(new string[] { file.FullName, locationPath });
                        this.AddToCab(file.FullName, locationPath);
                    }
                }
            }


            // Add the directories in the IncludeTemplateDirectory list
            foreach (DirectoryInfo childDir in IncludeTemplateDirectory)
            {
                AddTemplateFileReferences(childDir, templateFileReferenceList, parentPathIndex);
            }

            if (templateFileReferenceList.Count == 0)
            {
                return null;
            }
            return templateFileReferenceList.ToArray();
        }

        #endregion
    }
}
