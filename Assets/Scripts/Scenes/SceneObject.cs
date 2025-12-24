using UnityEngine;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GDS
{
    [System.Serializable]
    public class SceneObject
    {
        [SerializeField]
        private string m_SceneName;

        public string SceneName => m_SceneName;

        public static implicit operator string(SceneObject sceneObject)
        {
            return sceneObject.m_SceneName;
        }

        public static implicit operator SceneObject(string sceneName)
        {
            return new SceneObject() { m_SceneName = sceneName };
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneObject))]
    public class SceneObjectEditor : PropertyDrawer
    {
        protected SceneAsset GetSceneObject(string sceneObjectName)
        {
            if (string.IsNullOrEmpty(sceneObjectName))
                return null;

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                string name = Path.GetFileNameWithoutExtension(scene.path);
                if (name == sceneObjectName)
                {
                    return AssetDatabase.LoadAssetAtPath(scene.path, typeof(SceneAsset)) as SceneAsset;
                }
            }

            return null;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sceneObj = GetSceneObject(property.FindPropertyRelative("m_SceneName").stringValue);
            var newScene = EditorGUI.ObjectField(position, label, sceneObj, typeof(SceneAsset), false);
            if (newScene == null)
            {
                var prop = property.FindPropertyRelative("m_SceneName");
                prop.stringValue = "";
            }
            else
            {
                if (newScene.name != property.FindPropertyRelative("m_SceneName").stringValue)
                {
                    var scnObj = GetSceneObject(newScene.name);
                    if (scnObj == null)
                    {
                        Debug.LogWarning("The scene " + newScene.name + " cannot be used. To use this scene add it to the build settings for the project.");
                    }
                    else
                    {
                        var prop = property.FindPropertyRelative("m_SceneName");
                        prop.stringValue = newScene.name;
                    }
                }
            }
        }
    }
#endif
}