//
// C# 
// MSILid4Exe Config
// v 0.3, 24.06.2023
// https://github.com/dkxce
// en,ru,1251,utf-8
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MSILid4Exe
{
    public class Config: XMLSaved<Config>
    {
        public class Property
        {
            #region XML

            [XmlAttribute("name")]
            public string name;

            [XmlText]
            public string value;

            #endregion XML

            #region Constructor
            public Property() { }

            public Property(string name) { this.name = name; }

            public Property(string name, string value) { this.name = name; this.value = value; }

            #endregion Constructor

            #region Override
            public override string ToString() { return String.Format("{0,-20} = {1}", name, value); }
            #endregion Override
        }

        #region Private
        private string fileName;
        private string regex;
        #endregion Private

        #region Public (XML)

        [XmlElement("MODE")]
        public byte MODE = 1; // 0 - MSIDatabaseFile+MSIQDatabaseFile (faster, update existing records only); 1 - MSIQDatabaseFile Only (slower, can create new records)

        [XmlElement("FileName")]
        public string FileName
        {
            get { return fileName; }
            set { 
                string val = value.Replace("%CD%", XMLSaved<int>.CurrentDirectory()).Replace("\\\\", "\\");
                fileName = Environment.ExpandEnvironmentVariables(val).Replace("\\\\", "\\");
            }
        }

        [XmlElement("FileRegex")]
        public string FileRegex { get { return regex; } set { regex = value; } }

        [XmlArray("Properties")]
        public List<Property> Properties = new List<Property>();
        
        [XmlArray("SummaryInfo")]
        public List<Property> SummaryInfo = new List<Property>();

        [XmlElement("OnDone")]
        public string OnDone = null;

        [XmlElement("SelectFile")]
        public bool SelectFile = true;

        [XmlElement("CmdArgs")]
        public string CmdArgs = null;

        #endregion Public (XML)

        #region XmlIgnore

        [XmlIgnore]
        public string CfgFilePath
        {
            get {
                string fn = Path.GetFileName(fileName);
                int iof = fileName.LastIndexOf(fn);
                return fileName.Substring(0, iof);
            }
        }

        [XmlIgnore]
        public string CfgFileMask { get { return Path.GetFileName(fileName); } }        

        [XmlIgnore]
        public byte[] MsiTemplate { get { return global::MSILid4Exe.Properties.Resources.template; } }

        #endregion XmlIgnore

        #region Methods

        public List<string> GetFileNames()
        {
            List<string> res = new List<string>();
            foreach (string f in Directory.GetFiles(CfgFilePath, CfgFileMask, SearchOption.TopDirectoryOnly))
            {
                if (string.IsNullOrEmpty(regex))
                    res.Add(f);
                else
                    try
                    {
                        Regex rx = new Regex(regex, RegexOptions.IgnoreCase);
                        Match mx = rx.Match(Path.GetFileName(f));
                        if (mx.Success) res.Add(f);
                    }
                    catch { };
            };
            return res;
        }

        public string GetFileName()
        {
            foreach(string f in Directory.GetFiles(CfgFilePath, CfgFileMask, SearchOption.TopDirectoryOnly))
            {
                if (string.IsNullOrEmpty(regex)) return f;
                try
                {
                    Regex rx = new Regex(regex, RegexOptions.IgnoreCase);
                    Match mx = rx.Match(Path.GetFileName(f));
                    if (mx.Success) return f;
                }
                catch { };
            };
            return null;
        }

        public Dictionary<string, string> GetFileParts(string filename)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                Regex rx = new Regex(regex, RegexOptions.IgnoreCase);
                Match mx = rx.Match(Path.GetFileName(filename));
                foreach (Group g in mx.Groups)
                    res.Add(g.Name, g.Value);
            }
            catch { };
            return res;
        }

        public Dictionary<string, string> GetFileParams(string filename)
        {
            Dictionary<string, string> replaceFVI = GetReplaceFVI(filename);
            Dictionary<string, string> res = new Dictionary<string, string>();
            string path = Path.GetDirectoryName(filename);
            string file = Path.GetFileName(filename);

            foreach(Property p in Properties)
            {
                string v = p.value;
                v = v.Replace("%CD%", XMLSaved<int>.CurrentDirectory());
                foreach (KeyValuePair<string, string> kvp in replaceFVI)
                    v = v.Replace($"${kvp.Key}$", kvp.Value);
                foreach (KeyValuePair<string,string> kvp in GetFileParts(file))
                    v = v.Replace($"%{kvp.Key}%", kvp.Value);
                res.Add(p.name, v.Trim());
            };

            return res;
        }

        public Dictionary<string, string> GetFileSummary(string filename)
        {
            Dictionary<string, string> replaceFVI = GetReplaceFVI(filename);
            Dictionary<string, string> res = new Dictionary<string, string>();
            string path = Path.GetDirectoryName(filename);
            string file = Path.GetFileName(filename);

            foreach (Property p in SummaryInfo)
            {
                string v = p.value;
                v = v.Replace("%CD%", XMLSaved<int>.CurrentDirectory());
                foreach (KeyValuePair<string, string> kvp in replaceFVI)
                    v = v.Replace($"${kvp.Key}$", kvp.Value);
                foreach (KeyValuePair<string, string> kvp in GetFileParts(file))
                    v = v.Replace($"%{kvp.Key}%", kvp.Value);
                res.Add(p.name, v.Trim());
            };

            return res;
        }

        private static Dictionary<string, string> GetReplaceFVI(string filename)
        {
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(filename);
            Dictionary<string, string> replaceFVI = new Dictionary<string, string>();
            replaceFVI.Add("FILE_COMPANY", fvi.CompanyName);
            replaceFVI.Add("FILE_DESCRIPTION", fvi.FileDescription);
            replaceFVI.Add("FILE_COMMENT", fvi.Comments);
            replaceFVI.Add("FILE_VERSION", fvi.FileVersion);
            replaceFVI.Add("FILE_LANGUAGE", fvi.Language);
            replaceFVI.Add("FILE_COPYRIGHTS", fvi.LegalCopyright);
            replaceFVI.Add("PRODUCT_NAME", fvi.ProductName);
            replaceFVI.Add("PRODUCT_VERSION", fvi.ProductVersion);
            return replaceFVI;
        }

        #endregion Methods

        #region LoadSave

        public void Save()
        {
            string path = Path.Combine(XMLSaved<int>.CurrentDirectory(), "MSILid4Exe.xml");
            XMLSaved<Config>.Save(path, this);
        }

        public static Config Init(string file = null)
        {
            string path = string.IsNullOrEmpty(file) ? Path.Combine(XMLSaved<int>.CurrentDirectory(), "MSILid4Exe.xml") : file;
            return XMLSaved<Config>.Load(path);
        }

        #endregion LoadSave

        #region Static
        public static string ToReadableSize(double value)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            for (int i = 0; i < suffixes.Length; i++)
            {
                if (value <= (Math.Pow(1024, i + 1)))
                {
                    return ThreeNonZeroDigits(value /
                        Math.Pow(1024, i)) +
                        " " + suffixes[i];
                };
            };

            return ThreeNonZeroDigits(value /
                Math.Pow(1024, suffixes.Length - 1)) +
                " " + suffixes[suffixes.Length - 1];
        }

        private static string ThreeNonZeroDigits(double value)
        {
            if (value >= 100)
            {
                // No digits after the decimal.
                return value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (value >= 10)
            {
                // One digit after the decimal.
                return value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                // Two digits after the decimal.
                return value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        #endregion Static

        public override string ToString()
        {
            return $"{FileName} SI{SummaryInfo?.Count} P{Properties?.Count}";
        }
    }
}

/* SAMPLE 
 
<?xml version="1.0" encoding="utf-8"?>
<Config>  

  <!-- 0 - MSIDatabaseFile+MSIQDatabaseFile (faster, update existing records only); 1 - MSIQDatabaseFile Only (slower, can create new records) -->
  <MODE>1</MODE>  

  <FileName>%CD%\*.exe</FileName>
  <FileRegex>^(?&lt;FileName&gt;(?!MSILid4Exe).*\.exe)$</FileRegex>  
  <CmdArgs>/default</CmdArgs>  
  <SelectFile>1</SelectFile>
  
  <SummaryInfo>
	  <Property name="Author">$FILE_COMPANY$</Property>
	  <Property name="Comments">artem.karimov@weadmire.io</Property>
	  <Property name="Keywords">$PRODUCT_NAME$ Setup Installer</Property>
	  <Property name="LastSavedBy">MSILid4Exe</Property>
	  <Property name="RevisionNumber">{8F41CB95-0000-0000-0000-FD8C1BA7FF03}</Property>
	  <Property name="Subject">$PRODUCT_NAME$</Property>
	  <Property name="Title">$PRODUCT_NAME$ Installer</Property>
  </SummaryInfo>
  
  <Properties>
    <Property name="ARPCONTACT">artem.karimov@weadmire.io</Property>
    <Property name="ARPHELPLINK">artem.karimov@weadmire.io</Property>
    <Property name="ARPURLINFOABOUT">artem.karimov@weadmire.io</Property>
    <Property name="Manufacturer">$FILE_COMPANY$</Property>
    <Property name="ProductName">$PRODUCT_NAME$ Installer</Property>
    <Property name="ProductVersion">$PRODUCT_VERSION$</Property>
    <Property name="ProductCode">{8F41CB95-0000-0000-0000-FD8C1BA7FF01}</Property>
    <Property name="UpgradeCode">{8F41CB95-0000-0000-0000-FD8C1BA7FF02}</Property>
    <Property name="ARPPRODUCTICON">%FileName%.ico</Property>
    <Property name="DISPLAYLANGUAGE">EN</Property>
  </Properties>
  
  <OnDone>notepad.exe MSILid4Exe.log</OnDone>
  <!--OnDone>_MSILid4Exe.cmd</OnDone-->
  <!--OnDone>MSILid4Exe.cmd</OnDone-->
  
  <!-- 
	PROPERTIES REPLACEMENTS:
	
	%CD% - Current Directory
	
	%ENVIRONMENT_VARIABLE% - Windows Environment Variables
	%ALLUSERSPROFILE%
	%APPDATA%
	%CommonProgramFiles%
	%COMMONPROGRAMFILES(x86)%
	%COMPUTERNAME%
	%DATE%
	%HOMEDRIVE%
	%HOMEPATH%
	%LOCALAPPDATA%
	%PROCESSOR_ARCHITECTURE%
	%ProgramData%
	%ProgramFiles%
	%ProgramFiles(x86)%
	%ProgramW6432%
	%Public%
	%RANDOM%
	%SYSTEMDRIVE%
	%SYSTEMROOT%
	%TEMP%
	%TIME%
	%TMP%
	%USERNAME%
	%USERPROFILE%
	%WINDIR%
	
	%ANY_NAME% - FOR REPLACE WITH REGEX (?<ANY_NAME>...)
	%FileName% 
	%FileVersion% 
	
	$FILE_COMPANY$
	$FILE_DESCRIPTION$
	$FILE_COMMENT$
	$FILE_COMMENT$
	$FILE_VERSION$
	$FILE_LANGUAGE$
	$FILE_COPYRIGHTS$
	$PRODUCT_NAME$
	$PRODUCT_VERSION$
  -->
  
</Config>

*/