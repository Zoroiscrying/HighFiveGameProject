using System;
using UnityEditor;
using UnityEngine;

namespace HighFive.Control.Movers.Editor
{
    [CustomEditor(typeof(BaseMover))]
    public class BaseMoverEditor:UnityEditor.Editor
    {
        #region Properties

        private SerializedProperty skinWidthProp;
        private SerializedProperty totalHorizontalRaysProp;
        private SerializedProperty totalVerticalRaysProp;

        private SerializedProperty gravityProp;
        private SerializedProperty gravityScaleProp;
        private SerializedProperty velocityProp;
        private SerializedProperty velocityMultiplierProp;
        private SerializedProperty canMoveProp;

        private SerializedProperty platformMaskProp;
        private SerializedProperty triggerMaskProp;

        private SerializedProperty moverInputProp;
        private SerializedProperty canMoveVerticallyProp;

        private SerializedProperty onWayPlatformMaskProp;
        private SerializedProperty collisionStateProp;        

        #endregion

        private int selectedIndex = 1;
        private string[] titles = {"Raycast", "IBaseControl", "MoverInput", "Others"};
        

        protected virtual void OnEnable()
        {
            skinWidthProp = serializedObject.FindProperty("_skinWidth");
            totalVerticalRaysProp = serializedObject.FindProperty("totalVerticalRays");
            totalHorizontalRaysProp = serializedObject.FindProperty("totalHorizontalRays");

            gravityProp = serializedObject.FindProperty("gravity");
            gravityScaleProp = serializedObject.FindProperty("gravityScale");
            velocityProp = serializedObject.FindProperty("velocity");
            velocityMultiplierProp = serializedObject.FindProperty("velocityMultiplier");
            canMoveProp = serializedObject.FindProperty("canMove");
            platformMaskProp = serializedObject.FindProperty("platformMask");
            triggerMaskProp = serializedObject.FindProperty("triggerMask");
            moverInputProp = serializedObject.FindProperty("moverInput");
            canMoveVerticallyProp = serializedObject.FindProperty("canMoveVertically");
            onWayPlatformMaskProp = serializedObject.FindProperty("oneWayPlatformMask");
            collisionStateProp = serializedObject.FindProperty("collisionState");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            selectedIndex = GUILayout.Toolbar(selectedIndex, titles);
            switch (selectedIndex)
            {
                case 0:// Raycast
                    EditorGUILayout.PropertyField(skinWidthProp);
                    EditorGUILayout.PropertyField(totalHorizontalRaysProp);
                    EditorGUILayout.PropertyField(totalVerticalRaysProp);                    
                    break;
                case 1:// IBaseControl
                    EditorGUILayout.PropertyField(gravityProp);
                    EditorGUILayout.PropertyField(gravityScaleProp);
                    EditorGUILayout.PropertyField(velocityProp);
                    EditorGUILayout.PropertyField(velocityMultiplierProp);
                    EditorGUILayout.PropertyField(canMoveProp);
                    EditorGUILayout.PropertyField(platformMaskProp);
                    EditorGUILayout.PropertyField(triggerMaskProp);
                    EditorGUILayout.PropertyField(onWayPlatformMaskProp);
                    break;
                case 2:// MoverInput
                    EditorGUILayout.PropertyField(moverInputProp);
                    EditorGUILayout.PropertyField(canMoveVerticallyProp);
                    EditorGUILayout.PropertyField(collisionStateProp);
                    break;
                case 3:// Others
                    break;
            }

            
            serializedObject.ApplyModifiedProperties();
        }
    }
}