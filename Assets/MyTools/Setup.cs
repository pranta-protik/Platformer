using UnityEditor;
using UnityEngine;
using static System.IO.Path;
using static System.IO.Directory;
using static UnityEditor.AssetDatabase;

public static class Setup
{
    [MenuItem("Tools/Setup/Create Default Folders")]
    public static void CreateDefaultFolders()
    {
        Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts", "Settings");
        Refresh();
    }

    [MenuItem("Tools/Setup/Import My Favourite Assets")]
    public static void ImportMyFavouriteAssets()
    {
        Assets.ImportAsset("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation");
    }

    private static class Folders
    {
        public static void CreateDefault(string root, params string[] folders)
        {
            var fullPath = Combine(Application.dataPath, root);
            foreach (var folder in folders)
            {
                var path = Combine(fullPath, folder);
                if (!Exists(path))
                {
                    CreateDirectory(path);
                }
            }
        }
    }
    
    private static class Assets
    {
        public static void ImportAsset(string asset, string subfolder, string folder = "C:/Users/Pranta/AppData/Roaming/Unity/Asset Store-5.x")
        {
            ImportPackage(Combine(folder, subfolder, asset), false);
        }
    }
}