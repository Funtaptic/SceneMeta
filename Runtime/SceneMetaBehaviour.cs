using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneMeta
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class SceneMetaBehaviour : MonoBehaviour
    {
        [SerializeReference]
        private List<SceneComponent> _components = new List<SceneComponent>();

        public IReadOnlyList<SceneComponent> Components => _components;

        private static List<int> _instances = new List<int>();

        private void Awake()
        {
            _instances.Add(GetInstanceID());
        }

        private void OnDestroy()
        {
            _instances.Remove(GetInstanceID());
        }

        public IEnumerable<T> GetSceneComponentsOfType<T>() where T : class
        {
            if (_components == null)
                yield break;

            foreach (var behaviourObject in _components)
            {
                if (behaviourObject is T casted)
                    yield return casted;
            }
        }

        public bool TryGetSceneComponent<T>(out T result)
            where T : class
        {
            if (_components == null)
            {
                result = null;
                return false;
            }

            foreach (var behaviourObject in _components)
            {
                if (behaviourObject is not T casted)
                    continue;

                result = casted;
                return true;
            }

            result = null;
            return false;
        }

        public static bool TryGetMetaBehaviour(Scene scene, out SceneMetaBehaviour behaviour)
        {
            foreach (var comp in _instances)
            {
                behaviour = Resources.InstanceIDToObject(comp) as SceneMetaBehaviour;
                if (behaviour == null)
                    continue;

                if (behaviour.gameObject.scene == scene)
                    return true;
            }

            behaviour = default;
            return false;
        }

#if UNITY_EDITOR
        public static void SetComponents(Scene scene, [CanBeNull] IEnumerable<SceneComponent> objects)
        {
            SceneMetaBehaviour existing;

            if (objects == null)
            {
                if (TryGetMetaBehaviour(scene, out existing))
                    DestroyImmediate(existing.gameObject);

                return;
            }


            if (TryGetMetaBehaviour(scene, out existing) == false)
            {
                var go = new GameObject();
                go.hideFlags = HideFlags.NotEditable;
                go.transform.hideFlags = HideFlags.HideInInspector;
                SceneManager.MoveGameObjectToScene(go, scene);
                var behaviour = go.AddComponent<SceneMetaBehaviour>();
                behaviour.gameObject.name = nameof(SceneMetaBehaviour);
                existing = behaviour;
            }

            existing._components = new List<SceneComponent>(objects);
        }
#endif
    }
}