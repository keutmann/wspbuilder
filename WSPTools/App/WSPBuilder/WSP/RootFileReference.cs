/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public partial class SolutionHandler
    {
        private bool DoLocationExist(List<RootFileReference> rootFileReferenceList, string location)
        {
            if (!Config.Current.BuildSolution)
            {
                // Do not check for Existing location files, because its not possible in a project only build.
                return false;
            }

            bool found = false;
            foreach (RootFileReference rootFileReference in rootFileReferenceList)
            {
                if (rootFileReference.Location.Equals(location, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    Log.Warning("Multiple root files found with the same name '" + rootFileReference.Location + "', therefore only the first one found is used.");
                    break;
                }
            }
            return found;
        }


        private void AddRootFileReferences(DirectoryInfo parentDir, List<RootFileReference> rootFileReferenceList, int pathIndex)
        {
            // Do not include the "template" folder
            if (!parentDir.Name.Equals("template", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (FileInfo file in parentDir.GetFiles())
                {
                    if (FileProvider.IncludeFile(file))
                    {
                        string spPathName = file.FullName.Substring(pathIndex);
                        if (!DoLocationExist(rootFileReferenceList, spPathName))
                        {
                            RootFileReference rootFileReference = new RootFileReference();
                            rootFileReference.Location = spPathName;

                            rootFileReferenceList.Add(rootFileReference);

                            Log.Verbose("Root file added: " + rootFileReference.Location);

                            this.AddToCab(file.FullName, spPathName);
                        }
                    }
                }

                foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
                {
                    AddRootFileReferences(childDir, rootFileReferenceList, pathIndex);
                }
            }
        }


        public RootFileReference[] BuildRootFileReference(DirectoryInfo parentDir, RootFileReference[] rootFileReferences)
        {

            List<RootFileReference> rootFileReferenceList = (rootFileReferences != null) ? new List<RootFileReference>(rootFileReferences) : new List<RootFileReference>();

            AddRootFileReferences(parentDir, rootFileReferenceList, parentDir.FullName.Length + 1); // Plus one for the slash

            if (rootFileReferenceList.Count == 0)
            {
                return null;
            }
            return rootFileReferenceList.ToArray();
        }
    }
}
