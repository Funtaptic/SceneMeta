using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SceneMeta.Editor
{
    [CreateAssetMenu(fileName = "SceneMetaData", menuName = "Scene/MetaData")]
    public class SceneMetaData : ScriptableObject
    {
        public List<SceneComponent> Components = new List<SceneComponent>();
        
        public static bool TryGetData(SceneAsset sceneAsset, out SceneMetaData metaData)
        {
            if (sceneAsset == null)
            {
                metaData = default;
                return false;
            }

            var targetPath = AssetDatabase.GetAssetPath(sceneAsset);
            return TryGetData(targetPath, out metaData);
        }

        public static bool TryGetData(string path, out SceneMetaData metaData)
        {
            if (string.IsNullOrEmpty(path))
            {
                metaData = default;
                return false;
            }

            var importer = AssetImporter.GetAtPath(path);

            if (importer == null)
            {
                metaData = default;
                return false;
            }

            var userData = importer.userData;

            if (GlobalObjectId.TryParse(userData, out var id))
            {
                var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
                if (obj is SceneMetaData sceneMetaData)
                {
                    metaData = sceneMetaData;
                    return true;
                }
            }

            metaData = default;
            return false;
        }

        public static void SetData(SceneAsset sceneAsset, SceneMetaData data)
        {
            var targetPath = AssetDatabase.GetAssetPath(sceneAsset);
            SetData(targetPath, data);
        }

        public static void SetData(string targetPath, SceneMetaData data)
        {
            var importer = AssetImporter.GetAtPath(targetPath);
            var id = GlobalObjectId.GetGlobalObjectIdSlow(data);

            var importerUserData = data == null ? null : id.ToString();

            if (importerUserData != importer.userData)
            {
                importer.userData = importerUserData;
                importer.SaveAndReimport();
            }
        }
    }
}