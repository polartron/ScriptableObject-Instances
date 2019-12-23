using System.Collections.Generic;
using Fasteraune.SO.Instances.Editor;
using UnityEditor;
using UnityEngine;
using Fasteraune.SO.Instances.Variables;

namespace Fasteraune.SO.Instances.Variables.Editor
{
    [CustomPropertyDrawer(typeof(VariableReference), true)]
    public class VariablesDrawer : PropertyDrawer
    {
        private readonly string[] popupOptions =
            { "Use Constant Value", "Use Shared Variable", "Use Instanced Variable" };

        private readonly Dictionary<string, bool> toggles = new Dictionary<string, bool>();

        private GUIStyle popupStyle;
        private GUIStyle toggleButtonStyle;

        private Texture2D backgroundTexture = new Texture2D(0, 0);

        private void DrawInstancedVariableRuntimeValue(Variable variable, Rect position,
            InstanceOwner owner)
        {
            if (owner != null && variable != null)
            {
                if (variable.instances.ContainsKey(owner))
                {
                    var instancedVariable = variable.instances[owner] as Variable;
                    DrawVariableRuntimeValue(variable, instancedVariable, position);
                }
                else
                {
                    EditorGUI.LabelField(position, "Not instantiated yet or part of a prefab");
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
            var instancedVariableOwner = connection.objectReferenceValue as InstanceOwner;
            var variable = variableProperty.objectReferenceValue as Variable;

            switch (referenceTypeEnum)
            {
                case ReferenceType.Constant:
                    label.text += " (Constant)";
                    break;
                case ReferenceType.Shared:
                    label.text += " (Shared)";
                    break;
                case ReferenceType.Instanced:
                    label.text += " (Instanced)";
                    break;
            }

            var original = new Rect(position);
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

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
                case ReferenceType.Instanced:
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
                                variableProperty.objectReferenceValue = Utils.MakeNewScriptableObject(variableProperty);
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

                case ReferenceType.Shared:
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
                                variableProperty.objectReferenceValue = Utils.MakeNewScriptableObject(variableProperty);
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

                case ReferenceType.Constant:
                {
                    DrawProperty(position, constantValue);
                    break;
                }
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
            if (property == null)
            {
                EditorGUI.LabelField(position, "Property does not have a value");
                return;
            }
            
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
                case ReferenceType.Constant:
                    return constantValue;

                case ReferenceType.Shared:
                case ReferenceType.Instanced:
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
            var heightProperty = GetTargetProperty(property);

            if (heightProperty == null || !heightProperty.hasVisibleChildren ||
                heightProperty.hasVisibleChildren && !heightProperty.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return EditorGUI.GetPropertyHeight(heightProperty) + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
