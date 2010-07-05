using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Lephone.MSBuild
{
    public class ProcessorTask : Task
    {
        [Required]
        public string TargetPath { get; set; }

        [Required]
        public string TargetDir { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string ProjectDir { get; set; }

        [Required]
        public string SolutionDir { get; set; }

        public string ProcessorPath { get; set; }

        public override bool Execute()
        {
            TargetPath = TargetPath.Replace("\\bin\\", "\\obj\\");
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
            var fn = TargetPath.Substring(0, TargetPath.Length - 4) + ".bak";
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
            var result = GetArgument(TargetPath);
            if(!string.IsNullOrEmpty(keyPath))
            {
                result += " " + GetArgument(keyPath);
            }
            return result;
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
            var xml = new XmlDocument();
            xml.Load(ProjectPath);
            var keyElements = xml.GetElementsByTagName("AssemblyOriginatorKeyFile");
            if (keyElements.Count > 0 && !string.IsNullOrEmpty(keyElements[0].InnerText))
            {
                return Path.Combine(ProjectDir, keyElements[0].InnerText);
            }
            return null;
        }
    }
}
