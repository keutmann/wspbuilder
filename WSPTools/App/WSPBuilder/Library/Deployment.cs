#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#endregion

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public class Deployment
    {
        #region fields
        #endregion

        #region public properties
        #endregion

        #region constructor & descructor
        #endregion

        #region public methods

        public static void HandleArguments()
        {
            if (Config.Current.Deploy)
            {
                PreFlight();
                string wspFilename = GetWSPFilename("default");
                Deploy(wspFilename);
            }
            if (Config.Current.Retract)
            {
                PreFlight();
                string wspFilename = GetWSPFilename("default");
                Retract(wspFilename);
            }
            if (!String.IsNullOrEmpty(Config.Current.Install))
            {
                PreFlight();
                string wspFilename = GetWSPFilename(Config.Current.Install);
                Install(wspFilename);
            }
            if (!String.IsNullOrEmpty(Config.Current.Uninstall))
            {
                PreFlight();
                string wspFilename = GetWSPFilename(Config.Current.Uninstall);
                Uninstall(wspFilename);
            }
            if (!String.IsNullOrEmpty(Config.Current.Upgrade))
            {
                PreFlight();
                string wspFilename = GetWSPFilename(Config.Current.Upgrade);
                Upgrade(wspFilename);
            }
        }

        /// <summary>
        /// Deploys the WSP package.
        /// </summary>
        public static void Deploy(string wspFilename)
        {
            // Deploy the WSP package.

            Installer ins = new Installer(wspFilename);
            if (ins.IsAlreadyInstalled())
            {
                Uninstall(wspFilename);
            }
            Log.Information("Install and deploying " + wspFilename);
            ins.DeploySolution();
        }

        public static void Retract(string wspFilename)
        {
            Installer ins = new Installer(wspFilename);
            if (ins.IsAlreadyInstalled())
            {
                Log.Information("Retracting " + wspFilename);
                
                ins.RetractSolution();
            }
            else
            {
                Log.Information("No "+wspFilename + " solution was found therefore not able to retract.");
            }
        }

        public static void Install(string wspFilename)
        {
            Installer ins = new Installer(wspFilename);
            if (ins.IsAlreadyInstalled())
            {
                Uninstall(wspFilename);
            }
            Log.Information("Install WSP package.");
            ins.InstallSolution();
        }

        public static void Uninstall(string wspFilename)
        {
            Installer ins = new Installer(wspFilename);
            if(ins.IsAlreadyInstalled())
            {
                Log.Information("Uninstall " + wspFilename);
                ins.UninstallSolution();
            }
            else
            {
                Log.Information("No " + wspFilename + " solution was found therefore not able to uninstall.");
            }

        }

        public static void Upgrade(string wspFilename)
        {
            Installer ins = new Installer(wspFilename);
            if (ins.IsAlreadyInstalled())
            {
                Log.Information("Upgrading WSP package.");
                ins.UpgradeSolution();
            }
            else
            {
                Deploy(wspFilename);
            }
        }

        #endregion

        #region private methods

        private static void PreFlight()
        {
            string[] preflight = Installer.GetPreFlightErrors();
            if (preflight.Length > 0)
            {
                foreach (string err in preflight)
                {
                    Log.Error(err);
                }

                throw new Exception("Preflight requirements failed.");
            }
        }

        private static string GetWSPFilename(string argument)
        {
            string wspFilename = argument;
            if (wspFilename.Equals("default", StringComparison.InvariantCultureIgnoreCase))
            {
                wspFilename = Config.Current.WSPName;
            }

            if (!File.Exists(Config.Current.OutputPath + @"\" + wspFilename))
            {
                throw new ApplicationException("Cannot find WSP file '" + wspFilename + "' therefore not able to execute command.");
            }

            return wspFilename;
        }

        #endregion
    }
}