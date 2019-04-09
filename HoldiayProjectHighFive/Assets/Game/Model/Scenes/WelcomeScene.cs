using Game.View;
using Game.View.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Model.Scenes
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
