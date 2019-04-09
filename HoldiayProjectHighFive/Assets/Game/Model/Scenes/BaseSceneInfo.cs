using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Game.Model.Scenes
{
    public class BaseSceneInfo
    {
        #region static_All_Instances

        private static string sceneBefore = null;
        private static Dictionary<string, BaseSceneInfo> sceneDis = new Dictionary<string, BaseSceneInfo>();
        public static void InitScene<T>(string sceneName) where T : BaseSceneInfo, new()
        {
            sceneDis.Add(sceneName, new T().Init(sceneName));
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
            if (sceneBefore != null)
                sceneDis[sceneBefore].OnSceneUnload();
            Debug.Log(this.SceneName + "LoadScene");
            SceneManager.LoadScene(this.SceneName);
        }

        public void LoadSceneAsync()
        {
            if (sceneBefore != null)
                sceneDis[sceneBefore].OnSceneUnload();
            SceneManager.LoadSceneAsync(this.SceneName);
        }
    }
}
