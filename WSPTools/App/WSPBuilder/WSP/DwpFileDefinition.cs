/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public partial class SolutionHandler
    {
        private bool DoLocationExist(List<DwpFileDefinition> dwpFileDefinitionList, string location)
        {
            if (!Config.Current.BuildSolution)
            {
                // Do not check for Existing location files, because its not possible in a project only build.
                return false;
            }

            bool found = false;
            foreach (DwpFileDefinition dwpFileDefinition in dwpFileDefinitionList)
            {
                if (dwpFileDefinition.Location.Equals(location, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    Log.Warning("Multiple WebPart (DWP) files found with the same name '" + dwpFileDefinition.Location + "', therefore only the first one found is used.");
                    break;
                }
            }
            return found;
        }


        public DwpFileDefinition[] BuildDwpFileDefinition(DirectoryInfo dir, DwpFileDefinition[] dwpFiles )
        {
            List<DwpFileDefinition> dwpFileDefinitionList =
                (dwpFiles != null) ? new List<DwpFileDefinition>(dwpFiles) : new List<DwpFileDefinition>();

            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                string locationPath = file.Name;

                if (FileProvider.IncludeFile(file) && (
                    locationPath.EndsWith(".dwp", StringComparison.InvariantCultureIgnoreCase) ||
                    locationPath.EndsWith(".webpart", StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (!DoLocationExist(dwpFileDefinitionList, locationPath))
                    {

                        DwpFileDefinition dwpFileDefinition = new DwpFileDefinition();
                        dwpFileDefinition.Location = locationPath;

                        dwpFileDefinitionList.Add(dwpFileDefinition);

                        Log.Verbose("WebPart added: " + locationPath);

                        //this.CabFiles.Add(new string[] { file.FullName, locationPath });
                        this.AddToCab(file.FullName, locationPath);
                    }
                }
            }

            if (dwpFileDefinitionList.Count == 0)
            {
                return null;
            }

            return dwpFileDefinitionList.ToArray();
        }
    }
}
