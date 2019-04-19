using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Common;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 触发器抽象模板工厂类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SkillTriggerFactory<T> : Singleton<SkillTriggerFactory<T>>, ISkillTriggerFactory
        where T : ISkillTrigger, new()
    {
        public ISkillTrigger CreateTrigger(string args)//type,int id,float startTime,float lastTime,string args)
        {
            var t = new T();
            t.Init(args);//type,id,startTime,lastTime,args);
            return t;
        }
    }
}
