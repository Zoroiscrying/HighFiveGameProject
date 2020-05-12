using UnityEditor;
using UnityEngine;

namespace HighFive.Control.Movers.Editor
{
    [CustomEditor(typeof(FlyActorMover))]
    public class FlyActorMoverEditor:ActorMoverEditor
    {
        private SerializedProperty maxFlyHeightProp;
        private SerializedProperty minFlyHeightProp;

        private string[] titles = {"ActorMover", "FlyActor", "Others"};
        private int titleIndex = 0;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            maxFlyHeightProp = serializedObject.FindProperty("_maxFlyHeight");
            minFlyHeightProp = serializedObject.FindProperty("_minFlyHeight");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            titleIndex = GUILayout.Toolbar(titleIndex, titles);
            switch (titleIndex)
            {
                case 0:// ActorMover
                    base.OnInspectorGUI();
                    break;
                case 1:// FlyActor
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