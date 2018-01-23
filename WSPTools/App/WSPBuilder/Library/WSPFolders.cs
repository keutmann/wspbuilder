/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Properties;

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    [Category("Folders")]
    public class WSPFolders
    {
        private string _folderDestination = string.Empty;

        public StringBuilder sb = new StringBuilder();

        private Dictionary<string, bool> StopFolder = null;


        public WSPFolders(string folderDestination)
        {
            _folderDestination = folderDestination;

            StopFolder = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
            StopFolder.Add("Features", true);
            StopFolder.Add("SiteTemplates", true);
            StopFolder.Add("LOGS", true);
            StopFolder.Add("THEMES", true);
        }


        public void Create()
        {
            Log.Information("Creating the 12 hive.");
            Create12();
            Log.Information("Creating the 80 folder, for the WebApplications.");
            Create80();
            Log.Information("Creating the GAC folder, for global assembly cache dll's.");
            CreateGAC();
        }

        private void Create12File(string path)
        {

                DirectoryInfo sourceDir = new DirectoryInfo(path);

                CreateFile(sourceDir, "");

                using (StreamWriter sw = new StreamWriter(@"C:\temp\folders.txt"))
                {
                    sw.Write(sb.ToString());
                    sw.Close();
                }
        }

        private void Create12()
        {
            if (Directory.Exists(_folderDestination))
            {
                string[] folders12 = Resources.Folders12.Split(Environment.NewLine.ToCharArray());

                foreach (string folder in folders12)
                {
                    Directory.CreateDirectory(_folderDestination + folder);
                }
            }
            else
            {
                throw new ApplicationException("Can not find the path specified '" + _folderDestination + "'.");
            }
        }

        private void CreateFile(DirectoryInfo sourceDir, string targetPath)
        {
            string childPath = targetPath + "/" + sourceDir.Name;
            //Directory.CreateDirectory(childPath);
            sb.AppendLine(childPath);
            if (!StopFolder.ContainsKey(sourceDir.Name))
            {
                foreach (DirectoryInfo sourceChildDir in sourceDir.GetDirectories())
                {
                    CreateFile(sourceChildDir, childPath);
                }
            }
        }



        private void Create80()
        {
            if (Directory.Exists(_folderDestination))
            {
                Directory.CreateDirectory(_folderDestination + @"\80");
                Directory.CreateDirectory(_folderDestination + @"\80\bin");
                Directory.CreateDirectory(_folderDestination + @"\80\wpresources");
                Directory.CreateDirectory(_folderDestination + @"\80\wpcatalog");
            }
            else
            {
                throw new ApplicationException("Can not find the path specified '" + _folderDestination + "'.");
            }
        }

        private void CreateGAC()
        {
            string path = Config.Current.ProjectPath;
            if (Directory.Exists(_folderDestination))
            {
                Directory.CreateDirectory(_folderDestination + @"\GAC");
            }
            else
            {
                throw new ApplicationException("Can not find the path specified '" + path + "'.");
            }
        }
    }
}
