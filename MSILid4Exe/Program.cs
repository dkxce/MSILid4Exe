//
// C# 
// MSILid4Exe
// v 0.3, 27.06.2023
// https://github.com/dkxce
// en,ru,1251,utf-8
//

// HELP:
//   http://installsite.org/pages/en/msi/tips.htm#7zip

/*
 **More Info**:
- https://learn.microsoft.com/en-us/windows/win32/msi/orca-exe    
- https://en.wikipedia.org/wiki/Windows_Installer
- https://github.com/CodeCavePro/OpenMCDF      
- https://github.com/ironfede/openmcdf
- https://www.exetomsi.com/freeware/     
- https://www.revenera.com/install/products/installshield
- https://en.wikipedia.org/wiki/WiX
- http://www.instedit.com/
- https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-cfb/53989ce4-7b05-4f8d-829b-d08d6148375b
- https://dennisbareis.com/makemsi.htm            
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MSILid4Exe
{
    internal class Program
    {
        private static string cfgFile  = "MSILid4Exe.xml";
        private static Config cfg      = null;
        private static int sleep_error = 2500;
        private static int sleep_ok    = 10;
        private static byte msi_mode   = 0; // 0 - MSIDatabaseFile+MSIQDatabaseFile (faster, update existing records only); 1 - MSIQDatabaseFile Only (slower, can create new records)

        private static string msi_binary_data_const = "_D7D112F049BA1A655B5D9A1D0702DEE5";     // CustomAction.Source WHERE Type EXE
        private static string msi_action_data_const = "_B3D13F97_1369_417D_A477_B4C42B829328"; // CustomAction.Action WHERE Type EXE 

        static void Main(string[] args)
        {                 
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string fileVersion = fvi.FileVersion;
            string description = fvi.Comments;
            string subVersion  = fvi.LegalTrademarks;

            // BEGIN OUTPUT
            {
                WriteNoLog($"{description}");
                WriteNoLog($"  v{fileVersion} {subVersion}");
                WriteNoLog($"  Usage: MSILid4Exe.exe [config_file]");                                
            };            

            // INIT DLLS
            InitDlls();
            
            // LOAD CONFIG
            if (args != null && args.Length > 0) {
                foreach (string arg in args) { try { if (File.Exists(arg)) {
                    cfgFile = Path.GetFileName(arg);
                    cfg = Config.Init(arg);
                }; } catch { }; }; }
            else cfg = Config.Init();

            string mode_text = "UNKNOWN";
            if (cfg.MODE == 0) mode_text = "UPDATE";
            if (cfg.MODE == 1) mode_text = "INSERT";

            // INIT LOG
            ClearLog();
            WriteToLog($";# {description}");
            WriteToLog($";# v{fileVersion} {subVersion}");
            WriteToLog($";# Launched at {DateTime.Now}", false);
            WriteToLog($";# MSI MODE {cfg.MODE} {mode_text}", false);
            WriteToLog();
            
            WriteNoLog($"\r\nMSI MODE : {cfg.MODE} {mode_text}");
            
            if (cfg.MODE > 1)
            {
                WriteWithLog("RESULT: {");
                WriteWithLog("  Status : {0}", "Failed");
                WriteWithLog("  Error  : {0}", $"Wrong MODE {cfg.MODE}");
                WriteWithLog("  Exit   : {0}", 255);
                WriteWithLog("}");
                Exit(255); return;
            };

            // CONFIGURATION
            {
                WriteWithLog("CONFIGURATION: {");                
                WriteWithLog("  File   : {0}", cfgFile);
                WriteWithLog("  Path   : {0}", cfg.CfgFilePath);
                WriteWithLog("  Mask   : {0}", cfg.CfgFileMask);
                WriteWithLog("  Regex  : {0}", cfg.FileRegex);
                WriteWithLog("  OnDone : {0}", cfg.OnDone);
                WriteWithLog("  Select : {0}", cfg.SelectFile);
                WriteWithLog("  SummaryInfo : {0}", cfg.SummaryInfo.Count);
                foreach (Config.Property p in cfg.SummaryInfo)
                    WriteWithLog("    {0}", p);
                WriteWithLog("  Properties : {0}", cfg.Properties.Count);
                foreach (Config.Property p in cfg.Properties)
                    WriteWithLog("    {0}", p);                
                WriteWithLog("}");
            };

            // LOAD FILE(S)
            List<string> files = cfg.GetFileNames();
            if (files.Count == 0) // ERROR
            {
                WriteWithLog("RESULT: {");
                WriteWithLog("  Status : {0}", "Failed");
                WriteWithLog("  Error  : {0}", "No any file found");
                WriteWithLog("  Exit   : {0}", 2);
                WriteWithLog("}");
                Exit(2); return;
            };


            string f = files[0];
            if (files.Count > 1 && cfg.SelectFile) // SELECT
            {
                WriteWithLog("FILES: {");
                int fid = 0;
                foreach (string fi in files)
                    WriteWithLog("  {0}: {1}", fid++ ,Path.GetFileName(fi));
                WriteWithLog("}");

                WriteWithLog("SELECT: {");
                Console.WriteLine("  -- TYPE exit OR abort TO EXIT --");                
                int top = Console.CursorTop;
                while (true)
                {
                    Console.Write("  CHOOSE FILE NUMBER: ");
                    string fNo = Console.ReadLine().Trim();
                    
                    Console.CursorTop = top; Console.CursorLeft = 0;
                    for (int i = 0; i < Console.WindowWidth * 2; i++) Console.Write(" ");
                    Console.CursorTop = top;

                    if (fNo == "abort" || fNo == "exit") 
                    {                        
                        WriteWithLog("  NUMBER: {0}", -1);
                        WriteWithLog("  FILE  : {0}", "NONE");
                        WriteWithLog("}");
                        WriteWithLog("RESULT: {");
                        WriteWithLog("  Status : {0}", "Failed");
                        WriteWithLog("  Error  : {0}", "Aborted by User");
                        WriteWithLog("  Exit   : {0}", 1);
                        WriteWithLog("}");
                        Exit(1); return;
                    };
                    if (int.TryParse(fNo, out fid) && fid >= 0 && fid < files.Count) break;                    
                };
                f = files[fid];
                WriteWithLog("  NUMBER: {0}", fid);
                WriteWithLog("  FILE  : {0}", Path.GetFileName(files[fid]));
                WriteWithLog("}");                
            };

            // PRE INITIALIZATION
            Dictionary<string, string> sumin = null;
            Dictionary<string, string> props = null;

            try // INITIALIZATION
            {
                WriteWithLog("INITIALIZATION: {");
                WriteWithLog("  Path   : {0}", Path.GetDirectoryName(f));
                WriteWithLog("  File   : {0}", Path.GetFileName(f));
                WriteWithLog("  Size   : {0}", Config.ToReadableSize((new FileInfo(f)).Length));
                sumin = cfg.GetFileSummary(f);
                WriteWithLog("  SummaryInfo : " + sumin.Count.ToString());
                foreach (KeyValuePair<string, string> p in sumin)
                    WriteWithLog(String.Format("    {0,-20} = {1}", p.Key, p.Value));
                props = cfg.GetFileParams(f);
                WriteWithLog("  Properties : " + props.Count.ToString());
                foreach (KeyValuePair<string, string> p in props)
                    WriteWithLog(String.Format("    {0,-20} = {1}", p.Key, p.Value));
                WriteWithLog("}");
            }
            catch (Exception ex)
            {
                WriteWithLog("}");
                WriteWithLog("RESULT: {");
                WriteWithLog("  Status : {0}", "Failed");
                WriteWithLog("  Error  : {0}", ex.Message);
                WriteWithLog("  Exit   : {0}", 3);
                WriteWithLog("}");
                Exit(3); return;
            };

            // EXTRACT
            string msiFile = Path.Combine(Path.GetDirectoryName(f), Path.GetFileNameWithoutExtension(f) + ".msi");
            try { File.WriteAllBytes(msiFile, cfg.MsiTemplate); }
            catch (Exception ex)
            {
                WriteWithLog("RESULT: {");
                WriteWithLog("  Status : {0}", "Failed");
                WriteWithLog("  Error  : {0}", ex.Message);
                WriteWithLog("  Exit   : {0}", 4);
                WriteWithLog("}");
                Exit(4); return;
            };

            // SET ARGS
            string cmdargs = string.IsNullOrEmpty(cfg.CmdArgs) ? " " : cfg.CmdArgs;

            // PATCH BINARY & PROPERTIES            
            MSIDatabaseFile msi = null;            
            try
            {
                string msi_binary_data_name = msi_binary_data_const; // CustomAction.Source WHERE Type EXE
                string msi_action_data_name = msi_action_data_const; // CustomAction.Action WHERE Type EXE 

                using (MSIQDatabaseFile msisi = new MSIQDatabaseFile(msiFile))
                {
                    msi_binary_data_name = msisi.ExeSourceName;
                    msi_action_data_name = msisi.ExeActionName;
                };

                if (cfg.MODE == 0)// faster
                {
                    msi = new MSIDatabaseFile(msiFile);
                    msi.Open();
                    msi.LoadBinaryDataFromFile(msi_binary_data_name, f);
                    msi.SetProperties(props);
                    msi.SetTableValue("CustomAction", "Action", msi_action_data_name, cmdargs, 4);
                    msi.Close();
                };

                using (MSIQDatabaseFile msisi = new MSIQDatabaseFile(msiFile))
                {
                    if (cfg.MODE == 1) msisi.LoadBinaryDataFromFile(msi_binary_data_name, f);
                    if (cfg.MODE == 1) msisi.SetProperties(props); // or msi.SetProperties(props)
                    if (cfg.MODE == 1) msisi.ExeActionArgs = cmdargs; // or msi.SetTableValue("CustomAction", ...
                    msisi.SetSummaryProperties(sumin);
                };
            }
            catch (Exception ex)
            {
                WriteWithLog("}");
                WriteWithLog("RESULT: {");
                WriteWithLog("  Status : {0}", "Failed");
                WriteWithLog("  Error  : {0}", ex.Message);
                WriteWithLog("  Exit   : {0}", 5);
                WriteWithLog("}");
                Exit(5); return;
            }
            finally { if(msi != null) msi.Close(); };

            // PATCH SUMMARY INFO
            try
            {
                MSIQDatabaseFile msisi = new MSIQDatabaseFile(msiFile);
                msisi.SetSummaryProperties(sumin);
                msisi.Dispose();
            }
            catch (Exception ex)
            {
                WriteWithLog("}");
                WriteWithLog("RESULT: {");
                WriteWithLog("  Status : {0}", "Failed");
                WriteWithLog("  Error  : {0}", ex.Message);
                WriteWithLog("  Exit   : {0}", 6);
                WriteWithLog("}");
                Exit(6); return;
            }
            finally { if (msi != null) msi.Close(); };

            // COMPILATION
            {
                WriteWithLog("COMPILATION: {");
                WriteWithLog("  Path   : {0}", Path.GetDirectoryName(msiFile));
                WriteWithLog("  File   : {0}", Path.GetFileName(msiFile));
                WriteWithLog("  Size   : {0}", Config.ToReadableSize((new FileInfo(msiFile)).Length));
                WriteWithLog("}");
            };

            // RESULT
            {
                WriteWithLog("RESULT: {");
                WriteWithLog("  Status : {0}", "Build succeeded successfully");
                WriteWithLog("  Exit   : {0}", 0);
                WriteWithLog("}");

                try { if (!string.IsNullOrEmpty(cfg.OnDone)) System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(cfg.OnDone, $"\"{msiFile}\"") { UseShellExecute = true }); }
                catch { try { if (!string.IsNullOrEmpty(cfg.OnDone)) System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/C {cfg.OnDone}") { UseShellExecute = true }); } catch { }};

                Exit(0); return;
            };
        }

        private static void InitDlls()
        {
            string CMD = Path.Combine(XMLSaved<int>.CurrentDirectory(), "MSILid4Exe.cmd");
            if (!File.Exists(CMD)) File.WriteAllText(CMD, global::MSILid4Exe.Properties.Resources.CMD);

            string XML = Path.Combine(XMLSaved<int>.CurrentDirectory(), "MSILid4Exe.xml");
            if (!File.Exists(XML)) File.WriteAllText(XML, global::MSILid4Exe.Properties.Resources.XML);
        }
        private static void Exit(int code)
        {
            WriteToLog();
            WriteToLog($";# Ended at {DateTime.Now}", false);
            System.Threading.Thread.Sleep(code == 0 ? sleep_ok : sleep_error);
            Environment.Exit(code);
        }

        private static void ClearLog()
        {
            string logFile = Path.Combine(XMLSaved<int>.CurrentDirectory(), "MSILid4Exe.log");
            try { if (File.Exists(logFile)) File.Delete(logFile); } catch { }
        }

        private static void WriteNoLog(string line = "", params object[] pars)
        {
            line = pars == null || pars.Length == 0 ? line : String.Format(line, pars);
            Console.WriteLine(line);            
        }

        private static void WriteToLog(string line = "", params object[] pars)
        {
            line = pars == null || pars.Length == 0 ? line : String.Format(line, pars);
            string logFile = Path.Combine(XMLSaved<int>.CurrentDirectory(), "MSILid4Exe.log");
            try
            {
                using (FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                    sw.WriteLine(line);
            }
            catch { };
        }

        private static void WriteWithLog(string line = "", params object[] pars)
        {
            line = pars == null || pars.Length == 0 ? line : String.Format(line, pars);
            Console.WriteLine(line);
            string logFile = Path.Combine(XMLSaved<int>.CurrentDirectory(), "MSILid4Exe.log");
            try
            {
                using (FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                    sw.WriteLine(line);
            }
            catch { };
        }
    }
}
