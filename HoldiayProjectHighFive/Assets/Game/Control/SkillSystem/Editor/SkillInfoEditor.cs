using System.IO;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game.Control.SkillSystem.Editor
{
    [CustomEditor(typeof(SkillInfoAsset))]
    public class SkillInfoEditor:UnityEditor.Editor
    {
        [MenuItem("ReadyGamerOne/Create/RPG/SkillAsset")]
        public static void CreateInstance()
        {
            string[] strs = Selection.assetGUIDs;

            string path = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (path.Contains("."))
            {
                path=path.Substring(0, path.LastIndexOf('/'));
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            AssetDatabase.CreateAsset(CreateInstance<SkillInfoAsset>(), path + "/NewSkillInfo.asset");
            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SkillInfoAsset>(path + "/NewSkillInfo.asset");
        }

        private Vector2 detailPos;
        private ReorderableList triggerList;
        private SerializedProperty triggerListProp;
        private SerializedProperty skillNameProp;
        private SkillInfoAsset _skillInfoAsset;
        private int selectIndex;
        private void OnEnable()
        {
            this._skillInfoAsset=target as SkillInfoAsset;
            this.triggerListProp = serializedObject.FindProperty("triggers");
            this.skillNameProp = serializedObject.FindProperty("skillName");
            this.triggerList=new ReorderableList(serializedObject,triggerListProp,true,true,true,true);

            this.triggerList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;

            triggerList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var triggerProp = triggerListProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, triggerProp);
            };
            
            
            triggerList.onSelectCallback = (list) => { this.selectIndex = list.index; };

            triggerList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "触发器");

            triggerList.onAddDropdownCallback = (rect, list) =>
            {
                var menu = new GenericMenu();
                var enumIndex = -1;
                foreach (var value in EnumUtil.GetValues<TriggerType>())
                {   
                    enumIndex++;

                    menu.AddItem(new GUIContent(((TriggerType)value).ToString()), false, OnAddUnitCallBack,enumIndex);

                }

                menu.ShowAsContext();
            };
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            this.detailPos = GUILayout.BeginScrollView(this.detailPos ,GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(skillNameProp);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _skillInfoAsset.name = _skillInfoAsset.skillName.StringValue;
                var path= AssetDatabase.GetAssetPath(_skillInfoAsset);
                AssetDatabase.RenameAsset(path,
                    _skillInfoAsset.skillName.StringValue);
                AssetDatabase.Refresh();
            }
            
            if (GUILayout.Button("清空触发器列表"))
                triggerListProp.arraySize = 0;
            triggerList.DoLayoutList();
            if (selectIndex != -1 && selectIndex <triggerList.serializedProperty.arraySize)
            {

                var prop = triggerListProp.GetArrayElementAtIndex(selectIndex);
                var rect = GUILayoutUtility.GetRect(100, EditorGUI.GetPropertyHeight(prop),
                    GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                _skillInfoAsset.triggers[selectIndex].OnDrawMoreInfo(prop, rect);
                GUILayout.EndScrollView();
            }
            
            serializedObject.ApplyModifiedProperties();
        }


        private void OnAddUnitCallBack(object obj)
        {
            var enumIndex = (int) obj;

            var index = triggerListProp.arraySize;
            
            triggerListProp.arraySize++;
            var triggerProp = triggerListProp.GetArrayElementAtIndex(index);
            this.triggerList.index = index;
            selectIndex = index;

            triggerProp.FindPropertyRelative("type").enumValueIndex = enumIndex;

            triggerProp.FindPropertyRelative("id").intValue = _skillInfoAsset.GetID();

            serializedObject.ApplyModifiedProperties();
            
        }
        
        
        
        
        
        
        
        
    }
}