using System.IO;
using UnityEngine;
using UnityEditor.Build;

namespace LowEngine
{
    using System.Linq;
    using UnityEditor.Build.Reporting;

    public class OnBuildRuntime : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPostprocessBuild(BuildReport report)
        {
            var item = report.files[0].path; // Get the first created file.
            var folderLoc = FindDataPath(item); // Get the folderpath

            if (folderLoc.ToCharArray().Length <= 2) return; // Path invalid do not continue

            //Create a version code
            var version = $"version:{Application.version}";

            File.Create(folderLoc + "/version.txt").Dispose();
            File.WriteAllText(folderLoc + "/version.txt", version);

            //Create a local data folder
            Directory.CreateDirectory(folderLoc + "/LocalData");

            //Export Mods
            var copyToDir = folderLoc + "/LocalData/";

            CopyFiles($"{Application.dataPath}/LocalData/", copyToDir);
        }

        private static void CopyFiles(string fromDir, string toDir)
        {
            Directory.CreateDirectory(toDir);

            foreach (var dir in Directory.EnumerateDirectories(fromDir))
            {
                var dirNames = dir.Split('\\');
                if (dirNames == null || dirNames.Length == 0) dirNames = dir.Split('/');

                string dirName = "";

                for (int i = 0; i < dirNames.Length; i++)
                    if (dirNames[i].Length > 2) dirName = dirNames[i];

                toDir = $"{toDir}\\{dirName}\\";

                foreach (var mod in Directory.EnumerateFiles(dir))
                {
                    if (mod.Contains(".meta")) continue;

                    string modName = mod.Split('\\').LastOrDefault();
                    File.Copy(mod, $"{toDir}\\{modName}");
                }

                // Try to get all the child folders too
                CopyFiles(dir, toDir);
            }
        }

        private string FindDataPath(string dataPath)
        {
            var toReturn = "";

            int iterations = 0;
            while (true)
            {
                if (iterations > 10) break;// Don't excede 10 attempts so we don't wait forever

                var split = dataPath.Split('/');

                for (int i = 0; i < split.Length; i++)
                {
                    var item = split[i];

                    toReturn += i > 0 ? "/" + item : item;

                    if (item.Contains("_Data"))
                        break;
                }

                if (toReturn.Contains("_Data"))
                    break;

                iterations++;
            }

            return toReturn;
        }
    }
}