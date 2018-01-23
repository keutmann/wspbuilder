/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * This class uses the code based on Elmue's excelent work on CabLib.
 * 
 * http://www.codeproject.com/cs/files/CABCompressExtract.asp
 * 
 * 
 */

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

//using CabLib;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Keutmann.SharePoint.WSPBuilder
{
    public class CabHandler
    {

        /// <summary>
        /// Creates the CAB with the name of the directory.
        /// </summary>
        /// <param name="files"></param>
        public static void CreateCab(Dictionary<string, string> files)
        {
            ArrayList list = new ArrayList();
            foreach (KeyValuePair<string, string> entry in files)
            {
                list.Add(new string[] { entry.Value, entry.Key });
            }

            CreateCab(Config.Current.OutputPath, list);
        }
        

        /// <summary>
        /// Creates the CAB with the name of the directory.
        /// </summary>
        /// <param name="files"></param>
        public static void CreateCab(ArrayList files)
        {
            CreateCab(Config.Current.OutputPath, files);

        }


        /// <summary>
        /// Creates the CAB with the name of the directory specified with the parameter "path".
        /// </summary>
        /// <param name="path"></param>
        /// <param name="files"></param>
        public static void CreateCab(string path, ArrayList files)
        {
            DirectoryInfo outputDir = new DirectoryInfo(path);
            DirectoryInfo solutionDir = new DirectoryInfo(Config.Current.SolutionPath);
            // Name the wsp file the same of the path directory.
            CreateCab(Config.Current.WSPName, outputDir.FullName, files);
        }


        /// <summary>
        /// Creates the CAB wsp file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <param name="files"></param>
        public static void CreateCab(string filename, string path, ArrayList files)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            // Name the wsp file the same of the path directory.
            CreateCab(files, dir.FullName + @"\" + filename);
        }


        /// <summary>
        /// Creates the CAB.
        /// </summary>
        /// <param name="files">The files to .</param>
        /// <param name="compressFile">The compress file.</param>
        public static void CreateCab(ArrayList files, string compressFile)
        {
            if (files.Count > 0)
            {
                // Change this to split the archive into multiple files (200000 --> CAB split size = 200kB)
                // ATTENTION: In this case Parameter 1 of CompressFile() MUST contain "%d"
                int s32_SplitSize = int.MaxValue;

                CabLib.Compress cab = new CabLib.Compress();
                
                cab.CompressFileList(files, compressFile, true, true, s32_SplitSize);
            }
        }
    }
}

