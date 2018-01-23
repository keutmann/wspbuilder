using System;
using System.Collections.Generic;
using System.Text;
using Keutmann.SharePoint.WSPBuilder;
using Keutmann.SharePoint.WSPBuilder.Library;

namespace TestWSPBuilder.Library
{
    public class WSPBuilderHandler
    {
        private string _arguments = "-cleanup false -buildcab false -TraceLevel Verbose";
        private string _projectName = string.Empty;
        private string _projectPath = string.Empty;

        public string Arguments
        {
            get 
            { 
                return _arguments; 
            }
            set 
            { 
                _arguments = value; 
            }
        }


        public string ProjectPath
        {
            get
            {
                if (String.IsNullOrEmpty(_projectPath))
                {
                    _projectPath = TestInvironment.ProjectsPath + @"\" + _projectName;
                }
                return _projectPath; 
            }
            set { _projectPath = value; }
        }


        public WSPBuilderHandler(string projectName)
        {
            _projectName = projectName;
        }

        public int Build()
        {
            string tempPath = Environment.CurrentDirectory;
            Environment.CurrentDirectory = ProjectPath;
            Config.Current = new Config();
            Config.Current.Arguments.Parse(Arguments, " ");
            int error = Program.Main(null);

            Environment.CurrentDirectory = tempPath;
            return error;
        }

        public int BuildCommand(string[] args)
        {
            string tempPath = Environment.CurrentDirectory;
            Environment.CurrentDirectory = ProjectPath;
            int result = Program.Main(args);

            Environment.CurrentDirectory = tempPath;
            return result;
        }

    }
}
