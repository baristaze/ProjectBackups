using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace YummyZone.WebService
{
    public partial class PListSerializer : XmlObjectSerializer
    {
        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            // this does nothing... but we still need to have this
            throw new NotImplementedException();
        }

        public override void WriteStartObject(XmlWriter writer, object graph)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("plist");
            writer.WriteAttributeString("version", "1.0");
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.Writer.WriteObjectContent", Guid.Empty, "Method started", null);

                StringBuilder sb = new StringBuilder(1024);

                writer.WriteStartElement("dict");
                if (this.DataMemberFieldsOfType != null)
                {
                    foreach (PropertyInfo propertyInfo in this.DataMemberFieldsOfType)
                    {
                        if (IsPListBasicType(propertyInfo.PropertyType))
                        {
                            object obj = propertyInfo.GetValue(graph, null);

                            writer.WriteElementString("key", propertyInfo.Name);
                            if (obj is bool)
                            {
                                if ((bool)obj)
                                {
                                    writer.WriteElementString("true", String.Empty);
                                }
                                else
                                {
                                    writer.WriteElementString("false", String.Empty);
                                }
                            }
                            else
                            {
                                string objVal = obj == null ? String.Empty : obj.ToString();
                                if (obj is DateTime)
                                {
                                    // UTC is assumed
                                    objVal = ((DateTime)obj).ToString("yyyy-MM-ddTHH:mm:ssZ");
                                }

                                writer.WriteElementString(PListTypeFromPrimitiveType(propertyInfo.PropertyType), objVal);
                            }
                        }
                        else if (IsPListArrayType(propertyInfo.PropertyType))
                        {
                            IList listObject = (propertyInfo.GetValue(graph, null) as IList);
                            writer.WriteElementString("key", propertyInfo.Name);
                            WriteArrayObjectContent(writer, listObject);
                        }
                        else if (IsPListDictType(propertyInfo.PropertyType))
                        {
                            writer.WriteElementString("key", propertyInfo.Name);
                            ConverterLookupTable[propertyInfo.PropertyType].WriteObjectContent(writer, propertyInfo.GetValue(graph, null));
                        }
                        else
                        {
                            Logger.LogAsInfo(
                                "WriteObjectContent",
                                Guid.Empty,
                                "Property {0}.{1} couldn't be categorized for PList serialization",
                                graph.GetType().Name,
                                propertyInfo.Name);
                        }
                    }
                }

                writer.WriteEndElement();
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.Writer.WriteObjectContent", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static void WriteArrayObjectContent(XmlDictionaryWriter writer, IList listObject)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.Writer.WriteArrayObjectContent", Guid.Empty, "Method started", null);

                if (listObject != null)
                {
                    IEnumerator enumarator = listObject.GetEnumerator();
                    Object obj = null;
                    enumarator.Reset();

                    writer.WriteStartElement("array");

                    while (enumarator.MoveNext())
                    {
                        obj = enumarator.Current;

                        if (IsPListBasicType(obj.GetType()))
                        {
                            if (obj is bool)
                            {
                                if ((bool)obj)
                                {
                                    writer.WriteElementString("true", String.Empty);
                                }
                                else
                                {
                                    writer.WriteElementString("false", String.Empty);
                                }
                            }
                            else
                            {
                                writer.WriteElementString(PListTypeFromPrimitiveType(obj.GetType()), obj.ToString());
                            }
                        }
                        else if (IsPListArrayType(obj.GetType()))
                        {
                            IList subListObject = obj as IList;
                            WriteArrayObjectContent(writer, subListObject);
                        }
                        else if (IsPListDictType(obj.GetType()))
                        {
                            ConverterLookupTable[obj.GetType()].WriteObjectContent(writer, obj);
                        }
                    }

                    writer.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.Writer.WriteArrayObjectContent", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }
    }
}