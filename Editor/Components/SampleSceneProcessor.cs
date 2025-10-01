using System;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneMeta
{
    public class SampleSceneProcessor : SceneComponent, ISceneProcessor
    {
        private void Awake()
        {
            hideFlags = HideFlags.DontSaveInBuild;
        }

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            Debug.Log("Processing Scene");
        }
    }
}