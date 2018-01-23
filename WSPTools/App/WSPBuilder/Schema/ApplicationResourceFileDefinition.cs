using System;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using Keutmann.SharePoint.WSPBuilder.Commands;
using System.Xml.Serialization;

public partial class ApplicationResourceFileDefinition
{
    public const string ELEMENT_NAME = "ApplicationResourceFile";
    public const string PATH_NAME = "wpresources";

    public void Move(Extractwsp handler)
    {
        Log.Verbose("Moving application resource file: " + this.Location);

        string targetPath = Path.Combine(handler.TargetPath, "80\\"+ PATH_NAME +"\\" + this.Location);
        string sourcePath = Path.Combine(handler.SourcePath, this.Location);

        FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
    }

}

