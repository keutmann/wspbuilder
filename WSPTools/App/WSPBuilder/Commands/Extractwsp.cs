using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using CabLib;
using System.Collections.Generic;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using Keutmann.SharePoint.WSPBuilder.Framework.Serialization;

namespace Keutmann.SharePoint.WSPBuilder.Commands
{
    public class Extractwsp : ConsoleCommand
    {
        private Dictionary<string, Extract.kCabinetFileInfo> _wspfileList = new Dictionary<string, Extract.kCabinetFileInfo>(StringComparer.OrdinalIgnoreCase);


        private string _fileName = null;
        [Description("Define the WSP file to extract. Default is the last created WSP file from the current directory.")]
        public string Filename 
        {
            get
            {
                if (String.IsNullOrEmpty(_fileName))
                {
                    DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
                    FileInfo fileFound = null;
                    foreach(FileInfo file in dir.GetFiles("*.wsp"))
                    {
                        if(fileFound == null)
                        {
                            fileFound = file;
                        }
                        else
                        {
                            if(file.LastWriteTime > fileFound.LastWriteTime)
                            {
                                fileFound = file;
                            }
                        }
                    }
                    if (fileFound != null)
                    {
                        _fileName = fileFound.FullName;
                    }
                    else
                    {
                        throw new IOException(String.Format("No WSP file found in the directory {0}", Environment.CurrentDirectory));
                    }
                }
                return _fileName;
            }
            set
            {
                _fileName = value;

                if (!Path.IsPathRooted(_fileName))
                {
                    _fileName = Path.GetFullPath(_fileName);
                }
                _fileName = CabLib.Compress.Sprintf(_fileName, new object[] { 1 });
            }
        }

        private string _targetPath = null;
        [Description("Defines the target path to extract the wsp file in. Default is the current directory.")]
        public string TargetPath
        {
            get
            {
                if (String.IsNullOrEmpty(_targetPath))
                {
                    _targetPath = Environment.CurrentDirectory;
                }
                return _targetPath;
            }
            set
            {
                _targetPath = value;
                if (String.IsNullOrEmpty(_targetPath))
                {
                    _targetPath = Environment.CurrentDirectory;
                }
                if (!Directory.Exists(_targetPath))
                {
                    throw new IOException(String.Format("Directory with path {0} do not exist!", _targetPath));
                }
                if (_targetPath.EndsWith(@"\"))
                {
                    _targetPath = _targetPath.Substring(0, _targetPath.Length - 1);
                }
            }
        }

        private bool _overwrite = true;
        [Description("Overwrite existing files.")]
        public bool Overwrite
        {
            get
            {
                return _overwrite;
            }
            set
            {
                _overwrite = true;
            }
        }


        private string _sourcePath = null;

        internal string SourcePath
        {
            get 
            {
                if (String.IsNullOrEmpty(_sourcePath))
                {
                    _sourcePath = Path.Combine(this.TargetPath, Guid.NewGuid().ToString());
                }
                return _sourcePath; 
            }
            set { _sourcePath = value; }
        }



        internal Solution Manifest { get; set; }
        internal Dictionary<string, Extract.kCabinetFileInfo> WSPFileList
        {
            get { return _wspfileList; }
            set { _wspfileList = value; }
        }

        private Extract _extractor = new Extract();

        internal Extract Extractor
        {
            get { return _extractor; }
            set { _extractor = value; }
        }

        internal string CurrentFolder { get; set; }

        public override void Execute()
        {
            Log.Information("Extrating...");

            CabLib.Extract.kHeaderInfo k_Info;
            if (Extractor.IsFileCabinet(this.Filename, out k_Info))
            {
                Extractor.ExtractFile(this.Filename, this.SourcePath);
                
                string manifestFilename = Path.Combine(this.SourcePath, "manifest.xml");
                this.Manifest = FileSystem.Load<Solution>(manifestFilename);

                this.Manifest.Move(this);

                DirectorySystem.DeleteEmptyFolders(this.SourcePath);
                //Directory.Delete(this.SourcePath, true);
            }
            else
            {
                Log.Error("The file is not a valid Cabinet: " + this.Filename);
            }
        }
    }
}
