using UnityEngine;
using UnityEditor;
using System.IO;

namespace _Game.Editor
{
    public static class AppIconInstaller
    {
        [MenuItem("Tools/TrickyArrow/Install App Icon from Uploads")]
        public static void InstallIcon()
        {
            // Search brain directories for uploaded image
            string userProfile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            string brainUploadsDir = Path.Combine(userProfile, @".gemini\antigravity-ide\brain\bfaf6043-6b50-4be2-909d-a0f9d2dc3215\.user_uploaded");

            string sourceImage = null;
            if (Directory.Exists(brainUploadsDir))
            {
                string[] files = Directory.GetFiles(brainUploadsDir, "media_1788619900544.jpg");
                if (files.Length > 0)
                {
                    sourceImage = files[0];
                }
                else
                {
                    string[] allFiles = Directory.GetFiles(brainUploadsDir);
                    if (allFiles.Length > 0) sourceImage = allFiles[allFiles.Length - 1];
                }
            }

            string targetDir = Path.Combine(Application.dataPath, "_Game", "Sprites");
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            string targetPath = Path.Combine(targetDir, "AppIcon.png");

            if (!string.IsNullOrEmpty(sourceImage) && File.Exists(sourceImage))
            {
                File.Copy(sourceImage, targetPath, true);
                AssetDatabase.Refresh();

                // Configure TextureImporter
                string assetRelativePath = "Assets/_Game/Sprites/AppIcon.png";
                TextureImporter importer = AssetImporter.GetAtPath(assetRelativePath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Default;
                    importer.isReadable = true;
                    importer.npotScale = TextureImporterNPOTScale.None;
                    importer.SaveAndReimport();
                }

                Texture2D iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetRelativePath);
                if (iconTexture != null)
                {
                    PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new Texture2D[] { iconTexture }, IconKind.Any);
                    PlayerSettings.SetIcons(NamedBuildTarget.Android, new Texture2D[] { iconTexture }, IconKind.Any);
                    PlayerSettings.SetIcons(NamedBuildTarget.iOS, new Texture2D[] { iconTexture }, IconKind.Any);
                    PlayerSettings.SetIcons(NamedBuildTarget.Standalone, new Texture2D[] { iconTexture }, IconKind.Any);

                    EditorUtility.DisplayDialog("Tricky Arrow App Icon", "App Icon successfully set for all platforms (Default, Android, iOS, Standalone)!", "OK");
                    Debug.Log("<color=green>[AppIconInstaller]</color> App Icon assigned successfully to PlayerSettings!");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Icon Source Not Found", "Please drag and drop your icon into Assets/_Game/Sprites/AppIcon.png and run this again.", "OK");
            }
        }
    }
}
