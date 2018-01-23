using System;
using System.Collections.Generic;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using Keutmann.SharePoint.WSPBuilder.Commands;

public partial class TemplateFileReference
{
    public void Move(Extractwsp handler)
    {
        Log.Verbose("Moving template file: " + this.Location);

        string targetPath = Path.Combine(handler.TargetPath, @"12\template\" + this.Location);
        string sourcePath = Path.Combine(handler.SourcePath, this.Location);

        FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
    }

}
