using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace Game.Control.EffectSystem
{
    [CustomPropertyDrawer(typeof(EffectUnitInfo))]
    public class EffectUnitInfoDrawer:PropertyDrawer
    {
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

            
            #endregion
            
            position = position.GetRight(0.8f);
            
            var text = "";
            bool error = false;
            switch (EnumUtil.GetEnumValue<EffectType>(sp.enumValueIndex))
            {
                
            }
            
            if(error)
                EditorGUI.HelpBox(position,text,MessageType.Error);
            else
                EditorGUI.LabelField(position,text);
            
        }
    }
}