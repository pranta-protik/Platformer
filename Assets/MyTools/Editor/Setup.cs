using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace MyTools
{
    public static class Setup
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts/UI", "Settings");
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/Setup/Install Required Packages")]
        public static void InstallRequiredPackages()
        {
            Packages.InstallPackages(new[]
            {
                "com.unity.ai.navigation"
            });
        }
        
        [MenuItem("Tools/Setup/Install My Favourite Open Source")]
        public static void InstallOpenSource()
        {
            Packages.InstallPackages(new[]
            {
                "git+https://github.com/KyleBanks/scene-ref-attribute"
            });
        }

        [MenuItem("Tools/Setup/Import My Favourite Assets")]
        public static void ImportMyFavouriteAssets()
        {
            Assets.ImportAsset("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation");
            Assets.ImportAsset("Cartoon FX Remaster Free.unitypackage", "Jean Moreno/Particle Systems");
            Assets.ImportAsset("FREE Casual Game SFX Pack.unitypackage", "Dustyroom/AudioSound FX");
            Assets.ImportAsset("Joystick Pack.unitypackage", "Fenerax Studios/ScriptingInput - Output");
            Assets.ImportAsset("Folder In Hierarchy.unitypackage", "The AAA/Editor ExtensionsUtilities");
            Assets.ImportAsset("Selection History.unitypackage", "Staggart Creations/Editor ExtensionsUtilities");
            Assets.ImportAsset("Free Stylized Skybox.unitypackage", "Yuki2022/Textures MaterialsSkies");
            Assets.ImportAsset("Jammo Character Mix and Jam.unitypackage", "Mix and Jam/3D ModelsCharacters");
            Assets.ImportAsset("POLYGON Prototype - Low Poly 3D Art by Synty.unitypackage", "Synty Studios/3D ModelsPropsExterior");
        }
        
        private static class Folders
        {
            public static void CreateDefault(string root, params string[] folders)
            {
                var fullPath = Path.Combine(Application.dataPath, root);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                
                foreach (var folder in folders)
                {
                    CreateSubFolders(fullPath, folder);
                }
            }

            private static void CreateSubFolders(string rootPath, string folderHierarchy)
            {
                var folders = folderHierarchy.Split('/');
                var currentPath = rootPath;
                foreach (var folder in folders)
                {
                    currentPath = Path.Combine(currentPath, folder);
                    if (!Directory.Exists(currentPath))
                    {
                        Directory.CreateDirectory(currentPath);
                    }
                }
            }
        }
        
        private static class Packages
        {
            private static AddRequest Request;
            private static Queue<string> PackagesToInstall = new();

            public static void InstallPackages(string[] packages)
            {
                foreach (var package in packages)
                {
                    PackagesToInstall.Enqueue(package);
                }
                
                // Start the installation of the first package
                if (PackagesToInstall.Count > 0)
                {
                    Request = Client.Add(PackagesToInstall.Dequeue());
                    EditorApplication.update += Progress;
                }
            }

            private static async void Progress()
            {
                if (Request.IsCompleted)
                {
                    if (Request.Status == StatusCode.Success)
                    {
                        Debug.Log("Installed: " + Request.Result.packageId);
                    }
                    else if (Request.Status >= StatusCode.Failure)
                    {
                        Debug.Log(Request.Error.message);
                    }

                    EditorApplication.update -= Progress;
                    
                    // If there are more packages to install, start the next one
                    if (PackagesToInstall.Count >0)
                    {
                        // Add delay before next package install
                        await Task.Delay(1000);
                        Request = Client.Add(PackagesToInstall.Dequeue());
                        EditorApplication.update += Progress;
                    }
                }
            }

        }

        private static class Assets
        {
            public static void ImportAsset(string asset, string subfolder, string rootFolder = "C:/Users/Pranta/AppData/Roaming/Unity/Asset Store-5.x")
            {
                AssetDatabase.ImportPackage(Path.Combine(rootFolder, subfolder, asset), false);
            }
        }
    }
}