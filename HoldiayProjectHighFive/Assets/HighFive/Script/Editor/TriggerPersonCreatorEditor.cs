using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HighFive.Script
{
    [CustomEditor(typeof(TriggerPersonCreator))]
    public class TriggerPersonCreatorEditor:PersonCreatorEditor
    {
        private SerializedProperty timeDelayProp;
        private SerializedProperty workOnceProp;
        private void OnEnable()
        {
            base.OnEnable();
            timeDelayProp = serializedObject.FindProperty("timeDelay");
            workOnceProp = serializedObject.FindProperty("workOnce");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(timeDelayProp);
            EditorGUILayout.PropertyField(workOnceProp);

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}