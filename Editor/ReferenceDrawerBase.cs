// ------------------------------------------------------------------------------------------------------
// Heavily inspired by Ryan Hipple, 10/04/17 from Unite 2017 - Game Architecture with Scriptable Objects
// ------------------------------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using Xprees.Variables.Base;

namespace Xprees.Variables.Editor.Editor
{
    [CustomPropertyDrawer(typeof(ReferenceBase<>), true)]
    public class ReferenceDrawerBase : PropertyDrawer
    {
        /// Options to display in the popup to select constant or variable.
        private readonly string[] _popupOptions = { "Inlined Value", "Variable" };

        private GUIStyle _popupStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _popupStyle ??= new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
            {
                imagePosition = ImagePosition.ImageOnly,
            };

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();
            // Get properties
            var useConstant = property.FindPropertyRelative("useInlined");
            var constantValue = property.FindPropertyRelative("inlinedValue");
            var variable = property.FindPropertyRelative("variable");

            // Calculate rect for configuration button
            var buttonRect = new Rect(position);
            buttonRect.yMin += _popupStyle.margin.top;
            buttonRect.width = _popupStyle.fixedWidth + _popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, _popupOptions, _popupStyle);

            useConstant.boolValue = result == 0;

            EditorGUI.PropertyField(position, useConstant.boolValue ? constantValue : variable, GUIContent.none);


            if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}