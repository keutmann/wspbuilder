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
using System.GACManagedAccess;

#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library.Commands
{
    public class CopyToGAC : CommandBase
    {
        #region constructor & descructor

        public CopyToGAC(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods

        public static CopyToGAC Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            CopyToGAC instance = new CopyToGAC(dteHandler);

            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "COPYTOGAC",
                "Copy to &GAC",
                "Copies the DLL's to Global Assembly Cache.",
                "Global::Ctrl+Shift+Alt+B,g", 
                new ExecuteDelegate(instance.Execute),
                new StatusDelegate(instance.Status));

            dteHandler.Menu.AddToCommandBars(instance.CommandInstance, beginGroup, commandbars);

            return instance;
        }

        protected override void Execute()
        {
            WSPFileHandle wspHandle = new WSPFileHandle(this.DTEInstance);

            DialogResult result = DialogResult.No;

            if (result != DialogResult.Cancel)
            {
                if(result == DialogResult.Yes)
                {
                    this.DTEInstance.Application.Solution.SolutionBuild.Build(true);
                    if (this.DTEInstance.Application.Solution.SolutionBuild.LastBuildInfo > 0)
                    {
                        MessageBox.Show("Build Failed. Unable to deploy");
                        return; // Exit
                    }
                }

                this.DTEInstance.BuildWindow.Clear();
                this.DTEInstance.BuildWindow.Activate();

                // First recycle all Application pools
                AppPoolHandle appPools = new AppPoolHandle(this.DTEInstance);
                appPools.RecycleAppPools();

                // Should the owstimer service be recycled?
                // Should the Admin service be recycled?


                // Copy to Global Assembly Cache.
                // Install the assembly into the GAC with force
                // --------------------------------------------
                ProjectPaths projectPaths = new ProjectPaths(this.DTEInstance.SelectedProject);

                int count = 0;
                bool errorFound = false;
                string[] dllFiles = GetDLLS(projectPaths.OutputPathDLL);

                foreach (string dllPath in dllFiles)
                {
                    if (dllPath.EndsWith("Microsoft.SharePoint.dll", StringComparison.OrdinalIgnoreCase) ||
                        dllPath.EndsWith("Microsoft.SharePoint.Security.dll", StringComparison.OrdinalIgnoreCase) ||
                        dllPath.EndsWith("Microsoft.SharePoint.Search.dll", StringComparison.InvariantCultureIgnoreCase) ||
                        dllPath.EndsWith("Microsoft.Office.Server.Search.dll", StringComparison.InvariantCultureIgnoreCase)
                        )
                    {
                        // Do not include Microsoft dlls
                        continue;
                    }
                    else
                    {
                        InstallAssemblyIntoGAC(dllPath);
                    }
                    
                    count++; 
                }

                if (errorFound)
                {
                    this.DTEInstance.WriteBuildAndStatusBar("Error copying to Global Assembly Cache!");
                }
                else
                {
                    if (count > 0)
                    {
                        this.DTEInstance.WriteBuildAndStatusBar("Done copy to Global Assembly Cache!");
                    }
                    else
                    {
                        this.DTEInstance.WriteBuildAndStatusBar("No DLL's found!");
                    }
                }
            }
        }


        private void InstallAssemblyIntoGAC(string dllPath)
        {
            FileInfo fileInfo = new FileInfo(dllPath);
            this.DTEInstance.WriteBuildWindow("Installing " + fileInfo.Name + " into Global Assembly Cache.");
            try
            {
                AssemblyCache.InstallAssembly(dllPath, null, AssemblyCommitFlags.Force);
            }
            catch
            {
                try
                {
                    // If an error is thrown, then try to uninstall it first!
                    AssemblyCacheUninstallDisposition dis = new AssemblyCacheUninstallDisposition();
                    AssemblyCache.UninstallAssembly(dllPath, null, out dis);
                    if (dis == AssemblyCacheUninstallDisposition.Uninstalled)
                    {
                        // Now uninstalled, try to reinstall
                        AssemblyCache.InstallAssembly(dllPath, null, AssemblyCommitFlags.Force);
                    }
                    else
                    {
                        this.DTEInstance.WriteBuildWindow("Unable to uninstall/install the assembly " + dllPath + " beacuse: " + dis.ToString());
                    }
                }
                catch 
                {
                    this.DTEInstance.WriteBuildWindow("Unable to uninstall/install the assembly " + dllPath);
                }
            }
        }


        protected override vsCommandStatus Status()
        {
            vsCommandStatus status = vsCommandStatus.vsCommandStatusNinched | vsCommandStatus.vsCommandStatusSupported;

            if (this.DTEInstance.SelectedProject != null)
            {
                ProjectPaths projectPaths = new ProjectPaths(this.DTEInstance.SelectedProject);

                string[] dllFiles = Directory.GetFiles(projectPaths.OutputPathDLL, "*.dll");
                foreach (string dllPath in dllFiles)
                {
                    if (dllPath.EndsWith("Microsoft.SharePoint.Search.dll", StringComparison.InvariantCultureIgnoreCase) ||
                        dllPath.EndsWith("Microsoft.Office.Server.Search.dll", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Do not include Microsoft dlls
                        continue;
                    }
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    break;
                }
            }

            return status;
        }

        private string[] GetDLLS(string path)
        {
            List<string> result = new List<string>();
            result.AddRange(Directory.GetFiles(path, "*.dll"));

            string reference = Path.Combine(path, "Reference");
            string references = Path.Combine(path, "References");

            if (Directory.Exists(reference))
            {
                result.AddRange(Directory.GetFiles(reference, "*.dll"));
            }

            if (Directory.Exists(references))
            {
                result.AddRange(Directory.GetFiles(references, "*.dll"));
            }

            return result.ToArray();
        }

        #endregion

    }
}