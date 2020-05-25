using UnityEditor;
using UnityEngine;

namespace HighFive.Control.Movers.Editor
{
    [CustomEditor(typeof(FlyActorMover),true)]
    public class FlyActorMoverEditor:ActorMoverEditor
    {
        private SerializedProperty maxFlyHeightProp;
        private SerializedProperty minFlyHeightProp;
        private SerializedProperty isRandomFlyingProp;
        private SerializedProperty _relativeGroundPosYProp;

        private string[] flyTitles = {"ActorMover", "FlyActor", "Others"};
        private int flyMoverIndex = 0;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            maxFlyHeightProp = serializedObject.FindProperty("_maxFlyHeight");
            minFlyHeightProp = serializedObject.FindProperty("_minFlyHeight");
            isRandomFlyingProp = serializedObject.FindProperty("_isRandomFlying");
            _relativeGroundPosYProp = serializedObject.FindProperty("_relativeFlyHeight");
//            _relativeGroundPosYProp.se
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            flyMoverIndex = GUILayout.Toolbar(flyMoverIndex, flyTitles);
            switch (flyMoverIndex)
            {
                case 0:// ActorMover
                    base.OnInspectorGUI();
                    break;
                case 1:// FlyActor
                    EditorGUILayout.PropertyField(isRandomFlyingProp);
                    EditorGUILayout.PropertyField(_relativeGroundPosYProp);
                    EditorGUILayout.PropertyField(maxFlyHeightProp);
                    EditorGUILayout.PropertyField(minFlyHeightProp);
                    break;
                case 2:// Others
                    break;
            }
            

            serializedObject.ApplyModifiedProperties();
        }
        
        
    }
}