using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public class SolutionIdFile
    {
        public static Guid GetID(string path, FileAccess fileAccess)
        {
            Guid result = Guid.Empty;

            // Read the guid from the file if it exist or create it if allowed by fileAccess
            if (File.Exists(path) || fileAccess == FileAccess.ReadWrite)
            {
                // Open or create the solutionid file.
                using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, fileAccess))
                {
                    string id = "";
                    if (fs.Length == 0)
                    {
                        id = Guid.NewGuid().ToString();
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(id);
                            sw.Close();
                        }
                    }
                    else
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            id = sr.ReadLine();
                            sr.Close();
                        }
                    }

                    try
                    {
                        result = new Guid(id);
                    }
                    catch
                    {
                        throw new ApplicationException("Invalid GUID value '" + id + "' for -SolutionId.");
                    }

                    fs.Close();
                }
            }
            else
            {
                // return Empty guid for detection that there was no file!
                result = Guid.Empty;
            }


            return result;
        }
    }
}
