using ReadyGamerOne.Common;
using UnityEngine;

namespace HighFive.Global
{
    public class GameSettings:ScriptableSingleton<GameSettings>
    {
        [Header("金币获取倍率")]
        public int moneyScale = 3;
        [Header("灵气获取倍率")]
        public int reikiScale = 3;
    }
}