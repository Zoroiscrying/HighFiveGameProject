using System;
using System.Collections;
using System.Collections.Generic;
using Game.Script;
using Game;
using Game.Serialization;
using Game.View;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Modal
{
    public class SceneMgr : Singleton<SceneMgr>
    {
        
        #region 构造

        public SceneMgr()
        {
        }
        
        #endregion

        public string CurruentScene { get; private set; }
        
        public void LoadScene( string name)
        {
            this.CurruentScene = name;
            BaseSceneInfo.GetScene(name).LoadScene();
        }

        public void LoadSceneAsync(string name)
        {
            this.CurruentScene = name;
            BaseSceneInfo.GetScene(name).LoadSceneAsync();
        }
    }


    internal  class BaseSceneInfo
    {
        #region static_All_Instances

        private static string sceneBefore=null;
        private static SerializableDictionary<string,BaseSceneInfo> sceneDis=new SerializableDictionary<string, BaseSceneInfo>();
        public static BaseSceneInfo GetScene(string name)
        {
            if (sceneDis.ContainsKey(name))
                return sceneDis[name];
            else
            {
                var s = new BaseSceneInfo(name);
                return s;
            }
        }
        
        
        #endregion
        public string SceneName { get; protected set; }
        
        public event Action<string> onSceneLoad;
        public event Action<string> onSceneUnload;

        protected BaseSceneInfo(string name)
        {
            this.SceneName = name;
            sceneDis.Add(name, this);
        }
        
        /// <summary>
        /// 加载场景
        /// 这里要做好UI，音乐，等其他资源的初始化
        /// </summary>
        protected virtual void OnSceneLoad()
        {
            if (onSceneLoad != null)
                onSceneLoad(SceneName);
        }

        protected virtual void OnSceneUnload()
        {
            if (onSceneUnload != null)
                onSceneUnload(SceneName);
        }

        public void LoadScene()
        {
            if(sceneBefore!=null)
                sceneDis[sceneBefore].OnSceneUnload();
            SceneManager.LoadScene(this.SceneName);
            OnSceneLoad();
        }

        public void LoadSceneAsync()
        {
            if(sceneBefore!=null)
                sceneDis[sceneBefore].OnSceneUnload();
            SceneManager.LoadSceneAsync(this.SceneName);
            OnSceneLoad();
        }
    }


    internal class TestSceneInfo : BaseSceneInfo
    {
        protected override void OnSceneLoad()
        {
            base.OnSceneLoad();
            
            UIManager.Instance.PushPanel(new BattlePanel());
            
            
        }

        protected override void OnSceneUnload()
        {
            base.OnSceneUnload();
            UIManager.Instance.PopPanel();
        }

        protected TestSceneInfo(string name) : base(name)
        {
        }
    }

    internal class JbSceneInfo : BaseSceneInfo
    {
        protected override void OnSceneLoad()
        {
            base.OnSceneLoad();
            UIManager.Instance.PushPanel(new BattlePanel());
        }

        protected override void OnSceneUnload()
        {
            base.OnSceneUnload();
            UIManager.Instance.PopPanel();
        }

        protected JbSceneInfo(string name) : base(name)
        {
        }
    }

    internal class WelcomeScene : BaseSceneInfo
    {
        protected override void OnSceneLoad()
        {
            base.OnSceneLoad();
            UIManager.Instance.PushPanel(new WelcomePanel("选一个玩耍吧", "TestScene", "Jb", "游戏开始"));
        }

        protected override void OnSceneUnload()
        {
            base.OnSceneUnload();
            UIManager.Instance.PopPanel();
        }

        protected WelcomeScene(string name) : base(name)
        {
            
        }
    }
}
