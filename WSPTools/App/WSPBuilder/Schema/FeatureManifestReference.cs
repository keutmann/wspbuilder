using System;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Commands;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;

public partial class FeatureManifestReference
{
    public void Move(Extractwsp handler)
    {
        Log.Verbose("Moving:" + this.Location);

        string folderName = Path.GetDirectoryName(this.Location);
        string targetPath = Path.Combine(handler.TargetPath, @"12\template\features\" + this.Location);
        string sourcePath = Path.Combine(handler.SourcePath, this.Location);

        FileSystem.Move(sourcePath, targetPath, handler.Overwrite);

        FeatureDefinition feature = FileSystem.Load<FeatureDefinition>(targetPath);

        if (feature.ElementManifests != null && feature.ElementManifests.Items != null)
        {
            foreach (ElementManifestReference elementManifestRef in feature.ElementManifests.Items)
            {
                targetPath = Path.Combine(handler.TargetPath, String.Format(@"12\template\features\{0}\{1}", folderName, elementManifestRef.Location));
                sourcePath = Path.Combine(handler.SourcePath, String.Format(@"{0}\{1}", folderName, elementManifestRef.Location));
                FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
            }
        }
    }
}

