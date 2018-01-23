using System;

namespace WSPTools.BaseLibrary.Xml.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XmlAttributeNewline : Attribute
    {
        public XmlAttributeNewline()
        {
        }

        public XmlAttributeNewline(string tagName)
        {
            this.TagName = tagName;
        }

        public string TagName = null;
    }
}
