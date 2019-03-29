using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Game.Script;

namespace Game.Modal
{
    public abstract class AbstractSpiritItem
    {

        #region Static
        private static Dictionary<string, AbstractSpiritItem> spiritDic = new Dictionary<string, AbstractSpiritItem>();

        public static void RegisterSpiritItem<T>(string name) where T : AbstractSpiritItem, new()
        {
            if (spiritDic.ContainsKey(name))
            {
                Debug.Log("重复注册SpiritMent");
                return;
            }
            var s = new T();
            s.Init(name);
            spiritDic.Add(name, s);
        }
        
        public static AbstractSpiritItem GetInstance(string name)
        {
            return spiritDic[name];
        }
        #endregion
        
        public string Name { get; protected set; }

        public abstract  void OnEnable();

        public abstract  void OnDisable();

        protected abstract void Execute();

        public virtual void Init(string args)
        {
            this.Name = args.Trim();
        }
    }

    public class ShitSpirit : AbstractSpiritItem
    {
        private UpdateTestPair pair;

        protected override void Execute()
        {
            Debug.Log("灵器检测成功");
        }
        
        public override void OnEnable()
        {
            pair = new UpdateTestPair(()=>Input.GetKeyDown(KeyCode.K), Execute);
            MainLoop.Instance.AddUpdateTest(pair);
        }

        public override void OnDisable()
        {
            MainLoop.Instance.RemoveUpdateTest(pair);
            
            pair = null;
        }

        public override void Init(string args)
        {
            base.Init(args);
        }
    }
    
}
