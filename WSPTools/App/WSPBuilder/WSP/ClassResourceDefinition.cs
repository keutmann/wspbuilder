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
        private void AddClassResources(DirectoryInfo parentDir, int baseIndex, List<ClassResourceDefinition> classResourceDefinitionList)
        {
            foreach (FileInfo file in parentDir.GetFiles())
            {
                if (FileProvider.IncludeFile(file))
                {
                    string spPathName = file.FullName.Substring(baseIndex);

                    ClassResourceDefinition classResourceDefinition = new ClassResourceDefinition();

                    classResourceDefinition.FileName = file.Name;
                    classResourceDefinition.Location = spPathName;

                    classResourceDefinitionList.Add(classResourceDefinition);

                    Log.Verbose("ClassResource added: " + spPathName);

                    //this.CabFiles.Add(new string[] { file.FullName, spPathName });
                    this.AddToCab(file.FullName, spPathName);
                }
            }
            foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                AddClassResources(childDir, baseIndex, classResourceDefinitionList);
            }
        }


        public ClassResourceDefinition[] BuildClassResourceDefinition(FileInfo dllFileInfo)
        {
            List<ClassResourceDefinition> classResourceDefinitionList = new List<ClassResourceDefinition>();

            // Get the DLL resource path
            string resoucePath = Config.Current.Dir80 + @"\wpresources\" + Path.GetFileNameWithoutExtension(dllFileInfo.Name);

            if (Directory.Exists(resoucePath))
            {
                int baseIndex = resoucePath.Length+1;
                DirectoryInfo resouceDir = new DirectoryInfo(resoucePath);
                AddClassResources(resouceDir, baseIndex, classResourceDefinitionList);
            }
            if (classResourceDefinitionList.Count == 0)
            {
                return null;
            }

            return classResourceDefinitionList.ToArray();
        }
    }
}
