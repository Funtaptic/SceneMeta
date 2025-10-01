using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneMeta.Editor
{
    public class RuntimeSceneMetaProcessor : IProcessSceneWithReport
    {
        public int callbackOrder => -1000;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (SceneMetaData.TryGetData(scene.path, out var data) == false)
            {
                SceneMetaBehaviour.SetComponents(scene, null);
                return;
            }

            List<SceneComponent> components = null;

            foreach (var asset in data.Components)
            {
                if (asset == null)
                    continue;

                if ((asset.hideFlags & HideFlags.DontSaveInBuild) == HideFlags.DontSaveInBuild)
                    continue;

                components ??= new List<SceneComponent>();

                var copy = Object.Instantiate(asset);
                components.Add(copy);
            }

            SceneMetaBehaviour.SetComponents(scene, components);
        }
    }
}