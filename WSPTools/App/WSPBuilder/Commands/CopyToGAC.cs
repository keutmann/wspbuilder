using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.SystemServices;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Reflection;

namespace Keutmann.SharePoint.WSPBuilder.Commands
{
    [Description("Installs assemblies into Global Assembly Cache. If no parameters is specified then the current directory will be probe for assemblies.")]
    public class CopyToGAC : TraceLevelCommand
    {
        // -o CopyToGAC
        // [-dir]
        // [-filename]

        #region Properties

        private string _filename = null;
        [Description("Define the DLL file to install.")]
        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;

                if (!Path.IsPathRooted(_filename))
                {
                    _filename = Path.GetFullPath(_filename);
                }

                if (File.Exists(_filename))
                {
                    throw new IOException("Cannot find the file : " + _filename);
                }
            }
        }

        private string _dir = null;
        [Description("Defines the directory containing the assemblies to be install into Global Assembly Cache. Default is the current directory.")]
        public string Dir
        {
            get
            {
                if (String.IsNullOrEmpty(_dir))
                {
                    _dir = Environment.CurrentDirectory;
                }
                return _dir;
            }
            set
            {
                _dir = value;
                if (String.IsNullOrEmpty(_dir))
                {
                    _dir = Environment.CurrentDirectory;
                }

                if (!Path.IsPathRooted(_dir))
                {
                    _dir = Path.GetFullPath(_dir);
                }

                if (!Directory.Exists(_dir))
                {
                    throw new IOException(String.Format("Directory with path {0} do not exist!", _dir));
                }

                if (_dir.EndsWith(@"\"))
                {
                    _dir = _dir.Substring(0, _dir.Length - 1);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Will copy assemblies to the Global Assembly Cache.
        /// First recycle the Application Pool
        /// Then try to install the assemblies into GAC
        /// </summary>
        public override void Execute()
        {
            // Call base execute to ensure that base functionality is called if there are any.
            base.Execute();

            // First recycle all Application pools
            IIServerManager.RecycleApplicationPools();

            // Should the owstimer service be recycled?
            // Should the Admin service be recycled?

            // Copy to Global Assembly Cache.
            // Install the assembly into the GAC with force
            // --------------------------------------------
            int assembliesInstalled = 0;
            int failedInstallations = 0;
            string[] assemblyPaths = GetAssemblyPaths(this.Dir);

            foreach (string path in assemblyPaths)
            {
                if (IsAssemblyToBeInstalled(path))
                {
                    if (InstallAssembly(path))
                    {
                        // Success
                        assembliesInstalled++;
                    }
                    else
                    {
                        failedInstallations++;
                    }
                }
                else
                {
                    // Ignore assemblies that are not to be installed
                }
            }

            if (failedInstallations > 0)
            {
                Log.Error("Error copying to Global Assembly Cache!");
            }
            else
            {
                if (assembliesInstalled > 0)
                {
                    Log.Information("Done copy to Global Assembly Cache!");
                }
                else
                {
                    Log.Information("No assemblies found!");
                }
            }
        }

        /// <summary>
        /// Checks that the assembly can be installed into Global Assembly Cache
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsAssemblyToBeInstalled(string path)
        {
            bool doCopy = false;

            try
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);

                if (assemblyName != null && assemblyName.Name != null && assemblyName.FullName != null)
                {
                    // Install the assembly if it is not an microsoft or WSPBuilder assembly
                    doCopy = assemblyName.FullName.IndexOf(AssemblyNames.MICROSOFT_SHAREPOINT_PUBLICKEYTOKEN, StringComparison.InvariantCultureIgnoreCase) < 0
                            && assemblyName.FullName.IndexOf(AssemblyNames.WSPBUILDER_PUBLICKEYTOKEN, StringComparison.InvariantCultureIgnoreCase) < 0;
                }
            }
            catch 
            {
                // The assembly is properly not a managed assembly therefore do not install
                doCopy = false;
            }

            return doCopy;
        }


        /// <summary>
        /// Installs the assembly with the specified path into Global Assembly Cache.
        /// </summary>
        /// <param name="path"></param>
        private bool InstallAssembly(string path)
        {
            bool copyDone = true;

            FileInfo fileInfo = new FileInfo(path);
            Log.Verbose("Installing " + fileInfo.Name + " into Global Assembly Cache.");
            try
            {
                AssemblyCache.InstallAssembly(path, null, AssemblyCommitFlags.Force);
            }
            catch
            {
                // If an error is thrown, then try to uninstall it first!
                AssemblyCacheUninstallDisposition dis = new AssemblyCacheUninstallDisposition();
                AssemblyCache.UninstallAssembly(path, null, out dis);
                if (dis == AssemblyCacheUninstallDisposition.Uninstalled)
                {
                    // Now uninstalled, try to reinstall
                    AssemblyCache.InstallAssembly(path, null, AssemblyCommitFlags.Force);
                }
                else
                {
                    copyDone = false;
                    Log.Error("Unable to uninstall/install the assembly " + path + " beacuse: " + dis.ToString());
                }
            }
            return copyDone;
        }


        /// <summary>
        /// Type: Function
        /// Returns the paths to the assemblies in the specified path.
        /// It also includes the assemblies in the Reference folder.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string[] GetAssemblyPaths(string path)
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
