using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace HighFive.Script
{
    [CustomPropertyDrawer(typeof(CharacterCreateInfo))]
    public class CharacterCreateInfoDrawer:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = 0;
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("characterInfo"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("position"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("color"));
        }
    }
}