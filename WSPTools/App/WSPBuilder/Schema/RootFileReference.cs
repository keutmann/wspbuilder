using System;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Commands;

public partial class RootFileReference
{
    public void Move(Extractwsp handler)
    {
        Log.Verbose("Moving root file: " + this.Location);

        string targetPath = Path.Combine(handler.TargetPath, @"12\" + this.Location);
        string sourcePath = Path.Combine(handler.SourcePath, this.Location);

        FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
    }

}
