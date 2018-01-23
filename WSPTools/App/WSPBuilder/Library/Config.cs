/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Schema;
using System.Collections.Specialized;
using System.Xml;
using Keutmann.SharePoint.WSPBuilder.Properties;
using System.Diagnostics;

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    /// <summary>
    /// Serves as the configuration class for building the wsp file.
    /// </summary>
    [Category("Configuration")]
    public sealed class Config
    {
        #region Const

        public const string BUILDWSP = "BuildWSP";
        public const string DEPLOYMENTSERVERTYPE = "DeploymentServerType";
        public const string INCLUDEFILES = "Includefiles";
        public const string EXCLUDEFILES = "Excludefiles";
        public const string INCLUDEFILETYPES = "Includefiletypes";
        public const string EXCLUDEFILETYPES = "Excludefiletypes";
        public const string EXCLUDEPATHS = "Excludepaths";
        public const string SOLUTIONID = "SolutionId";
        public const string SOLUTIONPATH = "SolutionPath";
        public const string BUILDSOLUTION = "BuildSolution";
        public const string PROJECTPATH = "ProjectPath";
        public const string PROJECT_SHAREPOINTROOT_PATH = "SharePointRootPath";
        public const string PROJECT12PATH = "12Path";
        public const string PROJECT80PATH = "80Path";
        public const string PROJECTGACPATH = "GACPath";
        public const string PROJECTBINPATH = "BinPath";
        public const string OUTPUTPATH = "OutputPath";
        public const string WSPNAME = "WSPName";
        public const string SILENCE = "silence";
        public const string CLEANUP = "cleanup";
        public const string BUILDMODE = "BuildMode";
        public const string BUILDCAS = "BuildCAS";
        public const string CUSTOMCAS = "CustomCAS";
        public const string CREATEWSPFILELIST = "CreateWSPFileList";
        public const string PERMISSIONSETLEVEL = "PermissionSetLevel";
        public const string TRACELEVEL = "tracelevel";
        public const string DEPLOY = "DEPLOY";
        public const string RETRACT = "Retract";
        public const string INSTALL = "Install";
        public const string UNINSTALL = "Uninstall";
        public const string UPGRADE = "Upgrade";
        public const string CREATEFOLDERS = "Createfolders";
        public const string FOLDERDESTINATION = "FolderDestination";
        public const string CREATEDEPLOYMENTFOLDER = "CreateDeploymentFolder";
        public const string MANIFESTCONFIGPATH = "ManifestConfigPath";
        public const string PROJECTASSEMBLY = "ProjectAssembly";

        #endregion

        #region Members

        private bool? _buildWSP = null;
        private DeploymentServerTypeAttr _deploymentServerTypeAttr = DeploymentServerTypeAttr.WebFrontEnd;
        private bool _deploymentServerTypeFieldSpecified = false;
        private ArgumentParameters _arguments = null;
        private string _firstArgument = string.Empty;
        private Guid _solutionId = Guid.Empty;
        private string _solutionPath = string.Empty;
        private bool? _buildSolution = null;

        private string _projectDLLPath = string.Empty;
        private string _projectPath = string.Empty;


        private string _wspName = string.Empty;
        private string _outputPath = string.Empty;
        private bool? _silence = null; // Obsolete!
        private bool? _cleanup = null;

        private string _createWSPFileList = string.Empty;

        private string _includefiles = string.Empty;
        private string _excludefiles = string.Empty;
        private List<string> _includefiletypes = null;
        private List<string> _excludefiletypes = null;
        private List<string> _excludepaths = null;

        private BuildModeType _buildMode = BuildModeType.Default;

        private string DefaultSolutionPath = string.Empty; // The default Solution path.
        private string DefaultGACPath = @"GAC";
        private string DefaultBINPath = @"80\Bin";
        private string DefaultOutPutPath = string.Empty;
        private string DefaultProjectDLLPathPart = @"\bin\release";
        private BuildModeType DefaultBuildMode = BuildModeType.Default;
        private string _customCAS = string.Empty;

        private bool? _deploy = null;
        private bool? _retract = null;

        private string _install = null;
        private string _uninstall = null;
        private string _upgrade = null;

        private string _CreateDeploymentFolder = null;
        private string _manifestConfigPath = string.Empty;
        private string _projectAssembly = null;
        #endregion

        #region Properties

        public bool ShowHelp
        {
            get 
            {
                return Arguments.ContainsKey("help"); 
            }
        }


        public string FirstArgument
        {
            get { return _firstArgument; }
        }


        public ArgumentParameters Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    _arguments = new ArgumentParameters();
                }
                return _arguments;
            }
        }

        public bool DeploymentServerTypeFieldSpecified
        {
            get
            {
                return _deploymentServerTypeFieldSpecified;
            }
            set
            {
                _deploymentServerTypeFieldSpecified = value;
            }
        }

        #region Arguments

        [DisplayName("-BuildWSP [True|False] (Default is true.)")]
        [Description("If set to true then a WSP file will be generated. If false the manifest.xml and DDF will not be created.")]
        public bool BuildWSP
        {
            get
            {
                if (_buildWSP == null)
                {
                    _buildWSP = GetBool(BUILDWSP, true);
                }
                return (bool)_buildWSP;
            }
            set
            {
                _buildWSP = value;
            }
        }

        [DisplayName("-DeploymentServerType [WebFrontEnd|ApplicationServer] (Argument ignored if empty)")]
        [Description("Specifies if the solution is a targeted to a webfront end or a backend application server. If no value are specified for this argument, then the argument is ignored.")]
        public DeploymentServerTypeAttr DeploymentServerType
        {
            get
            {
                string depServertype = GetString(DEPLOYMENTSERVERTYPE, "");
                if (!String.IsNullOrEmpty(depServertype))
                {
                    if (Enum.IsDefined(typeof(DeploymentServerTypeAttr), depServertype))
                    {
                        _deploymentServerTypeAttr = (DeploymentServerTypeAttr)Enum.Parse(typeof(DeploymentServerTypeAttr), depServertype, true);
                        DeploymentServerTypeFieldSpecified = true;
                    }
                    else
                    {
                        throw new ApplicationException("Invalid value '" + depServertype + "' for -DeploymentServerType. Valid values are [WebFrontEnd|ApplicationServer].");
                    }
                }
                return _deploymentServerTypeAttr;
            }
            set
            {
                _deploymentServerTypeAttr = value;
            }
        }



        [DisplayName("-Silence [True|False] (Obsolete! Use -TraceLevel)")]
        [Description("Setting it to True, is the same as using -TraceLevel Off")]
        public bool Silence
        {
            get
            {
                if (_silence == null)
                {
                    _silence = GetBool(SILENCE, false);
                }
                return (bool)_silence;
            }
            set
            {
                _silence = value;
            }
        }

        [DisplayName("-Cleanup [True|False] (Default value is true)")]
        [Description("The WSPBuilder cleans up any temporary file created to make the WSP package, this includes the manifest.xml.")]
        public bool Cleanup
        {
            get
            {
                if (_cleanup == null)
                {
                    _cleanup = GetBool(CLEANUP, true);
                }
                return (bool)_cleanup;
            }
            set
            {
                _cleanup = value;
            }
        }



        [DisplayName("-Includefiles [filename]")]
        [Description("The WSPBuilder will only include files in the WSP package, if they are specified in the file provided.\r\nTo create a file list first use the -CreateWSPFileList argument.")]
        public string Includefiles
        {
            get
            {
                if (string.IsNullOrEmpty(_includefiles))
                {
                    _includefiles = GetString(INCLUDEFILES, "");
                }
                return _includefiles;
            }
            set { _includefiles = value; }
        }


        [DisplayName("-Excludefiles [filename]")]
        [Description("The WSPBuilder will exclude files from the WSP package, if they are specified in the file provided.\r\nTo create a file list first use the -CreateWSPFileList argument.")]
        public string Excludefiles
        {
            get
            {
                if (string.IsNullOrEmpty(_excludefiles))
                {
                    _excludefiles = GetString(EXCLUDEFILES, "");
                }
                return _excludefiles;
            }
            set { _excludefiles = value; }
        }


        [DisplayName("-Includefiletypes [*|xml,gif,jpg, ...] (Default is asterisk '*' meaning every file type. Use comma as a separator.)")]
        [Description("Specifies which file types to include in the manifest and wsp file.\t\rIf the file type if not defined in the Includefiletypes or in the Excludefiletypes arguments then it is always included.\t\rThe Includefiletypes always overrules the Exludefiletypes argument.\t\rDefine more than one file type by using comma.")]
        public List<string> Includefiletypes
        {
            get
            {
                if (_includefiletypes == null)
                {
                    string typeString = GetString(INCLUDEFILETYPES, "*");

                    string[] types = typeString.Split(',');

                    _includefiletypes = new List<string>(types);
                }
                return _includefiletypes;
            }
            set
            {
                _includefiletypes = value;
            }
        }

        [DisplayName("-Excludefiletypes [\"cs,scc, ...\"] (Default is 'cs,scc'. Use comma as a separator.)")]
        [Description("Specifies which file types to exclude in the manifest and wsp file.\t\rDefine more than one file type by using comma.\t\rIt is possible to use asterisk '*' to exclude every file type, except those defined in the Includefiletypes argument.")]
        public List<string> Excludefiletypes
        {
            get
            {
                if (_excludefiletypes == null)
                {
                    string typeString = GetString(EXCLUDEFILETYPES, "cs,scc");

                    string[] types = typeString.Split(',');

                    _excludefiletypes = new List<string>(types);
                }

                return _excludefiletypes;
            }
            set
            {
                _excludefiletypes = value;
            }
        }


        [DisplayName("-Excludepaths [\"path1;path2\"] (Default is empty. Use semicolon as a separator.)")]
        [Description("Specifies which paths to exclude and not include in the building process.")]
        public List<string> Excludepaths
        {
            get
            {
                if (_excludepaths == null)
                {
                    string typeString = GetString(EXCLUDEPATHS, null);
                    List<string> list = new List<string>();

                    if (typeString != null)
                    {
                        string[] paths = typeString.Split(';');
                        foreach (string path in paths)
                        {

                            DirectoryInfo dir = new DirectoryInfo(path);
                            list.Add(dir.FullName);

                        }
                    }
                    // Exclude the Deploy/ProjectName/CabLib.dll
                    list.Add(Config.Current.DeployFolderPath);

                    _excludepaths = list;
                }

                return _excludepaths;
            }
            set
            {
                _excludepaths = value;
            }
        }


        [DisplayName(@"-BuildMode [Default|Debug|Release] (Default is 'Default')")]
        [Description(@"BuildMode is to determine where to read the DLL's from. 
In 'Default' mode the WSPBuilder will auto detect where to find the DLL's. It will use 'bin\Debug' or 'bin\release' directory depending on the current directory.
In 'Debug' mode the WSPBuilder will find the DLL's from the 'bin\debug' folder and in 'Release' mode the 'bin\release' folder are used. 
However the GAC and 80\bin folder are always included as normal.")]
        public BuildModeType BuildMode
        {
            get
            {
                if (_buildMode == BuildModeType.Default)
                {
                    string mode = GetString(BUILDMODE, DefaultBuildMode.ToString());
                    try
                    {
                        _buildMode = (BuildModeType)Enum.Parse(typeof(BuildModeType), mode, true);
                    }
                    catch
                    {
                        ExceptionHandler.Throw(BUILDMODE, mode, "[Default|Debug|Release]");
                    }
                }
                return _buildMode;
            }
            set { _buildMode = value; }
        }


        [DisplayName("-SolutionId [GUID] (Default is taken from the solutionid.txt. The file will be automatically be created if it do not exist)")]
        [Description("Specifies unique identifier of generated Solution.")]
        public Guid SolutionId
        {
            get
            {
                if (_solutionId.Equals(Guid.Empty))
                {
                    _solutionId = GetGuid(SOLUTIONID, Guid.Empty);

                    if (_solutionId.Equals(Guid.Empty))
                    {
                        string idfilename = SolutionPath + @"\solutionid.txt";

                        // If the file exist, then do not try to open it in write mode.
                        FileAccess access = (File.Exists(idfilename)) ? FileAccess.Read : FileAccess.ReadWrite;

                        _solutionId = SolutionIdFile.GetID(idfilename, access);
                    }

                }
                return _solutionId;
            }
            set
            {
                _solutionId = value;
            }
        }


        [DisplayName("-SolutionPath [Path] (Default is the current directory)")]
        [Description("Specifies where the solution is located.")]
        public string SolutionPath
        {
            get
            {
                if (string.IsNullOrEmpty(_solutionPath))
                {
                    // Try to get the path from the configuration
                    _solutionPath = GetString(SOLUTIONPATH, "");

                    if (string.IsNullOrEmpty(_solutionPath))
                    {
                        // Use the Default solution path 
                        _solutionPath = DefaultSolutionPath;
                    }
                    _solutionPath = FileProvider.GetCleanDirectoryPath(_solutionPath);
                }
                return _solutionPath;
            }
            set
            {
                _solutionPath = value;
            }

        }


        [DisplayName("-BuildSolution [True|False] (Default is false)")]
        [Description("Specifies that multiple projects have to be build into a single WSP file. All the projects have to be the subfolders of the solution folder specified with -SolutionPath")]
        public bool BuildSolution
        {
            get
            {
                if (_buildSolution == null)
                {
                    _buildSolution = GetBool(BUILDSOLUTION, false);
                }
                return (bool)_buildSolution;
            }
            set
            {
                _buildSolution = value;
            }
        }

        [DisplayName("-ProjectPath [Path] (Default is the current SolutionPath directory)")]
        [Description("Specifies where the project is located.")]
        public string ProjectPath
        {
            get
            {
                if (string.IsNullOrEmpty(_projectPath))
                {
                    // Try to get the path from the configuration
                    _projectPath = GetString(PROJECTPATH, "");

                    if (string.IsNullOrEmpty(_projectPath))
                    {
                        // Use the Default solution path 
                        _projectPath = DefaultSolutionPath;
                    }

                    // Ensure that the path exists now and remove tailing backslash
                    _projectPath = FileProvider.GetCleanDirectoryPath(_projectPath);

                }
                return _projectPath;
            }
            set
            {
                _projectPath = value;
            }

        }


        [DisplayName("-Outputpath [Path] (Default is the current directory)")]
        [Description("Specifies where the wsp and the manifest file are saved.")]
        public string OutputPath
        {
            get
            {
                if (string.IsNullOrEmpty(_outputPath))
                {
                    _outputPath = GetString(OUTPUTPATH, string.Empty);
                    if (string.IsNullOrEmpty(_outputPath))
                    {
                        _outputPath = DefaultOutPutPath;
                    }
                    _outputPath = FileProvider.GetCleanDirectoryPath(_outputPath);
                }
                return _outputPath;
            }
            set
            {
                _outputPath = value;
            }
        }

        [DisplayName("-SharePointRootPath [Path] (SharePoint 2007/2010: Default is the [current directory]/SharePointRoot)")]
        [Description("Specifies where the SharePoint hive folder is located (12 or 14).")]
        public DirectoryInfo DirSharePointRoot
        {
            get
            {
                DirectoryInfo result = GetArgumentDirectory(PROJECT_SHAREPOINTROOT_PATH, "SharePointRoot");
                if(result == null)
                {
                    // If the developer thinks that the new hive folder is called 14.
                    result = GetArgumentDirectory(PROJECT_SHAREPOINTROOT_PATH, "14");
               }
                return result;
            }
        }

        [DisplayName("-12Path [Path] (Default is the [current directory]/12)")]
        [Description("Specifies where the 12 folder is located.")]
        public DirectoryInfo Dir12
        {
            get
            {
                return GetArgumentDirectory(PROJECT12PATH, "12");
            }
        }

        [DisplayName("-80Path [Path] (Default is the [current directory]/80)")]
        [Description("Specifies where the 80 folder is located.")]
        public DirectoryInfo Dir80
        {
            get
            {
                return GetArgumentDirectory(PROJECT80PATH, "80");
            }
        }

        [DisplayName(@"-GACPath [Path] (Default is the [current directory]\GAC)")]
        [Description(@"Specifies where the GAC folder is located.")]
        public DirectoryInfo DirGAC
        {
            get
            {
                return GetArgumentDirectory(PROJECTGACPATH, DefaultGACPath);
            }
        }

        [DisplayName(@"-BinPath [Path] (Default is the [current directory]\80\bin)")]
        [Description(@"Specifies where the Bin folder is located.")]
        public DirectoryInfo DirBin
        {
            get
            {
                return GetArgumentDirectory(PROJECTBINPATH, DefaultBINPath);
            }
        }

        [DisplayName("-WSPName [name] (Default is the name of the solution folder)")]
        [Description("Specifies the name of the wsp solution file.")]
        public string WSPName
        {
            get
            {
                if (string.IsNullOrEmpty(_wspName))
                {
                    _wspName = GetString(WSPNAME, "");
                    if (string.IsNullOrEmpty(_wspName))
                    {
                        // Use the directory as the file name for the wsp file.
                        DirectoryInfo dir = new DirectoryInfo(SolutionPath);
                        _wspName = dir.Name + ".wsp";
                    }
                }
                return _wspName;
            }
            set { _wspName = value; }
        }


        [DisplayName("-CreateWSPFileList [outputfilename]")]
        [Description("Create a simple text file that contains a list of every file that goes into the WSP package.")]
        public string CreateWSPFileList
        {
            get
            {
                if (string.IsNullOrEmpty(_createWSPFileList))
                {
                    _createWSPFileList = GetString(CREATEWSPFILELIST, "");
                }
                return _createWSPFileList;
            }
            set { _createWSPFileList = value; }
        }


        private TraceLevel? _tracelevel = null;
        [DisplayName("-TraceLevel [Off|Error|Warning|Info|Verbose] (Defaut value is Info)")]
        [Description("The trace level switch setting for the application. It's possible to add more Trace listeners in WSPBuilder.exe.config file.")]
        public TraceLevel TraceLevel
        {
            get
            {
                if (_tracelevel == null)
                {
                    string parameter = GetString(TRACELEVEL, "Info");
                    try
                    {
                        _tracelevel = (System.Diagnostics.TraceLevel)Enum.Parse(typeof(System.Diagnostics.TraceLevel), parameter, true);
                    }
                    catch
                    {
                        ExceptionHandler.Throw("TraceLevel", parameter, "[Off|Error|Warning|Info|Verbose]");
                    }
                }
                return (TraceLevel)_tracelevel;
            }
            set { _tracelevel = value; }
        }

        [DisplayName("-CreateDeploymentFolder [stsadm|wspbuilder|ssi|all]")]
        [Description("Copy files to a folder under bin that can be copied for easy deployment on another system")]
        public string CreateDeploymentFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_CreateDeploymentFolder))
                {
                    _CreateDeploymentFolder = GetString(CREATEDEPLOYMENTFOLDER, "");
                }
                return _CreateDeploymentFolder;
            }
            set
            {
                _CreateDeploymentFolder = value;
            }
        }



        private Dictionary<PermissionSetLevelType, StringDictionary> _permissionSets = null;
        public Dictionary<PermissionSetLevelType, StringDictionary> PermissionSets
        {
            get
            {
                if (_permissionSets == null)
                {
                    _permissionSets = new Dictionary<PermissionSetLevelType, StringDictionary>();

                    // Load all standard permissionsets
                    _permissionSets.Add(PermissionSetLevelType.None, LoadPermissions(Resources.PermissionSetNone));
                    _permissionSets.Add(PermissionSetLevelType.Minimal, LoadPermissions(Resources.PermissionSetMinimal));
                    _permissionSets.Add(PermissionSetLevelType.Medium, LoadPermissions(Resources.PermissionSetMedium));
                }
                return _permissionSets;
            }
        }

        public StringDictionary PermissionSet
        {
            get
            {
                return this.PermissionSets[this.PermissionSetLevel];
            }
        }



        private StringDictionary _includeFilesDictionary = null;
        /// <summary>
        /// A list of files that are allowed into the WSP package.
        /// </summary>
        public StringDictionary IncludeFilesDictionary
        {
            get
            {
                if (_includeFilesDictionary == null)
                {
                    _includeFilesDictionary = LoadWSPFileList(this.Includefiles);
                }
                return _includeFilesDictionary;
            }
            set { _includeFilesDictionary = value; }
        }


        private StringDictionary _excludeFilesDictionary = null;
        /// <summary>
        /// A list of files that are not allowed into the WSP package.
        /// </summary>
        public StringDictionary ExcludeFilesDictionary
        {
            get
            {
                if (_excludeFilesDictionary == null)
                {
                    _excludeFilesDictionary = LoadWSPFileList(this.Excludefiles);
                }
                return _excludeFilesDictionary;
            }
            set { _excludeFilesDictionary = value; }
        }


        /// <summary>
        /// The path where the Project DLL's are located.
        /// </summary>
        public string ProjectDLLPath
        {
            get
            {
                if (BuildMode == BuildModeType.Debug)
                {
                    _projectDLLPath = ProjectPath + @"\bin\debug";
                }
                else
                    if (BuildMode == BuildModeType.Release)
                    {
                        _projectDLLPath = ProjectPath + @"\bin\release";
                    }
                    else
                    {
                        _projectDLLPath = ProjectPath + DefaultProjectDLLPathPart;
                    }
                return _projectDLLPath;
            }
        }


        [DisplayName("-ManifestConfigPath [filepath] (Default value is current project path)")]
        [Description("Defines the path to manifest.config.")]
        public string ManifestConfigPath
        {
            get
            {
                if (String.IsNullOrEmpty(_manifestConfigPath))
                {
                    _manifestConfigPath = Config.Current.ProjectPath+ @"\manifest.config";

                }
                return _manifestConfigPath;
            }
            set { _manifestConfigPath = value; }
        }

        #region PolicyItemDefinition

        [DisplayName(@"-BuildCAS [true|false] (Default value is true)")]
        [Description("Builds Code Access Security policy PermissionSets for assemblies found in the 80\\bin folder.\r\nIf no evidence is found on an assembly, a default PermissionSet is defined with wss_medium settings.")]
        public bool BuildCAS
        {
            get
            {
                return Config.Current.GetBool(BUILDCAS, true);
            }
        }

        [DisplayName(@"-PermissionSetLevel [None|Minimal|Medium] (Default value is Medium)")]
        [Description(@"A standard PermissionSet that are merge with the permission attibutes specified in the assemblies, to ensure that the assembly can run in SharePoint.
'None'      Only the permission from the Assembly are used to build the PermissionSet.
'Minimal'   The standard SharePoint minimal PermissionSet is merged with the assemblys permission attributes.
'Medium'    The standard SharePoint medium PermissionSet is merged with the assemblys permission attributes.
")]
        public PermissionSetLevelType PermissionSetLevel
        {
            get
            {
                PermissionSetLevelType result = PermissionSetLevelType.Medium;
                string level = Config.Current.GetString(PERMISSIONSETLEVEL, "");
                if (!String.IsNullOrEmpty(level))
                {
                    try
                    {
                        result = (PermissionSetLevelType)Enum.Parse(typeof(PermissionSetLevelType), level, true);
                    }
                    catch
                    {
                        ExceptionHandler.Throw("PermissionSetLevel", level, "[None|Minimal|Medium] (Default value is Medium)");
                    }
                }
                return result;
            }
        }


        [DisplayName("-CustomCAS [filepath] (Default is empty)")]
        [Description("Specifies the custom CAS for the solution.")]
        public string CustomCAS
        {
            get
            {
                if (string.IsNullOrEmpty(_customCAS))
                {
                    _customCAS = GetString(CUSTOMCAS, "");
                    if (!string.IsNullOrEmpty(_customCAS))
                    {
                        // Use the directory as the file name for the wsp file.
                        using (TextReader tr = new StreamReader(_customCAS))
                        {
                            _customCAS = tr.ReadToEnd();
                        }
                    }
                }
                return _customCAS;
            }
        }



        [DisplayName("-ProjectAssembly [Path] (Default is none. E.g 'GAC\\MyAssembly.dll')")]
        [Description("The path to the assembly that will act as the project assembly. The path can be relative to the project or a full path.")]
        public string ProjectAssembly
        {
            get
            {
                if (_projectAssembly == null)
                {
                    _projectAssembly = GetString(PROJECTASSEMBLY, string.Empty);
                }
                return _projectAssembly;
            }
            set
            {
                _projectAssembly = value;
            }
        }

        [XmlIgnore]
        internal string ProjectAssemblyPath
        {
            get
            {
                string path = this.ProjectAssembly;
                if (!String.IsNullOrEmpty(path) && !Path.IsPathRooted(path))
                {
                    path = Path.Combine(this.ProjectPath, path);
                }
                return path;
            }
        }
        #endregion

        #region Deployment

        [DisplayName("-Deploy [True|False] (Default is false.)")]
        [Description("Will install the solution into the SharePoint server.")]
        public bool Deploy
        {
            get
            {
                if (_deploy == null)
                {
                    _deploy = GetBool(DEPLOY, false);
                }
                return (bool)_deploy;
            }
            set
            {
                _deploy = value;
            }
        }

        [DisplayName("-Retract [True|False] (Default is false.)")]
        [Description("Will retract the solution from the SharePoint server.")]
        public bool Retract
        {
            get
            {
                if (_retract == null)
                {
                    _retract = GetBool(RETRACT, false);
                }
                return (bool)_retract;
            }
            set
            {
                _retract = value;
            }
        }


        [DisplayName("-Install [default|filename] (Default wsp file is used, nothing needs to be specified. The otherwise the filename is used)")]
        [Description("Will install the solution into the SharePoint server, but do not deploy the solution.")]
        public string Install
        {
            get
            {
                if (_install == null)
                {
                    _install = GetString(INSTALL, "");
                }
                return _install;
            }
            set
            {
                _install = value;
            }
        }


        [DisplayName("-Uninstall [default|filename] (Default wsp file is used, nothing needs to be specified. The otherwise the filename is used)")]
        [Description("Will try to retract the solution and uninstall it from the SharePoint server.")]
        public string Uninstall
        {
            get
            {
                if (_uninstall == null)
                {
                    _uninstall = GetString(UNINSTALL, "");
                }
                return _uninstall;
            }
            set
            {
                _uninstall = value;
            }
        }

        [DisplayName("-Upgrade [default|filename] (Default wsp file is used, nothing needs to be specified. The otherwise the filename is used)")]
        [Description("Will try to upgrade the solution is exist otherwise it will install and deploy globally.")]
        public string Upgrade
        {
            get
            {
                if (_upgrade == null)
                {
                    _upgrade = GetString(UPGRADE, "");
                }
                return _upgrade;
            }
            set
            {
                _upgrade = value;
            }
        }

        #endregion

        #region Folder Creation

        [DisplayName("-Createfolders [true|false] (Default is false)")]
        [Description("Creates the folder structure that supports building the wsp file.")]
        public bool CreateFolders
        {
            get
            {

                return GetBool(CREATEFOLDERS, false);
            }
        }


        [DisplayName("-FolderDestination [Path] (Current directory is default)")]
        [Description("The path to where the folders are created.")]
        public string FolderDestination
        {
            get
            {
                return Config.Current.GetString(FOLDERDESTINATION, Environment.CurrentDirectory);
            }
        }


        /// <summary>
        /// The full path to the WSP file. Read only
        /// </summary>
        public string WSPFullPath
        {
            get
            {
                return this.OutputPath + @"\" + this.WSPName;
            }
        }

        public string _deployFolderPath = null;
        /// <summary>
        /// Calculate the Deployment folder
        /// </summary>
        public string DeployFolderPath
        {
            get
            {
                if(String.IsNullOrEmpty(_deployFolderPath))
                {
                    _deployFolderPath = Path.Combine(Config.Current.SolutionPath, @"bin\deploy\" + Config.Current.WSPName.Replace(".wsp", ""));
                }
                return _deployFolderPath;
            }
        }
        

        #endregion

        #endregion

        #endregion

        #region Methods
        private static object LockDown = new object();

        private static Config _current = new Config();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Config()
        {
        }

        public Config()
        {
            //Pre-set some arguments acording to the default build location of the WSP
            PreInitializeArguments();

            // Parse the App.config
            // Do this parsing in the cc because it has to come before the Args Parser
            ParseAppConfig();

            // get settings from the local project configuration file
            ParseLocalConfig();


            // Parse the commandline.
            Arguments.Parse(Environment.CommandLine, " ");

            if (Silence)
            {
                // Get all the output and nothing with it.
                this.TraceLevel = TraceLevel.Off;
            }
        }


        public static Config Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
            }
        }

        /// <summary>
        /// Reinitialize the Config object. This is used by the BuildSolution Method to ensure that every argument will set to the
        /// correct value.
        /// </summary>
        public void ReInitialize()
        {
            lock (LockDown)
            {
                _current = new Config();
            }
        }

        /// <summary>
        /// //Pre-set some arguments acording to the default build location of the WSP
        /// </summary>
        public void PreInitializeArguments()
        {
            string path = Environment.CurrentDirectory;

            // Start with CurrentDirectory
            DefaultOutPutPath = path;

            if (path.EndsWith(@"\bin\release", StringComparison.InvariantCultureIgnoreCase))
            {
                DefaultBuildMode = BuildModeType.Release;
                DefaultProjectDLLPathPart = @"\bin\release";
                DefaultOutPutPath = path; // Use the release folder
                path = path.Substring(0, path.Length - @"\bin\release".Length);
            }
            else
                if (path.EndsWith(@"\bin\debug", StringComparison.InvariantCultureIgnoreCase))
                {
                    DefaultBuildMode = BuildModeType.Debug;
                    DefaultProjectDLLPathPart = @"\bin\debug";
                    DefaultOutPutPath = path; // Use the debug folder
                    path = path.Substring(0, path.Length - @"\bin\debug".Length);
                }
                else
                    if (path.EndsWith(@"\GAC", StringComparison.InvariantCultureIgnoreCase))
                    {
                        DefaultBuildMode = BuildModeType.Release;
                        path = path.Substring(0, path.Length - @"\GAC".Length);
                        DefaultOutPutPath = path; // Use the project folder
                    }
                    else
                        if (path.EndsWith(@"\80\bin", StringComparison.InvariantCultureIgnoreCase))
                        {
                            path = path.Substring(0, path.Length - @"\80\bin".Length);
                            DefaultOutPutPath = path; // Use the project folder
                        }

            DefaultSolutionPath = path;
        }


        /// <summary>
        /// Parse all the arguments from the app.config
        /// </summary>
        private void ParseAppConfig()
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            foreach (string key in appSettings.Keys)
            {
                string value = appSettings[key];
                if (Arguments.ContainsKey(key))
                {
                    Arguments[key] = value;
                }
                else
                {
                    Arguments.Add(key, value);
                }
            }
        }


        /// <summary>
        /// Parse all the arguments from the app.config
        /// </summary>
        private void ParseLocalConfig()
        {
            //Load the Config file from the current directory if possible.
            if (File.Exists(Environment.CurrentDirectory + @"\WSPBuilder.exe.config"))
            {
                ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
                ecfm.ExeConfigFilename = Environment.CurrentDirectory + @"\WSPBuilder.exe.config";
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
               // AppSettingsSection appSection = (AppSettingsSection)config.GetSection("appSettings");

                KeyValueConfigurationCollection appSettings = config.AppSettings.Settings;
                foreach (string key in appSettings.AllKeys)
                {
                    string value = appSettings[key].Value;
                    if (Arguments.ContainsKey(key))
                    {
                        Arguments[key] = value;
                    }
                    else
                    {
                        Arguments.Add(key, value);
                    }
                }
            }

        }




        /// <summary>
        /// Gets a value for the specified key. A default value is defined in case that the argument has not been defined.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetString(string key, string defaultValue)
        {
            string value = null;

            if (Arguments.ContainsKey(key))
            {
                value = Arguments[key];
            }

            if (String.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// Gets a value for the specified key. A default value is defined in case that the argument has not been defined.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetBool(string key, bool defaultValue)
        {
            string value = this.GetString(key, defaultValue.ToString());

            bool result = defaultValue;

            bool success = bool.TryParse(value, out result);

            if (!success)
            {
                throw new ApplicationException("Invalid value '" + value + "' for key '" + key + "'. The value has to be 'true' or 'false'.");
            }

            return result;
        }

        /// <summary>
        /// Gets a value for the specified key. A default value is defined in case that the argument has not been defined.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Guid GetGuid(string key, Guid defaultValue)
        {
            string value = this.GetString(key, defaultValue.ToString());

            Guid result = Guid.Empty;
            try
            {
                result = new Guid(value);
            }
            catch
            {
                throw new ApplicationException("Invalid value '" + value + "' for key '" + key + "'. The value has to be a valid GUID.");
            }
            return result;
        }


        public DirectoryInfo GetArgumentDirectory(string argumentName, string subpath)
        {
            DirectoryInfo result = null;
            // Try to get the path from the configuration
            string path = Config.Current.GetString(argumentName, "");

            if (string.IsNullOrEmpty(path))
            {
                // Use the current directory
                path = ProjectPath + @"\" + subpath;
                if (Directory.Exists(path))
                {
                    result = new DirectoryInfo(path);
                }
            }
            else
            {
                if (!Directory.Exists(path))
                {
                    string fullPath = Path.GetFullPath(path);
                    if (!Directory.Exists(fullPath))
                    {
                        throw new ApplicationException("The path '" + fullPath + "' do not exits. Please specify a valid path for argument " + argumentName);
                    }
                }

                // Ensure that the path exists now and remove tailing backslash
                // Or handle paths like "..\..\" and so on.
                result = new DirectoryInfo(path);
            }
            return result;
        }

        /// <summary>
        /// Loads a list of permissions from a string.
        /// </summary>
        /// <param name="permissionSet">Permissions in a string</param>
        /// <returns>A StringDictionary with permissions</returns>
        private StringDictionary LoadPermissions(string permissionSet)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(permissionSet);

            StringDictionary permissions = new StringDictionary();

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                XmlAttribute attClass = node.Attributes["class"];

                if (!permissions.ContainsKey(attClass.Value))
                {
                    permissions.Add(attClass.Value, node.OuterXml);
                }
            }
            return permissions;
        }

        /// <summary>
        /// Loads the file list and gets the Full path if the filename is relative.
        /// Thanks to Mikhail Dikov for this method.
        /// </summary>
        /// <param name="filename">The txt file containing the files paths</param>
        /// <returns>StringDictionary of file paths</returns>
        private StringDictionary LoadWSPFileList(string filename)
        {
            StringDictionary filelist = new StringDictionary();
            if (!String.IsNullOrEmpty(filename))
            {
                // Load the string lines into the Dictionary
                using (StreamReader sr = new StreamReader(filename))
                {
                    while (sr.Peek() >= 0)
                    {
                        string filepath = sr.ReadLine();
                        // Check if the path is absolute
                        if (!Path.IsPathRooted(filepath))
                        {
                            filepath = Path.GetFullPath(filepath);
                        }

                        filelist.Add(filepath, string.Empty);
                    }
                    sr.Close();
                }
            }
            return filelist;
        }

        #endregion
    }
}
