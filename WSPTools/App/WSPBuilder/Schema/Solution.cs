using System;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Commands;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;

    public partial class Solution
    {
        public void Move(Extractwsp handler)
        {

            if (this.FeatureManifests != null)
            {
                foreach (FeatureManifestReference featureRef in this.FeatureManifests)
                {
                    featureRef.Move(handler);
                }
            }


            if (this.TemplateFiles != null)
            {
                foreach (TemplateFileReference templateFileRef in this.TemplateFiles)
                {
                    templateFileRef.Move(handler);
                }
            }


            if (this.SiteDefinitionManifests != null)
            {
                foreach (SiteDefinitionManifestFileReference siteDefManifestFileRef in this.SiteDefinitionManifests)
                {
                    siteDefManifestFileRef.Move(handler);
                }
            }


            // Move the wpresources and App_GlobalResources files
            if (this.ApplicationResourceFiles != null && this.ApplicationResourceFiles.Items != null)
            {
                foreach (object item in this.ApplicationResourceFiles.Items)
                {
                    if (item is App_GlobalResourceFileDefinition)
                    {
                        App_GlobalResourceFileDefinition globalItem = item as App_GlobalResourceFileDefinition;
                    }
                    else
                    {
                        ApplicationResourceFileDefinition appResourceFileDef = item as ApplicationResourceFileDefinition;
                        appResourceFileDef.Move(handler);
                    }
                }
            }

            if (this.DwpFiles != null)
            {
                foreach (DwpFileDefinition dwpFileDef in this.DwpFiles)
                {
                    dwpFileDef.Move(handler);
                }
            }

            if (this.Resources != null)
            {
                foreach (ResourceDefinition resouceDef in this.Resources)
                {
                    resouceDef.Move(handler);
                }
            }

            if (this.RootFiles != null)
            {
                foreach (RootFileReference rootFileRef in this.RootFiles)
                {
                    rootFileRef.Move(handler);
                }
            }

            if (this.Assemblies != null)
            {
                foreach (AssemblyFileReference assemblyFileRef in this.Assemblies)
                {
                    assemblyFileRef.Move(handler);
                }
            }

            // Move the manifest.xml
            MoveManifest(handler);    

            // Create Solution ID file
            CreateSolutionIdFile(handler);
           
        }

        public void MoveManifest(Extractwsp handler)
        {
            Log.Verbose("Moving manifext file.");

            string filename = "Manifest.xml";
            string sourcePath = Path.Combine(handler.SourcePath, filename);
            string targetPath = Path.Combine(handler.TargetPath, filename);

            FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
        }


        public void CreateSolutionIdFile(Extractwsp handler)
        {
            string targetPath = Path.Combine(handler.TargetPath, "solutionid.txt");
            if (!handler.Overwrite && File.Exists(targetPath))
            {
                return;
            }
            using (StreamWriter sw = File.CreateText(targetPath))
            {
                sw.WriteLine(this.SolutionId);
            }
        }
    }
