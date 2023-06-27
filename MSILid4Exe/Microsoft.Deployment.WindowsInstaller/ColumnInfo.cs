using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Deployment.WindowsInstaller
{
	public class ColumnInfo
	{
		private string name;

		private Type type;

		private int size;

		private bool isRequired;

		private bool isTemporary;

		private bool isLocalizable;

		public string Name => name;

		public Type Type => type;

		public int DBType
		{
			get
			{
				if (type == typeof(short))
				{
					return 10;
				}
				if (type == typeof(int))
				{
					return 11;
				}
				if (type == typeof(Stream))
				{
					return 1;
				}
				return 16;
			}
		}

		public int Size => size;

		public bool IsRequired => isRequired;

		public bool IsTemporary => isTemporary;

		public bool IsLocalizable => isLocalizable;

		public string SqlCreateString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("`{0}` ", name);
				if (type == typeof(short))
				{
					stringBuilder.Append("SHORT");
				}
				else if (type == typeof(int))
				{
					stringBuilder.Append("LONG");
				}
				else if (type == typeof(string))
				{
					stringBuilder.AppendFormat("CHAR({0})", size);
				}
				else
				{
					stringBuilder.Append("OBJECT");
				}
				if (isRequired)
				{
					stringBuilder.Append(" NOT NULL");
				}
				if (isTemporary)
				{
					stringBuilder.Append(" TEMPORARY");
				}
				if (isLocalizable)
				{
					stringBuilder.Append(" LOCALIZABLE");
				}
				return stringBuilder.ToString();
			}
		}

		public string ColumnDefinitionString
		{
			get
			{
				char c = ((type == typeof(short) || type == typeof(int)) ? (isTemporary ? 'j' : 'i') : ((type != typeof(string)) ? (isTemporary ? 'O' : 'v') : (isTemporary ? 'g' : (isLocalizable ? 'l' : 's'))));
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}", isRequired ? c : char.ToUpper(c, CultureInfo.InvariantCulture), size);
			}
		}

		public ColumnInfo(string name, string columnDefinition)
			: this(name, typeof(string), 0, isRequired: false, isTemporary: false, isLocalizable: false)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (columnDefinition == null)
			{
				throw new ArgumentNullException("columnDefinition");
			}
			switch (char.ToLower(columnDefinition[0], CultureInfo.InvariantCulture))
			{
			case 'i':
				type = typeof(int);
				break;
			case 'j':
				type = typeof(int);
				isTemporary = true;
				break;
			case 'g':
				type = typeof(string);
				isTemporary = true;
				break;
			case 'l':
				type = typeof(string);
				isLocalizable = true;
				break;
			case 'o':
				type = typeof(Stream);
				isTemporary = true;
				break;
			case 's':
				type = typeof(string);
				break;
			case 'v':
				type = typeof(Stream);
				break;
			default:
				throw new InstallerException();
			}
			isRequired = char.IsLower(columnDefinition[0]);
			size = int.Parse(columnDefinition.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
			if (type == typeof(int) && size <= 2)
			{
				type = typeof(short);
			}
		}

		public ColumnInfo(string name, Type type, int size, bool isRequired)
			: this(name, type, size, isRequired, isTemporary: false, isLocalizable: false)
		{
		}

		public ColumnInfo(string name, Type type, int size, bool isRequired, bool isTemporary, bool isLocalizable)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (type == typeof(int))
			{
				size = 4;
				isLocalizable = false;
			}
			else if (type == typeof(short))
			{
				size = 2;
				isLocalizable = false;
			}
			else if (type != typeof(string))
			{
				if (type != typeof(Stream))
				{
					throw new ArgumentOutOfRangeException("type");
				}
				isLocalizable = false;
			}
			this.name = name;
			this.type = type;
			this.size = size;
			this.isRequired = isRequired;
			this.isTemporary = isTemporary;
			this.isLocalizable = isLocalizable;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
