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
    }
}