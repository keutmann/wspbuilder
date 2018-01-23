#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using Microsoft.Win32;
using WSPTools.VisualStudio.VSAddIn.Library;

#endregion

namespace WSPTools.VisualStudio.VSAddIn
{
    public class AppPoolHandle
    {

        #region fields

        private DTEHandler _DTEInstance = null;
        private string _machineName = null;

        DirectoryEntry _iis6appPoolsEntry = null;


        #endregion

        #region public properties

        public DTEHandler DTEInstance
        {
            get { return _DTEInstance; }
            set { _DTEInstance = value; }
        }

        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; }
        }

        public DirectoryEntry IIS6AppPoolsEntry
        {
            get 
            {
                if (_iis6appPoolsEntry == null)
                {
                    _iis6appPoolsEntry = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", this.MachineName));
                }
                return _iis6appPoolsEntry; 
            }
            set { _iis6appPoolsEntry = value; }
        }


        #endregion

        #region constructor & descructor

        public AppPoolHandle(DTEHandler handler)
        {
            this.DTEInstance = handler;
            this.MachineName = Environment.MachineName;   
        }

        #endregion

        #region public methods


        public void RecycleAppPools()
        {
            if (IsIIS7())
            {
                IIS7Handler iis7Handler = new IIS7Handler(this.DTEInstance);
                iis7Handler.RecycleAppPools();
            }
            else
            {
                foreach (DirectoryEntry appPoolEntry in IIS6AppPoolsEntry.Children)
                {
                    try
                    {

                        this.DTEInstance.WriteBuildWindow(String.Format("Recycling {0}",appPoolEntry.Name));
                        appPoolEntry.Invoke("Recycle", null);
                    }
                    catch
                    {
                        // If the Application Pool is stopped, an exception is thrown.
                        // Continue to the next AppPool
                        this.DTEInstance.WriteBuildWindow(String.Format("Cannot recycle {0}", appPoolEntry.Name));
                    }

                }
            }
        }

        #endregion

        #region private methods

        private bool IsIIS7()
        {
            bool result = false;
            RegistryKey vskey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\InetStp");
            if (vskey != null)
            {
                string versionString = vskey.GetValue("MajorVersion", "")+ string.Empty;
                int majorVersion = 0;
                if(int.TryParse(versionString, out majorVersion))
                {
                    if(majorVersion > 6)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        #endregion
    }
}