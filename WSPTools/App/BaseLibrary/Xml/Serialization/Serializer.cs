/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace WSPTools.BaseLibrary.Xml.Serialization
{
    public class Serializer
    {
        #region Members


        private static Object thisLock = new Object();
        private static Object lockDBAttributeOverride = new Object();

        private static Hashtable _serializerStore = new Hashtable();
        private static Hashtable _attributeOverrideStore = new Hashtable();

        private static XmlSerializerFactory factory = new XmlSerializerFactory();

        private static List<string> _newlineTags = null;

        public static List<string> NewlineTags
        {
            get
            {
                if (_newlineTags == null)
                {
                    _newlineTags = GetNewlineTags();
                }
                return _newlineTags;
            }
        }
        #endregion

        #region Methods


        public static XmlSerializer GetXmlSerializer(Type objType)
        {
            string key = "XML" + objType.FullName;

            XmlSerializer serializer = (XmlSerializer)GetSerializerObject(key);
            if (serializer == null)
            {
                serializer = factory.CreateSerializer(objType);

                SetSerializerObject(key, serializer);
            }
            return serializer;
        }

        public static XmlSerializer GetXmlSerializer(Type objType, XmlAttributeOverrides xmlOverrides)
        {
            string hashkey = string.Empty;
            if (xmlOverrides != null)
            {
                hashkey = xmlOverrides.GetHashCode().ToString();
            }
            string key = "XML" + objType.FullName + hashkey;

            XmlSerializer serializer = (XmlSerializer)GetSerializerObject(key);
            if (serializer == null)
            {
                serializer = factory.CreateSerializer(objType, xmlOverrides);
                SetSerializerObject(key, serializer);
            }
            return serializer;
        }


        private static object GetSerializerObject(string key)
        {
            object result = null;
            lock (thisLock)
            {
                if (_serializerStore.ContainsKey(key))
                {
                    result = _serializerStore[key];
                }
            }
            return result;
        }

        private static void SetSerializerObject(string key, object serializer)
        {
            lock (thisLock)
            {
                _serializerStore[key] = serializer;
            }
        }


        private static List<string> GetNewlineTags()
        {
            List<string> list = new List<string>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                Type[] types = asm.GetTypes();
                foreach (Type type in types)
                {
                    object[] attributes = type.GetCustomAttributes(true);
                    foreach (Attribute attrib in attributes)
                    {
                        if (attrib is XmlAttributeNewline)
                        {
                            XmlAttributeNewline newLineAttrib = (XmlAttributeNewline)attrib;
                            if (String.IsNullOrEmpty(newLineAttrib.TagName))
                            {
                                list.Add(type.Name);
                            }
                            else
                            {
                                list.Add(newLineAttrib.TagName);
                            }
                        }
                    }
                }
            }
            return list;
        }

        #endregion

        #region Serialize XML

        //public static string Serialize<T>(object obj)
        //{
        //    StringWriter sw = new StringWriter();
        //    XmlSerializer xs = new XmlSerializer(typeof(T));
        //    xs.Serialize(sw, obj);
        //    return sw.ToString();
        //}

        //public static T Deserialize<T>(string xml)
        //{
        //    StringReader sr = new StringReader(xml);
        //    XmlSerializer xs = new XmlSerializer(typeof(T));
        //    return (T)xs.Deserialize(sr);
        //}

        public static string ObjectToXML(object input, bool isWss40)
        {
            XmlAttributeOverrides xmlOverrides = new XmlAttributeOverrides();

            return ObjectToXML(input, isWss40, null);
        }


        public static string ObjectToXML(object input, bool isWss40, XmlAttributeOverrides xmlOverrides)
        {
            string resultXml = null;

            XmlSerializer serializer = GetXmlSerializer(input.GetType(), xmlOverrides);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                SPXmlTextWriter writer = new SPXmlTextWriter(memoryStream, null);
                writer.IsWss40 = isWss40;
                
                //XmlTextWriter writer = new XmlTextWriter(memoryStream, null);

                //writer.NewlineTags = NewlineTags;
                writer.Formatting = Formatting.Indented;

                XmlSerializerNamespaces namespaceSerializer = new XmlSerializerNamespaces();
                namespaceSerializer.Add("", "http://schemas.microsoft.com/sharepoint/");

                // Do the serialization here!
                serializer.Serialize(writer, input, namespaceSerializer);

                resultXml = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            return resultXml;
        }

        public static T XmlToObject<T>(string xml)
        {
            T resultObj = default(T);
            if (xml != null)
            {
                Type objType = typeof(T);
                XmlSerializer serializer = GetXmlSerializer(objType);
                StringReader sr = new StringReader(xml);
                try
                {
                    resultObj = (T)serializer.Deserialize(sr);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to deserialize object '" + objType.FullName + "' from this xml :" + xml, ex);
                }
                finally
                {
                    sr.Close();
                }
            }
            return resultObj;
        }


        public static object DeserializeFromElement(XmlElement element, Type objectType)
        {
            object result = null;
            using (XmlNodeReader reader = new XmlNodeReader(element))
            {
                XmlSerializer xs = Serializer.GetXmlSerializer(objectType);

                result = xs.Deserialize(reader);
            }
            return result;
        }

        public static string FormatXml(string xml)
        {
            XmlDocument xDoc = new XmlDocument();

            xDoc.LoadXml(xml);
            string result = string.Empty;

            using (StringWriter sw = new StringWriter())
            {
                //XmlTextWriter xtw = new XmlTextWriter(sw);
                SPXmlTextWriter writer = new SPXmlTextWriter(sw);
                writer.NewlineTags = NewlineTags;
                writer.Formatting = Formatting.Indented;

                xDoc.WriteTo(writer);
                result = sw.ToString();
            }

            return result;
        }

        private static string Utf8ToUnicode(string utf8)
        {
            return Encoding.Unicode.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Unicode, Encoding.UTF8.GetBytes(utf8)));
        }

        private static String UTF8ByteArrayToString(Byte[] characters)
        {

            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        private static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }


        #endregion

        #region Serialize Binary

        public static byte[] ObjectToBinary(object input, Type objType)
        {
            byte[] result = null;
            BinaryFormatter binFormatter = new BinaryFormatter();
            MemoryStream dataStream = new MemoryStream();
            try
            {
                binFormatter.Serialize(dataStream, input);
                result = dataStream.GetBuffer();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to binary serialize object '" + objType.FullName + "'", ex);
            }
            finally
            {
                dataStream.Close();
            }
            return result;
        }

        public static object BinaryToObject(byte[] data, Type objType)
        {
            object resultObj = null;
            BinaryFormatter binFormatter = new BinaryFormatter();
            MemoryStream dataStream = new MemoryStream(data);
            try
            {
                resultObj = binFormatter.Deserialize(dataStream);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to deserialize object '" + objType.FullName + "' from binary.", ex);
            }
            return resultObj;
        }
        #endregion

    }
}
