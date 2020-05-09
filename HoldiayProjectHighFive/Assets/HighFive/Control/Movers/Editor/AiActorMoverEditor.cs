using System;
using UnityEditor;
using UnityEngine;

namespace HighFive.Control.Movers.Editor
{
    [CustomEditor(typeof(AIActorMover))]
    public class AiActorMoverEditor:UnityEditor.Editor
    {
        private UnityEditor.Editor baseEditor;

        private string[] titles = {"ActorMover", "States", "Others"};
        private string[] states = {"AiPatrol", "Jump"};
        private int titleIndex = 0;
        private int stateIndex = 0;

        private SerializedProperty actorPatrolTypeProp;
        private SerializedProperty isPatrollingProp;
        private SerializedProperty patrolStopTimeProp;

        private SerializedProperty isJumpProp;
        private SerializedProperty jumpForceProp;
        private SerializedProperty autoJumpProp;
        private SerializedProperty jumpStopTimeProp;
        private SerializedProperty jumpCountMaxProp;

        private void OnEnable()
        {
            try
            {
                baseEditor = CreateEditor(target, typeof(ActorMoverEditor));
                actorPatrolTypeProp = serializedObject.FindProperty("actorPatrolType");
                isPatrollingProp = serializedObject.FindProperty("isPatrolling");
                patrolStopTimeProp = serializedObject.FindProperty("patrolStopTime");
                isJumpProp = serializedObject.FindProperty("isJumping");
                jumpForceProp = serializedObject.FindProperty("jumpForce");
                autoJumpProp = serializedObject.FindProperty("autoJump");
                jumpStopTimeProp=serializedObject.FindProperty("jumpStopTime");
                jumpCountMaxProp = serializedObject.FindProperty("jumpsCountMax");
            }
            catch (ArgumentException e)
            {
            }
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            titleIndex = GUILayout.Toolbar(titleIndex, titles);
            switch (titleIndex)
            {
                case 0:// ActorMover
                    baseEditor.OnInspectorGUI();
                    break;
                case 1:// states
                    stateIndex = GUILayout.Toolbar(stateIndex, states);
                    switch (stateIndex)
                    {
                        case 0:// AiPatrol
                            EditorGUILayout.PropertyField(actorPatrolTypeProp);
                            EditorGUILayout.PropertyField(isPatrollingProp);
                            EditorGUILayout.PropertyField(patrolStopTimeProp); 
                            break;
                        case 1:// Jump
                            EditorGUILayout.PropertyField(isJumpProp);
                            EditorGUILayout.PropertyField(jumpForceProp);
                            EditorGUILayout.PropertyField(autoJumpProp);
                            EditorGUILayout.PropertyField(jumpStopTimeProp);
                            EditorGUILayout.PropertyField(jumpCountMaxProp);
                            break;
                    }
                    break;
                case 2:// Others
                    break;
            }
            

            serializedObject.ApplyModifiedProperties();
        }
    }
}