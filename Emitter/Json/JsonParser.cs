/*
Copyright (c) 2016 Roman Atachiants
Copyright (c) 2011-2013 Mike Jones, Matt Weimer

The MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do 
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


// Source code is modified from Mike Jones's JSON Serialization and Deserialization library (https://www.ghielectronics.com/community/codeshare/entry/357)

using System;
using System.Collections;
using System.Globalization;
using System.Text;
#if (MF_FRAMEWORK_VERSION_V4_2 || MF_FRAMEWORK_VERSION_V4_3)
using Microsoft.SPOT;
#endif

namespace Emitter
{
	/// <summary>
	/// Parses JSON strings into a Hashtable.  The Hashtable contains one or more key/value pairs
	/// (DictionaryEntry objects).  Each key is the name of a property that (hopefully) exists
	/// in the class object that it represents.  Each value is one of the following:
	///   Hastable - Another list of one or more DictionaryEntry objects, essentially representing
	///              a property that is another class.
	///   ArrayList - An array of one or more objects, which themselves can be one of the items
	///               enumerated in this list.
	///   Value Type - an actual value, such as a string, int, bool, Guid, DateTime, etc
	/// </summary>
	internal class JsonParser
	{
		protected enum Token
		{
			None = 0,
			ObjectBegin,				// {
			ObjectEnd,					// }
			ArrayBegin,					// [
			ArrayEnd,					// ]
			PropertySeparator,			// :
			ItemsSeparator,				// ,
			StringType,					// "  <-- string of characters
			NumberType,					// 0-9  <-- number, fixed or floating point
			BooleanTrue,				// true
			BooleanFalse,				// false
			NullType					// null
		}

		/// <summary>
		/// Parses the string json into a value
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <returns>An ArrayList, a Hashtable, a double, long, a string, null, true, or false</returns>
		public static object JsonDecode(string json)
		{
			bool success = true;
			
			return JsonDecode(json, ref success);
		}

		/// <summary>
		/// Parses the string json into a value; and fills 'success' with the successfullness of the parse.
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <param name="success">Successful parse?</param>
		/// <returns>An ArrayList, a Hashtable, a double, a long, a string, null, true, or false</returns>
		public static object JsonDecode(string json, ref bool success)
		{
			success = true;
			if (json != null)
			{
                char[] charArray = json.ToCharArray();
                int index = 0;
				object value = ParseValue(charArray, ref index, ref success);
				return value;
            }
			else
			{
                return null;
            }
		}

		protected static Hashtable ParseObject(char[] json, ref int index, ref bool success)
		{
			Hashtable table = new Hashtable();
			Token token;

			// {
			NextToken(json, ref index);

			bool done = false;
			while (!done)
			{
				token = LookAhead(json, index);
				if (token == Token.None)
				{
					success = false;
					return null;
				}
				else if (token == Token.ItemsSeparator)
				{
					NextToken(json, ref index);
				}
				else if (token == Token.ObjectEnd)
				{
					NextToken(json, ref index);
					return table;
				}
				else
				{

					// name
					string name = ParseString(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}

					// :
					token = NextToken(json, ref index);
					if (token != Token.PropertySeparator)
					{
						success = false;
						return null;
					}

					// value
					object value = ParseValue(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}

					table[name] = value;
				}
			}

			return table;
		}

		protected static ArrayList ParseArray(char[] json, ref int index, ref bool success)
		{
			ArrayList array = new ArrayList();

			// [
			NextToken(json, ref index);

			bool done = false;
			while (!done)
			{
				Token token = LookAhead(json, index);
				if (token == Token.None)
				{
					success = false;
					return null;
				}
				else if (token == Token.ItemsSeparator)
				{
					NextToken(json, ref index);
				}
				else if (token == Token.ArrayEnd)
				{
					NextToken(json, ref index);
					break;
				}
				else
				{
					object value = ParseValue(json, ref index, ref success);
					if (!success)
					{
						return null;
					}

					array.Add(value);
				}
			}

			return array;
		}

		protected static object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (LookAhead(json, index))
			{
				case Token.StringType:
					return ParseString(json, ref index, ref success);

				case Token.NumberType:
					return ParseNumber(json, ref index, ref success);

				case Token.ObjectBegin:
					return ParseObject(json, ref index, ref success);

				case Token.ArrayBegin:
					return ParseArray(json, ref index, ref success);

				case Token.BooleanTrue:
					NextToken(json, ref index);
					return true;

				case Token.BooleanFalse:
					NextToken(json, ref index);
					return false;

				case Token.NullType:
					NextToken(json, ref index);
					return null;

				case Token.None:
					break;
			}

			success = false;
			return null;
		}

		protected static string ParseString(char[] json, ref int index, ref bool success)
		{
            StringBuilder s = new StringBuilder();
			
			EatWhitespace(json, ref index);

            // "
            char c = json[index++];
			bool complete = false;
			while (!complete)
			{
				if (index == json.Length)
				{
					break;
				}

                c = json[index++];
				if (c == '"')
				{
                    complete = true;
                    break;
				}
				else if (c == '\\')
				{
					if (index == json.Length)
					{
						break;
					}

					c = json[index++];
					if (c == '"')
					{
						s.Append('"');
					}
					else if (c == '\\')
					{
						s.Append('\\');
					}
					else if (c == '/')
					{
						s.Append('/');
					}
					else if (c == 'b')
					{
						s.Append('\b');
					}
					else if (c == 'f')
					{
						s.Append('\f');
					}
					else if (c == 'n')
					{
						s.Append('\n');
					}
					else if (c == 'r')
					{
						s.Append('\r');
					}
					else if (c == 't')
					{
						s.Append('\t');
					}
					else if (c == 'u')
					{
						int remainingLength = json.Length - index;
						if (remainingLength >= 4)
						{
							// parse the 32 bit hex into an integer codepoint
							uint codePoint;
							if (!(success = Utils.TryParseUInt32(new string(json, index, 4), NumberEncoding.Hexadecimal, out codePoint)))
							{
								return "";
							}

							// convert the integer codepoint to a unicode char and add to string
							s.Append(Utils.ConvertFromUtf32((int)codePoint));

							// skip 4 chars
							index += 4;
						}
						else
						{
							break;
						}
					}

				}
				else
				{
					s.Append(c);
				}
			}

			if (!complete)
			{
				success = false;
				return null;
			}

			return s.ToString();
		}

		/// <summary>
		/// Determines the type of number (int, double, etc) and returns an object
		/// containing that value.
		/// </summary>
		/// <param name="json"></param>
		/// <param name="index"></param>
		/// <param name="success"></param>
		/// <returns></returns>
		protected static object ParseNumber(char[] json, ref int index, ref bool success)
		{
			EatWhitespace(json, ref index);

			int lastIndex = GetLastIndexOfNumber(json, index);
			int charLength = (lastIndex - index) + 1;

			// We now have the number as a string.  Parse it to determine the type of number.
			string value = new string(json, index, charLength);

			// Since the Json doesn't contain the Type of the property, and since multiple number
			// values can fit in the various Types (e.g. 33 decimal fits in an Int16, UInt16,
			// Int32, UInt32, Int64, and UInt64), we need to be a bit smarter in how we deal with
			// the size of the number, and also the case (negative or positive).
			object result = null;
			string dot = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
			string comma = CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator;
			string minus = CultureInfo.CurrentUICulture.NumberFormat.NegativeSign;
			string plus = CultureInfo.CurrentUICulture.NumberFormat.PositiveSign;
			if (value.Contains(dot) || value.Contains(comma) || value.Contains("e") || value.Contains("E"))
			{
				// We have either a double or a float.  Force it to be a double
				// and let the deserializer unbox it into the proper size.
				result = Double.Parse(new string(json, index, charLength));
			}
			else
			{
				NumberEncoding style = NumberEncoding.Decimal;
				if(value.StartsWith("0x") || (value.IndexOfAny(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F' }) >= 0))
				{
					style = NumberEncoding.Hexadecimal;
				}

				result = Utils.ParseInt64(value, style);
			}

			index = lastIndex + 1;

			return result;
		}

		protected static int GetLastIndexOfNumber(char[] json, int index)
		{
			int lastIndex;

			for (lastIndex = index; lastIndex < json.Length; lastIndex++) {
				if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1) {
					break;
				}
			}
			return lastIndex - 1;
		}

		protected static void EatWhitespace(char[] json, ref int index)
		{
			for (; index < json.Length; index++) {
				if (" \t\n\r".IndexOf(json[index]) == -1) {
					break;
				}
			}
		}

		protected static Token LookAhead(char[] json, int index)
		{
			int saveIndex = index;
			return NextToken(json, ref saveIndex);
		}

		protected static Token NextToken(char[] json, ref int index)
		{
			EatWhitespace(json, ref index);

			if (index == json.Length) {
				return Token.None;
			}
			
			char c = json[index];
			index++;
			switch (c) {
				case '{':
					return Token.ObjectBegin;
				case '}':
					return Token.ObjectEnd;
				case '[':
					return Token.ArrayBegin;
				case ']':
					return Token.ArrayEnd;
				case ',':
					return Token.ItemsSeparator;
				case '"':
					return Token.StringType;
				case '0': case '1': case '2': case '3': case '4': 
				case '5': case '6': case '7': case '8': case '9':
				case '-':
					return Token.NumberType;
				case ':':
					return Token.PropertySeparator;
			}
			index--;

			int remainingLength = json.Length - index;

			// false
			if (remainingLength >= 5) {
				if (json[index] == 'f' &&
					json[index + 1] == 'a' &&
					json[index + 2] == 'l' &&
					json[index + 3] == 's' &&
					json[index + 4] == 'e') {
					index += 5;
					return Token.BooleanFalse;
				}
			}

			// true
			if (remainingLength >= 4) {
				if (json[index] == 't' &&
					json[index + 1] == 'r' &&
					json[index + 2] == 'u' &&
					json[index + 3] == 'e') {
					index += 4;
					return Token.BooleanTrue;
				}
			}

			// null
			if (remainingLength >= 4) {
				if (json[index] == 'n' &&
					json[index + 1] == 'u' &&
					json[index + 2] == 'l' &&
					json[index + 3] == 'l') {
					index += 4;
					return Token.NullType;
				}
			}

			return Token.None;
		}
	}
}
