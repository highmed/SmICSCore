using System;
using System.Diagnostics;
using System.IO;

namespace SmICSCoreLib.ScriptService
{
    public class ExternalProcess
    {
        public static string Execute(string filePath, string execPath, string args)
        {
            string file = filePath;
            string result = string.Empty;

            try
            {
                var info = new ProcessStartInfo();
                info.FileName = execPath;
                info.WorkingDirectory = Path.GetDirectoryName(filePath);
                info.Arguments = Path.GetFileName(filePath) + " " + args;

                info.RedirectStandardInput = false;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    result = proc.StandardOutput.ReadToEnd();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Script failed: " + result, ex);
            }
        }
       
    }
}
