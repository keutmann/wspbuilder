#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using Microsoft.Win32;
using Microsoft.Web.Administration;
using Keutmann.SharePoint.WSPBuilder.Library;


#endregion

namespace Keutmann.SharePoint.WSPBuilder.SystemServices
{
    public class IIServerManager
    {
        #region constructor & descructor

        private IIServerManager()
        {
        }

        #endregion

        #region public methods

        public static void RecycleApplicationPool(string name)
        {
            // Only recycle one app pool therefore the name is specified!
            RecycleApplicationPools(name);
        }

        public static void RecycleApplicationPools()
        {
            // Recycle all the app pools therefore the null value.
            RecycleApplicationPools(null);
        }


        private static void RecycleApplicationPools(string name)
        {
            //Log.Information("Recycling IIS Application Pools");
            bool found = false;

            if (IsIIS7())
            {
                ServerManager manager = new ServerManager();

                foreach (ApplicationPool pool in manager.ApplicationPools)
                {
                    if (String.IsNullOrEmpty(name) || pool.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        RecycleIIS7AppPool(pool);
                        found = true;
                    }
                }

            }
            else
            {
                DirectoryEntry iis6appPoolsEntry = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Environment.MachineName));

                foreach (DirectoryEntry appPoolEntry in iis6appPoolsEntry.Children)
                {
                    if (String.IsNullOrEmpty(name) || appPoolEntry.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        RecycleIIS6AppPool(appPoolEntry);
                        found = true;
                    }
                }
            }

            if (!String.IsNullOrEmpty(name) && !found)
            {
                throw new ArgumentException(String.Format("No application pool by the name {0} was found!", name));
            }
        }

        private static void RecycleIIS6AppPool(DirectoryEntry appPoolEntry)
        {
            try
            {
                Log.Verbose(String.Format("Recycling {0}", appPoolEntry.Name));
                appPoolEntry.Invoke("Recycle", null);
            }
            catch
            {
                // If the Application Pool is stopped, an exception is thrown.
                // Continue to the next AppPool
                Log.Error(String.Format("Cannot recycle {0}", appPoolEntry.Name));
            }
        }

        private static void RecycleIIS7AppPool(ApplicationPool pool)
        {
            if (pool.State == ObjectState.Started)
            {
                Log.Verbose(String.Format("Recycling {0}", pool.Name));
                pool.Recycle();
            }
            else
            {
                Log.Error(String.Format("Cannot recycle {0} because of state: {1}", pool.Name, pool.State));
            }
        }

        #endregion

        #region private methods

        private static bool IsIIS7()
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