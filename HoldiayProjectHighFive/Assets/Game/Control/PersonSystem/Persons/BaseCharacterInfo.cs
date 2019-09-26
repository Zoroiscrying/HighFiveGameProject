using System.Collections.Generic;
using System.IO;
using Game.Control.SkillSystem;
using ReadyGamerOne.EditorExtension;
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



        private void OnEnable()
        {
            hp = maxHp;
        }


        public string characterName;
        public Vector3 position;
        public ResourcesPathChooser prefabPath;

        public float baseAttack;
        public float attack_adder = 0f;
        public float attack_scaler = 1f;
        public int hp;
        public int maxHp;
        public float attackSpeed=1.0f;

        public bool IgnoreHitback;    //忽略击退
        public bool IsConst;    //是否可选中/无敌帧
        public float DefaultConstTime;//硬直时间
        public bool InputOk=true; //接受技能输入

        public List<SkillInfoAsset> skills;
    }
}