using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Lephone.MSBuild
{
    public class ProcessorTask : Task
    {
        public string KeyFile { get; set; }

        [Required]
        public string ProjectDir { get; set; }

        [Required]
        public string AssemblyName { get; set; }

        [Required]
        public string SolutionDir { get; set; }

        public string ProcessorPath { get; set; }

        private string _targetPath;

        [Required]
        public string[] ReferenceFiles { get; set; }

        public override bool Execute()
        {
            _targetPath = Path.Combine(ProjectDir, AssemblyName);
            var info = new ProcessStartInfo(GetProcessorPath())
                           {
                               Arguments = GetArguments(),
                               WorkingDirectory = GetWorkingDirectory(),
                               CreateNoWindow = true,
                               UseShellExecute = false,
                               RedirectStandardInput = true,
                               RedirectStandardOutput = true,
                               RedirectStandardError = true,
                           };
            var process = new Process {StartInfo = info};
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            try
            {
                RemoveBakFile();
            }
            catch(Exception ex)
            {
                Log.LogWarning(ex.ToString());
            }
            if(process.ExitCode == 0)
            {
                return true;
            }
            Log.LogError(result);
            return false;
        }

        private void RemoveBakFile()
        {
            var fn = _targetPath.Substring(0, _targetPath.Length - 4) + ".bak";
            if(File.Exists(fn))
            {
                File.Delete(fn);
            }
        }

        private string GetProcessorPath()
        {
            if (string.IsNullOrEmpty(ProcessorPath))
            {
#if DEBUG
                ProcessorPath = Path.Combine(SolutionDir, @"Lephone.Processor\bin\Debug\Lephone.Processor.exe");
#else
                ProcessorPath = Path.Combine(GetPath(GetType().Assembly.Location), "Lephone.Processor.exe");
                if(!File.Exists(ProcessorPath))
                {
                    ProcessorPath = Path.Combine(SolutionDir, @"..\bin\Lephone.Processor.exe");
                }
#endif
            }
            return ProcessorPath;
        }

        private string GetWorkingDirectory()
        {
            return GetPath(ProcessorPath);
        }

        private static string GetPath(string fullPath)
        {
            return fullPath.Substring(0, fullPath.Length - Path.GetFileName(fullPath).Length);
        }

        private string GetArguments()
        {
            var keyPath = GetKeyPath();
            var result = GetArgument(_targetPath);
            if(!string.IsNullOrEmpty(keyPath))
            {
                result += " " + GetArgument(keyPath);
            }
            result += " " + GetRefFiles();
            return result;
        }

        private string GetRefFiles()
        {
            var sb = new StringBuilder("@\"");
            foreach (var file in ReferenceFiles)
            {
                sb.Append(file).Append(";");
            }
            if(sb.Length > 1)
            {
                sb.Length--;
            }
            sb.Append("\"");
            return sb.ToString();
        }

        private static string GetArgument(string arg)
        {
            if(string.IsNullOrEmpty(arg))
            {
                return null;
            }
            if(arg.IndexOf(" ") > 0)
            {
                return "\"" + arg + "\"";
            }
            return arg;
        }

        private string GetKeyPath()
        {
            if (!string.IsNullOrEmpty(KeyFile))
            {
                return Path.Combine(ProjectDir, KeyFile);
            }
            return null;
        }
    }
}
