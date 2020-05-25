using System;
using System.IO;
using ReadyGamerOne.EditorExtension;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HighFive.Script
{
    [Serializable]
    public class SceneTriggerChooser
    {
        [SerializeField] private SceneField scene;
        
        public const float LabelWidth=0.375f;
        public const float ObjectFieldWidth = 0.2f;
        
        [SerializeField]
        private string guid;

        [SerializeField] 
        private int selectedIndex;

        [SerializeField] private Object sceneAsset;
        
        [SerializeField]private string typeName;

        public string TypeName => typeName;

        [SerializeField]private string sceneName;
        [SerializeField]private Vector3 targetPosition;

        public string SceneName => Path.GetFileNameWithoutExtension(scene.ScenePath);
        public string ScenePath => scene.ScenePath;
        public Vector3 TargetPosition => targetPosition;

        public SceneTriggerChooser(string typeName)
        {
            this.typeName = typeName;
        }
        
    }
}