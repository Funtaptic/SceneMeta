This package allows you to attach components to scenes, create asset of type SceneMetaData from "Scene/MetaData" and add it to a scene asset field

To create new scene components inherit from "SceneComponent"

Add components to the SceneMetaData scriptable and acces them using SceneMetaData.TryGetData(scene.path, out var data), if you use the pin icon they will be also accesible at runtime using a extension method SceneAsset.TryGetComponent<Component>(out var component)


If you don't pin the component it wont be accesible at runtime, this is intended to be used with you'r own editor tools