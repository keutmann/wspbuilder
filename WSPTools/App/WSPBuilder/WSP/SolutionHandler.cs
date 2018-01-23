/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml.Serialization;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Reflection;
using System.Xml;
using System.ComponentModel;
using System.Resources;
using System.Globalization;
using Keutmann.SharePoint.WSPBuilder.Properties;
using Keutmann.SharePoint.WSPBuilder.Framework.Serialization;
using WSPTools.BaseLibrary.Extensions;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{

    public partial class SolutionHandler
    {
        #region Const

        public const string MANIFESSTENCODING = "Manifestencoding";
        public const string BUILDWSP = "BuildWSP";
        public const string BUILDCAB = "BuildCAB";
        public const string BUILDDDF = "BuildDDF";
        public const string SOLUTION12PATH = "12path";
        public const string SOLUTION80PATH = "80path";
        public const string SOLUTIONGACPATH = "GACpath";
        public const string RESETWEBSERVERMODEONUPGRADE = "ResetWebServerModeOnUpgrade";

        public const string SOLUTION_TITLE = "Title";
        public const string SOLUTION_DESCRIPTION = "Title";
        public const string SOLUTION_RECEIVERASSEMBLY = "Title";
        public const string SOLUTION_RECEIVERCLASS = "Title";
        
        #endregion

        #region Members

        private Dictionary<string, string> _cabFiles = new Dictionary<string, string>();
        private string _manifestPath = string.Empty;
        private string _manifestContent = string.Empty;
        private Encoding _manifestEncoding = null;
        private Solution _solution = new Solution();
        private bool? _buildWSP = null;
        private bool? _buildCAB = null;
        private bool? _buildDDF = null;

        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _receiverAssembly = string.Empty;
        private string _receiverClass = string.Empty;

        #endregion

        #region Properties

        #region SharePoint 2010 Properties

        [DisplayName("-Title [string] (SharePoint 2010: The title of the solution.)")]
        [Description("SharePoint 2010: The title of the solution. The title will be set in the manifest.xml file.")]
        public string Title 
        { 
            get
            {
                if (String.IsNullOrEmpty(_title))
                {
                    _title = Config.Current.GetString(SOLUTION_TITLE, null);
                }
                return _title;
            }
            set
            {
                _title = value;
            }
        }


        [DisplayName("-Description [string] (SharePoint 2010: The description for the solution.)")]
        [Description("SharePoint 2010: The description for the solution. The description will be set in the manifest.xml file.")]
        public string Description 
        {
            get
            {
                if (String.IsNullOrEmpty(this._description))
                {
                    this._description = Config.Current.GetString(SOLUTION_DESCRIPTION, null);
                }
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }


        [DisplayName("-ReceiverAssembly [string] (SharePoint 2010: The ReceiverAssembly is fully qualified assembly name.)")]
        [Description("SharePoint 2010: The ReceiverAssembly specifieds the assembly where the receiver class is located.")]
        public string ReceiverAssembly 
        { 
            get
            {
                if (String.IsNullOrEmpty(this._receiverAssembly))
                {
                    this._receiverAssembly = Config.Current.GetString(SOLUTION_RECEIVERASSEMBLY, null);
                }
                return this._receiverAssembly;
            }
            set
            {
                this._receiverAssembly = value;
            }
        }


        [DisplayName("-ReceiverClass [string] (SharePoint 2010: The ReceiverClass is the class name with full namespace.)")]
        [Description("SharePoint 2010: The ReceiverClass specifieds the receiver class with full namespace to execute.")]
        public string ReceiverClass 
        {
            get
            {
                if (String.IsNullOrEmpty(this._receiverClass))
                {
                    this._receiverClass = Config.Current.GetString(SOLUTION_RECEIVERCLASS, null);
                }
                return this._receiverClass;
            }
            set
            {
                this._receiverClass = value;
            }
        }


        private ResetWebServerModeOnUpgradeAttr _ResetWebServerModeOnUpgrade = ResetWebServerModeOnUpgradeAttr.Recycle;
        [DisplayName("-ResetWebServerModeOnUpgrade [Recycle|StartStop] (SharePoint 2010: Argument ignored if empty)")]
        [Description("Specifies the reset mode of the webserver when upgrading the solution. If no value are specified for this argument, then the argument is ignored.")]
        public ResetWebServerModeOnUpgradeAttr ResetWebServerModeOnUpgrade
        {
            get
            {
                string upgradeMode = Config.Current.GetString(RESETWEBSERVERMODEONUPGRADE, "");
                if (!String.IsNullOrEmpty(upgradeMode))
                {
                    if (Enum.IsDefined(typeof(ResetWebServerModeOnUpgradeAttr), upgradeMode))
                    {
                        _ResetWebServerModeOnUpgrade = (ResetWebServerModeOnUpgradeAttr)Enum.Parse(typeof(ResetWebServerModeOnUpgradeAttr), upgradeMode, true);
                        ResetWebServerModeOnUpgradeSpecified = true;
                    }
                    else
                    {
                        string message = string.Format("Invalid value '{0}' for -{1}. Valid values are [{2}|{3}].", 
                            upgradeMode, 
                            RESETWEBSERVERMODEONUPGRADE,
                            ResetWebServerModeOnUpgradeAttr.Recycle,
                            ResetWebServerModeOnUpgradeAttr.StartStop);

                        throw new ApplicationException(message);
                    }
                }
                return _ResetWebServerModeOnUpgrade;
            }
            set
            {
                _ResetWebServerModeOnUpgrade = value;
            }
        }

        /// <summary>
        /// Helper property for ResetWebServerModeOnUpgrade
        /// </summary>
        public bool ResetWebServerModeOnUpgradeSpecified { get; set; }

        #endregion

        [DisplayName("-BuildCAB [True|False] (Default is true)")]
        [Description("If set to true then a WSP 'Cab' file will be generated. If false the manifest.xml and DDF can still be created.")]
        public bool BuildCAB
        {
            get
            {
                if (_buildCAB == null)
                {
                    _buildCAB = Config.Current.GetBool(BUILDCAB, true);
                }
                return (bool)_buildCAB;
            }
            set
            {
                _buildCAB = value;
            }
        }

        [DisplayName("-BuildDDF [True|False] (Default is false)")]
        [Description("If set to true, then a 'MakeCab.ddf' file will be placed in the output directory.")]
        public bool BuildDDF
        {
            get
            {
                if (_buildDDF == null)
                {
                    _buildDDF = Config.Current.GetBool(BUILDDDF, false);
                }
                return (bool)_buildDDF;
            }
            set
            {
                _buildDDF = value;
            }
        }


        [DisplayName("-ManifestEncoding [UTF-8, UTF-16 and other standard encoding formats] (Default value is UTF-8)")]
        [Description("Defines the encoding used in the manifest.xml file.")]
        public Encoding ManifestEncoding
        {
            get
            {
                if (_manifestEncoding == null)
                {
                    string encoding = Config.Current.GetString(MANIFESSTENCODING, "UTF-8");

                    _manifestEncoding = Encoding.GetEncoding(encoding);

                }
                return _manifestEncoding;
            }
            set 
            { 
                _manifestEncoding = value; 
            }
        }


        [XmlIgnore()]
        public string ManifestPath
        {
            get
            {
                if (String.IsNullOrEmpty(_manifestPath))
                {
                    _manifestPath = Config.Current.OutputPath + @"\manifest.xml";
                }
                return _manifestPath;
            }
            set { _manifestPath = value; }
        }


        [XmlIgnore()]
        public Dictionary<string, string> CabFiles
        {
            get { return _cabFiles; }
            set { _cabFiles = value; }
        }

        [XmlIgnore()]
        public string ManifestContent
        {
            get { return _manifestContent; }
            set { _manifestContent = value; }
        }

        [XmlIgnore()]
        public Solution Solution
        {
            get { return _solution; }
            set { _solution = value; }
        }

        [XmlIgnore]
        public bool IsWSS40 { get; set; }

        #endregion

        #region Methods


        public SolutionHandler(Solution solution)
        {
            this.IsWSS40 = true;

            this.Solution = solution;

            DeploymentServerTypeAttr serverType = Config.Current.DeploymentServerType;
            if (Config.Current.DeploymentServerTypeFieldSpecified)
            {
                this.Solution.DeploymentServerType = serverType;
                this.Solution.DeploymentServerTypeSpecified = Config.Current.DeploymentServerTypeFieldSpecified;
            }
        }

        public SolutionHandler(Guid solutionId) 
        {
            this.Solution.SolutionId = solutionId.ToString();

            this.Solution.DeploymentServerType = Config.Current.DeploymentServerType;
            this.Solution.DeploymentServerTypeSpecified = Config.Current.DeploymentServerTypeFieldSpecified;
        }


      
        /// <summary>
        /// Builds an entire solution.
        /// When more than one project in a solution, all projects gets build into the same WSP package.
        /// </summary>
        public void BuildSolution()
        {
            Dictionary<string, AssemblyInfo> assembliesFound = new Dictionary<string, AssemblyInfo>(StringComparer.InvariantCultureIgnoreCase);

            if (!Config.Current.BuildSolution)
            {
                // Set the Project path to the solution path
                //Config.Current.ProjectPath = Config.Current.SolutionPath;
                BuildProject(assembliesFound);
            }
            else
            {
                DirectoryInfo solutionDir = new DirectoryInfo(Config.Current.SolutionPath);
                foreach (DirectoryInfo dir in FileProvider.GetDirectories(solutionDir))
                {
                    // Reinitialize the Config object to ensure that its clean.
                    //Config.Current.ReInitialize();
                    Config.Current.ProjectPath = dir.FullName;

                    BuildProject(assembliesFound);
                }
                Config.Current.ProjectPath = Config.Current.SolutionPath;

            }

            SetWSS40SolutionProperties(this.Solution);

            // Create AssemblyFileReferences, add CodeGroups and CodeAccessSecurity
            this.Solution.Assemblies = BuildAssemblyFileReference(assembliesFound);

            // Add the CAS policy, if enabled
            if (Config.Current.BuildCAS)
            {
                this.Solution.CodeAccessSecurity = AppendArray<PolicyItemDefinition>(this.Solution.CodeAccessSecurity, BuildPolicyItemDefinition());
            }


            string compatibilityNote = null;
            if (this.IsWSS40)
            {
                compatibilityNote = "Solution compatibility: SharePoint 2010";
            }
            else
            {
                compatibilityNote = "Solution compatibility: SharePoint 2007 and SharePoint 2010";
            }

            // Create the wsp manifest file
            this.ManifestContent = Serializer.ObjectToXML(this.Solution, this.IsWSS40);

            // Add a note
            int index = this.ManifestContent.IndexOf("\r\n");
            if (index > 0)
            {
                this.ManifestContent = this.ManifestContent.Insert(index, "<!-- Solution created by WSPBuilder. " + DateTime.Now.ToString() + " * " + compatibilityNote + " -->");
            }

            if (Config.Current.BuildCAS)
            {
                // Apply the permissions to the PolicyItemDefinitions
                // This happens here after the serialization, because the wsp shema definition
                // do not implement the IPermission object in a useful way.
                // This is the last solution to the problem I wanted to implement, but the only 
                // one I was able to get to work properly.
                this.ManifestContent = ApplyPolicyPermissions(this.ManifestContent);
            }

            // Replace tokens in files with the Replacement Parameters.
            CreateFilesWithReplacementParameters();

            Log.Information(compatibilityNote);
        }

        /// <summary>
        /// Configurates the new attributes on the solution manifest
        /// </summary>
        /// <param name="solution"></param>
        private void SetWSS40SolutionProperties(Solution solution)
        {
            if(!String.IsNullOrEmpty(this.Title))
            {
                solution.Title = this.Title;
            }
            if (!String.IsNullOrEmpty(solution.Title))
            {
                this.IsWSS40 = true;
            }
            else
            {
                solution.Title = null;
            }


            if (!String.IsNullOrEmpty(this.Description))
            {
                solution.Description = this.Description;
            }
            if (!String.IsNullOrEmpty(solution.Description))
            {
                this.IsWSS40 = true;
            }
            else
            {
                solution.Description = null;
            }


            if (!String.IsNullOrEmpty(this.ReceiverAssembly))
            {
                solution.ReceiverAssembly = this.ReceiverAssembly;
            }
            if (!String.IsNullOrEmpty(solution.ReceiverAssembly))
            {
                this.IsWSS40 = true;
                try
                {
                    AssemblyName name = new AssemblyName(solution.ReceiverAssembly);
                    if (String.IsNullOrEmpty(this.ReceiverClass))
                    {
                        Log.Error("You need to specify the ReceiverClass for the solution!");
                    }
                }
                catch (Exception)
                {
                    Log.Error("Invalid assembly name in Solution ReceiverAssembly. The solution receiver class will not be executed!");
                }
            }
            else
            {
                solution.ReceiverAssembly = null;
            }

            if (!String.IsNullOrEmpty(this.ReceiverClass))
            {
                solution.ReceiverClass = this.ReceiverClass;
            }
            if (!String.IsNullOrEmpty(solution.ReceiverClass))
            {
                this.IsWSS40 = true;
                if (String.IsNullOrEmpty(solution.ReceiverAssembly))
                {
                    Log.Error("You need to specify the ReceiverAssembly for the solution!");
                }
                else
                {
                    // Test if the class exist in the assembly?
                }
            }
            else
            {
                solution.ReceiverClass = null;
            }

            ResetWebServerModeOnUpgradeAttr resetWebServerModeOnUpgrade = this.ResetWebServerModeOnUpgrade;
            if (this.ResetWebServerModeOnUpgradeSpecified)
            {
                this.IsWSS40 = true;
                solution.ResetWebServerModeOnUpgrade = resetWebServerModeOnUpgrade;
                solution.ResetWebServerModeOnUpgradeSpecified = this.ResetWebServerModeOnUpgradeSpecified;
            }
        }

        /// <summary>
        /// Builds a single project.
        /// </summary>
        private void BuildProject(Dictionary<string, AssemblyInfo> assembliesFound)
        {

            DirectoryInfo dirInfo = new DirectoryInfo(Config.Current.ProjectPath);

            // Build the wsp file from the files in the 12 folder
            this.BuildSharePointRoot(Config.Current.Dir12);

            if (Config.Current.DirSharePointRoot != null)
            {
                // Build the wsp file from the files in the SharePoint Hive folder
                this.BuildSharePointRoot(Config.Current.DirSharePointRoot);
            }

            // Build the files from the Template folder direktly from the project folder
            this.BuildTemplate();

            // Build the wsp file from the files in the 80 folder
            this.Build80();

            if (this.IncludeAssemblies)
            {
                // Add dlls to the manifest file
                FindAssemblies(dirInfo, assembliesFound);
                //this.Solution.Assemblies = BuildAssemblyFileReference(dirInfo, this.Solution.Assemblies);
            }

        }


        /// <summary>
        /// Add the files in the SharePoint hive folder to the solution manifest.
        /// </summary>
        /// <param name="dirInfo"></param>
        private void BuildSharePointRoot(DirectoryInfo hivePath)
        {
            if (FileProvider.IncludeDir(hivePath))
            {
                this.Solution.RootFiles = BuildRootFileReference(hivePath, this.Solution.RootFiles);

                string templatePath = hivePath.FullName + @"\template";
                if (Directory.Exists(templatePath))
                {
                    DirectoryInfo templateDir = new DirectoryInfo(templatePath);
                    if (FileProvider.IncludeDir(templateDir))
                    {
                        this.Solution.TemplateFiles = BuildTemplateFileReference(templateDir, this.Solution.TemplateFiles);
                    }
                }
            }
        }

        /// <summary>
        /// Add the files in the Template folder to the solution manifest.
        /// </summary>
        /// <param name="dirInfo"></param>
        private void BuildTemplate()
        {
            string templatePath = Config.Current.ProjectPath + @"\template";
            if (Directory.Exists(templatePath))
            {
                DirectoryInfo templateDir = new DirectoryInfo(templatePath);
                if (FileProvider.IncludeDir(templateDir))
                {
                    this.Solution.TemplateFiles = BuildTemplateFileReference(templateDir, this.Solution.TemplateFiles);
                }
            }
        }

        /// <summary>
        /// Add the files in the 80 folder to the solution manifest.
        /// </summary>
        /// <param name="dirInfo"></param>
        private void Build80()
        {
            if (FileProvider.IncludeDir(Config.Current.Dir80))
            {
                // Build the wpresources and App_GlobalResources objects
                this.Solution.ApplicationResourceFiles = BuildApplicationResourceFileDefinition(Config.Current.Dir80.FullName, this.Solution.ApplicationResourceFiles);

                // Build the wpcatalog objects
                string wpcatalog = Config.Current.Dir80.FullName + @"\wpcatalog";
                if (Directory.Exists(wpcatalog))
                {
                    DirectoryInfo wpcatalogDir = new DirectoryInfo(wpcatalog);
                    if (FileProvider.IncludeDir(wpcatalogDir))
                    {
                        this.Solution.DwpFiles = BuildDwpFileDefinition(wpcatalogDir, this.Solution.DwpFiles);
                    }
                }
            }
        }



       /// <summary>
        /// Build and saves a manifest.xml file to the current directory.
        /// </summary>
        public void Save()
        {
            this.Save(new FileInfo(ManifestPath));
        }


        /// <summary>
        /// Builds the solution manifest and saves it.
        /// </summary>
        /// <param name="file">The file where the solution is saved.</param>
        public void Save(FileInfo file)
        {
            string encodedManifest = this.ManifestContent.Replace("utf-16", ManifestEncoding.BodyName);
            
            using (StreamWriter sw = new StreamWriter(file.FullName, false, ManifestEncoding))
            {
                sw.Write(encodedManifest);
                sw.Close();
                this.AddToCab(file.FullName, file.Name);
            }

        }

        /// <summary>
        /// Build and saves a makecab.ddf file to the current directory.
        /// </summary>
        public void SaveDDF()
        {
            this.SaveDDF(new FileInfo(Config.Current.OutputPath + @"\makecab.ddf"));
        }


        /// <summary>
        /// Builds the makecab.ddf and saves it.
        /// </summary>
        /// <param name="file">The file where the ddf is saved.</param>
        public void SaveDDF(FileInfo file)
        {
            const string formatOutLine = "\"{0}\" \"{1}\"\r\n";
            const string formatSetSourceDir = ".Set SourceDir=\"{0}\"";
            const string formatSetDestDir = ".Set DestinationDir=\"{0}\"";

            using (StreamWriter sw = new StreamWriter(file.FullName, false, ManifestEncoding))
            {
                StringBuilder sbFiles = new StringBuilder();

                string sourceDir = "";  //keep track of setting value placed into ddf
                string destDir = "";    //keep track of setting value placed into ddf

                // Need to ensure each fileref line is <=256 chars total or makecab.exe will choke on it.
                // if line would be too long, we use use source & destination directory variables to shorten it.
                foreach (KeyValuePair<string, string> cabFile in this.CabFiles)
                {
                    string sourceFile = cabFile.Value;
                    string destFile = cabFile.Key;
                    string outLine = String.Format(formatOutLine, sourceFile, destFile);
                    if (outLine.Length <= 256)      // if posible, use fully-qualified paths for both source & dest
                    {
                        if (sourceDir != "")
                        {
                            sourceDir = "";
                            sbFiles.Append(string.Format(formatSetSourceDir, sourceDir));
                        }
                        if (destDir != "")
                        {
                            destDir = "";
                            sbFiles.Append(string.Format(formatSetDestDir, destDir));
                        }
                        sbFiles.Append(outLine);
                    }
                    else
                    {
                        string newSourceDir = Path.GetDirectoryName(sourceFile);
                        string newDestDir = Path.GetDirectoryName(destFile);
                        if (sourceDir != newSourceDir)
                        {
                            sourceDir = newSourceDir;
                            sbFiles.Append(string.Format(formatSetSourceDir, sourceDir));
                        }
                        if (destDir != newDestDir)
                        {
                            destDir = newDestDir;
                            sbFiles.Append(string.Format(formatSetDestDir, destDir));
                        }
                        sbFiles.Append(string.Format(formatOutLine, Path.GetFileName(sourceFile), Path.GetFileName(destFile)));
                    }

                }

                string ddf = string.Format(Resources.DDFTemplate, Config.Current.WSPName, sbFiles.ToString());

                sw.Write(ddf);
                sw.Close();
            }
        }

        /// <summary>
        /// Creates a File list text file, that can be used to include or exclude files from the WSP package.
        /// </summary>
        public void CreateFileList()
        {
            Log.Information("Creating a WSP File list");
            FileProvider.CreateFileList(Config.Current.CreateWSPFileList, this.CabFiles);
        }

        /// <summary>
        /// Removes temporay files needed to build the WSP package.
        /// The -Cleanup false argument can be used to disable this method.
        /// </summary>
        public void Cleanup()
        {
            Log.Information("Cleanup");
            if (File.Exists(this.ManifestPath))
            {
                Log.Verbose("Deleting : Manifest.xml");
                File.Delete(this.ManifestPath);
            }
        }

        /// <summary>
        /// Adds a file to the list of files that goes into the CAB file.
        /// There can not be more than one file with the same location in a CAB file, 
        /// therefore all other files are ignored.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="location"></param>
        public void AddToCab(string path, string location)
        {
            if (!CabFiles.ContainsKey(location))
            {
                CabFiles.Add(location, path);
            }
            else
            {
                Log.Warning("There can not be more files with the same location. Currently using '"+ CabFiles[location] +"'. The file '"+ path +"' is ignored.");
            }
        }


        




        #endregion 

        /// <summary>
        /// Copy files useful for deployment to a folder that is easily copied to another server
        /// </summary>
        /// <param name="includestsadmbat"></param>
        /// <param name="includewspbuilderdeploy"></param>
        /// <param name="includesetupexe"></param>
        public void CreateDeploymentFolder(bool includestsadmbat, bool includewspbuilderdeploy, bool includesetupexe)
        {
            string projectfolder = Config.Current.SolutionPath;
            string additionalfilespath = Path.Combine(Config.Current.SolutionPath,@"deploy");
            string stsadmpath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + @"\Microsoft Shared\web server extensions\12\BIN\stsadm.exe";
            string outputfoldername = Config.Current.DeployFolderPath;
            string wspfilename = Config.Current.WSPFullPath;
            string wspname = Config.Current.WSPName.Replace(".wsp", "");


            
            Log.Information("");
            

            DirectoryInfo outputfolder = Directory.CreateDirectory(outputfoldername);
           
            // copy wsp file
                Log.Information("Copying WSP file");
                FileInfo wspfile = new FileInfo(wspfilename);
                wspfile.CopyTo(Path.Combine(outputfolder.FullName, wspfile.Name), true);

            // copy additional files from deploy folder in project
            if (Directory.Exists(additionalfilespath))
            {
                foreach (string s in Directory.GetFiles(additionalfilespath))
                {
                    FileInfo f = new FileInfo(s);
                    Log.Information("Copying " + f.Name);
                    f.CopyTo(outputfolder.FullName + @"\" + f.Name, true);
                }
            }

            // copy contents of additional files folder

            if (includestsadmbat)
            {
                // create bat file                
                Log.Information("Creating stsadm bat file");
                FileStream fs = new FileStream(outputfolder.FullName + @"\deploywithstsadm.bat", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(string.Format("\"{0}\" -o retractsolution -name \"{1}.wsp\" -immediate -allcontenturls", stsadmpath, wspname));
                sw.WriteLine(string.Format("\"{0}\" -o execadmsvcjobs", stsadmpath));

                sw.WriteLine(string.Format("\"{0}\" -o deletesolution -name \"{1}.wsp\"", stsadmpath, wspname));
                sw.WriteLine(string.Format("\"{0}\" -o execadmsvcjobs", stsadmpath));

                sw.WriteLine(string.Format("\"{0}\" -o addsolution -filename \"{1}.wsp\"", stsadmpath, wspname));
                sw.WriteLine(string.Format("\"{0}\" -o execadmsvcjobs", stsadmpath));

                sw.WriteLine(string.Format("\"{0}\" -o deploysolution -name \"{1}.wsp\" -immediate -allcontenturls -allowGacDeployment -allowCasPolicies -force", stsadmpath, wspname));
                sw.WriteLine(string.Format("\"{0}\" -o execadmsvcjobs", stsadmpath));


                sw.Close();
                fs.Close();

                // create bat file without the -allcontenturls. This is used for solutions with no web scoped
                // changes - usually safecontrols entries. Not sure if there is an easy way to detect this.
                Log.Information("Creating stsadm bat file (no -allcontenturls)");
                fs = new FileStream(outputfolder.FullName + @"\deploywithstsadmnocontenturls.bat", FileMode.Create);
                sw = new StreamWriter(fs);

                sw.WriteLine(string.Format("\"{0}\" -o retractsolution -name \"{1}.wsp\" -immediate", stsadmpath, wspname));
                sw.WriteLine(string.Format("\"{0}\" -o execadmsvcjobs", stsadmpath));

                sw.WriteLine(string.Format("\"{0}\" -o deletesolution -name \"{1}.wsp\"", stsadmpath, wspname));
                sw.WriteLine(string.Format("\"{0}\" -o execadmsvcjobs", stsadmpath));

                sw.WriteLine(string.Format("\"{0}\" -o addsolution -filename \"{1}.wsp\"", stsadmpath, wspname));
                sw.WriteLine(string.Format("\"{0}\" -o execadmsvcjobs", stsadmpath));

                sw.WriteLine(string.Format("\"{0}\" -o deploysolution -name \"{1}.wsp\" -immediate -allowGacDeployment -allowCasPolicies -force", stsadmpath, wspname));
                sw.WriteLine(string.Format("\"{0}\" -o execadmsvcjobs", stsadmpath));


                sw.Close();
                fs.Close();


            }

            if (includewspbuilderdeploy)
            {

                Assembly a = Assembly.GetExecutingAssembly();

                if (a.Location.ToLower().EndsWith("wspbuilder.exe"))
                {
                    FileInfo f = new FileInfo(a.Location);
                    Log.Information("Copying " + f.Name);
                    f.CopyTo(outputfolder.FullName + @"\" + f.Name, true);

                    f = new FileInfo(Path.Combine(f.DirectoryName, "cablib.dll"));
                    if (!f.Exists)
                    {
                        f = new FileInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\WSPTools\WSPBuilderExtensions\cablib.dll");
                    }
                    CopyFile(outputfolder, f);


                    f = new FileInfo(Path.Combine(f.DirectoryName, "Mono.Cecil.dll"));
                    if (!f.Exists)
                    {
                        f = new FileInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\WSPTools\WSPBuilderExtensions\Mono.Cecil.dll");
                    }
                    CopyFile(outputfolder, f);
                }
                else
                {
                    FileInfo f = new FileInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\WSPTools\WSPBuilderExtensions\WspBuilder.exe");
                    CopyFile(outputfolder, f);

                    f = new FileInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\WSPTools\WSPBuilderExtensions\cablib.dll");
                    CopyFile(outputfolder, f);

                    f = new FileInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\WSPTools\WSPBuilderExtensions\Mono.Cecil.dll");
                    CopyFile(outputfolder, f);
                }


                // create bat file                
                Log.Information("Creating WSPBBuilder bat file");
                FileStream fs = new FileStream(outputfolder.FullName + @"\deploywithwspbuilder.bat", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(string.Format("wspbuilder.exe -WSPName {0}.wsp -BuildWSP false -deploy true", wspname));

                sw.Close();
                fs.Close();



            }

            if (includesetupexe)
            {
                // try to find setup.exe
                // look in wspbuilder install path
                FileInfo f = new FileInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\WSPTools\WSPBuilderExtensions\setup.exe");
                CopyFile(outputfolder, f);
            }
        }

        private void CopyFile(DirectoryInfo outputfolder, FileInfo file)
        {
            if (file.Exists)
            {
                Log.Information("Copying " + file.Name);
                file.CopyTo(outputfolder.FullName + @"\" + file.Name, true);
            }
        }


        /// <summary>
        /// Takes a target and a source array and return them togetter in the same collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private T[] AppendArray<T>(T[] target, T[] source)
        {
            if (target != null && source != null)
            {
                List<T> list = new List<T>(target);
                list.AddRange(source);
                return list.ToArray();
            }
            if (source != null)
            {
                return source;
            }
            return target;
        }
    }
}
