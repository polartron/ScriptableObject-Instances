using System.Collections.Generic;
using Fasteraune.SO.Instances.Editor;
using UnityEditor;
using UnityEngine;

namespace Fasteraune.SO.Instances.Events.Editor
{
    [CustomPropertyDrawer(typeof(EventReference), true)]
    public class EventsDrawer : PropertyDrawer
    {
        private readonly string[] popupOptions = { "Use Shared Event", "Use Instanced Event" };

        private readonly Dictionary<string, bool> toggles = new Dictionary<string, bool>();

        private GUIStyle popupStyle;
        private GUIStyle toggleButtonStyle;

        private Texture2D backgroundTexture = new Texture2D(0, 0);

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
            var variableProperty = property.FindPropertyRelative("Event");
            var connection = property.FindPropertyRelative("Connection");

            var referenceTypeEnum = (ReferenceType) referenceType.enumValueIndex;

            switch (referenceTypeEnum)
            {
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

            //EditorGUI.HelpBox(position, "", MessageType.Info);
            GUI.Label(original, backgroundTexture, EditorStyles.helpBox);
            position.size = new Vector2(position.size.x, EditorGUIUtility.singleLineHeight);

            // Calculate rect for configuration button
            var buttonRect = new Rect(position);
            buttonRect.yMin += popupStyle.margin.top;
            buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var result = EditorGUI.Popup(buttonRect, referenceType.enumValueIndex, popupOptions, popupStyle);
            referenceType.enumValueIndex = result + 1; //Events can't be constant

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

                    break;
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
