using System;
using System.Collections.Generic;
using DialogSystem.Model;
using DialogSystem.ScriptObject;

namespace DialogSystem.Scripts
{
    public abstract class AwakeAtGameProgress : UnityEngine.MonoBehaviour
    {
        public abstract Action<float> onAwakeOnProgress { get; }
        public List<BoolVarChooser> enableConditions = new List<BoolVarChooser>();

        protected virtual void Awake()
        {
            TriggerWithConditions(DialogProgressAsset.Instance.CurrentProgress);
            DialogProgressAsset.Instance.onProgressChanged += TriggerWithConditions;
        }

        protected virtual void OnDestroy()
        {
            DialogProgressAsset.Instance.onProgressChanged -= TriggerWithConditions;
        }

        private void TriggerWithConditions(float progress)
        {
            if (enableConditions.Count == 0)
            {
                //没有条件，直接调用
                onAwakeOnProgress(progress);
            }
            else
            {
                foreach (var VARIABLE in enableConditions)
                {
                    if (!VARIABLE.Value)
                    {
                        //有一个条件不满足，都不调用，直接返回
                        gameObject.SetActive(false);
                        return;
                    }
                }

                onAwakeOnProgress(progress);
            }
        }
    }
}