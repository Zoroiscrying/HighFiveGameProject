﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 触发器工厂接口
    /// </summary>
    public interface ISkillTriggerFactory
    {
        ISkillTrigger CreateTrigger(string args); //type, int id,float startTime,float lastTime,string args);
    }
}
