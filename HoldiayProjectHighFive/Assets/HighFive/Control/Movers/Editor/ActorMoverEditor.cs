using System;
using UnityEditor;
using UnityEngine;

namespace HighFive.Control.Movers.Editor
{
    [CustomEditor(typeof(ActorMover))]
    public class ActorMoverEditor:UnityEditor.Editor
    {

        private string[] titles = {"BaseActor", "PreciseMovementControl", "Others"};
        private int selectedIndex = 0;
        
        private UnityEditor.Editor baseEditor;
        
        // PreciseMovementControl
        private SerializedProperty accelerationTimeAirborneProp;
        private SerializedProperty accelerationTimeGroundedProp;
        protected SerializedProperty timeToJumpApexProp;
        protected SerializedProperty maxJumpHeightProp;
        protected SerializedProperty runSpeedProp;
        protected SerializedProperty horizontalSpeedMultiplierProp;
        protected SerializedProperty verticalSpeedMultiplierProp;
        protected SerializedProperty faceDirProp;
        
        // Others
        private SerializedProperty rayCastDebugProp;

        private void OnEnable()
        {
            try
            {
                accelerationTimeAirborneProp = serializedObject.FindProperty("accelerationTimeAirborne");
                accelerationTimeGroundedProp = serializedObject.FindProperty("accelerationTimeGrounded");
                timeToJumpApexProp = serializedObject.FindProperty("timeToJumpApex");
                maxJumpHeightProp = serializedObject.FindProperty("maxJumpHeight");
                runSpeedProp = serializedObject.FindProperty("runSpeed");
                horizontalSpeedMultiplierProp = serializedObject.FindProperty("horizontalSpeedMultiplier");
                verticalSpeedMultiplierProp = serializedObject.FindProperty("verticalSpeedMultiplier");
                faceDirProp = serializedObject.FindProperty("faceDir");
                rayCastDebugProp = serializedObject.FindProperty("rayCastDebug");

                baseEditor = CreateEditor(target, typeof(BaseMoverEditor));
            }
            catch (ArgumentException e)
            {
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            selectedIndex = GUILayout.Toolbar(selectedIndex, titles);
            switch (selectedIndex)
            {
                case 0:// BaseActor
                    baseEditor.OnInspectorGUI();
                    break;
                case 1:// PreciseMovementControl
                    EditorGUILayout.PropertyField(accelerationTimeAirborneProp);
                    EditorGUILayout.PropertyField(accelerationTimeGroundedProp);
                    EditorGUILayout.PropertyField(timeToJumpApexProp);
                    EditorGUILayout.PropertyField(maxJumpHeightProp);
                    EditorGUILayout.PropertyField(runSpeedProp);
                    EditorGUILayout.PropertyField(horizontalSpeedMultiplierProp);
                    EditorGUILayout.PropertyField(verticalSpeedMultiplierProp);
                    EditorGUILayout.PropertyField(faceDirProp);
                    break;
                case 2:
                    EditorGUILayout.PropertyField(rayCastDebugProp);
                    break;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}