using System;
using System.Collections.Generic;
using System.IO;
using Game.Control.EffectSystem;
using Game.Control.SkillSystem;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Control.PersonSystem
{
    public class BaseCharacterInfo:ScriptableObject
    {
        #region Editor

#if UNITY_EDITOR
        [MenuItem("ReadyGamerOne/Create/RPG/CharacterAsset")]
        public static void CreateInstance()
        {
            string[] strs = Selection.assetGUIDs;

            string path = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (path.Contains("."))
            {
                path=path.Substring(0, path.LastIndexOf('/'));
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            AssetDatabase.CreateAsset(CreateInstance<BaseCharacterInfo>(), path + "/NewCharacterInfo.asset");
            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<BaseCharacterInfo>(path + "/NewCharacterInfo.asset");
        }        
#endif        

        #endregion

//        protected virtual void OnDrawGizmos()
//        {
//            GizmosUtil.DrawSign(position,color,signalSize);
//        }
//        private void OnEnable()
//        {
//            AbstractGameMgr.onDrawGizomos += OnDrawGizmos;
//        }
//        private void OnDisable()
//        {
//            AbstractGameMgr.onDrawGizomos -= OnDrawGizmos;
//        }
//        public Color color=Color.cyan;
//        public float signalSize = 1.0f;
//        public Vector3 position;

        public EffectInfoAsset attackEffects;
        public EffectInfoAsset hitEffects;
        public EffectInfoAsset acceptEffects;
                

        public string characterName;
        public ResourcesPathChooser prefabPath;
        public int baseAttack;
        public Vector2 hitBackSpeed =Vector2.zero;    //击退
        public int maxHp;
        public float attackSpeed=1.0f;
        public bool IgnoreHitback;    //忽略击退
        public float DefaultConstTime;//硬直时间
    }
}