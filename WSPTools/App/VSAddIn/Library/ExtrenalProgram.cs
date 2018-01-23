#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
#endregion

namespace WSPTools.VisualStudio.VSAddIn
{
    public class ExternalProgram
    {
        #region constants
        #endregion

        #region fields

        private DTEHandler _DTEInstance = null;
        private bool _running = false;

        #endregion

        #region public properties

        public DTEHandler DTEInstance
        {
            get { return _DTEInstance; }
            set { _DTEInstance = value; }
        }

        public bool Running
        {
            get { return _running; }
            set { _running = value; }
        }

        #endregion

        #region constructor & descructor

        public ExternalProgram(DTEHandler dteInstance)
        {
            this.DTEInstance = dteInstance;
        }

        #endregion

        #region public methods

        public void RunAsync(string program, string arguments)
        {
            string[] param = new string[] { program, arguments };

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(ExecuteAsync);
            bw.RunWorkerAsync(param);
        }
        #endregion

        #region private methods

        private void ExecuteAsync(object sender, DoWorkEventArgs e)
        {
            try
            {
                string[] programInfo = e.Argument as string[];

                string programName = programInfo[0];
                string arguments = programInfo[1];

                Running = true;

                if (this.DTEInstance.Application != null)
                {
                    this.DTEInstance.StartNewBuildWindow();
                    this.DTEInstance.WriteBuildWindow(programName + " " + arguments);
                }

                // Set up process info.
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = programName;
                psi.Arguments = arguments;
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
                {
                    throw new Exception("Unable to start " + programName + " process.");
                }

                while (!p.HasExited)
                {
                    string text = p.StandardOutput.ReadLine();
                    if (!String.IsNullOrEmpty(text))
                    {
                        this.DTEInstance.WriteBuildWindow(text);
                    }
                    System.Threading.Thread.Sleep(50);
                }

                this.DTEInstance.WriteBuildWindow(p.StandardOutput.ReadToEnd());
                this.DTEInstance.WriteBuildWindow(p.StandardError.ReadToEnd());

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                Running = false;
            }
        }

        #endregion
    }
}