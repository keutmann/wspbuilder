/* Program : WSPBuilderCustomActions
 * Created by: Carsten Keutmann
 * Some original code from Tom Clarkson
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Xml;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Management;
using WSPTools.BaseLibrary.SystemEnvironment;
using WSPTools.BaseLibrary.IO.Compression;
using WSPTools.BaseLibrary.Win32;
using WSPBuilderCustomActions.Properties;


namespace WSPBuilderCustomActions
{
    [RunInstaller(true)]
    
    public partial class WSPBuilderVSInstaller : Installer
    {
        private const string ITEMTEMPLATES_NAME = "ItemTemplates";
        private const string PROJECTTEMPLATES_NAME = "ProjectTemplates";

        private string _sourcePath = null;
        public string SourcePath
        {
            get 
            {
                if (_sourcePath == null)
                {
                    _sourcePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                return _sourcePath; 
            }
        }

        
        /// <summary>
        /// Constructor. Initializes components.
        /// </summary>
        public WSPBuilderVSInstaller()
            : base()
        {
            //InitializeComponent();
        }

        /// <summary>
        /// Overrides Installer.Install, which will be executed during install process.
        /// </summary>
        /// <param name="savedState">The saved state.</param>
        public override void Install(IDictionary savedState)
        {
            // Uncomment the following line, recompile the setup
            // project and run the setup executable if you want
            // to debug into this custom action.
            //Debugger.Break();
            base.Install(savedState);

            InstallCabLib();

            Install(savedState, "8.0");
            Install(savedState, "9.0");
            Install(savedState, "10.0");
        }

        private void InstallCabLib()
        {
            Context.LogMessage("Installing Cablib.dll ");

            if(WinInfo.WinType == WinType.x64)
            {
                CopyCabLib("x64");
            }
            else
            {
                CopyCabLib("x86");
            }
             
        }

        private void CopyCabLib(string winType)
        {
            string sourceFile = String.Format("{0}\\Resources\\{1}\\cablib.dll", SourcePath, winType);
            string targetFile = String.Format("{0}\\cablib.dll", SourcePath);

            if ("x64".Equals(winType, StringComparison.OrdinalIgnoreCase))
            {
                // Its not possible to add a x64 dll to the MSI package that targets x86, therefore a zip file is used and then decompress on installation.
                string zipFile = String.Format("{0}\\Resources\\{1}\\cablibx64.zip", SourcePath, winType);
                byte[] sourceData = File.ReadAllBytes(zipFile);
                byte[] uncompressData = GZipLib.Decompress(sourceData);
                File.WriteAllBytes(sourceFile, uncompressData);
                File.Copy(sourceFile, targetFile, true);
                File.Delete(sourceFile);
            }
            else
            {
                File.Copy(sourceFile, targetFile, true);
            }
        }

        private void UninstallCabLib()
        {
            string targetFile = String.Format("{0}\\cablib.dll", SourcePath);
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
        }


        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            AddAssemblyBindings();
        }

        /// <summary>
        /// Create the assembly bindings in runtime for wspbuilder.exe in order to support SharePoint 2010
        /// </summary>
        private void AddAssemblyBindings()
        {
            if (SharePointRegistry.IsSharePoint14)
            {
                string configFile = String.Format("{0}\\wspbuilder.exe.config", SourcePath);

                string data = File.ReadAllText(configFile);
                data = data.Replace("</runtime>", Resources.SharePointAssemblyBinding + "\r\n\t</runtime>");
                File.WriteAllText(configFile, data);
            }
        }

        /// <summary>
        /// Overrides Installer.Rollback, which will be executed during rollback process.
        /// </summary>
        /// <param name="savedState">The saved state.</param>
        public override void Rollback(IDictionary savedState)
        {
            //Debugger.Break();
            base.Rollback(savedState);

            UninstallCabLib();

            Rollback(savedState, "8.0");
            Rollback(savedState, "9.0");
            Rollback(savedState, "10.0");
        }

        private void Rollback(IDictionary savedState, string vsversion)
        {
            if (savedState.Contains(ITEMTEMPLATES_NAME + vsversion))
            {
                DeleteInstalledFiles(savedState, vsversion);
            }
        }

        /// <summary>
        /// Overrides Installer.Uninstall, which will be executed during uninstall process.
        /// </summary>
        /// <param name="savedState">The saved state.</param>
        public override void Uninstall(IDictionary savedState)
        {
            //Debugger.Break();
            base.Uninstall(savedState);

            UninstallCabLib();

            Uninstall(savedState, "8.0");
            Uninstall(savedState, "9.0");
            Uninstall(savedState, "10.0");
        }

        public void Uninstall(IDictionary savedState, string vsversion)
        {
            if (savedState.Contains(ITEMTEMPLATES_NAME + vsversion))
            {
                DeleteInstalledFiles(savedState, vsversion);
            }
        }


        private void DeleteInstalledFiles(IDictionary savedState, string vsversion)
        {

            // Delete Project Templates
            try
            {
                string[] projecttemplatefilelist = (string[])savedState[ITEMTEMPLATES_NAME + vsversion];
                foreach (string filename in projecttemplatefilelist)
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            // Delete Item Templates
            try
            {
                string[] itemtemplatefilelist = (string[])savedState[ITEMTEMPLATES_NAME + vsversion];
                foreach (string filename in itemtemplatefilelist)
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            // Update the template cache
            string vsinstallDir = GetInstallDir(vsversion);
            if (!String.IsNullOrEmpty(vsinstallDir))
            {
                CallInstallTemplates(vsinstallDir);
            }
        }

        private void Install(IDictionary savedState, string vsversion)
        {
            string vsinstalldir = GetInstallDir(vsversion);
            if (!String.IsNullOrEmpty(vsinstalldir))
            {
                Context.LogMessage("Installing for Visual Studio " + vsversion);

                string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                // Copy item templates
                string templateTargetPath = vsinstalldir + @"ItemTemplates\{0}\WSPBuilder"; //CSharp
                string templateSourcePath = assemblyPath + @"\" + ITEMTEMPLATES_NAME;
                string templateName = ITEMTEMPLATES_NAME;

                CopyTemplates(savedState, vsversion, templateTargetPath, templateSourcePath, templateName);

                // Copy project templates
                templateTargetPath = vsinstalldir + @"ProjectTemplates\{0}\WSPBuilder";
                templateSourcePath = assemblyPath + @"\" + PROJECTTEMPLATES_NAME;
                templateName = PROJECTTEMPLATES_NAME;

                CopyTemplates(savedState, vsversion, templateTargetPath, templateSourcePath, templateName);

                // Update Visual Studio
                CallInstallTemplates(vsinstalldir);
            }

        }

        private void CopyTemplates(IDictionary savedState, string vsversion, string templateTargetPath, string templateSourcePath, string templateName)
        {
            string targetCSharpPath = String.Format(templateTargetPath, "CSharp");
            string targetVisualBasicPath = String.Format(templateTargetPath, "VisualBasic");

            // Item templates
            Directory.CreateDirectory(targetCSharpPath);
            Directory.CreateDirectory(targetVisualBasicPath);

            string[] templatefilelist = Directory.GetFiles(templateSourcePath, "*.zip");
            List<string> templateinstalledlist = new List<string>();
            foreach (string filename in templatefilelist)
            {
                Context.LogMessage("Installing for Visual Studio " + vsversion + " - " + filename);
                FileInfo fi = new FileInfo(filename);

                //Adding a small hack to ensure vbtemplates are copied to the correct directory
                //added by Chip Sockwell
                if (fi.Name.StartsWith("VB_", StringComparison.OrdinalIgnoreCase))
                {
                    string filePath = targetVisualBasicPath + @"\" + fi.Name;
                    File.Copy(filename, filePath, true);
                    templateinstalledlist.Add(filePath);
                }
                else
                {
                    string filePath = targetCSharpPath + @"\" + fi.Name;
                    File.Copy(filename, filePath, true);
                    templateinstalledlist.Add(filePath);
                }


            }
            savedState.Add(templateName + vsversion, templateinstalledlist.ToArray());
        }

        /// <summary>
        /// Installs the templates by calling the Visual Studio Devenv.exe file with the
        /// argument /installvstemplates 
        /// </summary>
        private void CallInstallTemplates(string vsinstalldir)
        {

            // Call this :
            //"C:\Program Files\Microsoft Visual Studio 8\Common7\IDE\devenv.exe" /installvstemplates 

            // Update the template cache
            Context.LogMessage("Running Visual Studio InstallVsTemplates");

                                                                       
            //ProcessStartInfo psi = new ProcessStartInfo("devenv.exe", "/InstallVsTemplates");
            //psi.WorkingDirectory = vsinstalldir;
            //Process p = Process.Start(psi);
            //p.WaitForExit();


            // Set up process info.
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = vsinstalldir+"devenv.exe";
            psi.Arguments = "/InstallVsTemplates";
            psi.WorkingDirectory = vsinstalldir;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            // Create the process.
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            // Associate process info with the process.
            p.StartInfo = psi;

            // Run the process.
            bool fStarted = p.Start();

            if (fStarted)
            {
                // Now wait for the Devenv.exe to finish
                p.WaitForExit();

                //string message = p.StandardOutput.ReadToEnd();
                //string error = p.StandardError.ReadToEnd();

                //MessageBox.Show(message);
                //MessageBox.Show(error);
            }
            else
            {
                throw new ApplicationException("Unable to start Devenv.exe process.");
            }
        }

        private string GetInstallDir(string vsversion)
        {
            string vsinstalldir = null;
            RegistryKey vskey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\" + vsversion);
            if (vskey == null)
            {
                Context.LogMessage("Visual Studio " + vsversion + " not installed (registry)");
            }
            else
            {
                vsinstalldir = vskey.GetValue("InstallDir", "").ToString();
                if (string.IsNullOrEmpty(vsinstalldir))
                {
                    Context.LogMessage("Visual Studio " + vsversion + " not installed (installdir)");
                }
            }
            return vsinstalldir;
        }
    }
}