using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneMeta.Editor
{
    [CustomEditor(typeof(SceneMetaData))]
    public class SceneMetaEditor : UnityEditor.Editor
    {
        private SceneMetaEditorElement _element;

        protected override bool ShouldHideOpenButton() => true;


        private void OnEnable()
        {
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            serializedObject.Update();
            _element.RebuildComponentsInspectors();
        }

        protected override void OnHeaderGUI()
        {
        }

        public override VisualElement CreateInspectorGUI()
        {
            _element = new SceneMetaEditorElement(target as SceneMetaData);
            _element.RebuildComponentsInspectors();

            _element.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                _element.parent.style.paddingLeft = 0;
                _element.parent.style.paddingRight = 0;
            });

            return _element;
        }
    }
}