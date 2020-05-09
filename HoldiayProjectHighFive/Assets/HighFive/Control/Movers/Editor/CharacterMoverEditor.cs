using System;
using UnityEditor;
using UnityEngine;

namespace HighFive.Control.Movers.Editor
{
    [CustomEditor(typeof(CharacterMover))]
    public class CharacterMoverEditor:UnityEditor.Editor
    {
        private UnityEditor.Editor baseEditor;
        private string[] titles = {"ActorMover","Character", "Others"};
        private int titleIndex = 0;

        private SerializedProperty minJumpHeightProp;
        private SerializedProperty canHighJumpProp;
        private SerializedProperty canAccelerationProp;

        private SerializedProperty maxUpSpeedProp;
        private SerializedProperty maxDownSpeedProp;

        private string[] states = {"HighJump", "Accelerate", "Dash", "Wall", "Jump"};
        private int stateIndex = 2;

        // high jump
        private SerializedProperty highJumpHeightProp;
        
        // acceleration
        private SerializedProperty accelerationSpeedProp;
        private SerializedProperty accelerationTimeProp;
        
        // dash
        private SerializedProperty canDashProp;
        private SerializedProperty dashTimeProp;
        private SerializedProperty dashDistanceProp;
        
        // wall
        private SerializedProperty wallSlideVelocityProp;
        private SerializedProperty wallJumpTimeProp;
        private SerializedProperty wallJumpClimbProp;
        private SerializedProperty wallJumpNormalProp;
        
        // jump
        private SerializedProperty canAirJumpProp;
        private SerializedProperty airJumpTimeProp;
        private SerializedProperty inControlProp;
        
        // animaton names
        private SerializedProperty idleAniNameProp;
        private SerializedProperty walkAniNameProp;
        private SerializedProperty jumpAniNameProp;
        private SerializedProperty runAniNameProp;
        private SerializedProperty dashAniNameProp;


        private SerializedProperty debugStateCheckerProp;


        private void OnEnable()
        {
            try
            {
                baseEditor = CreateEditor(target, typeof(ActorMoverEditor));
                minJumpHeightProp = serializedObject.FindProperty("minJumpHeight");
                canHighJumpProp = serializedObject.FindProperty("canHighJump");
                canAccelerationProp = serializedObject.FindProperty("canAcceleration");
                maxUpSpeedProp = serializedObject.FindProperty("maxUpYSpeed");
                maxDownSpeedProp = serializedObject.FindProperty("maxDownYSpeed");
                highJumpHeightProp = serializedObject.FindProperty("highJumpHeight");
                accelerationSpeedProp = serializedObject.FindProperty("accelerationSpeed");
                accelerationTimeProp = serializedObject.FindProperty("accelerationTime");
                canDashProp = serializedObject.FindProperty("canDash");
                dashTimeProp = serializedObject.FindProperty("dashTime");
                dashDistanceProp = serializedObject.FindProperty("dashDistance");
                wallSlideVelocityProp = serializedObject.FindProperty("wallSlideVelocity");
                wallJumpTimeProp = serializedObject.FindProperty("wallJumpTime");
                wallJumpClimbProp = serializedObject.FindProperty("wallJumpClimb");
                wallJumpNormalProp = serializedObject.FindProperty("wallJumpNormal");
                canAirJumpProp = serializedObject.FindProperty("canAirJump");
                airJumpTimeProp = serializedObject.FindProperty("airJumpTime");
                inControlProp = serializedObject.FindProperty("inControl");
                idleAniNameProp = serializedObject.FindProperty("idleAniName");
                walkAniNameProp = serializedObject.FindProperty("walkAniName");
                jumpAniNameProp = serializedObject.FindProperty("jumpAniName");
                runAniNameProp = serializedObject.FindProperty("runAniName");
                dashAniNameProp = serializedObject.FindProperty("dashAniName");
                debugStateCheckerProp = serializedObject.FindProperty("DebugStateChecker");
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
                case 1:// character

                    #region Character
                    EditorGUILayout.PropertyField(inControlProp);
                    EditorGUILayout.PropertyField(maxUpSpeedProp);
                    EditorGUILayout.PropertyField(maxDownSpeedProp);
                    stateIndex = GUILayout.Toolbar(stateIndex, states);
                    switch (stateIndex)
                    {
                        case 0:// high jump
                            EditorGUILayout.PropertyField(canHighJumpProp);
                            EditorGUILayout.PropertyField(highJumpHeightProp);
                            break;
                        case 1:// acceleration
                            EditorGUILayout.PropertyField(canAccelerationProp);
                            EditorGUILayout.PropertyField(accelerationSpeedProp);
                            EditorGUILayout.PropertyField(accelerationTimeProp);
                            break;
                        case 2:// dash
                            EditorGUILayout.PropertyField(canDashProp);
                            EditorGUILayout.PropertyField(dashTimeProp);
                            EditorGUILayout.PropertyField(dashDistanceProp);
                            break;
                        case 3:// wall
                            EditorGUILayout.PropertyField(wallSlideVelocityProp);
                            EditorGUILayout.PropertyField(wallJumpTimeProp);
                            EditorGUILayout.PropertyField(wallJumpClimbProp);
                            EditorGUILayout.PropertyField(wallJumpNormalProp);
                            break;
                        case 4:// jump
                            EditorGUILayout.PropertyField(canAirJumpProp);
                            EditorGUILayout.PropertyField(minJumpHeightProp);
                            EditorGUILayout.PropertyField(airJumpTimeProp);
                            break;
                    }


                    #endregion
                    
                    break;
                case 2:// Others
                    EditorGUILayout.PropertyField(debugStateCheckerProp);
                    EditorGUILayout.PropertyField(idleAniNameProp);
                    EditorGUILayout.PropertyField(walkAniNameProp);
                    EditorGUILayout.PropertyField(jumpAniNameProp);
                    EditorGUILayout.PropertyField(runAniNameProp);
                    EditorGUILayout.PropertyField(dashAniNameProp);
                    break;
            }
            
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}