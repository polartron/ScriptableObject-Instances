using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Fasteraune.Variables.Editor
{
    [CustomPropertyDrawer(typeof(BaseReference), true)]
    public class ReferenceDrawer : PropertyDrawer
    {
        private readonly string[] popupOptions =
            {"Use Constant Value", "Use Shared Variable", "Use Instanced Variable"};

        private readonly Dictionary<string, bool> toggles = new Dictionary<string, bool>();

        private GUIStyle popupStyle;
        private GUIStyle toggleButtonStyle;

        private Texture2D backgroundTexture = new Texture2D(0, 0);

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

        public ScriptableObject MakeNewScriptableObject(SerializedProperty property)
        {
            var saveDestination = "";

            var monoBehaviour = property.serializedObject.targetObject as MonoBehaviour;

            if (monoBehaviour != null && PrefabUtility.IsPartOfAnyPrefab(monoBehaviour.gameObject))
            {
                var prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(monoBehaviour.gameObject);

                if (prefab != null)
                {
                    saveDestination = Path.GetDirectoryName(AssetDatabase.GetAssetPath(prefab));
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

        private void DrawInstancedVariableRuntimeValue(Variable variable, Rect position,
            InstancedVariableOwner owner)
        {
            if (owner != null && variable != null)
            {
                if (variable.instanceValues.ContainsKey(owner))
                {
                    var instancedVariable = variable.instanceValues[owner] as Variable;
                    DrawVariableRuntimeValue(variable, instancedVariable, position);
                }
                else
                {
                    EditorGUI.LabelField(position, "Not initialized yet or part of a prefab");
                }
            }
        }

        private void DrawVariableRuntimeValue(Variable source, Variable target, Rect position)
        {
            if (target == null || source == null)
            {
                return;
            }

            var wrapper = target.GetRuntimeValueWrapper();

            if (wrapper == null || wrapper.targetObject == null)
            {
                return;
            }

            var wrapperValue = wrapper.FindProperty("ProxyValue");

            if (wrapperValue == null)
            {
                return;
            }

            var newButtonRect = new Rect(position);
            newButtonRect.x = position.xMax - 40;
            newButtonRect.width = 40;
            position.xMax -= newButtonRect.width;

            if (GUI.Button(newButtonRect, "Save"))
            {
                source.SaveRuntimeValue(target);
            }

            EditorGUI.BeginChangeCheck();

            DrawProperty(position, wrapperValue);

            if (EditorGUI.EndChangeCheck())
            {
                wrapper.ApplyModifiedProperties();
                target.ApplyModifiedValue(wrapper);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (popupStyle == null)
            {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
            }

            if (toggleButtonStyle == null)
            {
                toggleButtonStyle = new GUIStyle(GUI.skin.button);
                toggleButtonStyle.fontSize = 16;
            }

            // Get properties
            var referenceType = property.FindPropertyRelative("Type");
            var constantValue = property.FindPropertyRelative("ConstantValue");
            var variableProperty = property.FindPropertyRelative("Variable");
            var connection = property.FindPropertyRelative("Connection");

            var referenceTypeEnum = (ReferenceType) referenceType.enumValueIndex;
            var instancedVariableOwner = connection.objectReferenceValue as InstancedVariableOwner;
            var variable = variableProperty.objectReferenceValue as Variable;

            switch (referenceTypeEnum)
            {
                case ReferenceType.ConstantValue:
                    label.text += " (Constant)";
                    break;
                case ReferenceType.SharedReference:
                    label.text += " (Shared)";
                    break;
                case ReferenceType.InstancedReference:
                    label.text += " (Instanced)";
                    break;
            }
            
            Rect original = new Rect(position);
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            
            //EditorGUI.HelpBox(position, "", MessageType.Info);
            GUI.Label(original, backgroundTexture, EditorStyles.helpBox);
            position.size = new Vector2(position.size.x, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.BeginChangeCheck();

            // Calculate rect for configuration button
            var buttonRect = new Rect(position);
            buttonRect.yMin += popupStyle.margin.top;
            buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var result = EditorGUI.Popup(buttonRect, referenceType.enumValueIndex, popupOptions, popupStyle);
            referenceType.enumValueIndex = result;

            switch (referenceTypeEnum)
            {
                case ReferenceType.InstancedReference:
                {
                    if (!Application.isPlaying)
                    {
                        if (variableProperty.objectReferenceValue == null)
                        {
                            var newButtonRect = new Rect(position);
                            newButtonRect.x = position.xMax - 40;
                            newButtonRect.width = 40;
                            position.xMax -= newButtonRect.width;

                            if (GUI.Button(newButtonRect, "New"))
                            {
                                variableProperty.objectReferenceValue = MakeNewScriptableObject(variableProperty);
                            }
                        }

                        var toggleButtonRect = new Rect(position);
                        toggleButtonRect.width = 20;
                        toggleButtonRect.x = position.xMax - 20;
                        position.xMax -= toggleButtonRect.width;

                        var target = property.propertyPath;

                        if (!toggles.ContainsKey(target))
                        {
                            toggles.Add(target, connection.objectReferenceValue == null);
                        }

                        if (GUI.Button(toggleButtonRect, "⇅", toggleButtonStyle))
                        {
                            toggles[target] = !toggles[target];
                        }

                        if (toggles[target])
                        {
                            EditorGUI.PropertyField(position, connection, GUIContent.none);
                        }
                        else
                        {
                            EditorGUI.PropertyField(position, variableProperty, GUIContent.none);
                        }
                    }
                    else
                    {
                        DrawInstancedVariableRuntimeValue(variable, position, instancedVariableOwner);
                    }

                    break;
                }

                case ReferenceType.SharedReference:
                {
                    if (!Application.isPlaying)
                    {
                        if (variableProperty.objectReferenceValue == null)
                        {
                            var newButtonRect = new Rect(position);
                            newButtonRect.x = position.xMax - 40;
                            newButtonRect.width = 40;
                            position.xMax -= newButtonRect.width;

                            if (GUI.Button(newButtonRect, "New"))
                            {
                                variableProperty.objectReferenceValue = MakeNewScriptableObject(variableProperty);
                            }
                        }

                        EditorGUI.PropertyField(position, variableProperty, GUIContent.none);
                    }
                    else
                    {
                        DrawVariableRuntimeValue(variable, variable, position);
                    }

                    break;
                }

                case ReferenceType.ConstantValue:
                {
                    DrawProperty(position, constantValue);
                    break;
                }
            }
            
            var clampProperty = property.FindPropertyRelative("Clamp");

            var t = GetTargetProperty(property);
            if (clampProperty != null && t != null)
            {
                EditorGUI.indentLevel++;
                float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                Rect clampRect = new Rect(original);
                clampRect.y += EditorGUI.GetPropertyHeight(t);
                EditorGUI.PropertyField(clampRect, clampProperty, true);
                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private void DrawProperty(Rect position, SerializedProperty property)
        {
            if (!property.hasVisibleChildren)
            {
                EditorGUI.PropertyField(position, property, GUIContent.none);
                return;
            }

            //Move xMin slightly to the right to make the foldout arrow visible
            var valueRect = new Rect(position);
            valueRect.xMin += 15f;

            var label = new GUIContent(property.type);
            EditorGUI.PropertyField(valueRect, property, label, true);
        }

        private SerializedProperty GetTargetProperty(SerializedProperty property)
        {
            var referenceType = property.FindPropertyRelative("Type");
            var constantValue = property.FindPropertyRelative("ConstantValue");
            var variableProperty = property.FindPropertyRelative("Variable");
            
            var referenceTypeEnum = (ReferenceType) referenceType.enumValueIndex;

            switch (referenceTypeEnum)
            {
                case ReferenceType.ConstantValue:
                    return constantValue;

                case ReferenceType.SharedReference:
                case ReferenceType.InstancedReference:                    
                    var variable = variableProperty.objectReferenceValue as Variable;

                    if (variable == null)
                    {
                        break;
                    }

                    var wrapper = variable.GetRuntimeValueWrapper();
                    return wrapper.FindProperty("ProxyValue");
            }

            return null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var clampProperty = property.FindPropertyRelative("Clamp");

            SerializedProperty heightProperty = GetTargetProperty(property);

            if (heightProperty == null || !heightProperty.hasVisibleChildren ||
                heightProperty.hasVisibleChildren && !heightProperty.isExpanded)
            {
                float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                if (clampProperty != null)
                {
                    height += EditorGUI.GetPropertyHeight(clampProperty);
                }
                
                return height;
            }

            if (clampProperty != null)
            {
                return EditorGUI.GetPropertyHeight(heightProperty) +
                    EditorGUI.GetPropertyHeight(clampProperty) + EditorGUIUtility.standardVerticalSpacing;
            }

            return EditorGUI.GetPropertyHeight(heightProperty) + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}