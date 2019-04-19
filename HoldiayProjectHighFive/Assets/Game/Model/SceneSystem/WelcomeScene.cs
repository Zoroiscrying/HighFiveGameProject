using Game.View;
using Game.View.PanelSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Model.SceneSystem
{
    public class WelcomeScene : BaseSceneInfo
    {

        protected override void OnSceneUnload()
        {
            base.OnSceneUnload();
            UIManager.Instance.PopPanel();
        }
    }
}
