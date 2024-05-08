//
// Copyright (c) 2002-2023 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NDO.Application;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace NDO.Xml
{
	/// <summary>
	/// This class helps converting an Xml node to a given type and vice versa.
	/// </summary>
	public class XmlHelper
	{
        /// <summary>
        /// This enum is used to indicate if a Xml value is stored as an attribute or an element.
        /// </summary>
        public enum StorageType
        {
            /// <summary/>
            Element,
            /// <summary/>
            Attribute
        }

        /// <summary>
        /// Delegate type to be used by ExtractValueEvent.
        /// </summary>
        /// <param name="parentNode">The node containing the value.</param>
        /// <param name="valueName">The name of the xml subnode or attribute used to store the value.</param>
        /// <param name="type">The CLR type in which the value should be converted.</param>
        /// <param name="storageType">Indicates, if the value is stored as an attribute or an element.</param>
        /// <param name="result">The resulting object.</param>
        /// <returns>
        /// A boolean value. True indicates that the event handler did the extraction of the value.
        /// False indicates, that the XmlHelper should extract the value.
        /// </returns>
        public delegate bool ExtractValueHandler(XmlNode parentNode, string valueName, Type type, StorageType storageType, out object result);

        /// <summary>
        /// This event is used to customize the extraction of elements.
        /// If the event handler returns true, the XmlHelper returns the object created by the handler.
        /// </summary>
        public static event ExtractValueHandler ExtractValueEvent;

        /// <summary>
        /// Delegate to be used by the SetValueEvent.
        /// </summary>
        /// <param name="parentNode">The node containing the value.</param>
        /// <param name="valueName">The name of the xml subnode or attribute used to store the value.</param>
        /// <param name="type">The CLR type in which the value should be converted.</param>
        /// <param name="storageType">Indicates, if the value is stored as an attribute or an element.</param>
        /// <param name="value">The value to be set.</param>
        /// <returns>
        /// A boolean value. True indicates that the event handler set the value according to it's own rules.
        /// False indicates, that the XmlHelper should set the value.
        /// </returns>
        public delegate bool SetValueHandler(XmlNode parentNode, string valueName, Type type, StorageType storageType, object value);

        /// <summary>
        /// This event is used to customize the setting of element values.
        /// If the event handler returns true, the XmlHelper doesn't set the value.
        /// </summary>
        public static event SetValueHandler SetValueEvent;

		static CultureInfo cultureInfo;

		/// <summary>
		/// Gets or sets the culture info used to convert values from or to strings.
		/// </summary>
        /// <remarks>
        /// DateTime values will be extracted by an culture-independent algorithm capable of extracting
        /// the most common date and time formats.
        /// </remarks>
		public static CultureInfo CultureInfo
		{
			get 
			{ 
				if (cultureInfo == null)
					cultureInfo = CultureInfo.InvariantCulture;

				return cultureInfo; 
			}
			set { cultureInfo = value; }
		}



		static void LogError( Exception ex, string method, string attributeOrElementName )
		{
            var logger = NDOApplication.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<XmlHelper>();
			logger.LogError(method + ": Attribute or element: " + attributeOrElementName + ": " + ex.ToString());
        }

        enum TimeSig { pm, none }  // am results to the same time as none

        static DateTime ParseDt(string sourceString)
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add(@"^(\d{4})-(\d{2})-(\d{2})", "ymd");
                dict.Add(@"^(\d{4})/(\d{1,2})/(\d{1,2})", "ymd");
                dict.Add(@"^(\d{4})\.\s*(\d{1,2})\.\s*(\d{1,2})", "ymd");
                dict.Add(@"^(\d{1,2})/(\d{1,2})/(\d+)", "dmy");
                dict.Add(@"^(\d{1,2})/(\d{1,2})\s+(\d+)", "dmy");
                dict.Add(@"^(\d{2})\.\s*(\d+)\.\s*(\d+)", "dmy");
                dict.Add(@"^(\d+)-(\d+)-(\d+)", "dmy");
                //This code is an attempt to process the different DateTime formats of different cultures.
                //static readonly string odisDateTimeFormat = "yyyy-MM-ddTHH'h'mm'm'ss's'";
                //static readonly string deDateTimeFormat = "dd.MM.yyyy HH:mm:ss";
                //static readonly string usDateTimeFormat = "MM'/'dd'/'yyyy HH:mm:ss";

                int year = 0;
                int month = 0;
                int day = 0;
                int hour = 0;
                int minute = 0;
                int second = 0;

                Regex regex;
                Match match = null;

                string usPattern = @"^(\d{1,2})/(\d{1,2})/(\d{4})\s[^\s]+\s[A|P]M";  // US format must be distinguished from GB format
                regex = new Regex(usPattern);
                bool isUSFormat = regex.Match(sourceString).Success;


                foreach (string pattern in dict.Keys)
                {
                    regex = new Regex(pattern);
                    match = regex.Match(sourceString);
                    if (match.Success)
                    {
                        switch (dict[pattern])
                        {
                            case "ymd":
                                year = int.Parse(match.Groups[1].Value);
                                month = int.Parse(match.Groups[2].Value);
                                day = int.Parse(match.Groups[3].Value);
                                break;
                            case "mdy":
                                month = int.Parse(match.Groups[1].Value);
                                day = int.Parse(match.Groups[2].Value);
                                year = int.Parse(match.Groups[3].Value);
                                break;
                            case "dmy":
                                day = int.Parse(match.Groups[1].Value);
                                month = int.Parse(match.Groups[2].Value);
                                year = int.Parse(match.Groups[3].Value);
                                if (year < 100)
                                    year += 2000;
                                break;
                        }
                        break;
                    }
                }

                // Swap month and day values, if we have an US format
                if (isUSFormat)
                {
                    int temp = month;
                    month = day;
                    day = temp;
                }

                string timeString = sourceString;
                if (match != null)
                    timeString = sourceString.Substring(match.Length + 1);

                TimeSig timeSig = TimeSig.none;

                Dictionary<string, TimeSig> timeSigDict = new Dictionary<string, TimeSig>();
                timeSigDict.Add("PM", TimeSig.pm);
                timeSigDict.Add("p.m.", TimeSig.pm);
                timeSigDict.Add("MD", TimeSig.pm);
                timeSigDict.Add("CH", TimeSig.pm);
                timeSigDict.Add("Ale", TimeSig.pm);
                timeSigDict.Add("Yamma", TimeSig.pm);
                timeSigDict.Add("م", TimeSig.pm);
                timeSigDict.Add("ب.ظ", TimeSig.pm);
                timeSigDict.Add("Efifie", TimeSig.pm);
                timeSigDict.Add("nm", TimeSig.pm);
                timeSigDict.Add("μμ", TimeSig.pm);
                timeSigDict.Add("غ.و", TimeSig.pm);
                timeSigDict.Add("ප.ව.", TimeSig.pm);
                timeSigDict.Add("ܒ.ܛ", TimeSig.pm);



                foreach (KeyValuePair<string, TimeSig> kvp in timeSigDict)
                {
                    if (timeString.EndsWith(kvp.Key))
                    {
                        timeSig = kvp.Value;
                        break;
                    }
                }

                if (timeSig == TimeSig.none)
                {
                    if (timeString.IndexOf("下午") > -1)
                        timeSig = TimeSig.pm;
                    else if (timeString.IndexOf("আবেলি") > -1)
                        timeSig = TimeSig.pm;
                    else if (timeString.IndexOf("오후") > -1)
                        timeSig = TimeSig.pm;
                    else if (timeString.IndexOf("बेलुकी") > -1)
                        timeSig = TimeSig.pm;
                    else if (timeString.IndexOf("ਸ਼ਾਮ") > -1)
                        timeSig = TimeSig.pm;
                    else if (timeString.IndexOf(" PM ") > -1)
                        timeSig = TimeSig.pm;
                }


                dict.Clear();
                dict.Add(@"(\d{2}):(\d{2}):(\d{2})", "hms");
                dict.Add(@"(\d{2}):(\d{2})", "hm");
                dict.Add(@"(\d{2}).(\d{2}).(\d{2})", "hms");
                dict.Add(@"(\d{2})h(\d{2})m(\d{2})s", "hms");


                foreach (string pattern in dict.Keys)
                {
                    regex = new Regex(pattern);
                    match = regex.Match(timeString);
                    if (match.Success)
                    {
                        switch (dict[pattern])
                        {
                            case "hms":
                                hour = int.Parse(match.Groups[1].Value);
                                minute = int.Parse(match.Groups[2].Value);
                                second = int.Parse(match.Groups[3].Value);
                                break;
                            case "hm":
                                hour = int.Parse(match.Groups[1].Value);
                                minute = int.Parse(match.Groups[2].Value);
                                second = 0;
                                break;
                        }
                        break;
                    }
                }

                if (timeSig == TimeSig.pm)
                    hour += 12;

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch (Exception ex)
            {
                Exception ex2 = new Exception("Error while parsing a DateTime string. String = '" + sourceString + "' Message: " + ex.Message);
                throw (ex2);
            }
        }
		

		/// <summary>
		/// Gets the value described in an element.
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="elementName"></param>
		/// <param name="resultType"></param>
		/// <returns></returns>
		public static object GetElement( XmlNode xmlNode, string elementName, Type resultType )
		{
            try
            {
                if (ExtractValueEvent != null)
                {
                    object result;
                    if (ExtractValueEvent(xmlNode, elementName, resultType, StorageType.Element, out result))
                        return result;
                }

                XmlNode subNode = xmlNode.SelectSingleNode(elementName);
                if (subNode == null)
                {
                    return GetNullValue(resultType);
                }
                else
                {
                    return ConvertString(resultType, subNode.InnerText);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "GetElement", elementName);
            }
			return GetNullValue(resultType);
		}

		private static object ConvertString( Type resultType, string value )
		{
			if ( resultType == typeof( string ) )
				return value;

			if ( resultType == typeof( DateTime ) )
				return ParseDt( value );

            if (resultType == typeof(Guid))
                return new Guid(value);
            
            if (resultType.IsEnum)
                return Enum.Parse(resultType, value);

			return Convert.ChangeType( value, resultType );
		}

		

		/// <summary>
		/// Gets the value of a certain attribute and converts it to the given result type.
		/// </summary>
		/// <remarks>
		/// If the value can't be converted, a message to the logging object will be written. 
		/// If no logging object exists, an exception will be thrown. 
		/// </remarks>
		/// <param name="xmlNode">The parent node of the attribute.</param>
		/// <param name="attr">The attribute name.</param>
		/// <param name="resultType">The result type</param>
		/// <returns></returns>
		public static object GetAttribute( XmlNode xmlNode, string attr, Type resultType )
		{
			try
			{
                if (ExtractValueEvent != null)
                {
                    object result;
                    if (ExtractValueEvent(xmlNode, attr, resultType, StorageType.Attribute, out result))
                        return result;
                }

                if (xmlNode.Attributes == null)
                    throw new Exception("XmlHelper: Attributes property of the XmlNode '" + xmlNode.Name + "' is null. Can't resolve the attribute '" + attr + "'. If the node is an XmlDocument, you should execute an XPath query with SelectNodes or SelectSingleNode.");

                if (xmlNode.Attributes[attr] == null)
				{
					return GetNullValue( resultType );
				}
				else
				{
					string value = xmlNode.Attributes[attr].Value;
					return ConvertString(resultType, value);
				}
			}
			catch ( Exception ex )
			{
				LogError(ex, "GetAttribute", attr);
			}
			return GetNullValue(resultType);
		}

		private static object GetNullValue( Type resultType )
		{
			if ( resultType == typeof( DateTime ) )
				return DateTime.MinValue;
			if ( resultType.IsClass )  // This is the case with string
				return null;
            if (resultType == typeof(Guid))
                return Guid.Empty;
			return (Convert.ChangeType( 0, resultType ));
		}

		/// <summary>
		/// Creates a simple element and sets the value of the element. 
		/// </summary>
		/// <remarks>
		/// The element will be converted to a string according to the CultureInfo property of this class. 
		/// If no CultureInfo is set, CultureInfo.InvariantCulture will be used.
		/// </remarks>
		/// <param name="parentNode">Parent node of the element.</param>
		/// <param name="elementName">The element name.</param>
		/// <param name="value">The value of the element.</param>
		/// <param name="type">The type of the value.</param>
		public static void SetElement( XmlNode parentNode, string elementName, object value, Type type )
		{
            try
            {
                if (SetValueEvent != null)
                {
                    // The handler sets the value
                    if (SetValueEvent(parentNode, elementName, type, StorageType.Element, value))
                        return;
                }

                if (value == null)
                    return;
                string strValue = null;
                strValue = (string)Convert.ChangeType(value, typeof(String), CultureInfo);
                XmlDocument parentDocument = parentNode as XmlDocument;
                if (parentDocument == null)
                    parentDocument = parentNode.OwnerDocument;
                XmlElement subElement = parentDocument.CreateElement(elementName);
                parentNode.AppendChild(subElement);
                subElement.InnerText = strValue;
            }
            catch (Exception ex)
            {
                LogError(ex, "SetElement", elementName);
            }
		}



		/// <summary>
		/// Sets the value of an attribute.
		/// </summary>
		/// <param name="parentNode">The parent node.</param>
		/// <param name="attr">The name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <param name="type">The type of the value.</param>
		public static void SetAttribute( XmlNode parentNode, string attr, object value, Type type )
		{
            try
            {
                if (SetValueEvent != null)
                {
                    // The handler sets the value
                    if (SetValueEvent(parentNode, attr, type, StorageType.Attribute, value))
                        return;
                }

                if (value == null)
                    return;
                string strValue = null;
                if (type == typeof(DateTime))
                    strValue = ((DateTime)value).ToString(CultureInfo);
                else
                    strValue = (string)Convert.ChangeType(value, typeof(String), CultureInfo);
                ((XmlElement)parentNode).SetAttribute(attr, strValue);
            }
            catch (Exception ex)
            {
                LogError(ex, "SetAttribute", attr);
            }
		}
	}
}
