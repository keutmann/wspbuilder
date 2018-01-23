#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Globalization;
using Keutmann.SharePoint.WSPBuilder.SystemServices;
#endregion

namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public class AssemblyInfo
    {
        
        private string _key = null;
        private AssemblyName _name = null;
        private FileInfo _fileHandle = null;
        private bool _managed = true;
        private bool _resource = false;
        private bool _reference = false;

        private SolutionDeploymentTargetType _targetType = SolutionDeploymentTargetType.GlobalAssemblyCache;

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        } 

        public AssemblyName Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public FileInfo FileHandle
        {
            get { return _fileHandle; }
            set { _fileHandle = value; }
        }

        public bool Managed
        {
            get { return _managed; }
            set { _managed = value; }
        }

        /// <summary>
        /// Specifies that the assembly is not to be reflected on and no security policy is created.
        /// </summary>
        public bool Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }

        public bool Resource
        {
          get { return _resource; }
          set { _resource = value; }
        }

        public SolutionDeploymentTargetType TargetType
        {
            get { return _targetType; }
            set { _targetType = value; }
        }

        public bool IsUserAssembly
        {
            get
            {
                bool result = true;
                if (this.Name != null && this.Name.FullName != null)
                {
                    result = this.Name.FullName.StartsWith("CabLib", StringComparison.OrdinalIgnoreCase)
                        || (
                                this.Name.FullName.IndexOf(AssemblyNames.MICROSOFT_SHAREPOINT_PUBLICKEYTOKEN, StringComparison.InvariantCultureIgnoreCase) < 0
                            && this.Name.FullName.IndexOf(AssemblyNames.WSPBUILDER_PUBLICKEYTOKEN, StringComparison.InvariantCultureIgnoreCase) < 0
                            );
                }
                return result;
            }
        }

        public AssemblyInfo()
        {
        }

        public AssemblyInfo(FileInfo assemblyFileHandle, SolutionDeploymentTargetType targetType)
        {
            this.TargetType = targetType;

            // Get e.g. mytest.dll or en-US\mytest.resource.dll
            this.Key = assemblyFileHandle.Name;
            this.FileHandle = assemblyFileHandle;

            if (this.Key.EndsWith(".Resources.dll", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Key = assemblyFileHandle.Directory.Name + @"\" + assemblyFileHandle.Name;
                this.Resource = true;
            }

            // If the assembly is found in a References folder, then do not refect on it for safecontrols.
            string dirName = assemblyFileHandle.Directory.Name;
            if ("Reference".Equals(dirName, StringComparison.OrdinalIgnoreCase) || "References".Equals(dirName, StringComparison.OrdinalIgnoreCase))
            {
                this.Reference = true;
            }

            try
            {
                this.Name = AssemblyName.GetAssemblyName(this.FileHandle.FullName);
            }
            catch (BadImageFormatException imageEx)
            {
                // Check if the assembly is unmanage like Resource dlls
                int hrResult = Marshal.GetHRForException(imageEx);
                if (hrResult != AssemblyStore.COR_E_ASSEMBLYEXPECTED)
                {
                    this.Managed = false;
                }
            }
        }

    }
}