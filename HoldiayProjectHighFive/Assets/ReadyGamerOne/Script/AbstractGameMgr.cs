using System;
using ReadyGamerOne.Model.SceneSystem;
using UnityEngine;

namespace ReadyGamerOne.Script
{
    /// <summary>
    /// 使用SceneMgr推荐用脚本继承这个类
    /// </summary>
    public abstract class AbstractGameMgr:MonoBehaviour
    {
        public static event Action onDrawGizomos;
        
            
        protected virtual void Awake()
        {
            print("AbstractGameMgr_Awake——这句话应该只显示一次");
            
            AbstractSceneInfo.onAnySceneLoad += this.OnAnySceneLoad;
            AbstractSceneInfo.onAnySceneUnLoaded += this.OnAnySceneUnload;
            
            AddGlobalScript();
            WorkForOnlyOnce();
            RefreshGlobalVar();
            
            SceneMgr.LoadActiveScene();
        }

        protected virtual void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        protected virtual void WorkForOnlyOnce()
        {
            print("work for only once");
        }

        protected virtual void OnAnySceneLoad()
        {
            
        }

        protected virtual void OnAnySceneUnload()
        {
            MainLoop.Clear();
        }

        protected virtual void AddGlobalScript()
        {
            this.gameObject.AddComponent<MainLoop>();
        }

        protected virtual void RefreshGlobalVar()
        {
            
        }

        private void OnDrawGizmos()
        {
            onDrawGizomos?.Invoke();
        }
    }
}