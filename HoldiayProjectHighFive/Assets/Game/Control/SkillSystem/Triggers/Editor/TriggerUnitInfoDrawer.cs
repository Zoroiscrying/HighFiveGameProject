using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace Game.Control.SkillSystem.Editor
{
    [CustomPropertyDrawer(typeof(TriggerUnitInfo))]
    public class TriggerUnitInfoDrawer:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 2 * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = 0;
            #region LeftPart
            
            var sp = property.FindPropertyRelative("type");            
            GUIStyle titleStyle = new GUIStyle()
            {
                fontSize = 18,
                alignment = TextAnchor.UpperLeft
            };

            var leftPart = position.GetLeft(0.15f);
            
            // UnitType类型
            EditorGUI.LabelField(leftPart.GetUp(0.6f),sp.enumNames[sp.enumValueIndex],titleStyle);
            // ID
            EditorGUI.LabelField(leftPart.GetBottom(.5f),"ID:   "+property.FindPropertyRelative("id").intValue.ToString());
            
            #endregion

            position = position.GetRight(0.85f);
            var text = "";
            bool error = false;
            switch (EnumUtil.GetEnumValue<TriggerType>(sp.enumValueIndex))
            {
                
            }
            
            if(error)
                EditorGUI.HelpBox(position,text,MessageType.Error);
            else
                EditorGUI.LabelField(position,text);
        }
    }
}