using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;

namespace Lephone.MSBuild
{
    [RunInstaller(true)]
    public partial class DbEntryInstaller : Installer
    {
        public DbEntryInstaller()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            var path = Path.GetDirectoryName(typeof(DbEntryInstaller).Assembly.Location);
            base.OnAfterInstall(savedState);
            Environment.SetEnvironmentVariable("DbEntryPath", path, EnvironmentVariableTarget.Machine);

            var fn = Path.Combine(path, "DbEntryClassLibrary.vsix");
            var psi = new ProcessStartInfo { FileName = fn, Verb = "Open" };
            Process.Start(psi);
        }

        protected override void OnAfterRollback(IDictionary savedState)
        {
            base.OnAfterRollback(savedState);
            Environment.SetEnvironmentVariable("DbEntryPath", null, EnvironmentVariableTarget.Machine);
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);
            Environment.SetEnvironmentVariable("DbEntryPath", null, EnvironmentVariableTarget.Machine);
        }
    }
}
