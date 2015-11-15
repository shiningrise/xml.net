﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Xml.Net
{
    /// <summary>
    /// A selection of common methods called from multiple classes.
    /// </summary>
    internal class Utilities
    {
        /// <summary>
        /// Checks if a property should not be serialized.
        /// </summary>
        /// <param name="property">The property to check.</param>
        public static bool ShouldIgnoreProperty(PropertyInfo property)
        {
            return property == null ||
                property.GetCustomAttribute<XmlConvertIgnoredAttribute>() != null ||
                !property.CanRead ||
                !property.CanWrite;
        }

        /// <summary>
        /// Gets the XML identifier of the class from an object of that class.
        /// </summary>
        /// <param name="obj">The object to use.</param>
        /// <returns>The XML identifier of the class.</returns>
        public static string GetIdentifier(object obj)
        {
            var xmlConvertible = obj as IXmlConvertible;
            if (xmlConvertible != null)
            {
                return xmlConvertible.XmlIdentifier;
            }

            var typeInfo = obj.GetType().GetTypeInfo();
            return GetIdentifier(typeInfo);
        }

        /// <summary>
        /// Gets the XML identifier of the member.
        /// </summary>
        /// <param name="memberInfo">The information about the member to use.</param>
        /// <returns>The XML identifier of the member.</returns>
        public static string GetIdentifier(MemberInfo memberInfo)
        {
            var nameAttribute = memberInfo.GetCustomAttribute<XmlConvertCustomElementAttribute>();
            if (nameAttribute != null)
            {
                return nameAttribute.Name;
            }

            return memberInfo.Name;
        }
        
        /// <summary>
        /// Gets the custom name of collection elements of a property. Defaults to "Element".
        /// </summary>
        /// <param name="property">The property to use.</param>
        public static string GetCollectionElementName(PropertyInfo property)
        {
            var elementNameAttribute = property?.GetCustomAttribute<XmlConvertElementsNameAttribute>();
            if (elementNameAttribute?.Name != null)
            {
                return elementNameAttribute.Name;
            }

            return "Element";
        }

        /// <summary>
        /// Gets the custom name of collection key and value elements of a property.
        /// </summary>
        /// <param name="property">The property to use.</param>
        public static KeyValuePair<string, string> GetDictionaryElementName(PropertyInfo property)
        {
            var keyName = "Key";
            var valueName = "Value";

            var elementNameAttribute = property?.GetCustomAttribute<XmlConvertKeyValueElementAttribute>();
            if (elementNameAttribute != null)
            {
                if (elementNameAttribute.KeyName != null)
                {
                    keyName = elementNameAttribute.KeyName;
                }

                if (elementNameAttribute.ValueName != null)
                {
                    valueName = elementNameAttribute.ValueName;
                }
            }

            return new KeyValuePair<string, string>(keyName, valueName);
        }


        /// <summary>
        /// Gets the type of an object from its serialized XElement (e.g. if it has a type attribute) and its parent container type (e.g. if it is generic).
        /// </summary>
        /// <param name="element">The XElement representing the object used to get the type.</param>
        /// <param name="parentType">The type of the object's parent container used to check if the object is in a generic container.</param>
        /// <param name="genericArgumentIndex">The index of the object's type in the list of its parent container's generic arguments.</param>
        /// <returns>The type of an object from its serialized XElement and its parent container type.</returns>
        public static Type GetElementType(XElement element, Type parentType, int genericArgumentIndex)
        {
            Type type = null;

            var typeString = element?.Attribute("Type")?.Value;
            if (typeString != null)
            {
                type = Type.GetType(typeString);
            }

            if (type == null)
            {
                var genericArguments = parentType?.GetTypeInfo().GenericTypeArguments;
                if (genericArguments != null && 
                    genericArgumentIndex >= 0 &&
                    genericArguments.Length > genericArgumentIndex)
                {
                    type = genericArguments[genericArgumentIndex];
                }
            }

            return type;
        }

        /// <summary>
        /// Adds the specified XElement to the parent XElement if it is not null;
        /// </summary>
        /// <param name="child">The child XElement to add to the parent XElement.</param>
        /// <param name="parent">The parent XElement to add the child XElement to.</param>
        public static void AddChildElement(XElement child, XElement parent)
        {
            if (child == null || parent == null)
            {
                return;
            }

            parent.Add(child);
        }

        /*
        /// <summary>
        /// Formats and adds a serialized object XElement to a parent element with options.
        /// </summary>
        /// <param name="value">The object serialized.</param>
        /// <param name="element">The serialized XElement of the object.</param>
        /// <param name="parentElement">The parent element of the serialized XElement.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        private static void SetupSerializedElement(object value, XElement element, XElement parentElement, XmlConvertOptions options)
        {
            if (value == null) { return; }
            if (element == null) { return; }
            if (parentElement == null) { return; }

            if (!options.HasFlag(XmlConvertOptions.ExcludeTypes))
            {
                var typeName = value.GetType().FullName;
                element.Add(new XAttribute("Type", typeName));
            }

            parentElement.Add(element);
        }*/

        /// <summary>
        /// Gets the name-specified child XElement of the parent XElement.
        /// </summary>
        /// <param name="name">The name of the child XElement to get.</param>
        /// <param name="parent">The parent of the child XElement to get.</param>
        /// <returns>The name-specified child XElement of the parent XElement.</returns>
        public static XElement GetChildElement(string name, XElement parent)
        {
            var element = parent.Element(name);
            if (element == null) { /*No such element*/ }

            return element;
        }
    }
}