using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Deployment.WindowsInstaller
{
	public sealed class CustomActionData : IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		public const string PropertyName = "CustomActionData";

		private const char DataSeparator = ';';

		private const char KeyValueSeparator = '=';

		private IDictionary<string, string> data;

		public ICollection<string> Keys => data.Keys;

		public ICollection<string> Values => data.Values;

		public string this[string key]
		{
			get
			{
				return data[key];
			}
			set
			{
				ValidateKey(key);
				data[key] = value;
			}
		}

		public int Count => data.Count;

		public bool IsReadOnly => false;

		public CustomActionData()
			: this(null)
		{
		}

		public CustomActionData(string keyValueList)
		{
			data = new Dictionary<string, string>();
			if (keyValueList != null)
			{
				Parse(keyValueList);
			}
		}

		public void Add(string key, string value)
		{
			ValidateKey(key);
			data.Add(key, value);
		}

		public void AddObject<T>(string key, T value)
		{
			if (value == null)
			{
				Add(key, null);
				return;
			}
			if (typeof(T) == typeof(string) || typeof(T) == typeof(CustomActionData))
			{
				Add(key, value.ToString());
				return;
			}
			string value2 = Serialize(value);
			Add(key, value2);
		}

		public T GetObject<T>(string key)
		{
			string text = this[key];
			if (text == null)
			{
				return default(T);
			}
			if (typeof(T) == typeof(string))
			{
				return (T)(object)text;
			}
			if (typeof(T) == typeof(CustomActionData))
			{
				return (T)(object)new CustomActionData(text);
			}
			if (text.Length == 0)
			{
				return default(T);
			}
			return Deserialize<T>(text);
		}

		public bool ContainsKey(string key)
		{
			return data.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			return data.Remove(key);
		}

		public bool TryGetValue(string key, out string value)
		{
			return data.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<string, string> item)
		{
			ValidateKey(item.Key);
			data.Add(item);
		}

		public void Clear()
		{
			if (data.Count > 0)
			{
				data.Clear();
			}
		}

		public bool Contains(KeyValuePair<string, string> item)
		{
			return data.Contains(item);
		}

		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			data.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, string> item)
		{
			return data.Remove(item);
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)data).GetEnumerator();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> datum in data)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(datum.Key);
				if (datum.Value != null)
				{
					stringBuilder.Append('=');
					stringBuilder.Append(Escape(datum.Value));
				}
			}
			return stringBuilder.ToString();
		}

		private static void ValidateKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			checked
			{
				for (int i = 0; i < key.Length; i++)
				{
					char c = key[i];
					if (!char.IsLetterOrDigit(c) && c != '_' && c != '.' && (i <= 0 || i >= key.Length - 1 || c != ' '))
					{
						throw new ArgumentOutOfRangeException("key");
					}
				}
			}
		}

		private static string Serialize<T>(T value)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = true;
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
			{
				XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
				xmlSerializerNamespaces.Add(string.Empty, string.Empty);
				new XmlSerializer(typeof(T)).Serialize(xmlWriter, value, xmlSerializerNamespaces);
				return stringWriter.ToString();
			}
		}

		private static T Deserialize<T>(string value)
		{
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(value)))
			{
				return (T)new XmlSerializer(typeof(T)).Deserialize(xmlReader);
			}
		}

		private static string Escape(string value)
		{
			value = value.Replace(string.Empty + ";", string.Empty + ";;");
			return value;
		}

		private static string Unescape(string value)
		{
			value = value.Replace(string.Empty + ";;", string.Empty + ";");
			return value;
		}

		private void Parse(string keyValueList)
		{
			int num = 0;
			checked
			{
				while (num < keyValueList.Length)
				{
					int num2 = num - 2;
					do
					{
						num2 = keyValueList.IndexOf(';', num2 + 2);
					}
					while (num2 >= 0 && num2 < keyValueList.Length - 1 && keyValueList[num2 + 1] == ';');
					if (num2 < 0)
					{
						num2 = keyValueList.Length;
					}
					int num3 = num - 2;
					do
					{
						num3 = keyValueList.IndexOf('=', num3 + 2);
					}
					while (num3 >= 0 && num3 < keyValueList.Length - 1 && keyValueList[num3 + 1] == '=');
					if (num3 < 0 || num3 > num2)
					{
						num3 = num2;
					}
					string text = keyValueList.Substring(num, num3 - num);
					string value = null;
					if (num3 < num2)
					{
						value = keyValueList.Substring(num3 + 1, num2 - (num3 + 1));
						value = Unescape(value);
					}
					if (text.Length > 0 && !data.ContainsKey(text))
					{
						data.Add(text, value);
					}
					num = num2 + 1;
				}
			}
		}
	}
}
