using System;
using UnityEditor;
using UnityEngine;

namespace HighFive.Control.Movers.Editor
{
    [CustomEditor(typeof(AIActorMover))]
    public class AiActorMoverEditor:ActorMoverEditor
    {
        private string[] aiActorTitles = {"ActorMover", "States", "Others"};
        private string[] aiActorStates = {"AiPatrol", "Jump"};
        private int aiActorIndex = 0;
        private int aiActorStateIndex = 0;

        private SerializedProperty actorPatrolTypeProp;
        private SerializedProperty isPatrollingProp;
        private SerializedProperty patrolStopTimeProp;

        private SerializedProperty isJumpProp;
        private SerializedProperty jumpForceProp;
        private SerializedProperty autoJumpProp;
        private SerializedProperty jumpStopTimeProp;
        private SerializedProperty jumpCountMaxProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            actorPatrolTypeProp = serializedObject.FindProperty("actorPatrolType");
            isPatrollingProp = serializedObject.FindProperty("isPatrolling");
            patrolStopTimeProp = serializedObject.FindProperty("patrolStopTime");
            isJumpProp = serializedObject.FindProperty("isJumping");
            jumpForceProp = serializedObject.FindProperty("jumpForce");
            autoJumpProp = serializedObject.FindProperty("autoJump");
            jumpStopTimeProp=serializedObject.FindProperty("jumpStopTime");
            jumpCountMaxProp = serializedObject.FindProperty("jumpsCountMax");

        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            aiActorIndex = GUILayout.Toolbar(aiActorIndex, aiActorTitles);
            switch (aiActorIndex)
            {
                case 0:// ActorMover
                    base.OnInspectorGUI();
                    break;
                case 1:// aiActorStates
                    aiActorStateIndex = GUILayout.Toolbar(aiActorStateIndex, aiActorStates);
                    switch (aiActorStateIndex)
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