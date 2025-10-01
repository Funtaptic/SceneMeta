using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SceneMeta.Editor
{
    [CustomEditor(typeof(SceneAsset), true)]
    public class SceneAssetEditor : UnityEditor.Editor
    {
        private bool _isDirty;

        private SceneMetaData _metaData;

        private ObjectField _referenceField;

        private VisualElement _root;

        public override VisualElement CreateInspectorGUI()
        {
            SceneMetaData.TryGetData((SceneAsset)target, out _metaData);
            _root = new VisualElement();

            _referenceField = new ObjectField("MetaData")
            {
                value = _metaData,
                objectType = typeof(SceneMetaData)
            };

            _referenceField.RegisterValueChangedCallback(evt =>
            {
                _metaData = evt.newValue as SceneMetaData;
                _isDirty = true;
            });

            _root.Add(_referenceField);
            
            return _root;
        }


        private void OnDisable()
        {
            if (_isDirty == false)
                return;

            SceneMetaData.SetData((SceneAsset)target, _metaData);
        }
    }
}