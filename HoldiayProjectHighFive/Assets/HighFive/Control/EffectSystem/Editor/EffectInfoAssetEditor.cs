using HighFive.Control.EffectSystem.Effects;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HighFive.Control.EffectSystem.Editor
{
    [CustomEditor(typeof(EffectInfoAsset))]
    public class EffectInfoAssetEditor:UnityEditor.Editor
    {
        private Vector2 pos;
        private SerializedProperty EffectUnitInfosProp;
        private SerializedProperty effectorTypeProp;
        private ReorderableList EffectUnitInfoList;
        private EffectInfoAsset EffectInfoAsset;
        
        private int selectIndex;

        private void OnEnable()
        {
            this.EffectInfoAsset=target as EffectInfoAsset;
            this.effectorTypeProp = serializedObject.FindProperty("effectorType");
            this.EffectUnitInfosProp = serializedObject.FindProperty("EffectUnitInfos");
            this.EffectUnitInfoList =
                new ReorderableList(serializedObject, EffectUnitInfosProp, true, true, true, true);

            this.EffectUnitInfoList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;
            this.EffectUnitInfoList.drawElementCallback = (rect, index, a, b) =>
            {
                var prop = EffectUnitInfosProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, prop);
            };
            this.EffectUnitInfoList.drawHeaderCallback = (rect) =>
                EditorGUI.LabelField(rect, "特效列表");
            this.EffectUnitInfoList.onSelectCallback = (list) => selectIndex = EffectUnitInfoList.index;
            
            EffectUnitInfoList.onAddDropdownCallback=(rect,list)=>
            {
                var menu = new GenericMenu();
                var enumIndex = -1;
                foreach (var value in EnumUtil.GetValues<EffectType>())
                {   
                    enumIndex++;
                    menu.AddItem(new GUIContent(((EffectType)value).ToString()), false, OnAddUnitCallBack,enumIndex);
                }
                menu.ShowAsContext();
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(effectorTypeProp);
            pos = GUILayout.BeginScrollView(pos);
            
            EffectUnitInfoList.DoLayoutList();
            
            if (selectIndex != -1 && selectIndex <EffectUnitInfoList.serializedProperty.arraySize)
            {

                var prop = this.EffectUnitInfosProp.GetArrayElementAtIndex(selectIndex);
                var rect = GUILayoutUtility.GetRect(100, EditorGUI.GetPropertyHeight(prop),
                    GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                EffectInfoAsset.EffectUnitInfos[selectIndex].OnDrawMoreInfo(prop, rect);
            }
            
            GUILayout.EndScrollView();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnAddUnitCallBack(object userdata)
        {
            var enumIndex = (int) userdata;
            var index = EffectUnitInfosProp.arraySize;
            EffectUnitInfosProp.arraySize++;
            EffectUnitInfoList.index = index;
            selectIndex = index;
            var prop = EffectUnitInfosProp.GetArrayElementAtIndex(index);
            prop.FindPropertyRelative("type").enumValueIndex = enumIndex;
            prop.FindPropertyRelative("EffectInfoAsset").objectReferenceValue = EffectInfoAsset;
            serializedObject.ApplyModifiedProperties();
        }
    
    }
}