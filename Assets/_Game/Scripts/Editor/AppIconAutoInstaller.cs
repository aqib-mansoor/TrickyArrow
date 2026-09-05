using UnityEngine;
using UnityEditor;
using System.IO;

namespace _Game.Editor
{
    [InitializeOnLoad]
    public static class AppIconAutoInstaller
    {
        private const string PrefKey = "TrickyArrow_AppIcon_Installed_V2";

        static AppIconAutoInstaller()
        {
            EditorApplication.delayCall += CheckAndInstallIcon;
        }

        private static void CheckAndInstallIcon()
        {
            if (EditorPrefs.GetBool(PrefKey, false)) return;

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
                try
                {
                    File.Copy(sourceImage, targetPath, true);
                    AssetDatabase.Refresh();

                    string assetRelativePath = "Assets/_Game/Sprites/AppIcon.png";
                    TextureImporter importer = AssetImporter.GetAtPath(assetRelativePath) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.isReadable = true;
                        importer.npotScale = TextureImporterNPOTScale.None;
                        importer.SaveAndReimport();
                    }

                    Texture2D iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetRelativePath);
                    Sprite iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetRelativePath);

                    if (iconTexture != null)
                    {
                        PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new Texture2D[] { iconTexture }, IconKind.Any);
                        PlayerSettings.SetIcons(NamedBuildTarget.Android, new Texture2D[] { iconTexture }, IconKind.Any);
                        PlayerSettings.SetIcons(NamedBuildTarget.iOS, new Texture2D[] { iconTexture }, IconKind.Any);
                        PlayerSettings.SetIcons(NamedBuildTarget.Standalone, new Texture2D[] { iconTexture }, IconKind.Any);

                        // Configure Unity splash screen logo
                        PlayerSettings.SplashScreenLogo splashLogo = PlayerSettings.SplashScreenLogo.Create(2.5f, iconSprite);
                        PlayerSettings.SplashScreen.logos = new PlayerSettings.SplashScreenLogo[] { splashLogo };

                        EditorPrefs.SetBool(PrefKey, true);
                        Debug.Log("<color=green>[TrickyArrow]</color> App Icon & Splash Screen logo successfully configured!");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("[TrickyArrow] Auto-installer exception: " + ex.Message);
                }
            }
        }
    }
}
