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
        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            // this does nothing... but we still need to have this
            throw new NotImplementedException();
        }

        public override object ReadObject(XmlDictionaryReader reader)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ReadObject", Guid.Empty, "Method started", null);

                if (IsPListBasicType(this.targetType))
                {
                    return ReadPrimitiveType(reader, this.targetType);
                }
                else
                {
                    if (CheckNextName(reader, false) == "dict")
                    {
                        return ReadSingleObject(reader, this.targetType);
                    }
                    else if (CheckNextName(reader, false) == "array")
                    {
                        return ReadArray(reader, this.targetType);
                    }
                    else
                    {
                        throw new YummyZoneException("Unexpected element");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ReadObject", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static object ReadPrimitiveType(XmlDictionaryReader reader, Type targetType)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ReadPrimitiveType", Guid.Empty, "Method started", null);

                // read beginning part; e.g. <string>
                string plistType = ConsumeNextPListType(reader);

                if (String.IsNullOrEmpty(plistType))
                {
                    string msg = String.Format(
                        CultureInfo.InvariantCulture,
                        "The type of the primitive couldn't be found. ");

                    throw new YummyZoneException(msg);
                }

                if (plistType == "dict" || plistType == "array")
                {
                    throw new YummyZoneException("Unexpected non basic PList type: " + plistType);
                }

                // special case for boolean
                if (plistType == "true")
                {
                    return true;
                }

                if (plistType == "false")
                {
                    return false;
                }

                object targetObject = null;

                // <string>Baris</string>
                // <string></string>            
                string primitiveDataAsString = ConsumeNextText(reader);
                if (String.IsNullOrEmpty(primitiveDataAsString))
                {
                    Logger.LogAsInfo("ReadPrimitiveType", Guid.Empty, "Text element for data is null or empty. Returning default value...");
                    return null;
                }
                else
                {
                    // primitive type is not null
                    targetObject = ConvertStringToTypedPrimitive(targetType, primitiveDataAsString);
                }

                // read closing part; e.g. </string>
                if (!ConsumeNextEndPListType(reader, plistType))
                {
                    string msg = String.Format(
                        CultureInfo.InvariantCulture,
                        "Unclosing type element for the primitive {0}",
                        plistType);

                    throw new YummyZoneException(msg);
                }

                return targetObject;
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ReadPrimitiveType", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static object ReadSingleObject(XmlDictionaryReader reader, Type targetType)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ReadSingleObject", Guid.Empty, "Method started", null);

                // read <dict> part
                if (!ConsumeNextElement(reader, "dict"))
                {
                    throw new YummyZoneException("Unexpected element. We were expecting a beginning 'dict' element");
                }

                object targetObject = Activator.CreateInstance(targetType);

                while (!reader.EOF && CheckNextName(reader, true) != "dict")
                {
                    PropertyInfo propertyInfo = ConsumeNextProperty(reader, targetType);

                    string plistType = CheckNextName(reader, false);
                    if (String.IsNullOrEmpty(plistType))
                    {
                        string msg = String.Format(
                            CultureInfo.InvariantCulture,
                            "The value for property '{0}' couldn't be found. It will be left as null",
                            propertyInfo.Name);

                        Logger.LogAsInfo("ReadSingleObject", Guid.Empty, msg);
                        continue;
                    }

                    if (!IsPListType(plistType))
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "key")
                        {
                            string msg = String.Format(
                                CultureInfo.InvariantCulture,
                                "The value for property '{0}' couldn't be found. It will be left as null",
                                propertyInfo.Name);

                            Logger.LogAsInfo("ReadSingleObject", Guid.Empty, msg);
                            continue;
                        }
                        else
                        {
                            throw new YummyZoneException("Unexpected type: " + plistType);
                        }
                    }

                    if (plistType == "array")
                    {
                        // read
                        IList items = (IList)ReadArray(reader, propertyInfo.PropertyType);

                        // get the list reference from the target object
                        IList targetListObject = (IList)propertyInfo.GetValue(targetObject, null);

                        // set
                        foreach (object o in items)
                        {
                            targetListObject.Add(o);
                        }
                    }
                    else if (plistType == "dict")
                    {
                        object propertyValue = ReadSingleObject(reader, propertyInfo.PropertyType);
                        propertyInfo.SetValue(targetObject, propertyValue, null);
                    }
                    else // primitive
                    {
                        object primitiveDataAsObj = ReadPrimitiveType(reader, propertyInfo.PropertyType);
                        propertyInfo.SetValue(targetObject, primitiveDataAsObj, null);
                    }
                }

                // read </dict> part
                if (!ConsumeNextEndElement(reader, "dict"))
                {
                    throw new YummyZoneException("Unexpected element. We were expecting a closing 'dict' element");
                }

                return targetObject;
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ReadSingleObject", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static object ReadArray(XmlDictionaryReader reader, Type targetType)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ReadArray", Guid.Empty, "Method started", null);

                // read <array> part
                if (!ConsumeNextElement(reader, "array"))
                {
                    throw new YummyZoneException("Unexpected element. We were expecting a beginning 'array' element");
                }

                if (null == targetType.GetInterface("System.Collections.IList"))
                {
                    throw new YummyZoneException("Property mismatch. It must be a type of IList; e.g. List, Collection, etc");
                }

                Type listItemType = null;
                if (targetType.IsGenericType)
                {
                    listItemType = targetType.GetGenericArguments()[0];
                }
                else
                {
                    throw new YummyZoneException("Only generic list are supported as containers");
                }

                // create a temp list
                Type genericListType = typeof(List<>);
                System.Type specificListType = genericListType.MakeGenericType(new System.Type[] { listItemType });
                object listObject = Activator.CreateInstance(specificListType, null);

                /*
                // get the list reference from the target object
                object targetListObject = propertyInfo.GetValue(targetObject, null);
                */

                // add items to the list on the target object
                SkipIgnorables(reader);
                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name != null && reader.Name.ToLower() == "array"))
                {
                    PListSerializer serializer = new PListSerializer(listItemType);
                    object listItem = serializer.ReadObject(reader);
                    ((System.Collections.IList)listObject).Add(listItem);
                    SkipIgnorables(reader);
                }

                // read </array> part
                if (!ConsumeNextEndElement(reader, "array"))
                {
                    throw new YummyZoneException("Unexpected element. We were expecting a closing 'array' element");
                }

                return listObject;
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ReadArray", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static PropertyInfo ConsumeNextProperty(XmlDictionaryReader reader, Type targetType)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ConsumeNextProperty", Guid.Empty, "Method started", null);

                // consume <key> part
                if (!ConsumeNextElement(reader, "key"))
                {
                    throw new YummyZoneException("Unexpected element. We were expecting a beginning 'key' element");
                }

                // consume data between <key>  and </key> parts
                // data refers to the property name here
                string propertyName = ConsumeNextText(reader);
                if (String.IsNullOrEmpty(propertyName))
                {
                    throw new YummyZoneException("Unexpected element. We were expecting a text element which represents a property name");
                }

                // consume </key> part
                if (!ConsumeNextEndElement(reader, "key"))
                {
                    throw new YummyZoneException("Unexpected element. We were expecting a closing 'key' element");
                }

                // check if the type has such a property
                PropertyInfo propertyInfo = targetType.GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    string error = String.Format(
                        CultureInfo.InvariantCulture,
                        "Public property '{0}' couldn't be found on type '{1}'.",
                        propertyName,
                        targetType.Name);

                    // consider to ignore this in the future
                    throw new YummyZoneException(error);
                }

                return propertyInfo;
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ConsumeNextProperty", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static void SkipIgnorables(XmlDictionaryReader reader)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.SkipIgnorables", Guid.Empty, "Method started", null);

                while (!reader.EOF)
                {
                    if ((reader.IsEmptyElement && reader.Name != "true" && reader.Name != "false") ||
                        reader.NodeType == XmlNodeType.None ||
                        reader.NodeType == XmlNodeType.Whitespace ||
                        reader.NodeType == XmlNodeType.XmlDeclaration ||
                        reader.NodeType == XmlNodeType.SignificantWhitespace ||
                        reader.NodeType == XmlNodeType.Comment ||
                        reader.NodeType == XmlNodeType.CDATA ||
                        (reader.NodeType == XmlNodeType.Element && reader.Name != null && reader.Name.ToLower() == "plist") ||
                        (reader.NodeType == XmlNodeType.EndElement && reader.Name != null && reader.Name.ToLower() == "plist"))
                    {
                        reader.Read();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.SkipIgnorables", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }
      
        private static string CheckNextName(XmlDictionaryReader reader, bool includeEndElement)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.CheckNextName", Guid.Empty, "Method started", null);

                // skip whitespaces, empties, etc
                SkipIgnorables(reader);

                // check EOF
                if (reader.EOF)
                {
                    return null;
                }

                bool isElement = reader.NodeType == XmlNodeType.Element;
                if (includeEndElement)
                {
                    isElement |= reader.NodeType == XmlNodeType.EndElement;
                }

                // consume element; e.g. key, string, int
                if (isElement && reader.Name != null)
                {
                    // do NOT consume!
                    return reader.Name.ToLower();
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.CheckNextName", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static bool ConsumeNextElement(XmlDictionaryReader reader, string name)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ConsumeNextElement", Guid.Empty, "Method started", null);

                // skip whitespaces, empties, etc
                SkipIgnorables(reader);

                // check EOF
                if (reader.EOF)
                {
                    return false;
                }

                // consume element; e.g. key, string, int
                if (reader.NodeType == XmlNodeType.Element &&
                    reader.Name != null &&
                    reader.Name.ToLower() == name)
                {
                    // consume
                    reader.Read();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ConsumeNextElement", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static string ConsumeNextText(XmlDictionaryReader reader)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ConsumeNextText", Guid.Empty, "Method started", null);

                // skip whitespaces, empties, etc
                SkipIgnorables(reader);

                // check EOF
                if (reader.EOF)
                {
                    return null;
                }

                // consume text
                string value = string.Empty;
                while (reader.NodeType == XmlNodeType.Text &&
                    String.IsNullOrEmpty(reader.Name) &&
                    !String.IsNullOrEmpty(reader.Value))
                {
                    // consume
                    value += reader.Value;
                    reader.Read();

                    while (reader.NodeType == XmlNodeType.Whitespace)
                    {
                        value += " ";
                        reader.Read();
                    }
                }

                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                else
                {
                    return value.Trim();
                }
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ConsumeNextText", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static bool ConsumeNextEndElement(XmlDictionaryReader reader, string name)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ConsumeNextEndElement", Guid.Empty, "Method started", null);

                // skip whitespaces, empties, etc
                SkipIgnorables(reader);

                // check EOF
                if (reader.EOF)
                {
                    return false;
                }

                // consume element; e.g. key, string, int
                if (reader.NodeType == XmlNodeType.EndElement &&
                    reader.Name != null &&
                    reader.Name.ToLower() == name)
                {
                    // consume
                    reader.Read();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ConsumeNextEndElement", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static string ConsumeNextPListType(XmlDictionaryReader reader)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ConsumeNextPListType", Guid.Empty, "Method started", null);

                // skip whitespaces, empties, etc
                SkipIgnorables(reader);

                // check EOF
                if (reader.EOF)
                {
                    return null;
                }

                // consume text
                if (reader.NodeType == XmlNodeType.Element &&
                    reader.Name != null)
                {
                    string type = reader.Name.ToLower();
                    if (IsPListType(type))
                    {
                        // consume
                        reader.Read();
                        return type;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ConsumeNextPListType", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }

        private static bool ConsumeNextEndPListType(XmlDictionaryReader reader, string type)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.ConsumeNextEndPListType", Guid.Empty, "Method started", null);

                // skip whitespaces, empties, etc
                SkipIgnorables(reader);

                // check EOF
                if (reader.EOF)
                {
                    return false;
                }

                // consume text
                if (reader.NodeType == XmlNodeType.EndElement &&
                    reader.Name != null &&
                    reader.Name.ToLower() == type)
                {
                    reader.Read();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.ConsumeNextEndPListType", Guid.Empty, ex.ToString(), null);
                throw ex;
            }
        }
    }
}