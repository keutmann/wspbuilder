using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using Keutmann.SharePoint.WSPBuilder.Library;

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public partial class FileProvider
    {
        /// <summary>
        /// Create a file that contains a list of every file that goes into the WSP. 
        /// </summary>
        /// <param name="filename">The filename of the file that contains the list.</param>
        /// <param name="cabFiles">ArrayList of string[]</param>
        public static void CreateFileList(string filename, Dictionary<string, string> cabFiles)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (KeyValuePair<string, string> entry in cabFiles)
                {
                    // Write the source file into the steam
                    sw.WriteLine(entry.Value);// Path of file
                }
                sw.Close();
            }
        }


        /// <summary>
        /// Checks the file if it should be included.
        /// </summary>
        /// <param name="file">FileInfo object.</param>
        /// <returns>True the file should be included.</returns>
        public static bool IncludeFile(FileInfo file)
        {
            // Do not include the file if its hidden
            if ((file.Attributes & FileAttributes.Hidden) != 0)
            {
                return false;
            }

            if (!String.IsNullOrEmpty(Config.Current.Includefiles))
            {
                // Checks that the file is specified in the Included file list
                if (!Config.Current.IncludeFilesDictionary.ContainsKey(file.FullName))
                {
                    return false;
                }
            }

            if (!String.IsNullOrEmpty(Config.Current.Excludefiles))
            {
                // Checks that the file is specified in the Included file list
                if (Config.Current.ExcludeFilesDictionary.ContainsKey(file.FullName))
                {
                    return false;
                }
            }

            return IncludeFileExtension(file.Extension);
        }


        /// <summary>
        /// Checks the file extension if it should be included.
        /// </summary>
        /// <param name="extension">A file extension.</param>
        /// <returns>'True' if the file should be included.</returns>
        public static bool IncludeFileExtension(string extension)
        {
            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }

            bool result = true;
            if (Config.Current.Includefiletypes.BinarySearch(extension, StringComparer.InvariantCultureIgnoreCase) < 0)
            {
                if (Config.Current.Excludefiletypes.BinarySearch(extension, StringComparer.InvariantCultureIgnoreCase) >= 0)
                {
                    result = false;
                }
                else
                    if (Config.Current.Includefiletypes.BinarySearch("*", StringComparer.InvariantCultureIgnoreCase) < 0 &&
                        Config.Current.Excludefiletypes.BinarySearch("*", StringComparer.InvariantCultureIgnoreCase) >= 0)
                    {
                        result = false;
                    }
            }
            return result;

        }


        /// <summary>
        /// Returns if the directory is not excluded from the process.
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <returns></returns>
        public static bool IncludeDir(DirectoryInfo dirInfo)
        {
            if (dirInfo == null)
            {
                return false;
            }
            // Do not include the directory if its hidden
            if ((dirInfo.Attributes & FileAttributes.Hidden) != 0)
            {
                return false;
            }

            bool include = true;
            foreach (string exludepath in Config.Current.Excludepaths)
            {
                string path = (exludepath.EndsWith(@"\")) ? exludepath.Substring(0, exludepath.Length-1) : exludepath;
                if (dirInfo.FullName.StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
                {
                    include = false;
                    break;
                }
            }
            return include;
        }


        /// <summary>
        /// Ensure that the path exists now and remove tailing backslash.
        /// </summary>
        /// <param name="path">Path to a directory</param>
        /// <returns>A clean path</returns>
        public static string GetCleanDirectoryPath(string path)
        {
            string result = string.Empty;
            // Ensure that the path exists now and remove tailing backslash
            try
            {
                result = new DirectoryInfo(path).FullName;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("The Path: '"+path+ "' \r\n"+ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get the child directories for the specified directory.
        /// Hidden directories are not included.
        /// </summary>
        /// <param name="dirInfo">Parent directory</param>
        /// <returns>A List of child directories.</returns>
        public static List<DirectoryInfo> GetDirectories(DirectoryInfo dirInfo)
        {
            List<DirectoryInfo> dirs = new List<DirectoryInfo>();
            foreach (DirectoryInfo childDirs in dirInfo.GetDirectories())
            {
                // Include all directories that are not hidden
                if ((childDirs.Attributes & FileAttributes.Hidden) == 0)
                {
                    bool found = false;
                    foreach (string exludepath in Config.Current.Excludepaths)
                    {
                        if (childDirs.FullName.StartsWith(exludepath, StringComparison.InvariantCultureIgnoreCase))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        dirs.Add(childDirs);
                    }
                }
            }
            return dirs;
        }
    }
}
