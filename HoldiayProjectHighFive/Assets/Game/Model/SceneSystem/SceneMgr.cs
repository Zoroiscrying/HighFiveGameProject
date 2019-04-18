using Game.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Model.SceneSystem
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
            get { return this.curruentScene; }
        }

        public void LoadScene(string name)
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
}
