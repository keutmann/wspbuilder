/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.IO;
using System.Security;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public sealed class SPPaths
    {

        private string _SPDirectory = string.Empty;
        private string SPDirectory
        {
            get
            {
                if (_SPDirectory.Length == 0)
                {
                    _SPDirectory = GetSharePointDirectory();
                }
                return _SPDirectory;
            }
        }

        private string _SPImageDirectory = string.Empty;
        private string SPImageDirectory
        {
            get
            {
                if (_SPImageDirectory.Length == 0)
                {
                    _SPImageDirectory = Path.Combine(SharePointDirectory, @"TEMPLATE\IMAGES\");
                }
                return _SPImageDirectory;
            }
        }

        private string _SPTemplateDirectory = string.Empty;
        private string SPTemplateDirectory
        {
            get
            {
                if (_SPTemplateDirectory.Length == 0)
                {
                    _SPTemplateDirectory = Path.Combine(SharePointDirectory, @"TEMPLATE");
                }
                return _SPTemplateDirectory;
            }
        }

        private string GetSharePointDirectory()
        {
            string key = @"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\12.0";
            string name = "Location";

            string path = String.Empty;
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(key);
                path = regKey.GetValue(name) as string;
                regKey.Close();
            }
            catch (SecurityException)
            {
                path = String.Empty;
            }
            catch (ArgumentNullException)
            {
                path = String.Empty;
            }
            catch (ArgumentException)
            {
                path = String.Empty;
            }
            catch (ObjectDisposedException)
            {
                path = String.Empty;
            }
            catch (IOException)
            {
                path = String.Empty;
            }
            catch (UnauthorizedAccessException)
            {
                path = String.Empty;
            }

            return path;
        }

        #region Static methods

        public static string SharePointDirectory
        {
            get
            {
                return Instance.SPDirectory;
            }
        }

        public static string ImageDirectory
        {
            get
            {
                return Instance.SPImageDirectory;
            }
        }

        public static string TemplateDirectory
        {
            get
            {
                return Instance.SPTemplateDirectory;
            }
        }

        #endregion 

        #region Singleton

        SPPaths()
        {
        }

        public static SPPaths Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly SPPaths instance = new SPPaths();
        }
        #endregion
    }

}

