#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using System.IO;
using WSPTools.VisualStudio.VSAddIn.Library;
#endregion

namespace WSPTools.VisualStudio.VSAddIn
{
    public class ProjectPaths
    {
        #region fields

        private Project _selectedProject = null;

        private string _fullPath = null;
        private DirectoryInfo _dirInfo = null;
        private string _sharePointRoot = null;
        private string _path80 = null; 
        private string _pathGAC = null;
        private string _outputPath = null;

        #endregion

        #region public properties

        public Project SelectedProject
        {
            get { return _selectedProject; }
            set { _selectedProject = value; }
        }

        public string FullPath
        {
            get 
            {
                if (_fullPath == null)
                {
                    if (SelectedProject != null)
                    {
                        _fullPath = SelectedProject.Properties.Item("FullPath").Value as string;
                    }
                }
                return _fullPath; 
            }
            set { _fullPath = value; }
        }

        public DirectoryInfo DirInfo
        {
            get 
            {
                if (_dirInfo == null)
                {
                    if (Directory.Exists(this.FullPath))
                    {
                        _dirInfo = new DirectoryInfo(this.FullPath);
                    }
                }
                return _dirInfo; 
            }
            set { _dirInfo = value; }
        }


        public string PathSharePointRoot
        {
            get
            {
                if (_sharePointRoot == null)
                {
                    if (Directory.Exists(this.FullPath + @"12"))
                    {
                        _sharePointRoot = this.FullPath + @"12";
                    }
                    if (Directory.Exists(this.FullPath + @"14"))
                    {
                        _sharePointRoot = this.FullPath + @"14";
                    }
                    if (Directory.Exists(this.FullPath + @"SharePointRoot"))
                    {
                        _sharePointRoot = this.FullPath + @"SharePointRoot";
                    }

                    if (String.IsNullOrEmpty(_sharePointRoot))
                    {
                        _sharePointRoot = this.FullPath + @"SharePointRoot";
                    }
                }
                return _sharePointRoot; 
            }
            set { _sharePointRoot = value; }
        }


        public string Path80
        {
            get 
            {
                if (_path80 == null)
                {
                    _path80 = this.FullPath + @"80";
                }
                return _path80; 
            }
            set { _path80 = value; }
        }


        public string PathGAC
        {
            get 
            {
                if (_pathGAC == null)
                {
                    _pathGAC = this.FullPath + @"GAC";
                }
                return _pathGAC; 
            }
            set { _pathGAC = value; }
        }



        public string OutputPathDLL
        {
            get
            {
                if (_outputPath == null)
                {
                    _outputPath = Utility.CombinePaths(this.FullPath, this.SelectedProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value as string);
                }
                return _outputPath;
            }
        }

        public string OutputFilename
        {
            get
            {
                string result = "";
                if (this.SelectedProject != null)
                {
                    result = this.SelectedProject.Properties.Item("OutputFileName").Value.ToString();
                }
                return result;
            }
        }

        public string OutputFilePath
        {
            get
            {
                return Path.Combine(this.OutputPathDLL, this.OutputFilename);
            }
        }

        #endregion

        #region constructor & descructor

        public ProjectPaths(Project project)
        {
            this.SelectedProject = project;
        }
        #endregion

        #region public methods

        public bool DoSharePointRootExist()
        {
            bool result = Directory.Exists(this.PathSharePointRoot);
 
            return result;
        }

        #endregion

        #region private methods
        #endregion
    }
}