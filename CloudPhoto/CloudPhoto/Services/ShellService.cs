using Cloudphoto.Models;
using System.Diagnostics;

namespace Cloudphoto.Services
{
    internal class ShellService
    {
        public static void SetEnvironmentVariable()
        {
            string path =
                Path.GetDirectoryName(
                    Process.GetCurrentProcess().MainModule?.FileName);

            if (path is null)
            {
                return;
            }

            if (OperatingSystem.IsWindows())
            {
                SetEnvironmentVariableForWindows(path);
            }
            else if (OperatingSystem.IsLinux())
            {
                SetEnvironmentVariableForLinux(path);
            }

            Console.Clear();
        }

        private static void SetEnvironmentVariableForWindows(string path)
        {
            InvokeCommand(new CmdData()
            {
                FileName = "cmd",
                Arguments =
                    $"/k setx PATH \"%PATH%;{path}\" && exit"
            });
        }

        private static void SetEnvironmentVariableForLinux(string path)
        {
            string docsPath =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments);
            string bashrc = $"{docsPath}/.bashrc";
            string bashProfile = $"{docsPath}/.bash_profile";

            string envPath = File.Exists(bashrc) ?
                bashrc : File.Exists(bashProfile) ?
                    bashProfile : null;

            if (envPath is null)
            {
                return;
            }

            InvokeCommand(new CmdData()
            {
                FileName = "bash",
                Arguments = string.Concat(
                    "-c \"",
                    $"echo 'export PATH=\"$PATH:{path}\"' >> {envPath}".Replace("\"", "\\\""),
                    "\"")
            });
        }

        private static void InvokeCommand(CmdData cmd)
        {
            var process = Process.Start(
                cmd.FileName,
                cmd.Arguments);

            process?.WaitForExit();
        }
    }
}