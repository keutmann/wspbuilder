/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under the GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using Keutmann.SharePoint.WSPBuilder.WSP;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Reflection;
using System.Resources;
using System.Globalization;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using Keutmann.SharePoint.WSPBuilder.Commands;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;


namespace Keutmann.SharePoint.WSPBuilder
{
    public class Program
    {

        /// <summary>
        /// Builds the WSP package.
        /// </summary>
        private static void BuildWSP()
        {
            SolutionHandler solutionHandler = null;
            
            Solution manifestConfig =  ManifestConfig.Load(Config.Current.ManifestConfigPath);
            if (manifestConfig != null)
            {
                solutionHandler = new SolutionHandler(manifestConfig);
            }
            else
            {
                solutionHandler = new SolutionHandler(Config.Current.SolutionId);
            }

            Log.Information("Building the solution - please wait");
            // Build the manifest file and cab list 
            solutionHandler.BuildSolution();

            Log.Information("Saving the Manifest.xml file");
            // Save the manifest file to the file system
            solutionHandler.Save();

            if (solutionHandler.BuildDDF)
            {
                Log.Information("Creating the Cabinet.ddf file");
                // Save the ddf to the file system
                solutionHandler.SaveDDF();
            }

            if (!String.IsNullOrEmpty(Config.Current.CreateWSPFileList))
            {
                Log.Information("Creating the filelist " + Config.Current.CreateWSPFileList);
                solutionHandler.CreateFileList();
            }

            if (solutionHandler.BuildCAB)
            {
                Log.Information("Creating the WSP file");
                // Create and save the WSP (cab) file
                CabHandler.CreateCab(solutionHandler.CabFiles);
            }
            if (!string.IsNullOrEmpty(Config.Current.CreateDeploymentFolder))
            {
                Log.Information("Creating the deployment folder");
                string s = Config.Current.CreateDeploymentFolder.ToLower();
                solutionHandler.CreateDeploymentFolder(s == "stsadm" || s == "all", s == "wspbuilder" || s == "all", s == "ssi" || s == "all");
            }   



            if (Config.Current.Cleanup)
            {
                solutionHandler.Cleanup();
            }
        }


        public static int Main(string[] args)
        {
            int result = 0;
            try
            {
                if (args != null 
                    && args.Length > 0 
                    && "-o".Equals(args[0], StringComparison.OrdinalIgnoreCase))
                {
                    CommandParser.Execute(args);
                }
                else
                {
                    ExecuteOldStyle();
                }
                Log.Information("Done!");
            }
            catch (Exception ex)
            {
                HandleException(ex);

                // Return error
                result = 1;
            }
            return result;
        }



        private static void ExecuteOldStyle()
        {
            HelpHandler help = new HelpHandler();

            if (!Config.Current.ShowHelp)
            {
                Log.Information(help.Copyleft());

                if (Config.Current.CreateFolders)
                {
                    // Creates a folder structure for the WSP package
                    WSPFolders wspFolders = new WSPFolders(Config.Current.FolderDestination);
                    wspFolders.Create();
                }
                else
                {
                    if (Config.Current.BuildWSP)
                    {
                        // Build the WSP package.
                        BuildWSP();
                    }


                    // Handles any deployment arguments
                    Deployment.HandleArguments();
                }
            }
            else
            {
                Console.WriteLine(help.Print());
            }
        }

        /// <summary>
        /// Handles the exceptions.
        /// </summary>
        /// <param name="ex"></param>
        private static void HandleException(Exception ex)
        {
            Log.Error(ex.Message);

            Exception inner = ex.InnerException;

            // Dead man's switch
            int count = 0;

            while (inner != null && count < 20)
            {
                count++;
                Log.Error("Inner exception(" + count + "): " + inner.Message);
                inner = inner.InnerException;
            }

            Log.Verbose(ex.StackTrace);
        }

    }
}
