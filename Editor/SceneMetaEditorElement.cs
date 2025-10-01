using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneMeta.Editor
{
    public class SceneMetaEditorElement : VisualElement
    {
        private readonly SceneMetaData _metaData;

        private readonly ScrollView _componentsList;

        public SceneMetaEditorElement(SceneMetaData metaData)
        {
            _metaData = metaData;

            var titleLabel = new Label(_metaData.name)
            {
                style =
                {
                    fontSize = 20,
                    paddingLeft = 10,
                    paddingRight = 10,
                    paddingTop = 10,
                    paddingBottom = 0,
                    marginBottom = 0,
                }
            };

            Add(titleLabel);

            _componentsList = new ScrollView()
            {
            };

            var addComponentButton = new Button(AddComponent)
            {
                text = "Add Component",
                style =
                {
                    width = 200,
                    marginTop = 10,
                    marginBottom = 10,
                    alignSelf = Align.Center,
                    paddingTop = 4,
                    paddingBottom = 4,
                }
            };
            
            RebuildComponentsInspectors();

            Add(_componentsList);

            Add(addComponentButton);
        }

        public void RebuildComponentsInspectors()
        {
            _componentsList.Clear();

            var components = _metaData.Components;

            for (var index = 0; index < components.Count; index++)
            {
                var component = components[index];

                if (component == null)
                {
                    var copy = index;
                    
                    var removeButton = new Button(() =>
                    {
                        Undo.RecordObject(_metaData, "Remove Null");
                        components.RemoveAt(copy);
                        EditorUtility.SetDirty(_metaData);
                        RebuildComponentsInspectors();
                    })
                    {
                        text = "NULL",
                    };
                    _componentsList.Add(removeButton);
                    continue;
                }
                
                var element = CreateInspector(component);
                _componentsList.Add(element);
            }
        }

        private VisualElement CreateInspector(SceneComponent sceneComponent)
        {
            var container = new Foldout()
            {
                value = true,
                text = sceneComponent.GetType().Name,

                style =
                {
                    paddingLeft = 0,
                    paddingRight = 0,
                }
            };
            container.contentContainer.style.marginLeft = 0;

            var toggle = container.Q<Toggle>();
            toggle.AddToClassList("unity-box");
            toggle.style.marginLeft = 0;
            toggle.style.marginRight = 0;
            
            var disabled = EditorGUIUtility.IconContent("pin@2x");

            var enabled = EditorGUIUtility.IconContent("pinned@2x");

            var toggleRuntime = new EditorToolbarToggle()
            {
                style =
                {
                    width = 22,
                    height = 22
                },
            };

            toggleRuntime.onIcon = enabled.image as Texture2D;
            toggleRuntime.offIcon = disabled.image as Texture2D;
            toggleRuntime.tooltip = "Toggle Runtime";

            toggleRuntime.value = (sceneComponent.hideFlags & HideFlags.DontSaveInBuild) == 0;

            toggleRuntime.RegisterValueChangedCallback(evt =>
            {
                Undo.RegisterCompleteObjectUndo(sceneComponent, "Toggle Runtime");

                if (evt.newValue==false)
                    sceneComponent.hideFlags |= HideFlags.DontSaveInBuild;
                else
                    sceneComponent.hideFlags &= ~HideFlags.DontSaveInBuild;

                EditorUtility.SetDirty(_metaData);
            });

            toggle.Add(toggleRuntime);

            var minus = EditorGUIUtility.FindTexture("CrossIcon");
            var removeButton = new ToolbarButton(() =>
            {
                Undo.SetCurrentGroupName("Remove component");
                int group = Undo.GetCurrentGroup();

                Undo.RegisterCompleteObjectUndo(_metaData, "Remove from list and destroy");
                _metaData.Components.Remove(sceneComponent);
                EditorUtility.SetDirty(_metaData);
                Undo.DestroyObjectImmediate(sceneComponent);
                _componentsList.Remove(container);

                Undo.CollapseUndoOperations(group);
            })
            {
                style =
                {
                    width = 22,
                    height = 22,
                    alignContent = Align.Center,
                    justifyContent = Justify.Center,
                    backgroundImage = minus
                }
            };

            toggle.Add(removeButton);

            var editor = new InspectorElement(sceneComponent)
            {
                name = "inspectorElement",
            };

            container.Add(editor);

            return container;
        }

        private void AddComponent()
        {
            var inheritors = TypeCache.GetTypesDerivedFrom<SceneComponent>();

            var menu = new GenericMenu();
            foreach (var type in inheritors)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    Undo.SetCurrentGroupName("Add Component");
                    int group = Undo.GetCurrentGroup();

                    var instance = (SceneComponent)ScriptableObject.CreateInstance(type);
                    instance.name = type.Name;
                    instance.hideFlags |= HideFlags.HideInHierarchy;
                    AssetDatabase.AddObjectToAsset(instance, _metaData);
                    Undo.RegisterCreatedObjectUndo(instance, "Add Component");

                    Undo.RecordObject(_metaData, "Add Component");
                    _metaData.Components.Add(instance);

                    EditorUtility.SetDirty(_metaData);
                    Undo.CollapseUndoOperations(group);


                    var element = CreateInspector(instance);
                    _componentsList.Add(element);
                });
            }

            menu.ShowAsContext();
        }
    }
}