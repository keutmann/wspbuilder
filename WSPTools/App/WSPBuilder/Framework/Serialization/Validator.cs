using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace Keutmann.SharePoint.WSPBuilder.Framework.Serialization
{
    public class Validator
    {

        //public static void ValidateXml(string xmlPath)
        //{
        //    if (File.Exists(xmlPath))
        //    {
        //        string xsdPath = SPUtility.GetGenericSetupPath(@"Template\xml\");
        //        XmlSchema xmlSchema = XmlSchema.Read(new XmlTextReader(xsdPath + "CoreDefinitions.xsd"), new ValidationEventHandler(validatingReader_ValidationEventHandler));
        //        XmlSchema xmlSchema2 = XmlSchema.Read(new XmlTextReader(xsdPath + "wss.xsd"), new ValidationEventHandler(validatingReader_ValidationEventHandler));

        //        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
        //        xmlReaderSettings.ValidationType = ValidationType.Schema;

        //        xmlReaderSettings.Schemas.Add(xmlSchema);
        //        xmlReaderSettings.Schemas.Add(xmlSchema2);


        //        xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(validatingReader_ValidationEventHandler);

        //        XmlTextReader xmlFileReader = new XmlTextReader(xmlPath);
        //        using (XmlReader xmlReader = XmlReader.Create(xmlFileReader, xmlReaderSettings))
        //        {
        //            while (xmlReader.Read()) ;
        //        }
        //    }
        //}

        static void validatingReader_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine(e.Exception.Message);
            Console.Error.WriteLine(e.Exception.StackTrace);
        }
    }
}
