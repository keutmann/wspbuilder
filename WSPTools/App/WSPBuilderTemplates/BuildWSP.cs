/* Program : WSPBuilderCustomActions
 * Created by: Tom Clarkson
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;
using System.IO;

namespace WSPBuilderTemplates
{
    public class BuildWSP : Task
    {

        private string _OutputFolder;
        [Required]
        public string OutputFolder
        {
            get { return _OutputFolder; }
            set { _OutputFolder = value; }
        }

        private string _ProjectFolder;
        [Required]
        public string ProjectFolder
        {
            get { return _ProjectFolder; }
            set { _ProjectFolder = value; }
        }

        private string _AssemblyName;
        [Required]
        public string AssemblyName
        {
            get { return _AssemblyName; }
            set { _AssemblyName = value; }
        }

        private string _WSPBuilderInstallPath;

        public string WSPBuilderInstallPath
        {
            get { 
                if (string.IsNullOrEmpty(_WSPBuilderInstallPath)) {
                    _WSPBuilderInstallPath = ((string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\8.0\MSBuild\SafeImports", "WSPBuilder", "")).Replace("WSPBuilder.targets", "");
                }
                
                return _WSPBuilderInstallPath; }
        }


        private string _DeployFolder;

        public string DeployFolder
        {
            get {
                if (string.IsNullOrEmpty(_DeployFolder))
                {
                    _DeployFolder = ProjectFolder + OutputFolder + "deploy\\";
                }
                return _DeployFolder; }
        }
	


        private void CopyFromInstallToDeploy(string filename)
        {
            try
            {
                File.Copy(WSPBuilderInstallPath + filename, DeployFolder + filename, true);
                Log.LogMessageFromText("Copied "+filename+" to deploy folder", MessageImportance.High);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true);
            }
        }




        private void RunWSPBuilder()
        {
//            List<object> lo = (List<object>)o;
  //          Project proj = (Project)lo[0];
    //        string deployfolder = (string)lo[1];

            try
            {
                Log.LogMessageFromText("Building Solution Package", MessageImportance.High);
                // display parameters
                Log.LogMessageFromText("OutputFolder = " + OutputFolder, MessageImportance.High);
                
            //    string projectfile = BuildEngine.ProjectFileOfTaskNode;
              //  string projectfolder = projectfile.Substring(0, projectfile.LastIndexOf("\\");

                Log.LogMessageFromText("ProjectFolder = " + ProjectFolder, MessageImportance.High);


                Log.LogMessageFromText("AssemblyName = " + AssemblyName, MessageImportance.High);


                Log.LogMessageFromText("DeployFolder = " + DeployFolder, MessageImportance.High);
                             
                
                // prepare deploy folder
                
                Directory.CreateDirectory(DeployFolder);


                // put the project output in the GAC
                
                File.Copy(ProjectFolder + OutputFolder + AssemblyName + ".dll", ProjectFolder + "GAC\\" + AssemblyName + ".dll", true);

                // copy files to help with deployment
                CopyFromInstallToDeploy("wspdeploy.exe");
                CopyFromInstallToDeploy("wspdeploy.lib.dll");
                CopyFromInstallToDeploy("cablib.dll");
                CopyFromInstallToDeploy("setup.exe");

                // Copy deployment config files

                File.Copy(ProjectFolder + "deploy\\setup.exe.config", DeployFolder + "setup.exe.config", true);
                File.Copy(ProjectFolder + "deploy\\eula.rtf", DeployFolder + "eula.rtf", true);
                

                // run wspbuilder

                string wspbuilderexe = WSPBuilderInstallPath+"WSPBuilder.exe";
                string args = "-SolutionPath \"" + ProjectFolder.Substring(0, ProjectFolder.Length - 1) + "\" -Outputpath \"" + DeployFolder.Substring(0, DeployFolder.Length - 1) + "\" ";


                string commandline = "\"" + wspbuilderexe + "\" " + args;
                Log.LogCommandLine(MessageImportance.High, commandline);


                System.Diagnostics.ProcessStartInfo psi = new ProcessStartInfo(wspbuilderexe);
                psi.Arguments = args;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.WorkingDirectory = ProjectFolder;
                psi.RedirectStandardOutput = true;
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                //Log.LogMessagesFromStream(p.StandardOutput, MessageImportance.High);

                p.WaitForExit();
                string output = p.StandardOutput.ReadToEnd();

                Log.LogMessageFromText(output, MessageImportance.High);

                if (!output.Contains("Done!")) {
                    throw new Exception("WSP Build did not complete.");
                }
                

            }
            catch (Exception ex)
            {                
                Log.LogErrorFromException(ex, true);
            }


        }



        public override bool Execute()
        {

            RunWSPBuilder();

            return !Log.HasLoggedErrors;
            
        }
    }
}
