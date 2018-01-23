using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using WSPTools.BaseLibrary.Win32;
using System.Reflection;
using WSPTools.BaseLibrary.IO;

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public class ManifestConfig
    {
        public static Solution Load(string path)
        {
            Solution result = null;

            if (!File.Exists(path) && SharePointRegistry.IsSharePoint14)
            {
                ResourceReader resources = new ResourceReader(Config.Current.ProjectPath);
                resources.CreateFile(ResourceReader.MANIFEST_CONFIG);
            }

            // Ensure the correct SolutionID in the solution if it exist
            if (File.Exists(path))
            {
                result = FileSystem.Load<Solution>(path);

                // Use the id from the solutionID file if it exits
                string idfilename = Config.Current.SolutionPath + @"\solutionid.txt";
                Guid solutionID = SolutionIdFile.GetID(idfilename, FileAccess.Read);
                if (!solutionID.Equals(Guid.Empty))
                {
                    Log.Information("The solution id from the solutionid.txt is used. The SolutionID from the Manifest.Config file is ignored!");

                    result.SolutionId = solutionID.ToString();
                }

                // Check the id of the solution and create a new one if its empty
                if (result.SolutionId == Guid.Empty.ToString())
                {
                    result.SolutionId = Guid.NewGuid().ToString();

                    ManifestExtensions.UpdateSolutionID(path, result.SolutionId);
                    Log.Information("The Manifest.Config file has been updated with the solution id : " + result.SolutionId);
                }

            }

            return result;
        }
    }
}
