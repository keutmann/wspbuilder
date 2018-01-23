/* Program : WSPBuilderVSAddIn
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using EnvDTE80;
using EnvDTE;
using System.ComponentModel;

namespace WSPTools.VisualStudio.VSAddIn
{
    public class WSPBuilderHandle
    {
        #region Const
        public const string WSPTOOLSKEY = @"HKEY_LOCAL_MACHINE\Software\WSPTools";
        public const string WSPTOOLSPATH = "WSPToolsPath";

        #endregion 

        #region Members

        private DTEHandler _DTEInstance = null;

        private string _wspToolsPath = null;
        private bool _running = false;

        #endregion

        #region Properties

        public DTEHandler DTEInstance
        {
            get { return _DTEInstance; }
            set { _DTEInstance = value; }
        }

        public string WSPToolsPath
        {
            get 
            {
                if (_wspToolsPath == null)
                {
                    _wspToolsPath = Registry.GetValue(WSPTOOLSKEY, WSPTOOLSPATH, "") as string;
                }
                return _wspToolsPath; 
            }
            set { _wspToolsPath = value; }
        }

        public bool Running
        {
            get { return _running; }
            set { _running = value; }
        }

        #endregion

        #region Methods

        public WSPBuilderHandle()
        {
        }

        public WSPBuilderHandle(DTEHandler handler)
        {
            this.DTEInstance = handler;
        }

        public void BuildAsync(ProjectPaths projectPaths)
        {
            //string output = projectPaths.OutputPath;
            string[] param = new string[] { projectPaths.FullPath, "-TraceLevel information" };
            Log("Building WSP file!");
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync(param);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] param = e.Argument as string[];
            Run(param[0], param[1]);
        }

        public void RunWSPBuilder(string path, string arguments)
        {
            string[] param = new string[] { path, arguments };
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync(param);
        }


        public void Deploy(string path)
        {
            string[] param = new string[] { path, "-BuildWSP false -Deploy true" };
            Log("Deploying to SharePoint!");
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync(param);
        }

        public void Retract(string path)
        {
            string[] param = new string[] { path, "-BuildWSP false -Retract true" };
            Log("Retracting the solution from SharePoint!");
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync(param);
        }

        public void Install(string path)
        {
            string[] param = new string[] { path, "-BuildWSP false -Install default" };
            Log("Installing the solution into SharePoint!");
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync(param);
        }

        public void Uninstall(string path)
        {
            string[] param = new string[] { path, "-BuildWSP false -Uninstall default" };
            Log("Uninstall the solution from SharePoint!");
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync(param);
        }

        public void Upgrade(string path)
        {
            string[] param = new string[] { path, "-BuildWSP false -Upgrade default" };
            Log("Upgrading the solution!");
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync(param);
        }

        public void Run(string path, string arguments)
        {
            //string temppath = Environment.CurrentDirectory;
            try
            {
                //Environment.CurrentDirectory = path;

                Running = true;

                if (this.DTEInstance != null)
                {
                    this.DTEInstance.StartNewBuildWindow();
                }

                // Set up process info.
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = WSPToolsPath + @"wspbuilder.exe";
                psi.Arguments = arguments;
                psi.WorkingDirectory = path;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                // Create the process.
                System.Diagnostics.Process p = new System.Diagnostics.Process();

                // Associate process info with the process.
                p.StartInfo = psi;
                
                // Run the process.
                bool fStarted = p.Start();
                

                if (!fStarted)
                    throw new Exception("Unable to start WSPBuilder.exe process.");

                while (!p.HasExited)
                {
                    string text = p.StandardOutput.ReadLine();
                    if (!String.IsNullOrEmpty(text))
                    {
                        if (this.DTEInstance != null)
                        {
                            this.DTEInstance.WriteBuildWindow(text);
                        }
                    }
                    System.Threading.Thread.Sleep(100);
                }

                if (this.DTEInstance != null)
                {
                    this.DTEInstance.WriteBuildWindow(p.StandardOutput.ReadToEnd());
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                Running = false;
                //Environment.CurrentDirectory = temppath;
            }

        }

        private void Log(string message)
        {
            if (this.DTEInstance != null)
            {
                this.DTEInstance.WriteBuildAndStatusBar(message);
            }
        }




        #endregion
    }
}
