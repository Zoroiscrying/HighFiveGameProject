#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HighFive.Global
{
    public class GameSettings: ReadyGamerOne.Common.ScriptableSingleton<GameSettings>
    {

        
#if UNITY_EDITOR
        [MenuItem("ReadyGamerOne/Show/GameSettings")]
        public static void CreateAsset()
        {
            Selection.activeInstanceID = GameSettings.Instance.GetInstanceID();
        }
#endif

        [Header("GUI显示玩家状态信息")]
        public bool showPlayerBuffInfo = true;
        
        [Header("金币获取倍率")]
        public int moneyScale = 3;
        [Header("灵气获取倍率")]
        public int reikiScale = 3;
        [Header("伤害/药引转化比率")]
        public float damageToDragScale = 1;
        [Header("Z强化时间")]
        public float superTime = 3;

        [Header("Z强化消耗药引百分比")]
        [Range(0, 1f)] 
        public float superDragConsumeRate = 1 / 3f;

        [Header("Z强化的攻击倍率")]
        public float superAttackScale = 2;

        [Header("是否启用场景触发器")]
        public bool enableSceneTrigger = true;


        [Header("伤害数字")] public int normalSize = 20;
        public Color normalColor=Color.yellow;
        public int critSize = 30;
        public Color critColor=Color.red;
        public int skillSize = 25;
        public int missingSize=30;
        public Color missingColor=Color.cyan;
        public Color enemyDamageColor=new Color(0.04f, 0.09f, 0.72f);
        
    }
}