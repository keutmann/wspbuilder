using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TestWSPBuilder.Library
{
    public class TestUtil
    {



        public static long FileSize(string path)
        {
            FileInfo file = new FileInfo(path);
            return file.Length;
        }
    }
}
