using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace HighFive.Control.EffectSystem.Editor
{
    [CustomPropertyDrawer(typeof(EffectMgr.EffectAssetMsg))]
    public class EffectAssetMsgDrawer:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 3 * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = 0;
            EditorGUI.LabelField(position.GetRectAtIndex(index++),"ID", property.FindPropertyRelative("id").intValue.ToString());
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("tag"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("EffectInfoAsset"));
        }
    }
}