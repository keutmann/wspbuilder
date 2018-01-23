using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using System.Reflection;
using System.Collections.Specialized;

namespace Keutmann.SharePoint.WSPBuilder.Framework.Serialization
{
    public class SPXmlTextWriter : XmlTextWriter
    {

        public bool IsWss40 { get; set; } 


        private bool _attributeNewline = false;
        public bool AttributeNewline
        {
            get { return _attributeNewline; }
            set { _attributeNewline = value; }
        }

        private TextWriter _internalWriter = null;
        public TextWriter InternalWriter
        {
            get 
            {
                if (_internalWriter == null)
                {
                    FieldInfo TextWriterField = typeof(XmlTextWriter).GetField("textWriter", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
                    _internalWriter = (TextWriter)TextWriterField.GetValue(this);
                }
                return _internalWriter; 
            }
        }

        private List<string> _newlineTags = null;
        public List<string> NewlineTags
        {
            get { return _newlineTags; }
            set { _newlineTags = value; }
        }
        

        public SPXmlTextWriter(TextWriter w) : base(w)
        {
        }
        public SPXmlTextWriter(Stream w, Encoding encoding) : base(w, encoding)
        {
        }
        public SPXmlTextWriter(string filename, Encoding encoding) : base(filename, encoding)
        {
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            if (NewlineTags != null)
            {
                AttributeNewline = IsNewLineTag(localName);
            }
            base.WriteStartElement(prefix, localName, ns);
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            if (AttributeNewline)
            {
                InternalWriter.Write(Environment.NewLine + "\t");
            }
            base.WriteStartAttribute(prefix, localName, ns);
        }


        private bool IsNewLineTag(string name)
        {
            bool found = false;

            foreach (string item in NewlineTags)
            {
                if (item.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }

            return found;
        }
    }
}
