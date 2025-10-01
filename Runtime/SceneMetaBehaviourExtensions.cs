using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneMeta
{
    /// <summary>
    /// Extension methods for <see cref="SceneMetaBehaviour"/>
    /// </summary>
    public static class SceneMetaBehaviourExtensions
    {
        public static IEnumerable<T> GetComponents<T>(this Scene scene) where T : class
        {
            if (SceneMetaBehaviour.TryGetMetaBehaviour(scene, out var behaviour) == false)
                yield break;

            foreach (var behaviourObject in behaviour.GetSceneComponentsOfType<T>())
                yield return behaviourObject;
        }

        public static bool TryGetComponent<T>(this Scene scene, out T result)
            where T : class
        {
            if (SceneMetaBehaviour.TryGetMetaBehaviour(scene, out var behaviour) == false)
            {
                result = null;
                return false;
            }

            return behaviour.TryGetSceneComponent(out result);
        }
    }

}