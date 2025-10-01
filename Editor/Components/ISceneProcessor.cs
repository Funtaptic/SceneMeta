using SceneMeta.Editor;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace SceneMeta
{
    /// <summary>
    /// Create a <see cref="SceneComponent"/> and implement this class to get scene process events for a certain scene
    /// </summary>
    public interface ISceneProcessor
    {
        void OnProcessScene(Scene scene, BuildReport report);
    }

    public class ProcessSceneEvent : UnityEditor.Build.IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (SceneMetaData.TryGetData(scene.path, out var data) == false)
            {
                SceneMetaBehaviour.SetComponents(scene, null);
                return;
            }

            foreach (var comp in data.Components)
            {
                if (comp is ISceneProcessor processorComponent)
                    processorComponent.OnProcessScene(scene, report);
            }
        }
    }
}