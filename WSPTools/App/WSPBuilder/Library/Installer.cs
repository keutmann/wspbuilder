/* Program : WSPBuilder
 * Created by: Chris Doty <cdoty1@yahoo.com>
 * Date : 7/30/2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 * Changes
 * ---------------------------------------------------------------------------- 
 * 2008     Keutmann
 * Log statements added.
 * Install and uninstall methods added.
 * WaitForJobToFinish.
 *
 * 20080221 TQC
 * Modified GetSolutionIdFromPackage to cache ids and use the value from solutionid.txt if there is an 
 * error. That should fix the x64 error, but the solution needs to be checked for unintended consequences.
 * 
 * 20091102 Keutmann
 * Modified to support SharePoint 2010
 */


#region Includes

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;
using WSPTools.BaseLibrary.Win32;
using Keutmann.SharePoint.WSPBuilder.SystemServices;
using WSPTools.BaseLibrary.SystemServices;

#endregion Includes

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public class Installer
    {
        #region Members

        private string _wspFilename = null;
        private Guid _solutionID = Guid.Empty;

        #endregion

        #region Properties 


        public string SolutionFilename
        {
            get { return _wspFilename; }
            set { _wspFilename = value; }
        }

        public Guid SolutionID
        {
            get 
            {
                if (_solutionID.Equals(Guid.Empty))
                {
                    _solutionID = GetSolutionIdFromPackage(this.SolutionFilename);
                }
                return _solutionID; 
            }
            set { _solutionID = value; }
        }

        #endregion

        #region Contructors

        public Installer()
        {
        }

        public Installer(string wspFilename)
        {
            this.SolutionFilename = wspFilename;
        }

        #endregion

        #region IsAlreadyInstalled

        /// <summary>
        /// returns true if the solution is already installed in the sharepoint server
        /// </summary>
        /// <param name="solutionID">the SolutionId (GUID) of the solution</param>
        /// <returns>true if installed</returns>
        public bool IsAlreadyInstalled()
        {
            return IsAlreadyInstalled(this.SolutionID);
        }

        /// <summary>
        /// returns true if the solution is already installed in the sharepoint server
        /// </summary>
        /// <param name="packageName">full path to the solution package</param>
        /// <returns>true if installed</returns>
        public static bool IsAlreadyInstalledByPackage(string packageName)
        {
            return IsAlreadyInstalled(GetSolutionIdFromPackage(packageName));
        }

        /// <summary>
        /// returns true if the solution is already installed in the sharepoint server
        /// </summary>
        /// <param name="solutionID">the SolutionId (GUID) of the solution</param>
        /// <returns>true if installed</returns>
        public static bool IsAlreadyInstalledByID(string solutionID)
        {
            return IsAlreadyInstalled(new Guid(solutionID));
        }

        /// <summary>
        /// returns true if the solution is already installed in the sharepoint server
        /// </summary>
        /// <param name="solutionID">the SolutionId (GUID) of the solution</param>
        /// <returns>true if installed</returns>
        public static bool IsAlreadyInstalled(Guid solutionID)
        {
            try
            {
                SPSolution sln = SPFarm.Local.Solutions[solutionID];
                if (sln != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message, ee);
            }
        }

        #endregion IsAlreadyInstalled

        #region Install solution

        public bool InstallSolution()
        {
            return InstallSolution(this.SolutionFilename, this.SolutionID);
        }

        public bool InstallSolution(string solutionFile)
        {
            return InstallSolution(solutionFile, GetSolutionIdFromPackage(solutionFile));
        }

       public bool InstallSolution(string solutionFile, Guid solutionID)
        {
            try
            {
                SPSolution solution = LocateInFarm(solutionID);
                if (solution != null && solution.JobExists)
                {
                    KillRunningJobs(solution);

                    // force add
                    solution = null;
                }

                // does not exist so add it
                if (solution == null)
                {
                    solution = SPFarm.Local.Solutions.Add(solutionFile);
                }
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (Exception ee)
            {
                throw new Exception("Unable to install solution", ee);
            }
            return true;
        }

        #endregion

        #region Uninstall solution

        public bool UninstallSolution()
        {
            return UninstallSolution(this.SolutionFilename, this.SolutionID);
        }

        public bool UninstallSolution(string solutionFile)
        {
            return UninstallSolution(solutionFile, GetSolutionIdFromPackage(solutionFile));
        }

        
        public bool UninstallSolution(string solutionFile, Guid solutionID)
        {
            try
            {
                SPSolution solution = SPFarm.Local.Solutions[solutionID];
                
                if (RetractSolution(solutionFile, solutionID))
                {
                    // Wait for it to end.
                    WaitForJobToFinish(solution);

                    SPFarm.Local.Solutions.Remove(solutionID);
                }
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (Exception ee)
            {
                throw new Exception("Unable to uninstall solution", ee);
            }
            return true;
        }


        #endregion
        
        #region DeploySolution

        public bool DeploySolution()
        {
            return this.DeploySolution(this.SolutionFilename, this.SolutionID, true, true);
        }

        /// <summary>
        /// Deploys a solution .wsp file to sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <returns>true if it was able to deploy</returns>
        public bool DeploySolution(string solutionFile)
        {
            return this.DeploySolution(solutionFile, GetSolutionIdFromPackage(solutionFile), true, true);
        }

        /// <summary>
        /// Deploys a solution .wsp file to sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <param name="solutionID">the SolutionId (GUID) of the solution</param>
        /// <returns>true if it was able to deploy</returns>
        public bool DeploySolution(string solutionFile, string solutionID)
        {
            return this.DeploySolution(solutionFile, new Guid(solutionID), true, true);
        }

        /// <summary>
        /// Deploys a solution .wsp file to sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <param name="deployToAllWebApplications">true if you want to register package with all of the WebApplications on sharepoint server</param>
        /// <param name="deployToAllContentWebApplications">true if you want to register package with all of the content WebApplications on the sharepoint server</param>
        /// <returns>true if it was able to deploy</returns>
        public bool DeploySolution(string solutionFile, bool deployToAllWebApplications, bool deployToAllContentWebApplications)
        {
            return this.DeploySolution(solutionFile, GetSolutionIdFromPackage(solutionFile), deployToAllWebApplications, deployToAllContentWebApplications);
        }

        /// <summary>
        /// Deploys a solution .wsp file to sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <param name="solutionID">the SolutionId (GUID) of the solution</param>
        /// <param name="deployToAllWebApplications">true if you want to register package with all of the WebApplications on sharepoint server</param>
        /// <param name="deployToAllContentWebApplications">true if you want to register package with all of the content WebApplications on the sharepoint server</param>
        /// <returns>true if it was able to deploy</returns>
        public bool DeploySolution(string solutionFile, Guid solutionID, bool deployToAllWebApplications, bool deployToAllContentWebApplications)
        {
            try
            {
                SPSolution solution = LocateInFarm(solutionID);
                if (solution != null && solution.JobExists)
                {
                    KillRunningJobs(solution);

                    // force add
                    solution = null;
                }

                // does not exist so add it
                if (solution == null)
                {
                    Log.Verbose("WSP package installing.");
                    solution = SPFarm.Local.Solutions.Add(solutionFile);
                }

                // check to see if this has web application stuff
                if (solution.ContainsWebApplicationResource)
                {
                    Collection<SPWebApplication> webApplications = new Collection<SPWebApplication>();

                    // add request web applications
                    if (deployToAllWebApplications) AddAllWebApplications(webApplications);
                    if (deployToAllContentWebApplications) AddAllContentWebApplications(webApplications);

                    if (webApplications.Count == 0) // try to make sure we have at least one
                    {
                        SPWebApplication app = SPWebService.AdministrationService.WebApplications.GetEnumerator().Current;
                        if (app == null) app = SPWebService.ContentService.WebApplications.GetEnumerator().Current;
                    }

                    // deploy it
                    Log.Verbose("WSP package deploying to " + webApplications.Count + " webapplication(s).");
                    solution.Deploy(Installer.Immediately, true, webApplications, true);
                }
                else
                {
                    Log.Verbose("WSP package deploying global.");
                    // deploy it without web app stuff
                    solution.Deploy(Installer.Immediately, true, true);
                }
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (Exception ee)
            {
                throw new Exception("Unable to deploy solution", ee);
            }
            return true;
        }
        #endregion DeploySolution

        #region UpgradeSolution

        public bool UpgradeSolution()
        {
            return UpgradeSolution(this.SolutionFilename, this.SolutionID);
        }


        /// <summary>
        /// Upgrades a solution using a .wsp file to sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <returns>true if it was able to upgrade the solution</returns>
        public bool UpgradeSolution(string solutionFile)
        {
            return UpgradeSolution(solutionFile, GetSolutionIdFromPackage(solutionFile));
        }

        /// <summary>
        /// Upgrades a solution using a .wsp file to sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <param name="solutionID">the SolutionId (GUID) of the solution</param>
        /// <returns>true if it was able to upgrade the solution</returns>
        public bool UpgradeSolution(string solutionFile, string solutionID)
        {
            return UpgradeSolution(solutionFile, new Guid(solutionID));
        }

        /// <summary>
        /// Upgrades a solution using a .wsp file to sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <param name="solutionID">the SolutionId (GUID) of the solution</param>
        /// <returns>true if it was able to upgrade the solution</returns>
        public bool UpgradeSolution(string solutionFile, Guid solutionID)
        {
            try
            {
                if (string.IsNullOrEmpty(solutionFile))
                {
                    throw new Exception("No solution file specified.");
                }
                if (!File.Exists(solutionFile))
                {
                    throw new Exception("Solution file not found.");
                }

                SPSolution solution = SPFarm.Local.Solutions[solutionID];
                KillRunningJobs(solution);

                solution.Upgrade(solutionFile, Immediately);
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch(InvalidOperationException) {
                return DeploySolution(solutionFile, solutionID.ToString());                                    
            }
            catch (Exception eee)
            {
                throw new Exception("Unable to upgrade solution.", eee);
            }
            return true;
        }

        #endregion UpgradeSolution

        #region RetractSolution

        public bool RetractSolution()
        {
            return RetractSolution(this.SolutionFilename, this.SolutionID);
        }

        /// <summary>
        /// Retracts a solution using a .wsp file from a sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <returns>true if it was able to retract the solution</returns>
        public bool RetractSolution(string solutionFile)
        {
            return RetractSolution(solutionFile, GetSolutionIdFromPackage(solutionFile));
        }

        /// <summary>
        /// Retracts a solution using a .wsp file from a sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <param name="solutionID">the SolutionId (GUID) of the solution</param>
        /// <returns>true if it was able to retract the solution</returns>
        public bool RetractSolution(string solutionFile, string solutionID)
        {
            return RetractSolution(solutionFile, solutionID);
        }

        /// <summary>
        /// Retracts a solution using a .wsp file from a sharepoint server
        /// </summary>
        /// <param name="solutionFile">the full path to the solution package file</param>
        /// <param name="solutionID">the SolutionId (GUID) of the solution</param>
        /// <returns>true if it was able to retract the solution</returns>
        public bool RetractSolution(string solutionFile, Guid solutionID)
        {
            try
            {
                SPSolution solution = SPFarm.Local.Solutions[solutionID];
                if (solution == null)
                    throw new Exception("Solution currently not deployed to server.  Can not retract.");

                KillRunningJobs(solution);

                if (solution.Deployed)
                {
                    if (solution.ContainsWebApplicationResource)
                    {
                        Collection<SPWebApplication> deployedWebApplications = new Collection<SPWebApplication>();
                        AddAllConfiguredWebApplications(solution, deployedWebApplications);
                        solution.Retract(Immediately, deployedWebApplications);
                    }
                    else
                    {
                        solution.Retract(Immediately);
                    }
                    // Wait for the retract job to finish
                    WaitForJobToFinish(solution);
                }

                //try
                //{
                //    SPFarm.Local.Solutions.Remove(solution.Id);
                //}
                //catch { }
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (Exception ee)
            {
                throw new Exception("Unable to retract solution", ee);
            }
            return true;
        }

        #endregion RetractSolution
        
        #region Private Helpers


        /// <summary>
        /// If a job is running on the solution, this method waits it to finish.
        /// </summary>
        /// <param name="solution"></param>
        private void WaitForJobToFinish(SPSolution solution)
        {
            if (solution == null) return;

            try
            {
                while (solution.JobExists
                    && (solution.JobStatus == SPRunningJobStatus.Initialized 
                    || solution.JobStatus == SPRunningJobStatus.Scheduled))
                {
                    Thread.Sleep(500);
                }
            }
            catch (Exception ee)
            {
                throw new Exception("Error while waiting to finish running jobs.", ee);
            }
        }


        /// <summary>
        /// Kills previously scheduled jobs for a solution
        /// </summary>
        /// <param name="solution">solution to kill jobs on</param>
        private void KillRunningJobs(SPSolution solution)
        {
            if (solution == null) return;

            try
            {
                if (solution.JobExists)
                {
                    // is the job already running
                    if (solution.JobStatus == SPRunningJobStatus.Initialized)
                    {
                        throw new Exception("A deployment job already running for this solution.");
                    }

                    // find the running job
                    SPJobDefinition definition = null;
                    foreach (SPJobDefinition jobdefs in SPFarm.Local.TimerService.JobDefinitions)
                    {
                        if ((jobdefs.Title != null) && jobdefs.Title.Contains(solution.Name))
                        {
                            definition = jobdefs;
                            break;
                        }
                    }

                    if (definition != null)
                    {
                        definition.Delete();    // kill if it was found
                        Thread.Sleep(1000);     // give it time to delete
                    }
                }
            }
            catch (Exception ee)
            {
                throw new Exception("Error while trying to kill running jobs.", ee);
            }
        }

        /// <summary>
        /// used to tell sharepoint timer service when to perform operations requested of it
        /// </summary>
        private static DateTime Immediately
        {
            get { return DateTime.Now - TimeSpan.FromDays(1); }
        }

        /// <summary>
        /// gets all of the currently configured applications for a solution
        /// </summary>
        /// <param name="solution">solution get get configured applications for</param>
        /// <param name="applications">the collection to add it to</param>
        private void AddAllConfiguredWebApplications(SPSolution solution, Collection<SPWebApplication> applications)
        {
            foreach (SPWebApplication app in solution.DeployedWebApplications)
            {
                applications.Add(app);
            }
        }

        /// <summary>
        /// gets all of the currently configured applications on the current sharepoint server
        /// </summary>
        /// <param name="applications">the collection to add it to</param>
        private void AddAllWebApplications(Collection<SPWebApplication> applications)
        {
            foreach (SPWebApplication app1 in SPWebService.AdministrationService.WebApplications)
                applications.Add(app1);
        }

        /// <summary>
        ///  gets all of the currently configured applications on the current sharepoint content server
        /// </summary>
        /// <param name="applications">the collection to add it to</param>
        private void AddAllContentWebApplications(Collection<SPWebApplication> applications)
        {
            foreach (SPWebApplication app2 in SPWebService.ContentService.WebApplications)
                applications.Add(app2);
        }

        /// <summary>
        /// Finds a solution in the SharePoint farm
        /// created because it sometimes would lie to me when passing it a solution GUID, wierd
        /// </summary>
        /// <param name="solutionID">the SolutionID (GUID) of the solution to </param>
        /// <returns>returns the soltion if found, and null if not found</returns>
        private SPSolution LocateInFarm(Guid solutionID)
        {
            string id = solutionID.ToString();

            // move through all of the solutions we can find and see if 
            // we can locate the one we are looking for
            foreach (SPSolution sol in SPFarm.Local.Solutions)
            {
                if (string.Equals(sol.SolutionId.ToString(), id, StringComparison.InvariantCultureIgnoreCase))
                    return sol;
            }
            return null;
        }

        #endregion Private Helpers

        #region Public Helpers

        private static Dictionary<string, Guid> SolutionIdCache = new Dictionary<string, Guid>();

        /// <summary>
        /// This extracts a SolutionId from a .wsp solution package. 
        /// It opens the CAB extracts the manifest.xml, and then reads the SolutionId
        /// </summary>
        /// <param name="packageName">the full path to the solution package</param>
        /// <returns>SolutionId (GUID) contained in solution</returns>
        public static Guid GetSolutionIdFromPackage(string packageName)
        {
            if (SolutionIdCache.ContainsKey(packageName))
            {
                return SolutionIdCache[packageName];
            }
                Guid solutionID;

            try
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(packageName);
                packageName = fi.FullName;

                string temppath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString().Replace("-", "");

                XmlDocument xdoc = new XmlDocument();
                try
                {
                    // make sure temp path is there
                    System.IO.Directory.CreateDirectory(temppath);

                    // extract the manifest.xml
                    CabLib.Extract cab = new CabLib.Extract();
                    cab.SetSingleFile("manifest.xml");
                    cab.ExtractFile(packageName, temppath);

                    // load it into the main xml doc
                    xdoc.Load(temppath + "\\" + "manifest.xml");
                }
                catch (Exception ee)
                {
                    throw new Exception("Unable to extract manifest.xml from: " + packageName, ee);
                }

                try
                {
                    solutionID = new Guid(xdoc.DocumentElement.Attributes["SolutionId"].Value);
                }
                catch
                {
                    throw new Exception("Unable to parse SolutionId from manifest.xml");
                }

                try
                {
                    // don't care if this fails, but we should try to cleanup anyways
                    System.IO.Directory.Delete(temppath, true);
                }
                catch { }
               
            }
            catch (Exception ex)
            {
                
                Log.Warning("Unable to get solution id from manifest: " + ex.Message);
                solutionID = Config.Current.SolutionId;
            }

            SolutionIdCache[packageName] = solutionID;
            return solutionID;
        }

        #endregion Public Helpers

        #region Static Pre Flight Checks

        /// <summary>
        /// returns true when it detects sharepoint in the system registry
        /// </summary>
        public static bool IsSharePointInstalled
        {
            get
            {
                string value = SharePointRegistry.GetValue("SharePoint");

                if ("Installed".Equals(value))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// returns true when the current user running this thread has acces to install packages on this server
        /// </summary>
        public static bool HasInstallationPermissions
        {
            get
            {
                try
                {
                    if (SPFarm.Local.CurrentUserIsAdministrator())
                    {
                        return true;
                    }
                }
                catch (NullReferenceException)
                {
                    return false;
                }
                catch (Exception ee)
                {
                    throw new Exception(ee.Message, ee);
                }
                return false;
            }
        }

        /// <summary>
        /// returns true when it detects tha "Microsoft Sharepoint Services Administration" service is running
        /// </summary>
        public static bool SPAdministrationRunning
        {
            get
            {

                try
                {
                    ServiceController adminService = new ServiceController(WindowsServices.Current.SPAdminName);
                    if (adminService.Status == ServiceControllerStatus.Running)
                    {
                        return true;
                    }
                }
                catch (NullReferenceException)
                {
                    return false;
                }
                catch (Win32Exception ee)
                {
                    throw new Exception(ee.Message, ee);
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                return false;
            }
        }

        /// <summary>
        /// returns true when it detects tha "Microsoft Sharepoint Timer Administration" service is running
        /// </summary>
        public static bool SPTimerRunning
        {
            get
            {
                try
                {
                    TimeSpan span = new TimeSpan(0, 0, 60);
                    ServiceController timerService = new ServiceController(WindowsServices.Current.SPTimerName);
                    if (timerService.Status == ServiceControllerStatus.Running)
                    {
                        timerService.Stop();
                        timerService.WaitForStatus(ServiceControllerStatus.Stopped, span);
                    }
                    timerService.Start();
                    timerService.WaitForStatus(ServiceControllerStatus.Running, span);
                    return true;
                }
                catch (NullReferenceException)
                {
                    return false;
                }
                catch (System.ServiceProcess.TimeoutException e)
                {
                    throw new Exception(e.Message, e);
                }
                catch (Win32Exception ee)
                {
                    throw new Exception(ee.Message, ee);
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
        }

        #region GetPreFlightErrors

         /// <summary>
        /// Does a preflight check to make sure the basic services are running
        /// </summary>
        /// <param name="requiresMOSS">true if you want to make sure MOSS is running</param>
        /// <returns>An array of strings with all of the errors detected.  one string for each error</returns>
        public static string[] GetPreFlightErrors()
        {
            List<string> list = new List<string>();

            if (!Installer.IsSharePointInstalled) list.Add("SharePoint is not installed on " + Environment.MachineName);
            if (!Installer.HasInstallationPermissions) list.Add("User " + Environment.UserName + " does not have installation permissions on " + Environment.MachineName);
            if (!Installer.SPAdministrationRunning) list.Add("Microsoft SharePoint Services Administration is not running on " + Environment.MachineName);
            if (!Installer.SPTimerRunning) list.Add("Microsoft SharePoint Services Timer is not running on " + Environment.MachineName);

            return list.ToArray();
        }

        #endregion GetPreFlightErrors

        #endregion Static Pre Flight Checks


    }
}


