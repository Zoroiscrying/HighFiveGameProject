using System;
using UnityEditor;
using UnityEditorInternal;

namespace Game.Scripts
{
    [CustomEditor(typeof(PersonCreater))]
    public class PersonCreaterEditor:Editor
    {
        private SerializedProperty createOnStartProp;
        private SerializedProperty signalSizeProp;
        private SerializedProperty listProp;
        private ReorderableList list;
        private PersonCreater pc;

        private void OnEnable()
        {
            pc=target as PersonCreater;
            listProp = serializedObject.FindProperty("CharacterInfos");
            createOnStartProp = serializedObject.FindProperty("createOnStart");
            signalSizeProp = serializedObject.FindProperty("signalSize");
            list = new ReorderableList(serializedObject, listProp, true, true, true, true);
            list.elementHeight = 3 * EditorGUIUtility.singleLineHeight;
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var prop = listProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, prop);
            };
            list.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "创建信息");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(createOnStartProp);
            EditorGUILayout.PropertyField(signalSizeProp);
            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}