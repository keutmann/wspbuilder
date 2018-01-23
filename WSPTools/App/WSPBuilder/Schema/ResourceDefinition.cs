using System;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Commands;

public partial class ResourceDefinition
{
    public void Move(Extractwsp handler)
    {
        Log.Verbose("Moving resource file: " + this.Location);

        string targetPath = Path.Combine(handler.TargetPath, @"12\template\features\" + this.Location);
        string sourcePath = Path.Combine(handler.SourcePath, this.Location);

        FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
    }
}

