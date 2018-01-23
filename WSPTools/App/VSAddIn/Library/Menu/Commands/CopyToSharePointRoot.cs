#region namespace references
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.CommandBars;
using EnvDTE;
using EnvDTE80;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using WSPTools.BaseLibrary.Win32;

#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library.Commands
{
    public class CopyToSharePointRoot : CommandBase
    {

        #region constructor & descructor

        public CopyToSharePointRoot(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods


        public static CopyToSharePointRoot Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            CopyToSharePointRoot instance = new CopyToSharePointRoot(dteHandler);
            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "COPYTO12HIVE",
                "&Copy to SharePoint Root",
                "Copy the files to the SharePoint Root",
                "Global::Ctrl+Shift+Alt+B,c",
                new ExecuteDelegate(instance.Execute),
                new StatusDelegate(instance.Status));

            dteHandler.Menu.AddToCommandBars(instance.CommandInstance, beginGroup, commandbars);

            return instance;
        }

        protected override void Execute()
        {
            if (SharePointRegistry.Instance.SharePointRootLocation != null)
            {

                SelectedItems items = this.DTEInstance.Application.SelectedItems;

                foreach (SelectedItem item in items)
                {
                    StringBuilder arguments = new StringBuilder();
                    string targetFolder = string.Empty;

                    string path = string.Empty;

                    ProjectPaths projectPaths = new ProjectPaths(this.DTEInstance.GetProject(item));

                    if (projectPaths.DoSharePointRootExist())
                    {
                        if (item.ProjectItem != null)
                        {
                            string featurefilename = item.ProjectItem.Properties.Item("FullPath").Value.ToString();
                            path = Path.GetFullPath(featurefilename);

                            string subSPPath = path.Substring(projectPaths.PathSharePointRoot.Length);
                            targetFolder = Utility.CombinePaths(SharePointRegistry.Instance.SharePointRootLocation, subSPPath);
                        }
                        else
                        {
                            // Use the project 12 folder
                            path = projectPaths.PathSharePointRoot;
                            targetFolder = SharePointRegistry.Instance.SharePointRootLocation;
                        }

                        if (ContainsXmlFiles(path))
                        {
                            // First recycle all Application pools
                            AppPoolHandle appPools = new AppPoolHandle(this.DTEInstance);
                            appPools.RecycleAppPools();
                        }

                        string program = "xcopy";

                        this.DTEInstance.StatusBar("xcopy started");

                        arguments.Append("\"" + Utility.RemoveTailingBackSlash(path) + "\"");

                        arguments.Append(" \"" + Utility.RemoveTailingBackSlash(targetFolder) + "\"");
                        arguments.Append(@" /E /R /Y");

                        ExternalProgram externalProgram = new ExternalProgram(this.DTEInstance);
                        externalProgram.RunAsync(program, arguments.ToString());
                    }
                }

                this.DTEInstance.StatusBar("Done xcopy!");
            }
        }

        private bool ContainsXmlFiles(string path)
        {
            bool result = false;
            if (File.Exists(path))
            {
                if(path.EndsWith(".xml"))
                {
                    result = true;
                }
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    if (dir.GetFiles("*.xml").Length > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        foreach (DirectoryInfo dirInfo in dir.GetDirectories())
                        {
                            result = ContainsXmlFiles(dirInfo.FullName);
                            if (result)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        protected override vsCommandStatus Status()
        {
            vsCommandStatus status = vsCommandStatus.vsCommandStatusNinched | vsCommandStatus.vsCommandStatusSupported;

            if (this.DTEInstance.SelectedProject != null)
            {
                bool ok = false;

                SelectedItems items = this.DTEInstance.Application.SelectedItems;
                if (items != null)
                {
                    foreach (SelectedItem item in items)
                    {
                        ProjectPaths projectPaths = new ProjectPaths(this.DTEInstance.GetProject(item));

                        if (projectPaths.DoSharePointRootExist())
                        {
                            ok = true;

                            if (item.ProjectItem != null)
                            {
                                string featurefilename = item.ProjectItem.Properties.Item("FullPath").Value.ToString();
                                string path = Path.GetFullPath(featurefilename);

                                if (!path.StartsWith(projectPaths.PathSharePointRoot, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (ok)
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                }
            }

            return status;
        }

        #endregion
    }
}