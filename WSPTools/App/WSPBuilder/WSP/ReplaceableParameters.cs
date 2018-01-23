using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.Library;
using WSPTools.BaseLibrary.Extensions;
using System.Reflection;
using Keutmann.Framework;
using System.Text.RegularExpressions;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{


    public partial class SolutionHandler
    {
        private List<string> _replacementFileExtensions = null;

        public List<string> ReplacementFileExtensions
        {
            get 
            {
                if (_replacementFileExtensions == null)
                {
                    _replacementFileExtensions = new List<string>();
                    _replacementFileExtensions.Add(".XML");
                    _replacementFileExtensions.Add(".ASCX");
                    _replacementFileExtensions.Add(".ASPX");
                    _replacementFileExtensions.Add(".WEBPART");
                    _replacementFileExtensions.Add(".DWP");
                }
                return _replacementFileExtensions; 
            }
        }

        private Dictionary<string, string> _replacementParameters = null;
        public Dictionary<string, string> ReplacementParameters
        {
            get 
            {
                if (_replacementParameters == null)
                {
                    _replacementParameters = BuildReplacementParameters();
                }
                return _replacementParameters; 
            }
        }


        public void CreateFilesWithReplacementParameters()
        {
            Log.Verbose("Processing files with Replacement Parameters");

            string programData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WSPBuilder");
            string tempProjectFolder = Path.Combine(programData, Config.Current.WSPName);
            DirectoryInfoExtensions.EnsureDirectory(tempProjectFolder);

            string search = CreateRegexSearch();

            Dictionary<string, string> modifiedCabFiles = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> cabFile in this.CabFiles)
            {
                string sourceFile = cabFile.Value;
                string destFile = cabFile.Key;

                string fileExtension = Path.GetExtension(sourceFile);
                if(!String.IsNullOrEmpty(fileExtension) && this.ReplacementFileExtensions.Contains(fileExtension.ToUpper()))
                {
                    Encoding coding = null;
                    string sourceData = FileExtensions.ReadAllTextSafely(sourceFile, out coding);
                    string result;

                    if (StringExtensions.MultipleReplace(sourceData, search, this.ReplacementParameters, out result, RegexOptions.IgnoreCase | RegexOptions.Compiled))
                    {
                        // Save the replaced content on a file and set the CabFile location
                        string targetFile = Path.Combine(tempProjectFolder, destFile);
                        FileInfo info = new FileInfo(targetFile);
                        DirectoryInfoExtensions.EnsureDirectory(info.DirectoryName);
                        File.WriteAllText(targetFile, result, coding);

                        //this.CabFiles[destFile] = targetFile;
                        modifiedCabFiles.Add(destFile, targetFile);
                    }
                }
            }

            // Update the CabFiles list
            foreach (KeyValuePair<string, string> entry in modifiedCabFiles)
            {
                this.CabFiles[entry.Key] = entry.Value;
            }


        }

        private string CreateRegexSearch()
        {
            string[] keys = new List<string>(this.ReplacementParameters.Keys).ToArray();
            string search = "(" + String.Join("|", keys) + ")";
            search = search.Replace("$", "\\$");
            return search;
        }

        public void CleanupReplacementParametersFiles()
        {
            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string tempProjectFolder = Path.Combine(programData, "WSPBuilder\\" + Config.Current.WSPName);

            Directory.Delete(tempProjectFolder, true);
        }

        private Dictionary<string, string> BuildReplacementParameters()
        {
            Dictionary<string, string> param = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!String.IsNullOrEmpty(Config.Current.ProjectAssemblyPath))
            {
                if (File.Exists(Config.Current.ProjectAssemblyPath))
                {
                    try
                    {
                        AssemblyName assemblyName = AssemblyName.GetAssemblyName(Config.Current.ProjectAssembly);
                        
                        param.Add("$SharePoint.Project.AssemblyFullName$", assemblyName.FullName);
                        param.Add("$SharePoint.Project.AssemblyFileName$", assemblyName.Name);
                        param.Add("$SharePoint.Project.AssemblyFileNameWithoutExtension$", Path.GetFileNameWithoutExtension(assemblyName.Name));
                        param.Add("$SharePoint.Project.AssemblyPublicKey$", AssemblyNameExtensions.GetPublicKey(assemblyName));

                        string keyToken = AssemblyNameExtensions.GetPublicKeyToken(assemblyName);
                        param.Add("$SharePoint.Project.AssemblyPublicKeyToken$", keyToken);
                        param.Add("$SharePoint.Project.AssemblyVersion$", AssemblyNameExtensions.GetVersion(assemblyName));
                        param.Add("$SharePoint.Project.AssemblyCulture$", AssemblyNameExtensions.GetCulture(assemblyName));

                        if (string.IsNullOrEmpty(keyToken))
                        {
                            Log.Warning("It seems that the defined Project Assembly is not strong named. Its recommended to strong name the assembly!");
                        }
                    }
                    catch 
                    {
                        Log.Error("The defined Project Assembly seems not to be a valid assembly!");
                        
                    }

                }
                else
                {
                    Log.Error("Can not find the defined Project Assembly : " + Config.Current.ProjectAssemblyPath);
                }
            }

            param.Add("$SharePoint.Package.Name$", Config.Current.WSPName);
            param.Add("$SharePoint.Package.FileName$", Config.Current.WSPName);
            param.Add("$SharePoint.Package.FileNameWithoutExtension$", Path.GetFileNameWithoutExtension(Config.Current.WSPName));
            param.Add("$SharePoint.Package.Id$", Config.Current.SolutionId.ToString());

            

            return param;
        }
    }
}
