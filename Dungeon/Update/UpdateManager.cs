﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Dungeon.Update
{
    public class UpdateManager
    {
        public static string LastVersion()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    return wc.DownloadString("http://213.226.127.77/version");
                }
            }
            catch
            {
                return default;
            }
        }

        public static bool CheckUpdate(string currentVersion, out string lastVersion)
        {
            lastVersion = LastVersion();
            var next = Semver.SemVersion.Parse(lastVersion);
            var now = Semver.SemVersion.Parse(currentVersion);

            return next > now;
        }

        public static string Notes(string platform, string version)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    return wc.DownloadString($"http://213.226.127.77/notes?platform={platform}&version={version}");
                }
            }
            catch
            {
                return default;
            }
        }

        public static void Download(string platform, string version, Action<int> updatedPercentage)
        {
            try
            {
                using var wc = new WebClient();
                wc.DownloadProgressChanged += (_, e) =>
                {
                    updatedPercentage?.Invoke(e.ProgressPercentage);
                };

                if (!Directory.Exists("patch"))
                {
                    Directory.CreateDirectory("patch");
                }

                wc.DownloadFileAsync(new System.Uri($"http://213.226.127.77/update?platform={platform}&version={version}"), $"patch\\{version}.zip");
            }
            catch
            {
            }
        }

        public static void Update(string version)
        {
            var updater = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dungeon.Updater.exe");
            var versionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Patch", $"{version}.zip");

            using (var process = new Process())
            {
                process.StartInfo.FileName = updater; // relative path. absolute path works too.
                process.StartInfo.Arguments = $"unpack {versionPath}";

                process.StartInfo.UseShellExecute = false;
               
                process.Start();
                DungeonGlobal.Exit();
            }
        }
    }
}