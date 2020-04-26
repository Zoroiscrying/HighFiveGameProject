using System;
using System.Collections.Generic;
using ReadyGamerOne.Common;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HighFive.Control.EffectSystem
{
    [ScriptableSingletonInfo("RpgEffectMgr")]
    public class EffectMgr: ReadyGamerOne.Common.ScriptableSingleton<EffectMgr>
    {

#if UNITY_EDITOR
        [MenuItem("ReadyGamerOne/RPG/ShowRpgEffectMgr")]
        public static void ShowEffects()
        {
            Selection.activeInstanceID = Instance.GetInstanceID();
        }
#endif
        
        

        [Serializable]
        public class EffectAssetMsg
        {
            public int id;
            public string tag;
            public EffectInfoAsset EffectInfoAsset;
        }
        
        public List<EffectAssetMsg> effectInfoMsgs=new List<EffectAssetMsg>();
        public int GetID()
        {
            var index = 0;
            while (true)
            {
                var ok = true;
                foreach (var unit in effectInfoMsgs)
                {
                    if (index == unit.id)
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok)
                    break;
                index++;
            }

            return index;
        }
    }
}