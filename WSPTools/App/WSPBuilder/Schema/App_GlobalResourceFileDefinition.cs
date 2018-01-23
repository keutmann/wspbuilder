using System;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using Keutmann.SharePoint.WSPBuilder.Commands;
using System.Xml.Serialization;

public partial class App_GlobalResourceFileDefinition 
{

    public const string ELEMENT_NAME = "App_GlobalResourceFile";
    public const string PATH_NAME = "App_GlobalResources";

    public void Move(Extractwsp handler)
    {
        Log.Verbose("Moving App_Global resource file: " + this.Location);

        string targetPath = Path.Combine(handler.TargetPath, "80\\" + PATH_NAME +"\\" + this.Location);
        string sourcePath = Path.Combine(handler.SourcePath, this.Location);

        FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
    }
}

