using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace FolderColor
{
    public class ProjectAssetViewerCustomisation
    {
        [System.Serializable]
        public class AssetModificationData
        {
            public List<string> assetModified = new List<string>();
            public List<string> assetModifiedTexturePath = new List<string>();
        }

        // Reference to the data
        public static AssetModificationData modificationData = new();

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            LoadData();

            //for each object
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Ensure assetType is not null before accessing it
            if (modificationData.assetModified != null && modificationData.assetModified.Contains(assetPath))
            {
                int t = modificationData.assetModified.IndexOf(assetPath);

                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(modificationData.assetModifiedTexturePath[t], typeof(Texture2D));

                if (tex == null)
                {
                    modificationData.assetModified.RemoveAt(t);
                    modificationData.assetModifiedTexturePath.RemoveAt(t);
                    SaveData();
                    return;
                }

                if (selectionRect.height == 16) GUI.DrawTexture(new Rect(selectionRect.x + 1.5f, selectionRect.y, selectionRect.height, selectionRect.height), tex);
                else GUI.DrawTexture(new Rect(selectionRect.x, selectionRect.y, selectionRect.height - 10, selectionRect.height - 10), tex);
            }
        }

        // Add a menu item in the Unity Editor to open the custom modification window
        [MenuItem("Assets/Custom Folder", false, 100)]
        private static void CustomModificationMenuItem()
        {
            // Get the selected asset path
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            CustomWindowFileImage.ShowWindow(assetPath);
        }

        // Validate function to enable/disable the menu item
        [MenuItem("Assets/Custom Folder", true)]
        private static bool ValidateCustomModificationMenuItem()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            return AssetDatabase.IsValidFolder(assetPath);
        }

        public static void SaveData()
        {
            // Create or update the modificationData
            if (modificationData == null)
            {
                modificationData = new AssetModificationData();
            }

            // Convert to JSON
            string jsonData = JsonUtility.ToJson(modificationData);

            // Save to PlayerPrefs or a file
            File.WriteAllText("Assets/FolderColor/SaveSetUp/FolderModificationData.json", jsonData);
        }

        private static void LoadData()
        {
            // Load data from PlayerPrefs or a file
            string filePath = "Assets/FolderColor/SaveSetUp/FolderModificationData.json";

            if (File.Exists(filePath))
            {
                // Read the JSON data
                string jsonData = File.ReadAllText(filePath);

                // Deserialize into modificationData
                modificationData = JsonUtility.FromJson<AssetModificationData>(jsonData);
            }
        }
    }
}