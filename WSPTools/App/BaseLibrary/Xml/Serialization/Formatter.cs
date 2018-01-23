using System;
using System.Collections.Generic;
using System.Text;

namespace WSPTools.BaseLibrary.Xml.Serialization
{
    public class Formatter
    {
        public static string GetCDATA(object text)
        {
            return string.Format("<![CDATA[{0}]]>", text as string);
        }

    }
}
