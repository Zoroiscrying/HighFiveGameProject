using System;
using System.Collections.Generic;

namespace HighFive.Model.SpiritItemSystem
{
    public abstract class AbstractSpiritItem
    {

        #region Static
        private static Dictionary<string, AbstractSpiritItem> spiritDic = new Dictionary<string, AbstractSpiritItem>();

        public static void RegisterSpiritItem<T>(string name) where T : AbstractSpiritItem, new()
        {
            if (spiritDic.ContainsKey(name))
            {
                throw new Exception("重复注册SpiritMent");
            }
            var s = new T();
            s.Init(name);
            spiritDic.Add(name, s);
        }

        public static AbstractSpiritItem GetInstance(string name)
        {
            if(!spiritDic.ContainsKey(name))
            {
                throw new Exception("没有注册这个灵器：" + name);
            }
            return spiritDic[name];
        }
        #endregion

        public string Name { get; protected set; }

        public abstract void OnEnable();

        public abstract void OnDisable();
        
        protected void Init(string args)
        {
            this.Name = args.Trim();
        }
    }
}
