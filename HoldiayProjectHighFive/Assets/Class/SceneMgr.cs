using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Game.Script;
using Game;
using Game.Const;
using Game.Control;
using Game.Global;
using Game.Serialization;
using Game.View;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.PlayerLoop;
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

        private string curruentScene;

        public string CurruentScene
        {
            get { return this.curruentScene;}
        }
        
        public void LoadScene( string name)
        {
            this.curruentScene = name;
            BaseSceneInfo.GetScene(name).LoadScene();
        }

        public void LoadSceneAsync(string name)
        {
            this.curruentScene = name;
            BaseSceneInfo.GetScene(name).LoadSceneAsync();
        }
    }


    public  class BaseSceneInfo
    {
        #region static_All_Instances

        private static string sceneBefore=null;
        private static SerializableDictionary<string,BaseSceneInfo> sceneDis=new SerializableDictionary<string, BaseSceneInfo>();
        public static void InitScene<T>(string sceneName)where T:BaseSceneInfo,new()
        {
            sceneDis.Add(sceneName,new T().Init(sceneName));
        }
        static BaseSceneInfo()
        { 
            InitScene<JbSceneInfo>(Const.SceneName.jbScene);
            InitScene<WelcomeScene>(Const.SceneName.welcomeScene);
            InitScene<TestSceneInfo>(Const.SceneName.testScene);
        }
        public static BaseSceneInfo GetScene(string name)
        {
            Assert.IsTrue(sceneDis.ContainsKey(name));
            return sceneDis[name];
        }

        
        #endregion
        public string SceneName { get; protected set; }
        
        public event Action<string> onSceneUnload;


        protected virtual BaseSceneInfo Init(string name)
        {
            this.SceneName = name;
            return this;
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
            Debug.Log(this.SceneName + "LoadScene");
            SceneManager.LoadScene(this.SceneName);
        }

        public void LoadSceneAsync()
        {
            if(sceneBefore!=null)
                sceneDis[sceneBefore].OnSceneUnload();
            SceneManager.LoadSceneAsync(this.SceneName);
        }
    }


    public class TestSceneInfo : BaseSceneInfo
    {
        

        protected override void OnSceneUnload()
        {
            base.OnSceneUnload();
            Debug.Log("TestScene卸载");
            UIManager.Instance.PopPanel();
        }
    }

    public class JbSceneInfo : BaseSceneInfo
    {     
        protected override void OnSceneUnload()
        {
            base.OnSceneUnload();
            Debug.Log("jbScene卸载");
            UIManager.Instance.PopPanel();
        }
    }

    public class WelcomeScene : BaseSceneInfo
    {

        protected override void OnSceneUnload()
        {
            base.OnSceneUnload();
            UIManager.Instance.PopPanel();
        }
    }
}
