using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Test.Editor
{
    [CustomEditor(typeof(Test))]
    public class TestEditor:UnityEditor.Editor
    {
        private SerializedProperty listProp;
        private ReorderableList list;

        private Test mb;
        private void OnEnable()
        {
            this.mb = target as Test;
            this.listProp = serializedObject.FindProperty("units");
            this.list = new ReorderableList(serializedObject, this.listProp,
                true, true, true, true);
            this.list.elementHeight = EditorGUIUtility.singleLineHeight;
            this.list.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "单元");
            this.list.drawElementCallback = (rect, index, x, y) =>
            {
                Debug.Log(1);
                var type = mb.units[index].GetType();
                Debug.Log(2);
                var fields = type.GetFields();
                var value = fields[0].GetValue(mb.units[index]);
                fields[0].SetValue(mb.units[index],
                    EditorGUI.IntField(rect,fields[0].Name, (int) value));
                Debug.Log(fields[0].FieldType);
                Debug.Log(4);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}