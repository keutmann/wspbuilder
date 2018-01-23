/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
// Application resources are placed in the Resources directory of the 
// Web application's virtual directory, and are designed to be used across 
// features and site definitions.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public partial class SolutionHandler
    {

        /// <summary>
        /// WPresources and App_GlobalResources files
        /// </summary>
        /// <param name="resourcesDir"></param>
        /// <returns></returns>
        public ApplicationResourceFileDefinitions BuildApplicationResourceFileDefinition(string dir80, ApplicationResourceFileDefinitions applicationResourceFiles)
        {
            Dictionary<string, object> items = GetApplicationResourceFileDefinitionItems(applicationResourceFiles);

            string wpresources = dir80 + @"\\" + ApplicationResourceFileDefinition.PATH_NAME;
            if (Directory.Exists(wpresources))
            {
                DirectoryInfo resourcesDir = new DirectoryInfo(wpresources);
                if (FileProvider.IncludeDir(resourcesDir))
                {
                    AddApplicationResourceFileDefinitions(resourcesDir, resourcesDir.FullName.Length + 1, items);
                }
            }

            string globalresources = dir80 + @"\\" + App_GlobalResourceFileDefinition.PATH_NAME;
            if (Directory.Exists(globalresources))
            {
                DirectoryInfo resourcesDir = new DirectoryInfo(globalresources);
                if (FileProvider.IncludeDir(resourcesDir))
                {
                    this.IsWSS40 = true;
                    AddApp_GlobalResourceFileDefinition(resourcesDir, resourcesDir.FullName.Length + 1, items);
                }
            }

            if (items.Count == 0)
            {
                return applicationResourceFiles;
            }

            if (applicationResourceFiles == null)
            {
                applicationResourceFiles = new ApplicationResourceFileDefinitions();
            }

            object[] tempItems = new object[items.Values.Count];
            items.Values.CopyTo(tempItems, 0);
            applicationResourceFiles.Items = tempItems;

            return applicationResourceFiles;
        }

        private Dictionary<string, object> GetApplicationResourceFileDefinitionItems(ApplicationResourceFileDefinitions applicationResourceFiles)
        {
            Dictionary<string, object> items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (applicationResourceFiles != null && applicationResourceFiles.Items != null)
            {
                foreach (object item in applicationResourceFiles.Items)
                {
                    if (item is App_GlobalResourceFileDefinition)
                    {
                        App_GlobalResourceFileDefinition globalItem = item as App_GlobalResourceFileDefinition;
                        items.Add(globalItem.Location, item);
                    }
                    else
                    {
                        ApplicationResourceFileDefinition appResourceFileDef = item as ApplicationResourceFileDefinition;
                        items.Add(appResourceFileDef.Location, item);
                    }
                }
            }
            return items;
        }



        private void AddApplicationResourceFileDefinitions(DirectoryInfo parentDir, int baseIndex, Dictionary<string, object> items)
        {
            foreach (FileInfo file in parentDir.GetFiles())
            {
                if (FileProvider.IncludeFile(file))
                {
                    string locationPath = file.FullName.Substring(baseIndex);

                    if (!items.ContainsKey(locationPath))
                    {
                        ApplicationResourceFileDefinition applicationResourceFileDefinition = new ApplicationResourceFileDefinition();

                        applicationResourceFileDefinition.Location = locationPath;

                        items.Add(locationPath, applicationResourceFileDefinition);

                        Log.Verbose("Application Resource File: " + locationPath);

                        this.AddToCab(file.FullName, locationPath);
                    }
                }
            }

            foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                AddApplicationResourceFileDefinitions(childDir, baseIndex, items);
            }
        }

        private void AddApp_GlobalResourceFileDefinition(DirectoryInfo parentDir, int baseIndex, Dictionary<string, object> items)
        {
            foreach (FileInfo file in parentDir.GetFiles())
            {
                if (FileProvider.IncludeFile(file))
                {
                    string locationPath = file.FullName.Substring(baseIndex);

                    if (!items.ContainsKey(locationPath))
                    {
                        App_GlobalResourceFileDefinition app_GlobalResourceFileDefinition = new App_GlobalResourceFileDefinition();

                        app_GlobalResourceFileDefinition.Location = locationPath;

                        items.Add(locationPath, app_GlobalResourceFileDefinition);

                        Log.Verbose("App_Global Resource File: " + locationPath);

                        this.AddToCab(file.FullName, locationPath);
                    }
                }
            }

            foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                AddApp_GlobalResourceFileDefinition(childDir, baseIndex, items);
            }
        }
    }
}
