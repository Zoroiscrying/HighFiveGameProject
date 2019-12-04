using System;
using ReadyGamerOne.Common;
using ReadyGamerOne.Utility;
using UnityEngine.SceneManagement;

namespace ReadyGamerOne.Script
{
    /// <summary>
    /// 使用SceneMgr推荐用脚本继承这个类
    /// </summary>
    public abstract class AbstractGameMgr<T>:MonoSingleton<T>
        where T:AbstractGameMgr<T>
    {
        public static event Action onDrawGizomos;
        protected override void Awake()
        {
            base.Awake();
            print("AbstractGameMgr_Awake——这句话应该只显示一次");

            RegisterSceneEvent();
            WorkForOnlyOnce();
            QuickStart.RegisterUi(this.GetType());
        }

        protected virtual void RegisterSceneEvent()
        {
            SceneManager.sceneLoaded +=(scene,mode)=> this.OnAnySceneLoad();
            SceneManager.sceneUnloaded +=(scene)=> this.OnAnySceneUnload();            
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
            MainLoop.Instance.Clear();
        }
        
        private void OnDrawGizmos()
        {
            onDrawGizomos?.Invoke();
        }
    }
}