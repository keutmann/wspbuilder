#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#endregion

namespace WSPTools.BaseLibrary.VisualStudio
{
    public class ProjectPaths
    {
        #region fields

        private string _fullPath = null;
        private DirectoryInfo _dirInfo = null;
        private string _sharePointRootName = null;
        private string _path80 = null; 
        private string _pathGAC = null;
        
        #endregion

        #region public properties

        public string FullPath
        {
            get 
            {
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

        
        

        public string SharePointRootName
        {
            get
            {
                if (_sharePointRootName == null)
                {
                    if (Directory.Exists(this.FullPath + @"12"))
                    {
                        _sharePointRootName = @"12";
                    }
                    if (Directory.Exists(this.FullPath + @"14"))
                    {
                        _sharePointRootName = @"14";
                    }
                    if (Directory.Exists(this.FullPath + @"SharePointRoot"))
                    {
                        _sharePointRootName = @"SharePointRoot";
                    }

                    if (String.IsNullOrEmpty(_sharePointRootName))
                    {
                        _sharePointRootName = @"SharePointRoot";
                    }
                }
                return _sharePointRootName; 
            }
            set { _sharePointRootName = value; }
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


        private string _pathFeatures = null;
        public string PathFeatures
        {
            get 
            {
                if (_pathFeatures == null)
                {
                    _pathFeatures = Path.Combine( this.FullPath, this.SharePointRootName + @"\TEMPLATE\FEATURES\");
                }
                return _pathFeatures; 
            }
            set { _pathFeatures = value; }
        }

        #endregion

        #region constructor & descructor

        public ProjectPaths(string projectPath)
        {
            this.FullPath = projectPath;
        }
        #endregion

        #region public methods

        public bool DoSharePointRootExist()
        {
            bool result = Directory.Exists(Path.Combine(this.FullPath, this.SharePointRootName));
 
            return result;
        }

        #endregion

        #region private methods
        #endregion
    }
}