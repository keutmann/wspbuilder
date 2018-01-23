using System;

namespace Keutmann.SharePoint.WSPBuilder.Framework.Serialization
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
