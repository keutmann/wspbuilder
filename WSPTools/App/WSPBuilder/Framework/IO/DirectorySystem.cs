using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Keutmann.SharePoint.WSPBuilder.Framework.IO
{
    public class DirectorySystem
    {
        public static void EnsureFileDirectory(string path)
        {
            string dir = path;
            // Check if the target directory exists, if not, create it.
            int index = path.LastIndexOf(@"\");
            if (index >= 0)
            {
                dir = path.Substring(0, index);
            }
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static bool DeleteEmptyFolders(string target)
        {
            DirectoryInfo dir = new DirectoryInfo(target);
            return DeleteEmptyFolders(dir);
        }

        public static void CopyTo(string sourcePath, string targetPath)
        {
            DirectoryInfo source = new DirectoryInfo(sourcePath);
            DirectoryInfo target = new DirectoryInfo(targetPath);

            CopyTo(source, target);
        }

        public static void CopyTo(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
                target = new DirectoryInfo(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo subDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(subDir.Name);
                CopyTo(subDir, nextTargetSubDir);
            }
        }


        public static void Move(string sourcePath, string targetPath, bool overwrite)
        {
            DirectoryInfo source = new DirectoryInfo(sourcePath);
            DirectoryInfo target = new DirectoryInfo(targetPath);

            MoveTo(source, target, overwrite);
        }

        public static void MoveTo(DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
                target = new DirectoryInfo(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                string path = Path.Combine(target.ToString(), fi.Name);
                FileSystem.Move(fi.FullName, path, overwrite);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo subDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(subDir.Name);

                MoveTo(subDir, nextTargetSubDir, overwrite);
            }
        }

        public static bool DeleteEmptyFolders(DirectoryInfo target)
        {
            bool delete = true;
            foreach (DirectoryInfo subDir in target.GetDirectories())
            {
                bool removeDir = DeleteEmptyFolders(subDir);
                if (!removeDir)
                {
                    delete = false;
                }
            }

            if (delete && target.GetFiles().Length == 0)
            {
                target.Delete(true);
            }

            return delete;
        }

    }

}
