using System;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Commands;

public partial class DwpFileDefinition
{
    public void Move(Extractwsp handler)
    {
        Log.Verbose("Moving DWP file: " + this.Location);

        string targetPath = Path.Combine(handler.TargetPath, @"80\wpcatalog\" + this.Location);
        string sourcePath = Path.Combine(handler.SourcePath, this.Location);

        FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
    }

}

