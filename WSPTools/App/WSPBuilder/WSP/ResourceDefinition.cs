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
        #region Methods 
        // The <Resource> element refers to any file stored inside a FEATURE\MyFeature folder.  NOT just .resx files.

        private void AddResourceDefinitionFiles(DirectoryInfo parentDir, int baseIndex, List<ResourceDefinition> resourceDefinitionList)
        {
            foreach (FileInfo file in parentDir.GetFiles())
            {
                if (FileProvider.IncludeFile(file))
                {
                    string relativePath = file.FullName.Substring(baseIndex);

                    // If the file has not been defined by the feature, then use the resources tag to include the file.
                    if(!this.ElementFiles.Exists(delegate(string path) 
                            { 
                                return path.Equals(relativePath, StringComparison.InvariantCultureIgnoreCase); 
                            }
                        ))
                    {
                        ResourceDefinition resourceDefinition = new ResourceDefinition();

                        resourceDefinition.Location = relativePath;

                        resourceDefinitionList.Add(resourceDefinition);
                    }

                    Log.Verbose("Resources file added: " + relativePath);

                    // Always include the file in the WSP file
                    this.AddToCab(file.FullName, relativePath);
                }

            }

            foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                AddResourceDefinitionFiles(childDir, baseIndex, resourceDefinitionList);
            }


        }


        public ResourceDefinition[] BuildResourceDefinition(DirectoryInfo resourcesDir)
        {
            List<ResourceDefinition> resourceDefinitionList = new List<ResourceDefinition>();

            AddResourceDefinitionFiles(resourcesDir, resourcesDir.FullName.Length + 1, resourceDefinitionList);

            if (resourceDefinitionList.Count == 0)
            {
                return null;
            }

            return resourceDefinitionList.ToArray();
        }

        #endregion
    }
}
