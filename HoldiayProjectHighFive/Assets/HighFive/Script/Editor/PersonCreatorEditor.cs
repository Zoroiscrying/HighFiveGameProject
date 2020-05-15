using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HighFive.Script
{
    [CustomEditor(typeof(PersonCreator),true)]
    public class PersonCreatorEditor:UnityEditor.Editor
    {
        private SerializedProperty signalSizeProp;
        private SerializedProperty listProp;
        private ReorderableList list;
        private PersonCreator pc;
        private int focusedIndex = -1;
//        private bool finded = false;
        protected virtual void OnEnable()
        {
            pc = target as PersonCreator;
            signalSizeProp = serializedObject.FindProperty("signalSize");
            listProp = serializedObject.FindProperty("createInfos");
            list = new ReorderableList(serializedObject, listProp, true, true, true, true);
            list.elementHeight = 4 * EditorGUIUtility.singleLineHeight;
            list.drawElementCallback = (rect, selectIndex, isActive, isFocused) =>
            {
                var prop = listProp.GetArrayElementAtIndex(selectIndex);
                var index = 0;
                if (isActive && isFocused)
                {
                    focusedIndex = selectIndex;
//                    finded = true;
                }
                else if(selectIndex==focusedIndex)
                {
                    focusedIndex = -1;
                }
                EditorGUI.PropertyField(rect.GetRectAtIndex(index++), prop.FindPropertyRelative("enable"));
                EditorGUI.PropertyField(rect.GetRectAtIndex(index++), prop.FindPropertyRelative("_personType"));
                EditorGUI.PropertyField(rect.GetRectAtIndex(index++), prop.FindPropertyRelative("position"));
                EditorGUI.PropertyField(rect.GetRectAtIndex(index++), prop.FindPropertyRelative("color"));
            };
            list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, listProp.displayName);
            list.onAddCallback = list =>
            {
                serializedObject.Update();
                var size = listProp.arraySize;
                listProp.arraySize++;
                var newProp = listProp.GetArrayElementAtIndex(size);
                list.index = size;
                newProp.FindPropertyRelative("color").colorValue = pc.DefaultGizmoColor;
                serializedObject.ApplyModifiedProperties();
            };

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(signalSizeProp);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All"))
            {
                for (var i = 0; i < listProp.arraySize; i++)
                {
                    var prop = listProp.GetArrayElementAtIndex(i);
                    prop.FindPropertyRelative("enable").boolValue = true;
                }
            }
            if (GUILayout.Button("Disable All"))
            {
                for (var i = 0; i < listProp.arraySize; i++)
                {
                    var prop = listProp.GetArrayElementAtIndex(i);
                    prop.FindPropertyRelative("enable").boolValue = false;
                }
            }
            GUILayout.EndHorizontal();
            
            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnSceneGUI()
        {
            if (focusedIndex != -1 && focusedIndex < listProp.arraySize)
            {
                serializedObject.Update();
                var prop = listProp.GetArrayElementAtIndex(focusedIndex);
                var positionProp = prop.FindPropertyRelative("position");
                positionProp.vector3Value = Handles.PositionHandle(positionProp.vector3Value, Quaternion.identity);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}