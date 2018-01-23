using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using Keutmann.SharePoint.WSPBuilder.Commands;

    public partial class SiteDefinitionManifestFileReference
    {
        public void Move(Extractwsp handler)
        {
            Log.Verbose("Moving site definition:" + this.Location);

            string sourcePath = Path.Combine(handler.SourcePath, this.Location);
            string targetPath = Path.Combine(handler.TargetPath, @"12\template\SiteTemplates\" + this.Location);

            DirectorySystem.Move(sourcePath, targetPath, handler.Overwrite);

            if (this.WebTempFile != null)
            {
                foreach (WebTempFileDefinition webTempFileDef in this.WebTempFile)
                {
                    targetPath = Path.Combine(handler.TargetPath, String.Format(@"12\template\{0}", webTempFileDef.Location));
                    sourcePath = Path.Combine(handler.SourcePath, webTempFileDef.Location);

                    // The file may have been move previously
                    if (File.Exists(sourcePath))
                    {
                        FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
                    }
                }
            }
        }

    }
