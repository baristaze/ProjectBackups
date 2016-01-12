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
        private Type targetType;
        private List<PropertyInfo> DataMemberFieldsOfType { get; set; }
        private static Dictionary<Type, PListSerializer> ConverterLookupTable = new Dictionary<Type, PListSerializer>();

        public PListSerializer(Type type)
        {
            this.targetType = type;
            this.Initialize(type);
        }

        private void Initialize(Type classType)
        {
            try
            {
                Logger.LogAsVerbose("PListSerializer.Initialize", Guid.Empty, "Method started for type: {0}", classType.Name);

                PropertyInfo[] propertyInfos = classType.GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    Attribute attrib = Attribute.GetCustomAttribute(propertyInfo, typeof(DataMemberAttribute));

                    if (attrib != null)
                    {
                        if (this.DataMemberFieldsOfType == null)
                        {
                            DataMemberFieldsOfType = new List<PropertyInfo>();
                        }

                        this.DataMemberFieldsOfType.Add(propertyInfo);

                        // if propertyType is not plist basic and not of array type, verify that it is a DataContract
                        if (IsPListArrayType(propertyInfo.PropertyType))
                        {
                            Type genericArgumentType = propertyInfo.PropertyType.GetGenericArguments()[0];
                            if (IsPListDictType(genericArgumentType))
                            {
                                if (!ConverterLookupTable.ContainsKey(genericArgumentType))
                                {
                                    ConverterLookupTable.Add(genericArgumentType, new PListSerializer(genericArgumentType));
                                }
                            }
                        }
                        else if (IsPListDictType(propertyInfo.PropertyType))
                        {
                            if (!ConverterLookupTable.ContainsKey(propertyInfo.PropertyType))
                            {
                                ConverterLookupTable.Add(propertyInfo.PropertyType, new PListSerializer(propertyInfo.PropertyType));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogAsError("PListSerializer.Initialize", Guid.Empty, "Error for type: {0}. Details: {1}", classType.Name, ex.ToString());
                throw ex;
            }
        }

        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            // this does nothing... but we still need to have this
            throw new NotImplementedException();
        }
        
        private static bool IsPListArrayType(Type objectType)
        {
            Type listType = objectType.GetInterface("System.Collections.IList");
            if (listType != null)
            {
                return true;
            }

            return false;
        }

        private static bool IsPListBasicType(Type objectType)
        {
            if (objectType == typeof(string) ||
                objectType == typeof(int) ||
                objectType == typeof(Guid) ||
                objectType == typeof(DateTime) ||
                objectType == typeof(long) ||
                objectType == typeof(uint) ||
                objectType == typeof(ulong) ||
                objectType == typeof(bool) ||
                objectType == typeof(float) ||
                objectType == typeof(double) || 
                objectType == typeof(decimal) || 
                objectType == typeof(short) || 
                objectType == typeof(ushort) ||
                objectType == typeof(byte))
                return true;

            return false;
        }

        private static bool IsPListDictType(Type objectType)
        {
            if (IsPListBasicType(objectType))
                return false;

            if (IsPListArrayType(objectType))
                return false;

            if (Attribute.GetCustomAttribute(objectType, typeof(DataContractAttribute)) != null)
                return true;

            return false;
        }

        private static bool IsPListType(string type)
        {
            if (type == "array" ||
                type == "dict" ||
                type == "string" ||
                type == "integer" ||
                type == "real" ||
                type == "true" ||
                type == "false" ||
                type == "date")
            {
                return true;
            }

            return false;
        }

        private static string PListTypeFromPrimitiveType(Type objectType)
        {
            if (objectType == typeof(string) ||
                objectType == typeof(Guid))
                return "string";

            if (objectType == typeof(int) ||
                objectType == typeof(short) ||
                objectType == typeof(ushort) ||
                objectType == typeof(byte))
                return "integer";

            if (objectType == typeof(DateTime))
                return "date";

            if (objectType == typeof(long) ||
                objectType == typeof(uint) ||
                objectType == typeof(ulong)||
                objectType == typeof(float) ||
                objectType == typeof(double) ||
                objectType == typeof(decimal))
                return "real";

            if (objectType == typeof(bool))
            {
                string errMsg = "Do not hit this line. Handle bool separately.";
                Logger.LogAsError("PListTypeFromPrimitiveType", Guid.Empty, errMsg);
                throw new YummyZoneException(errMsg);
            }

            string errorMessage = String.Format(
                CultureInfo.InvariantCulture,
                "{0} is not a base PLIST supported type",
                objectType.Name);

            Logger.LogAsError("PListTypeFromPrimitiveType", Guid.Empty, errorMessage);
            throw new YummyZoneException(errorMessage);
        }

        private static object ConvertStringToTypedPrimitive(Type primitiveType, string data)
        {
            if (primitiveType == typeof(string))
                return data;

            if (primitiveType == typeof(bool))
                return XmlConvert.ToBoolean(data.ToLower());

            if (primitiveType == typeof(byte))
                return XmlConvert.ToByte(data);

            if (primitiveType == typeof(short))
                return XmlConvert.ToInt16(data);

            if (primitiveType == typeof(ushort))
                return XmlConvert.ToUInt16(data);

            if (primitiveType == typeof(int))
                return XmlConvert.ToInt32(data);

            if (primitiveType == typeof(uint))
                return XmlConvert.ToUInt32(data);

            if (primitiveType == typeof(long))
                return XmlConvert.ToInt64(data);

            if (primitiveType == typeof(ulong))
                return XmlConvert.ToUInt64(data);

            if (primitiveType == typeof(float))
                return XmlConvert.ToSingle(data);

            if (primitiveType == typeof(double))
                return XmlConvert.ToDouble(data);

            if (primitiveType == typeof(decimal))
                return XmlConvert.ToDecimal(data);

            if (primitiveType == typeof(DateTime)) // UTC is assumed
                return XmlConvert.ToDateTime(data, "yyyy-MM-ddTHH:mm:ssZ").ToUniversalTime();

            if (primitiveType == typeof(Guid))
                return XmlConvert.ToGuid(data);

            string errorMessage = String.Format(
                CultureInfo.InvariantCulture,
                "{0} is not a base PLIST supported type",
                primitiveType.Name);

            throw new YummyZoneException(errorMessage);
        }
    }
}