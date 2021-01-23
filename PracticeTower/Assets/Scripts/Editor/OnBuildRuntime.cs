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
            var modsDir = folderLoc + "/LocalData/Mods";

            Directory.CreateDirectory(modsDir);
            foreach (var mod in Directory.EnumerateFiles(Modding.ModLoader.ModsDirectory))
            {
                if (mod.Contains(".meta")) continue;
                string modName = mod.Split('\\').LastOrDefault();

                File.Copy(mod, $"{modsDir}\\{modName}");
            }

            //Export Mod images
            var modImagesDir = folderLoc + "/LocalData/Mods/images";

            Directory.CreateDirectory(modImagesDir);
            foreach (var mod in Directory.EnumerateFiles(Modding.ModLoader.ModImagesDirectory))
            {
                if (mod.Contains(".meta")) continue;
                string modName = mod.Split('\\').LastOrDefault();

                File.Copy(mod, $"{modImagesDir}/{modName}");
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