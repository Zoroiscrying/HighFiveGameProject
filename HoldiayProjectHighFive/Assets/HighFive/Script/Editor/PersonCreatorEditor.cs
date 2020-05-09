using System;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HighFive.Script
{
    [CustomEditor(typeof(PersonCreator))]
    public class PersonCreatorEditor:Editor
    {
        private SerializedProperty listProp;
        private ReorderableList list;
        private PersonCreator pc;
        private void OnEnable()
        {
            pc=target as PersonCreator;
            listProp = serializedObject.FindProperty("createInfos");
            list = new ReorderableList(serializedObject, listProp, true, true, true, true);
            list.elementHeight = 4 * EditorGUIUtility.singleLineHeight;
            list.drawElementCallback = (rect, index, a, b) =>
            {
                var prop = listProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, prop);
            };
            list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, listProp.displayName);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

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
    }
}