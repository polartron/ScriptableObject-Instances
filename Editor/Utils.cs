using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Fasteraune.SO.Instances.Editor
{
    public class Utils
    {
        /// https://answers.unity.com/questions/929293/get-field-type-of-serializedproperty.html
        public static Type GetSerializedPropertyType(SerializedProperty property)
        {
            var parts = property.propertyPath.Split('.');

            var currentType = property.serializedObject.targetObject.GetType();

            for (var i = 0; i < parts.Length; i++)
            {
                var field = currentType.GetField(parts[i],
                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance);

                if (field != null)
                {
                    currentType = field.FieldType;
                }
            }

            var targetType = currentType;

            return targetType;
        }

        public static ScriptableObject MakeNewScriptableObject(SerializedProperty property)
        {
            var saveDestination = "";

            var monoBehaviour = property.serializedObject.targetObject as MonoBehaviour;

            if (monoBehaviour != null && PrefabUtility.IsPartOfAnyPrefab(monoBehaviour.gameObject))
            {
                var prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(monoBehaviour.gameObject);

                if (prefab != null)
                {
                    string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
                    saveDestination = Path.GetDirectoryName(assetPath);
                }
            }
            else
            {
                saveDestination = Application.dataPath;
            }

            var type = GetSerializedPropertyType(property);
            var niceName = ObjectNames.NicifyVariableName(type.Name);
            saveDestination = EditorUtility.SaveFilePanel("Save as", saveDestination, niceName, "asset");

            if (string.IsNullOrEmpty(saveDestination))
            {
                return null;
            }

            if (saveDestination.Contains("Assets/"))
            {
                saveDestination = "Assets" + saveDestination.Substring(Application.dataPath.Length);
            }
            else
            {
                Debug.LogError("Creating new variables this way outside of the Assets folder is not supported");
                return null;
            }

            if (!string.IsNullOrEmpty(saveDestination))
            {
                var obj = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(obj, saveDestination);
                AssetDatabase.Refresh();
                return obj;
            }

            return null;
        }
    }
}
