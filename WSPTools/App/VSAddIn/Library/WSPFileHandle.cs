#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library
{
    public class WSPFileHandle
    {
        #region fields

        private ProjectPaths _projectFolder = null;
        private string _wspFilename = null;
        private FileInfo _wspFileInfo = null;

        #endregion

        #region public properties

        public ProjectPaths SelectedProject
        {
            get { return _projectFolder; }
            set { _projectFolder = value; }
        }

        public string WspFilename
        {
            get 
            {
                if (_wspFilename == null)
                {
                    if (this.SelectedProject != null && this.SelectedProject.DirInfo != null)
                    {

                        string wspFilePath = this.SelectedProject.DirInfo.FullName + @"\" + this.SelectedProject.DirInfo.Name + ".wsp";
                        if (File.Exists(wspFilePath))
                        {
                            _wspFilename = wspFilePath;
                        }
                        else
                        {
                            if (Directory.Exists(this.SelectedProject.DirInfo.FullName))
                            {
                                DirectoryInfo projectDir = new DirectoryInfo(this.SelectedProject.DirInfo.FullName);
                                foreach (FileInfo file in projectDir.GetFiles("*.wsp"))
                                {
                                    _wspFilename = file.FullName;
                                    break;
                                }
                            }
                        }

                    }
                }
                return _wspFilename; 
            }
            set { _wspFilename = value; }
        }

        public FileInfo WspFileInfo
        {
            get 
            {
                if (_wspFileInfo == null)
                {
                    if (this.WspFilename != null)
                    {
                        _wspFileInfo = new FileInfo(this.WspFilename);
                    }
                }
                return _wspFileInfo; 
            }
            set { _wspFileInfo = value; }
        }


        #endregion

        #region constructor & descructor

        public WSPFileHandle(DTEHandler dteHandler)
        {
            this.SelectedProject = new ProjectPaths(dteHandler.SelectedProject);
        }

        #endregion

        #region public methods

        public bool ProjectFilesHaveChanged()
        {
            return CheckForNewerFiles(this.SelectedProject.DirInfo);
        }

        #endregion

        #region private methods

        private bool CheckForNewerFiles(DirectoryInfo parentInfo)
        {
            DateTime wspLastWriteTime = this.WspFileInfo.LastWriteTime;

            foreach (FileInfo file in parentInfo.GetFiles("*.cs"))
            {
                if (wspLastWriteTime < file.LastWriteTime)
                {
                    return true;
                }
            }
            foreach (FileInfo file in parentInfo.GetFiles("*.xml"))
            {
                if (wspLastWriteTime < file.LastWriteTime)
                {
                    return true;
                }
            }

            foreach (DirectoryInfo child in parentInfo.GetDirectories())
            {
                if (CheckForNewerFiles(child))
                {
                    return true;
                }
            }
            return false;
        }


        #endregion
    }
}