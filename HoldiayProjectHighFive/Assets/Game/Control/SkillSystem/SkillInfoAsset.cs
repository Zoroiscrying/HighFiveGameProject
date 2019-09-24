using System.Collections.Generic;
using Game.Control.PersonSystem;
using ReadyGamerOne.Const;
using ReadyGamerOne.Script;
using UnityEngine;

namespace Game.Control.SkillSystem
{  
    public class SkillInfoAsset:ScriptableObject
    {
        public ConstStringChooser skillName;
        public float startTime;
        public bool isUsed = false;
        public float LastTime
        {
            get
            {
                var time = 0f;
                foreach (var VARIABLE in triggers)
                {
                    time = Mathf.Max(time, VARIABLE.startTime + VARIABLE.lastTime);
                }

                return time;
            }
        }
        public List<TriggerUnitInfo> triggers=new List<TriggerUnitInfo>();

        public void RunSkill(AbstractPerson self, bool ignoreInput = false, float startTime = 0)
        {
            if (!self.InputOk)
                return;
            
            var lastTime = LastTime;
            if (ignoreInput)
            {
                self.IgnoreInput(lastTime);
            }
            if (isUsed)
            {
                foreach (var trigger in triggers)
                    trigger.Reset();
            }
            else
                this.isUsed = true;
            
            
            this.startTime = startTime;
            foreach (var trigger in triggers)
                trigger.RunTriggerUnit(self);
            
            MainLoop.Instance.ExecuteLater(() =>
            {
                if (isUsed)
                    isUsed = false;
                foreach (var VARIABLE in triggers)
                {
                    VARIABLE.Reset();
                }
            }, lastTime);
        }
        
        public int GetID()
        {
            var index = 0;
            while (true)
            {
                var ok = true;
                foreach (var unit in triggers)
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