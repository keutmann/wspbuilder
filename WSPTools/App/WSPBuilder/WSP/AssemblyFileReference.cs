/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 * -------------------------------------------------------------
 * 2008-04-06 Mono.Cecil Assembly logic added
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Serialization;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Runtime.InteropServices;
using Mono.Cecil;


namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public partial class SolutionHandler
    {
        #region Const
        private const int COR_E_ASSEMBLYEXPECTED = -2147024885;
      
        private const string RESETWEBSERVER = "ResetWebServer";
        private const string DEPLOYMENTTARGET = "DeploymentTarget";
        private const string INCLUDEASSEMBLIES = "IncludeAssemblies";
        private const string DLLREFERENCEPATH = "DLLReferencePath";
        private const string RESOLVEASSEMBLIES = "Resolveassemblies";

        #endregion

        #region Members

        private ApplicationException _resoleveException = null;
        private string _dllPath = string.Empty;
        private bool? _includeAssemblies = null;
        //private bool? _resolveAssemblies = null;
        private bool _gacDeployment = false;
        private TRUEFALSE? _resetWebServer = null;
        private bool _resetWebServerSpecified = false;




        #endregion

        #region Properties

        protected bool ResetWebServerSpecified
        {
            get { return _resetWebServerSpecified; }
            set { _resetWebServerSpecified = value; }
        }


        [DisplayName(@"-IncludeAssemblies [True|False] (Default is true)")]
        [Description(@"If true all assemblies specified in the GAC and bin gets included in the WSP file. Except Microsoft SharePoint DLL's, they are never included.")]
        public bool IncludeAssemblies
        {
            get
            {
                if (_includeAssemblies == null)
                {
                    _includeAssemblies = Config.Current.GetBool(INCLUDEASSEMBLIES, true);
                }
                return (bool)_includeAssemblies;
            }
            set
            {
                _includeAssemblies = value;
            }
        }

        //[DisplayName(@"-ResolveAssemblies [True|False] (Default is true)")]
        //[Description(@"If true all assemblies are resolved in order to find every Class for the SafeControls section. If false then '-ExpandTypes' argument is not fully functional.")]
        //public bool ResolveAssemblies
        //{
        //    get
        //    {
        //        if (_resolveAssemblies == null)
        //        {
        //            _resolveAssemblies = Config.Current.GetBool(RESOLVEASSEMBLIES, true);
        //        }
        //        return (bool)_resolveAssemblies;
        //    }
        //    set
        //    {
        //        _resolveAssemblies = value;
        //    }
        //}


        [DisplayName(@"-ResetWebServer [true|false] (Default is true for GAC deployment and false for only bin deployment)")]
        [Description(@"Resets the WebServer after installation.")]
        public TRUEFALSE ResetWebServer
        {
            get
            {
                if (_resetWebServer == null)
                {
                    _resetWebServer = TRUEFALSE.True;

                    string resetSet = Config.Current.GetString(RESETWEBSERVER, "");
                    if (!String.IsNullOrEmpty(resetSet))
                    {
                        ResetWebServerSpecified = true;
                        _resetWebServer = (Config.Current.GetBool(RESETWEBSERVER, _gacDeployment)) ? TRUEFALSE.True : TRUEFALSE.FALSE;
                    }
                }
                return (TRUEFALSE)_resetWebServer;
            }
        }


        [DisplayName(@"-DeploymentTarget [GAC|BIN|Auto] (Default is Auto)")]
        [Description(@"The default 'Auto' is that DLL's found in the \GAC folder is deployed to the Global Assembly Cache and DLL's found in the 80\bin folder are deployed to the virtual server bin directory")]
        public string DeploymentTarget
        {
            get
            {
                return Config.Current.GetString(DEPLOYMENTTARGET, "Auto");
            }
        }


        [DisplayName(@"-DLLReferencePath [Path]")]
        [Description(@"Specifies where to look for reference DLL's inherited by the class types in the solution. These reference DLL's are not included in the WSP package.")]
        public DirectoryInfo DLLReferencePath
        {
            get
            {
                return Config.Current.GetArgumentDirectory(DLLREFERENCEPATH, "");
            }
        }

        #endregion

        #region Private Properties 

        private Dictionary<string, AssemblyFileReference> AssemblyFileReferenceCollection = null;

        #endregion

        #region Methods


        public void FindAssemblies(DirectoryInfo parentDir, Dictionary<string, AssemblyInfo> assembliesFound)
        {
            // First probe the bin\debug or bin\release directories
            // HACK: To support WSPBuilder Extensions the bin folder is now probed for assemblies.
            // However only the newest assemblies are used.
            string binFolder = Config.Current.ProjectPath + @"\bin";
            if (Directory.Exists(binFolder))
            {
                DirectoryInfo projectDllPathDir = new DirectoryInfo(binFolder);
                if (FileProvider.IncludeDir(projectDllPathDir))
                {
                    SolutionDeploymentTargetType targetType = SolutionDeploymentTargetType.GlobalAssemblyCache;
                    if (Config.Current.BuildMode == BuildModeType.Debug)
                    {
                        targetType = SolutionDeploymentTargetType.WebApplication;
                    }
                    else
                    {
                        _gacDeployment = true;
                    }

                    // Only use them if they are newer!
                    FindAssembliesInDirectory(projectDllPathDir, targetType, assembliesFound);
                }
            }

            // Probe the GAC             
            if (FileProvider.IncludeDir(Config.Current.DirGAC))
            {
                // Only use them if they are newer!
                FindAssembliesInDirectory(Config.Current.DirGAC, SolutionDeploymentTargetType.GlobalAssemblyCache, assembliesFound);
                _gacDeployment = true;
            }

            // Probe the 80\Bin
            if (FileProvider.IncludeDir(Config.Current.DirBin))
            {
                // Only use them if they are newer!
                FindAssembliesInDirectory(Config.Current.DirBin, SolutionDeploymentTargetType.WebApplication, assembliesFound);
            }

            // Wait to specify the ResetWebServer to after that _gacDeployment have been set.
            this.Solution.ResetWebServer = this.ResetWebServer;
            this.Solution.ResetWebServerSpecified = this.ResetWebServerSpecified;
        }

        private void FindAssembliesInDirectory(DirectoryInfo parentDir, SolutionDeploymentTargetType targetType, Dictionary<string, AssemblyInfo> assembliesFound)
        {
            foreach (FileInfo dllFileInfo in parentDir.GetFiles("*.dll"))
            {

                if (FileProvider.IncludeFile(dllFileInfo))
                {
                    AddCandidateAssembly(dllFileInfo, targetType, assembliesFound);
                }
            }

            foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
            {
                FindAssembliesInDirectory(childDir, targetType, assembliesFound);
            }
        }

        private void AddCandidateAssembly(FileInfo candidateAssemblyFileHandle, SolutionDeploymentTargetType targetType, Dictionary<string, AssemblyInfo> assembliesFound)
        {
            // Create the asseblyInfo object form the file
            AssemblyInfo candidateAssemblyInfo = new AssemblyInfo(candidateAssemblyFileHandle, targetType);

            // Do not include Microsoft Assemblies and WSPBuilder, however include CabLib.dll
            if (candidateAssemblyInfo.IsUserAssembly)
            {
                if (AssemblyLocationCheck(candidateAssemblyInfo))
                {
                    if (MultipleAssemblyCheck(candidateAssemblyInfo, assembliesFound))
                    {
                        assembliesFound.Add(candidateAssemblyInfo.Key, candidateAssemblyInfo);
                    }
                }
            }
        }

        private bool AssemblyLocationCheck(AssemblyInfo info)
        {
            bool result = true;
            SolutionDeploymentTargetType targetType = GetDeploymentTarget(info.TargetType);
            if (!info.Managed && targetType == SolutionDeploymentTargetType.GlobalAssemblyCache)
            {
                result = false;
                Log.Error("The assembly " + info.FileHandle.FullName + " is unmanaged. The assembly will be excluded from the WSP package!");
            }

            return result;
        }

        private bool MultipleAssemblyCheck(AssemblyInfo candidateAssemblyInfo, Dictionary<string, AssemblyInfo> assembliesFound)
        {
            bool result = true;

            // If the assembly already have been added then check on LastWrite date
            if (assembliesFound.ContainsKey(candidateAssemblyInfo.Key))
            {
                AssemblyInfo assemblyInDictionary = assembliesFound[candidateAssemblyInfo.Key];

                Log.Warning("More than one " + candidateAssemblyInfo.Key + " assembly found!" +
                    "\r\nFirst found " + assemblyInDictionary.FileHandle.FullName +
                    "\r\nSecond found " + candidateAssemblyInfo.FileHandle.FullName);

                string fullName = string.Empty;
                // Check the write date
                if (candidateAssemblyInfo.FileHandle.LastWriteTime > assemblyInDictionary.FileHandle.LastWriteTime)
                {
                    assembliesFound.Remove(assemblyInDictionary.Key);
                    fullName = candidateAssemblyInfo.FileHandle.FullName;
                }
                else
                {
                    // Keep the assembly in the Dictionary
                    result = false;
                    fullName = assemblyInDictionary.FileHandle.FullName;
                }
                Log.Warning("The assembly " + fullName + " will be used!");
            }
            return result;
        }


        public AssemblyFileReference[] BuildAssemblyFileReference(Dictionary<string, AssemblyInfo> assembliesFound)
        {
            // Temporary list 
            this.AssemblyFileReferenceCollection = new Dictionary<string, AssemblyFileReference>(StringComparer.OrdinalIgnoreCase);

            // Add existing Assembly References from Manifest.config into the temporary list AssemblyFileReferenceCollection
            if (this.Solution.Assemblies != null)
            {
                foreach (AssemblyFileReference item in this.Solution.Assemblies)
                {
                    this.AssemblyFileReferenceCollection.Add(item.Location, item);
                }
            }

            //
            foreach (KeyValuePair<string, AssemblyInfo> assemblyEntry in assembliesFound)
            {
                // Create the AssemblyFileReference or use the one from the temporary list
                AssemblyFileReference reference = CreateAssemblyFileReference(assemblyEntry.Value);
                
                // Add the assembly Reference if it is not already is in the temporary list
                if (reference != null && !this.AssemblyFileReferenceCollection.ContainsKey(reference.Location))
                {
                    this.AssemblyFileReferenceCollection.Add(reference.Location, reference);
                }

                // Add the assembly to the CAB file list
                this.AddToCab(assemblyEntry.Value.FileHandle.FullName, assemblyEntry.Value.Key);
            }

            if (this.AssemblyFileReferenceCollection.Count == 0)
            {
                return null;
            }

            AssemblyFileReference[] result = new AssemblyFileReference[this.AssemblyFileReferenceCollection.Values.Count];
            this.AssemblyFileReferenceCollection.Values.CopyTo(result, 0);

            return result;
        }


        /// <summary>
        /// Creates a AssemblyFileReference object based on the parameters.
        /// It will also auto build the SafeControl objects.
        /// </summary>
        private AssemblyFileReference CreateAssemblyFileReference(AssemblyInfo assemblyInfo)
        {
            AssemblyFileReference assemblyFileReference = null;
            if (!assemblyInfo.Resource)
            {
                if (!assemblyInfo.Reference)
                {
                    if (assemblyInfo.Managed)
                    {
                        // Create normal dll reference
                        assemblyFileReference = CreateManagedAssemblyFileReference(assemblyInfo);
                    }
                    else
                    {
                        // Create Unmanaged reference
                        assemblyFileReference = new AssemblyFileReference(assemblyInfo, SolutionDeploymentTargetType.WebApplication);
                        Log.Verbose(string.Format("Unmanaged Assembly added: {0}", assemblyInfo.Key));
                    }
                }
                else
                {
                    // Add the assembly as a ResourceDLL
                    assemblyFileReference = new AssemblyFileReference(assemblyInfo, GetDeploymentTarget(assemblyInfo.TargetType));
                    Log.Verbose(string.Format("Reference Assembly added: {0}", assemblyInfo.Key));
                }
            }
            else
            {
                // Create Resource reference
                SolutionDeploymentTargetType targetType = (assemblyInfo.Managed) ? assemblyInfo.TargetType : SolutionDeploymentTargetType.WebApplication;
                assemblyFileReference = new AssemblyFileReference(assemblyInfo, targetType);
                Log.Verbose(string.Format("Resource Assembly added: {0}", assemblyInfo.Key));
            }

            return assemblyFileReference;
        }


        private AssemblyFileReference CreateManagedAssemblyFileReference(AssemblyInfo assemblyInfo)
        {
            Log.Verbose(string.Format("Adding assembly: {0}", assemblyInfo.Key));


            AssemblyFileReference assemblyFileReference = null;
            if (this.AssemblyFileReferenceCollection.ContainsKey(assemblyInfo.Key))
            {
                // Use an existing Assembly Reference from the Temporary List
                assemblyFileReference = this.AssemblyFileReferenceCollection[assemblyInfo.Key];
            }
            else
            {
                // Create a new Assembly Reference, it will be added later.
                assemblyFileReference = new AssemblyFileReference(assemblyInfo, GetDeploymentTarget(assemblyInfo.TargetType));
            }

            // Load the assembly into Cecil object model 'AssemblyDefinition'
            AssemblyDefinition assembly = AssemblyStore.GetAssembly(assemblyInfo.FileHandle.FullName, this.DLLReferencePath.FullName);
            if (assembly != null)
            {
                assemblyFileReference.AssemblyObject = assembly;

                // Build the safecontrols if the assembly is not a Resource or a reference dll
                if (this.BuildSafeControls)
                {
                    CreateSafeControls(assembly, assemblyFileReference);
                }

                // Add the class resources
                ClassResourceDefinition[] definitions = BuildClassResourceDefinition(assemblyInfo.FileHandle);
                assemblyFileReference.ClassResources = AppendArray<ClassResourceDefinition>(assemblyFileReference.ClassResources, definitions);
            }

            return assemblyFileReference;
        }


        private void CreateSafeControls(AssemblyDefinition assembly, AssemblyFileReference assemblyFileReference)
        {
            try
            {
                // Add the safe controls for the assembly
                assemblyFileReference.SafeControls = BuildSafeControlDefinition(assembly, assemblyFileReference.SafeControls);
            }
            catch
            {
                // Throw an exception with a better description
                if (_resoleveException != null)
                {
                    throw _resoleveException;
                }
                else
                {
                    throw;
                }
            }
        }


        private SolutionDeploymentTargetType GetDeploymentTarget(SolutionDeploymentTargetType recommendedTagetType)
        {
            SolutionDeploymentTargetType result;
            if ("GAC".Equals(DeploymentTarget, StringComparison.InvariantCultureIgnoreCase))
            {
                result = SolutionDeploymentTargetType.GlobalAssemblyCache;
            }
            else
                if ("BIN".Equals(DeploymentTarget, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = SolutionDeploymentTargetType.WebApplication;
                }
                else
                    if ("AUTO".Equals(DeploymentTarget, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = recommendedTagetType;
                    }
                    else
                        throw new ApplicationException("Invalid value '" + DeploymentTarget + "' for key '" + DEPLOYMENTTARGET + "'. Has to be (GAC|BIN|Auto)");
            return result;
        }

        #endregion
    }
}